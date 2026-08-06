// PlayerSwitcher

// Imports For unity
using UnityEngine;
using UnityEngine.InputSystem;

// Owns which PlayerController is currently being controlled by the user and forwards the input system to one player only
public class PlayerSwitcher : MonoBehaviour {
    [Header("Team")]
    public System.Collections.Generic.List<PlayerController> teamPlayers;

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
	// this is a reference that we can refer back to 'this' helps us find the other player when we pass the ball
	foreach (PlayerController pc in teamPlayers) {
	    pc.playerSwitcher = this;
	}
	
	// set player_1 to active at first
        SetActive(teamPlayers[0]);
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

	// Loop through all the players and 
	foreach (PlayerController pc in teamPlayers) {
	    pc.isAIControlled = (pc != activePlayer);
	}

	// Ball is with a player so false
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
	bool graceActive = Time.time < releaseTime + selfPossessionGrace;
	PlayerController closest = null;
	float closestDist = float.MaxValue;

	foreach (PlayerController pc in teamPlayers) {
	    if (graceActive && pc == lastPasser) {
		continue;
	    }

	    float dist = Vector3.Distance(pc.transform.position, ball.position);
	    if (dist <= possessionDistance && dist < closestDist) {
		closest = pc;
		closestDist = dist;
	    }
	}

	// Set the closest player to the ball as active
	if (closest != null) {
	    SetActive(closest);
	}
    }

    // get the closest or the best target person closest to your pass
    public PlayerController GetBestPassTarget(PlayerController passer) {
	PlayerController best = null;
	float bestScore = float.MinValue;

	foreach (PlayerController pc in teamPlayers) {
	    if (pc == passer) {
		continue;
	    }

	    // Closest teammate is roughlty ahead of the passer
	    Vector3 toTeammate = (pc.transform.position - passer.transform.position);
	    float forwardAlignment = Vector3.Dot(passer.transform.forward, toTeammate.normalized);
	    if (forwardAlignment > bestScore) {
		bestScore = forwardAlignment;
		best = pc;
	    }
	}
	return best;
    }
    
    // Called by the goal manager after the posititons are reset to the posetions are returned clean
    public void ResetPossessionTo() {
        SetActive(teamPlayers[0]);
    }
}
