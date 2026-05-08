// Unity Classes such as:
// MonoBehaviour, Vector3 and Transform
using UnityEngine;
using UnityEngine.UI;

// Script to control the logic of the Goal Manager
public class GoalManager : MonoBehaviour
{
    [Header("References")]
    // Assign the ball inspector
    public Transform ball;
    // Assign the player inspector
    public Transform player;
    public Transform defender;
    
    // The ball Start pos and player start pos
    public Transform ballStartPos;
    public Transform playerStartPos;
    public Transform defenderStartPos;
    
    // UI Text that Displays Score
    public Text scoreText;

    // UI Text that tells you a missed shot
    public Text missText;

    [Header("Settings")]
    public int score = 0;

    [Header("Effects")]
    public GameObject goalEffect; // this can be a particle, message or canvas

    // Private variables
    private Rigidbody ballRb;

    // Cache the BallRb
    void Start() {
	ballRb = ball.GetComponent<Rigidbody>();
    }
    
    // Calls every time a goal is scored
    public void GoalScored()
    {
        // Increase Player score
        score++;
        UpdateScoreUI();

	// Trigger the celebration for the player
	PlayerController pc = player.GetComponent<PlayerController>();
	if (pc != null) {
	    pc.PlayCelebration();
	}
	
        // show the goal effect
        if (goalEffect != null)
        {
            goalEffect.SetActive(true);
            // Optional hide after 1 second
            Invoke("HideGoalEffect", 1f);
        }

        // Reset the ball and player postion
        Invoke("ResetPositions", 3f);
    }
 
    // Calls every time the goal is missed
    public void GoalMissed() {
	// Show missed Text
	if (missText != null) {
	    missText.text = "Missed!";
	    missText.gameObject.SetActive(true);
	    Invoke("HideMissText", 2f);
	}
	
	// Reset the player and ball poistion after 3 seconds
	Invoke("ResetPositions", 3f);
    }

    // Calls Every time the Defender Steals the ball
    public void BallSteal() {
	// Adding a Ball Stolen
	if (missText != null) {
	    missText.text = "Tackled!";
	    missText.gameObject.SetActive(true);
	    Invoke("HideMissText", 2f);
	}

	// Reset after 3 sec
	Invoke("ResetPositions", 3f);
    }

    // Hide the miss text message
    void HideMissText() {
	if (missText != null) {
	    missText.gameObject.SetActive(false);
	}
    }
    
    // Reset the positions function
    void ResetPositions() {
        // Reset the ball position
        ball.position = ballStartPos.position;
	ballRb.linearVelocity = Vector3.zero;
	ballRb.angularVelocity = Vector3.zero;
	// This locks the ball until we start to dribble
	ballRb.isKinematic = true;

	// reset the players positon
	player.position = playerStartPos.position;
	player.rotation = playerStartPos.rotation;
	// Rigidbody ballRb = ball.GetComponent<Rigidbody>();

        // // Reset the player position
        // player.parent.position = playerStartPos.position;
        // Rigidbody playerRb = player.GetComponent<Rigidbody>();

        // Reset the player's shot
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null) {
            pc.ResetShot();
        }

	// Reset the Defender
	if (defender != null && defenderStartPos != null) {
	    defender.position = defenderStartPos.position;
	    defender.rotation = defenderStartPos.rotation;

	    // Reset Velovity of Defender
	    Rigidbody defRb = defender.GetComponent<Rigidbody>();
	    if (defRb != null) {
		defRb.linearVelocity = Vector3.zero;
		defRb.angularVelocity = Vector3.zero;
	    }
	}	
    }

    //  Update the score
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score = " + score;
        }
    }

    // Hide the goal effect making sure it goes away after reset
    void HideGoalEffect()
    {
        if (goalEffect != null)
        {
            goalEffect.SetActive(false);
        }
    }
}

