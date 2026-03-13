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
    // The ball Start pos and player start pos
    public Transform ballStartPos;
    public Transform playerStartPos;
    // UI Text that Displays Score
    public Text scoreText;

    [Header("Settings")]
    public int score = 0;

    [Header("Effects")]
    public GameObject goalEffect; // this can be a particle, message or canvas

    // Calls every time a goal is scored
    public void GoalScored()
    {
        // Increase Player score
        score++;
        UpdateScoreUI();

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

    // Reset the positions function
    void ResetPositions()
    {
        // Reset the ball position
        ball.position = ballStartPos.position;
        Rigidbody ballRb = ball.GetComponent<Rigidbody>();

        // Reset the player position
        player.parent.position = playerStartPos.position;
        Rigidbody playerRb = player.GetComponent<Rigidbody>();

        // Reset the player's shot
        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.ResetShot();
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

