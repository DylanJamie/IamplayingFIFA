// Unity Classes such as:
// MonoBehaviour, Vector3 and Transform
using UnityEngine;
using UnityEngine.UI;

// Unities MonoBehaviour behavior Class
// MonoBehaviour == allows a person to attach the script to a game object
public class PlayerController : MonoBehaviour
{
    // ----- Inspector Settings -----
    
    // speed variable is movement speed so here we are defining a 5 float speed
    [Header("Movement")]
    public float moveSpeed = 6f;
    // How fast the player Turns to face movement direction
    public float rotationSpeed = 720f;

    [Header("Dribbling")]
    // How far in front of the player the ball sits while dribbling
    public float dribbleDistance = 0.85f;
    // How high off the ground the ball sits while dribbling
    public float dribbleHeight = 0.2f;
    // How smooth the ball follows the player (Higher this value the Snappier)
    public float dribbleSmoothSpeed = 12f;
    // How much the Ball Bob up and down while dribbling
    public float dribbleBobAmount = 0.08f;
    // How fast the ball bobs while Dribbling
    public float dribbleBobSpeed = 8f;

    [Header("Shooting")]
    public float maxCharge = 30f;
    public float chargeSpeed = 15f;
    // Slight Upward angle on shots
    public float shotLift = 0.15f;

    [Header("Reference")]
    // Reference for ball
    public Transform ball;
    public Slider powerBar;

    // ----- Private State ------
    // For Animations
    private Animator anim;
    private Rigidbody ballRb;

    private bool hasShot = false;
    public bool HasShot => hasShot;

    private bool isCharging = false;
    private float currentPower = 0f;

    // used to smoothe ball bob while dribbling
    private float bobTimer = 0f;

    // Velocity ref for smooth ball movement
    private Vector3 ballVelocity = Vector3.zero;

    // ----- Unity Life Cycle ------
    
    // Start Animations
    void Start() {
        anim = GetComponentInChildren<Animator>();
	ballRb = ball.GetComponent<Rigidbody>();
    }
    
    // Update is a Unity function that is run once per frame 
    void Update() {
	HandleMovement();
	HandleDribble();
	HandleShooting();
	HandleAnimations();
    }
	
    // Handle all the movement for the player
    void HandleMovement() {
	// moveX & moveY get the keyboard inputs from the user for Left/Right or Up/Down
        // Returns a value from -1 to 1 based on the input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // new Vector3(moveX, 0, moveZ) decides which direction to move
        // inputdir creates a 3 directional movement (x, y, z) and then multiplies it by the speed
        // so if there is (0, 0, 1) * 5 == (0, 0, 5) there is no Y because we only move on the x and z axis
        // Time.delta time gets the time since the last frame Movement * frames = the amount of units needed to move
        Vector3 inputDir = new Vector3(moveX, 0, moveZ);

        // Rotate the player to face movement direction
        // If the player is moving make them face the direction of the movement
        if (inputDir.magnitude > 0.1f) {
	    // Smooth Rotatiton toward the movemnt
	    Quaternion targetRotation = Quaternion.LookRotation(inputDir);
	    transform.rotation = Quaternion.RotateTowards(
		transform.rotation,
		targetRotation,
		rotationSpeed * Time.deltaTime
	    );

	    // Move the player
            transform.position += inputDir.normalized * moveSpeed * Time.deltaTime;
        }
    }

    // ------ Dribbling ------
    void HandleDribble() {
        // If has shot is false
        if (hasShot) return;

	// Freeze the ball Physics while dribbling so the ball does not roll away
	ballRb.isKinematic = true;

	// Bob the ball up and down slightly for a natural feel
	bobTimer += Time.deltaTime * dribbleBobSpeed;
	float bob = Mathf.Sin(bobTimer) * dribbleBobAmount;

	// Target Position slightly in front and to the right
	// Offset to the right slightly so the ball is not in the players legs
	Vector3 targetPos = transform.position + transform.forward * dribbleDistance + transform.right * 0.2f + Vector3.up * (dribbleHeight + bob);

	// Smooth Damp for natural ball movement that lags slightly behind the player
	ball.position = Vector3.SmoothDamp(
	    ball.position,
	    targetPos,
	    ref ballVelocity,
	    1f / dribbleSmoothSpeed
	);
    }

    // ----- Shooting -----
    void HandleShooting() {
	// begin Charging
	if (Input.GetKeyDown(KeyCode.Space) && !hasShot) {
	    isCharging = true;
	    currentPower = 0f;
	}

	// Build Power when holding
	if (isCharging && Input.GetKey(KeyCode.Space)) {
	    currentPower = Mathf.Clamp(currentPower + chargeSpeed * Time.deltaTime, 0f, maxCharge);

	    if (powerBar != null) {
		powerBar.value = currentPower / maxCharge;
	    }
	}

	// Release and shoot the ball
	if (Input.GetKeyUp(KeyCode.Space) && isCharging) {
	    isCharging = false;
	    hasShot = true;
	    ShootBall(currentPower);

	    if (anim != null)
		anim.SetTrigger("Shoot");

	    // Reset the Power Bar
	    if (powerBar != null)
		powerBar.value = 0f;
	}
    }

    // Shooting the ball
    void ShootBall(float power) {
	// Re enable the physics of the ball
	ballRb.isKinematic = false;
	ballRb.linearVelocity = Vector3.zero;
	ballRb.angularVelocity = Vector3.zero;

	Vector3 shotDirection = transform.forward + new Vector3(0f, shotLift, 0f);
	ballRb.AddForce(shotDirection.normalized * power, ForceMode.Impulse);
    }

    // ------- Animations ------
    void HandleAnimations() {
	if (anim == null) return;

	float moveX = Input.GetAxis("Horizontal");
	float moveZ = Input.GetAxis("Vertical");
	float speed = new Vector3(moveX, 0, moveZ).magnitude;

	anim.SetFloat("Speed", speed);
    }

    // -------- Public Method -----------
    // Called by the goal manager to rest the player after a goal or miss
    public void ResetShot() {
	hasShot = false;
	isCharging = false;
	currentPower = 0f;
	bobTimer = 0f;
	ballVelocity = Vector3.zero;


	// Re enable dribbling physics lock
	if (ballRb != null)
	    ballRb.isKinematic = true;

	if (powerBar != null)
	    powerBar.value = 0f;

	if (anim != null) {
	    anim.ResetTrigger("Shoot");
	    anim.SetFloat("Speed", 0f);
	    anim.Play("Idle");
	}
    }
}
