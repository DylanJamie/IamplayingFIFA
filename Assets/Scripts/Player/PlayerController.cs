// Unity Classes such as:
// MonoBehaviour, Vector3 and Transform
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

    // Private variables to store the player movement
    private PlayerInput _playerInput;
    private Vector3 _moveInput;
    
    // ----- Unity Life Cycle ------
    
    // Start Animations
    void Start() {
        anim = GetComponentInChildren<Animator>();
	ballRb = ball.GetComponent<Rigidbody>();
    }
    
    // Update is a Unity function that is run once per frame 
    void Update() {
	if (!hasShot) {
	    HandleDribble();
	}

	// Handle the power bar for shooting
	if (isCharging) {
	    currentPower += chargeSpeed * Time.deltaTime;
	    currentPower = Mathf.Clamp(currentPower, 0, maxCharge);
	    if (powerBar != null)
		powerBar.value = currentPower / maxCharge;
	}
    }
	
    // Handle all the movement for the player
    public void ProcessMove(Vector2 Input) {
	// moveX & moveY get the keyboard inputs from the user for Left/Right or Up/Down
        // Returns a value from -1 to 1 based on the input
        // new Vector3(moveX, 0, moveZ) decides which direction to move
        // inputdir creates a 3 directional movement (x, y, z) and then multiplies it by the speed
        // so if there is (0, 0, 1) * 5 == (0, 0, 5) there is no Y because we only move on the x and z axis
        // Time.delta time gets the time since the last frame Movement * frames = the amount of units needed to move
	_moveInput = new Vector3(Input.x, 0, Input.y);

        // Rotate the player to face movement direction
        // If the player is moving make them face the direction of the movement
        if (_moveInput.magnitude > 0.1f) {
	    // Smooth Rotatiton toward the movemnt
	    Quaternion targetRotation = Quaternion.LookRotation(_moveInput);
	    transform.rotation = Quaternion.RotateTowards(
		transform.rotation,
		targetRotation,
		rotationSpeed * Time.deltaTime
	    );

	    // Move the player
            transform.position += _moveInput.normalized * moveSpeed * Time.deltaTime;
        }

	// Update our animations
	if (anim != null) {
	    anim.SetFloat("Speed", _moveInput.magnitude);
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
    public void StartCharging() {
	// begin Charging
	if (!hasShot) {
	    isCharging = true;
	    currentPower = 0f;
	}
    }
    
    // For the shot release
    public void ReleaseShot() {
	// Build Power when holding
	if (isCharging) {
	    isCharging = false;
	    hasShot = true;
	    ShootBall(currentPower);

	    // Play the shooting animation
	    if (anim != null)
		anim.SetTrigger("Shoot");

	    // Reset the Power Bar
	    if (powerBar != null)
		powerBar.value = 0f;
	}
    }
    
    // Shooting the ball
    void ShootBall(float power) {
	// Tell the ball to not be attached to the player when he shoots
	hasShot = true;
	
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
