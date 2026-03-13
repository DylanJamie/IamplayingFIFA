// Unity Classes such as:
// MonoBehaviour, Vector3 and Transform
using UnityEngine;

// Script to control the logic of the Goalie!
public class KeeperController : MonoBehaviour
{
    // These Values Appear in the Inspector and can be adjusted
    // This is how fast the keeper moves
    public float moveSpeed = 3f;

    // This is how far the keeper moves
    public float moveRange = 3f;

    // Ball Position variable
    public Transform ball;

    // Distance for when the Keeper Reacts
    public float reactDistance = 10f;

    // Reaction time for the keeper
    public float reactionTime = 0.25f;

    // Reaction Timer
    public float reactTimer = 0f;

    // Speed at which the keep saves
    public float saveSpeed = 5f;

    // This will store the Keepers Original X position when the game begins
    // This will help with reseting and how far the Range really is
    private float startX;
    // This determines whether the keeper moves
    private int direction = 1;

    // Keeper Error
    public float maxError = 1.0f;
    private float shotGuessOffset = 0f;

    // player variable
    public PlayerController player;

    // Start function is ran when the game starts and saves the Keepers orignial X position
    void Start()
    {
        startX = transform.position.x;
    }

    // Update that sends this function every frame
    void Update()
    {
        if (player.HasShot && shotGuessOffset == 0f)
        {
            shotGuessOffset = Random.Range(-maxError, maxError);
        }

        float newX = transform.position.x;

        // If the ball is closer than the keep react distance
        if (ball.position.z < reactDistance) 
        {
            reactTimer += Time.deltaTime;

            // Slow down the goalie to have a chance
            if (reactTimer >= reactionTime) 
            {
                // tracks the balls X position
                float targetX = ball.position.x + shotGuessOffset;
                // Start moving toward the player
                newX = Mathf.MoveTowards(transform.position.x, targetX, saveSpeed * Time.deltaTime);   
            }
        }
        else
        {
            // Ball far normal patrol
            reactTimer = 0f;
            // Normal side to side movement
            newX = transform.position.x + direction * moveSpeed * Time.deltaTime;

            // Reverse the diection when hit limit
            if (Mathf.Abs(newX - startX) >= moveRange) 
            {
                direction *= -1;
            }
        }
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

}
