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
    public float moveSpeed = 4f;
    // How fast the player Turns to face movement direction
    public float rotationSpeed = 720f;
    public float jog_speed = 4f;
    public float sprint_speed = 8f;

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

    [Header("Passing")]
    public float passPower = 12f;

    [Header("Skills")]
    public float skillcooldown = 1f;
    private float lastSkillTime;

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

    // Is the player being tackled
    private bool isBeingTackled = false;
    
    // used to smoothe ball bob while dribbling
    private float bobTimer = 0f;

    // Velocity ref for smooth ball movement
    private Vector3 ballVelocity = Vector3.zero;

    // Private variables to store the player movement
    private PlayerInput _playerInput;
    private Vector3 _moveInput;

    // True or false for the Player Sprinting
    private bool _isSprinting = false;

    // Keep track of the time when the user has shot the ball
    private float lastShotTime;

    // Player is celebrating
    public bool _isCelebrating = false;

    // AI Control and runs
    [Header("AI/Control")]
    public bool isAIControlled = false;
    public PlayerSwitcher playerSwitcher;

    [Header("AI Support")]
    public Vector3 pendingReceivePoint;
    public bool isReceivingPass = false;

    [Header("AI Supportting Runs")]
    public float aiRunSpeed = 5f;
    public float aiSupportDistance = 6f;
    public float aiLateralOffset = 6f;

    [Header("Positioning")]
    // each player's own formation spot
    public Transform startPosition;
    
    // ----- Sounds -----
    [Header("Sounds")]
    public AudioClip dribble_sound;
    public AudioClip pass_sound;
    public AudioClip shoot_sound;
    public AudioClip footstep_sound;
    public AudioClip sui_sound;
    
    // Cooldown so the footsteps dont overlap
    private float lastFootstepTime = 0f;
    public float footstepCooldown = 0.3f;
    
    private AudioSource audio_source;
    
    // ----- Unity Life Cycle ------
    
    // Start Animations
    void Start() {
        anim = GetComponentInChildren<Animator>();
	ballRb = ball.GetComponent<Rigidbody>();

	// add the audio source
	audio_source = GetComponent<AudioSource>();
    }
    
    // Update is a Unity function that is run once per frame 
    void Update() {
	// AI will skip all the input
	if (isAIControlled) {
	    HandleAISupportRun();
	    return;
	}
	
	// Play the sound for player walking
	PlayFootstepSound();
	
	// if player has not shot or is not being tackled start dribbling
	if (hasShot == false && isBeingTackled == false) {
	    HandleDribble();
	} else {
	    if (hasShot && Time.time > lastShotTime + 0.5f) {
		// add a small delay before shoot again
		// check the distance between ball and player
		float distanceToBall = Vector3.Distance(transform.position, ball.position);
		if (distanceToBall < 1.2f) {
		    hasShot = false;
		}
	    }
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
	// Check if it is an AI player
	if (isAIControlled) {
	    return;
	}
	
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
	    float visualSpeed = _moveInput.magnitude;

	    // if we are sprinting we boost the float sent to the animator
	    // This triggers the multiplyer in our inspector
	    if (_isSprinting && visualSpeed > 0.1f) {
		visualSpeed *= 1.5f;
	    }

	    // Actually change the value in speed
	    anim.SetFloat("Speed", visualSpeed);
	}
    }

    // ------ Sprinting ------
    public void Sprinting(bool isSprinting) {
	// Check if it is an AI player
	if (isAIControlled) {
	    return;
	}	

	// Store State for sprinting for the animation
	_isSprinting = isSprinting;
	
	// Later might add energy into the game
	// If the player is holding the Shift key sprint or increase move speed
	if (isSprinting == true) {
	    moveSpeed = sprint_speed;
	}
	else if (isSprinting == false) {
	    moveSpeed = jog_speed;
	}
    }
    
    // ------ Dribbling ------
    void HandleDribble() {
        // If has shot is false
        if (hasShot) {
	    return;
	}
	    
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

	// Rolling logic while dribbling
	// since we have kinimatic as true while dribbling. this freezes the balls physics.
	if (_moveInput.magnitude > 0.1f) {
	    // calculate how much the ball shour rotate based on movement
	    // 500f is a multiplier, we can adjust this to make the ball move faster or slower
	    float multiplier = 150f;
	    float rollAmount = moveSpeed * Time.deltaTime * 500f;

	    // Rotate the ball around the player's right axis
	    ball.Rotate(transform.right, rollAmount, Space.World);
	}
    }

    // ----- Skill Moves ------
    // Step over
    public void StepOver() {
	// Cool down make sure you can do the skill
	if (Time.time < lastSkillTime + skillcooldown || hasShot)
	    return;

	lastSkillTime = Time.time;

	// Play the Step over animation
	// make sure the animation is called step over
	if (anim != null) {
	    anim.SetTrigger("StepOver");
	}

	// Add a small speed boost at the end of skill
	StartCoroutine(SkillSpeedBurst());
    }

    // Play the sui celebration
    public void PlaySuiCelebration() {
	// if the player is in the celebration allow him to hit the sui
	if (_isCelebrating == true) {
	    anim.SetTrigger("Sui");
	}

	// Reset the Speed while animations playing
	moveSpeed = 0f;

	// Delay the sound by 1.5 sec
	Invoke("PlaySuiSound", 0.7f);
    }

    // Seprate method to play the sui sound
    void PlaySuiSound() {
	// Play Sui sound
	audio_source.PlayOneShot(sui_sound);
    }	
    
    // Give Player boost after performing the skill
    private System.Collections.IEnumerator SkillSpeedBurst() {
	float originalSpeed = moveSpeed;
	moveSpeed *= 1.5f;
	yield return new WaitForSeconds(0.5f);
	moveSpeed = originalSpeed;
    }
    
    // ----- Shooting -----
    public void StartCharging() {
	// begin Charging
	if (isAIControlled || !hasShot) {
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

	    // Shooting Sound
	    audio_source.PlayOneShot(shoot_sound);
	    
	    // Reset the Power Bar
	    if (powerBar != null)
		powerBar.value = 0f;
	}
    }
    
    // Shooting the ball
    void ShootBall(float power) {
	// Tell the ball to not be attached to the player when he shoots
	hasShot = true;
	lastShotTime = Time.time;
	
	// Re enable the physics of the ball
	ballRb.isKinematic = false;
	ballRb.linearVelocity = Vector3.zero;
	ballRb.angularVelocity = Vector3.zero;

	Vector3 shotDirection = transform.forward + new Vector3(0f, shotLift, 0f);
	ballRb.AddForce(shotDirection.normalized * power, ForceMode.Impulse);
    }

    // Passing the ball to a teammate
    public void PassBall() {
	if (isAIControlled || hasShot || teammate == null) {
	    return;
	}

	// makes the ball not follow a script and gets the ball ref obj
	Rigidbody ballRbRef = ball.GetComponent<Rigidbody>();
	ballRbRef.isKinematic = false;

	// Force and direction
	Vector3 passDirection = (teammate.transform.position - ball.position);
	passDirection.y = 0.1f;
	ballRbRef.AddForce(passDirection.normalized * passPower, ForceMode.Impulse);

	teammate.isReceivingPass = true;
	teammate.pendingReceivePoint = teammate.transform.position + passDirection.normalized * 5f; // rough landing estimate
	
	// Release all ball physics
	ReleaseBallPhysics();

	// Target transform for the pass dirction
	target.isReceivingPass = true;
	target.pendingReceivePoint = target.transform.position + passDirection.normalized * 5f;
	
	playerSwitcher.BallReleased(this);

	// Shooting Sound
	audio_source.PlayOneShot(pass_sound);
	
    }
	
    
    // Tell the Player Controller that they scored this way the player can celebrate
    public void PlayCelebration() {
	_isCelebrating = true;
	
	if (anim != null) {
	    // Trigger so it starts the animation immediately
	    anim.SetTrigger("GoalCelebration");
	}

	// make speed 0 and disable the controlers
    }

    // Play the footsteps for the player
    public void PlayFootstepSound() {
	// Play the sound for walking forward // Needs to be Fixed chatttt
	if (_moveInput.magnitude > 0.1f) {
	    if (Time.time >= lastFootstepTime + footstepCooldown) {
		audio_source.PlayOneShot(footstep_sound);
		lastFootstepTime = Time.time;
	    }
	}
    }

    
    // Does this player have possetion of the ball
    public void SetPossession(bool hasPossession) {
	// if this player loses the ball we are being tackled
	// ! flips the opperator from true to false and false to true
	isBeingTackled = !hasPossession;

	//
	if (hasPossession == false) {
	    // Immediatly stop deribbling phusics so the ball can move more freely to the defender
	    if (ballRb != null) {
		ballRb.isKinematic = false;
	    }

	    // Reset Movement related values so the ball does not teleport back to the player
	    ballVelocity = Vector3.zero;
	    bobTimer = 0f;
	}			    
    }

    // AI Support Run
    void HandleAISupportRun() {
	// check to see if the player is recieving a pass
	if (isReceivingPass) {
	    RunToReceivePoint();
	    return;
	}

	// Support relative to whoever currently has the ball not a fixed "teammate" reference
	PlayerController ballCarrier = (playerSwitcher.ActivePlayer != null) ? playerSwitcher.ActivePlayer : null;
	if (ballCarrier == null) {
	    // Ball is loose and nobody's receiving — chase the ball itself, not each other (Will need to assure that this doesnt make all the teammates go to the ball)
	    Vector3 loosePos = ball.position;
	    transform.position = Vector3.MoveTowards(transform.position, loosePos, aiRunSpeed * Time.deltaTime);
	    FaceTowards(loosePos);
	    return;
	}

	Vector3 targetPos = teammate.transform.position + teammate.transform.forward * aiSupportDistance + teammate.transform.right * aiLateralOffset;

	// Clamp the teammate between these values
	targetPos.x = Mathf.Clamp(targetPos.x, -46f, 46f);
	targetPos.z = Mathf.Clamp(targetPos.z, -133f, 0f);

	// Movetowards the target (transform pos, targetpos, step)
	transform.position = Vector3.MoveTowards(transform.position, targetPos, aiRunSpeed * Time.deltaTime);

	// calculate the direction that the AI player looks
	Vector3 look_direction = targetPos - transform.position;
        FaceTorwards(targetPos);
    }

    // if the player is reciveing he will run to the recieving point
    void RunToReceivePoint() {
	transform.position = Vector3.MoveTowards(transform.position, pendingReceivePoint, aiRunSpeed * Time.deltaTime);
	FaceTowards(pendingReceivePoint);

	if (Vector3.Distance(transform.position, ball.position) < 1.2f) {
	    // arrived, CheckForPossession will pick them up from here
	    isReceivingPass = false;
	}
    }

    // if the player is at the point of the vector. Turn toward the last player
    void FaceTowards(Vector3 targetPos) {
	Vector3 look_direction = targetPos - transform.position;
	look_direction.y = 0f;
	if (look_direction.sqrMagnitude > 0.01f) {
	    transform.rotation = Quaternion.LookRotation(look_direction);
	}
    }
    
    // Resets this player back to its own start position/rotation
    public void ResetToStart() {
	if (startPosition != null) {
	    transform.position = startPosition.position;
	    transform.rotation = startPosition.rotation;
	}

	// reuse your existing shot/ball-state reset
	ResetShot();
    }

    // Called when this player voluntarily releases the ball (e.g. passing).
    // Unlike SetPossession(false), this has nothing to do with being tackled.
    void ReleaseBallPhysics() {
	if (ballRb != null) {
	    ballRb.isKinematic = false;
	}
	
	ballVelocity = Vector3.zero;
	bobTimer = 0f;
    }
    
    // -------- Public Method -----------
    // Called by the goal manager to rest the player after a goal or miss
    public void ResetShot() {
	hasShot = false;
	isCharging = false;
	isBeingTackled = false;
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
