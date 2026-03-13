// Unity Classes such as:
// MonoBehaviour, Vector3 and Transform
using UnityEngine;
using UnityEngine.UI;

// Unities MonoBehaviour behavior Class
// MonoBehaviour == allows a person to attach the script to a game object
public class PlayerController : MonoBehaviour
{
    // speed variable is movement speed so here we are defining a 5 float speed
    public float speed = 5f;
    // Reference for ball
    public Transform ball;

    // Has the player shot the ball
    private bool hasshot = false;
    public bool HasShot => hasshot;

    // Power slider Variable
    public Slider PowerBar;
    // See if shot charging
    private bool chargeshot = false;
    // current power
    private float currentpower = 0f;
    // Maximum charge
    public float maxcharge = 30.0f;
    // Speed at which bar charges at
    public float chargespeed = 15.0f;

    // For Animations
    private Animator anim;
    

    // Update is a Unity function that is run once per frame 
    void Update()
    {
        // moveX & moveY get the keyboard inputs from the user for Left/Right or Up/Down
        // Returns a value from -1 to 1 based on the input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // move multiplies by the speed to scale the movement
        // new Vector3(moveX, 0, moveZ) decides which direction to move
        // move creates a 3 directional movement (x, y, z) and then multiplies it by the speed
        // so if there is (0, 0, 1) * 5 == (0, 0, 5) there is no Y because we only move on the x and z axis
        // Time.delta time gets the time since the last frame Movement * frames = the amount of units needed to move
        Vector3 move = new Vector3(moveX, 0, moveZ) * speed * Time.deltaTime;

        // Moves the player via vector move
        // Space world means movement is relative to the world, not the players current rotation
        transform.Translate(move, Space.World);

        // Rotate the player to face movement direction
        // If the player is moving make them face the direction of the movement
        if (move != Vector3.zero)
        {
            transform.forward = move;
        }

        // If has shot is false
        if (hasshot != true)
        {
            // Makes Smoother Dribbling (Arcade Dribbling)
            // This is the players current position
            // it auto updates based on where the player is facing
            // * 1f means to go one unit in front of the player
            Vector3 ballTargetPos = transform.position + transform.forward * 0.5f;
            // LERP = Linear Interpolation
            // Move the ball a little bit closer to the target each frame
            ball.position = Vector3.Lerp(ball.position, ballTargetPos, Time.deltaTime * 5f);
        }

        // Shoot button
        // Start charging the shot when you press the space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Reset the bar and set the charge shot to true
            chargeshot = true;
            currentpower = 0f;
        }

        // While holding the space bar increase the charge
        if (chargeshot && Input.GetKey(KeyCode.Space))
        {
            // power will be how long the space is held times the speed
            currentpower += chargespeed * Time.deltaTime;
            currentpower = Mathf.Clamp(currentpower, 0, maxcharge);

            // Update the UI bar visual
            PowerBar.value = currentpower / maxcharge;
        }

        // Release shot when space is released
        if (Input.GetKeyUp(KeyCode.Space))
        {
            // Set charge shot back to false
            chargeshot = false;
            // Check if player has shot
            hasshot = true;
            ShootBall(currentpower);
        }

        // Update Animations
        float currentSpeed = new Vector3(moveX, 0, moveZ).magnitude;
        anim.SetFloat("Speed", currentSpeed);


        if (Input.GetKeyUp(KeyCode.Space))
        {
            chargeshot = false;
            hasshot = true;
            anim.SetTrigger("Shoot"); 
            ShootBall(currentpower);
        }
    }

    // Shoot Ball function
    void ShootBall(float power)
    {
        Rigidbody ballrigidbody = ball.GetComponent<Rigidbody>();

        // Calculate the direction for the shot
        // this adds a bit of lift to the shot so it is not always on the ground
        Vector3 ShotDirection = transform.forward + new Vector3(0, 0.2f, 0);

        // Calculate the force on the ball from the player
        // Impulse for the instant burst
        ballrigidbody.AddForce(ShotDirection * power, ForceMode.Impulse);
    }

    // If Reset Shot set has shot back to false
    public void ResetShot()
    {
        hasshot = false;

        // Reset the animations
        if (anim != null)
        {
            anim.ResetTrigger("Shoot"); // Clear the shoot trigger
            anim.SetFloat("Speed", 0f); // return to idle
            anim.Play("Idle"); // Force the Idle animation
        }
    }

    // Start Animations
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }
}
