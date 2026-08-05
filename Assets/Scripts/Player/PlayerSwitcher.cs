// PlayerSwitcher

// Imports For unity
using UnityEngine;
using UnityEngine.InputSystem;

// Owns which PlayerController is currently being controlled by the user and forwards the input system to one player only
public class PlayerSwitcher : MonoBehaviour {
    [Header("Players")]
    public PlayerController player_1;
    public PlayerController player_2;

    [Header("Ball")]
    public Transform ball;
    // Distance to the ball for a player to become active
    public float possessionDistance = 1.4f;

    [Header("Pass Settings")]
    // this will prevent the player from passing and getting their own pass back
    public float selfPossessionGrace = 0.4f;
    
    private PlayerController activePlayer;
    // public pointer to active player
    public PlayerController ActivePlayer => activePlayer;

    // Other variables
    private bool ballIsLoose = false;
    private PlayerController lastPasser;
    private float releaseTime;
    
    // Start funct
    void Start() {
        // Wire each player to know about the other, and about this switcher
	// this makes it so the Player Switcher field 
        player_1.teammate = player_2;
	player_2.teammate = player_1;
	// this is a reference that we can refer back to 'this' helps us find the other player when we pass the ball
	player_1.playerSwitcher = this;
        player_2.playerSwitcher = this;

	// set player_1 to active at first
        SetActive(player_1);
    }

    // Update function constantly check if the ball is loose
    void Update() {
        if (ballIsLoose) {
            CheckForPossession();
        }
    }
    
    // helper function to switch and set the player controlling the ball
    void SetActive(PlayerController newActive) {
        activePlayer = newActive;

        player_1.isAIControlled = (player_1 != activePlayer);
        player_2.isAIControlled = (player_2 != activePlayer);

	ballIsLoose = false;
    }

    // Called by Player controller.PassBall() the moment the ball is kicked
    public void BallReleased(PlayerController passer) {
	passer.isAIControlled = true; // might have to change this 
	activePlayer = null;
	ballIsLoose = true;
	lastPasser = passer;
	releaseTime = Time.time;
    }

    // use this function for rebounds and blocked shots
    public void MarkBallLoose() {
	if (activePlayer != null) {
	    activePlayer.isAIControlled = true;
	}
	activePlayer = null;
	ballIsLoose = true;
	lastPasser = null;
    }
    
    // check to see if the player can get the possession
    void CheckForPossession() {

	// check to make sure player doesnt get their own pass
	bool graceActive = Time.time < releaseTime + selfPossessionGrace;

	// calculate the distance from player to ball for each player
	float dist_1 = Vector3.Distance(player_1.transform.position, ball.position);
        float dist_2 = Vector3.Distance(player_2.transform.position, ball.position);

	// see if the player has over 0.4 sec to last pass or was the last passer
	bool one_Eligible = !(graceActive && player_1 == lastPasser);
	bool two_Eligible = !(graceActive && player_2 == lastPasser);

	// see if the distance from the Eligible reciever is close enough to the ball to become active
	if (one_Eligible && dist_1 <= possessionDistance && (!two_Eligible || dist_1 <= dist_2)) {
            SetActive(player_1);
        } else if (two_Eligible && dist_2 <= possessionDistance && (!one_Eligible || dist_2 <= dist_1)) {
            SetActive(player_2);
        }
    }
	
    // Called by the goal manager after the posititons are reset to the posetions are returned clean
    public void ResetPossessionTo(PlayerController player) {
        SetActive(player);
    }
}
