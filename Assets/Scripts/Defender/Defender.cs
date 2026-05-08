// Use Unity classes within C Sharp
using UnityEngine;

// using unity monobehavior
public class Defender : MonoBehaviour {
    // Difficulty
    [Header("Difficulty")]
    public float speed = 0.05f;

    // References to obj in the game
    [Header("Reference")]
    // Reference for ball
    public Transform ball;
    public PlayerController playerController;
    // How far in front of the player the ball sits while dribbling
    public float dribbleDistance = 0.85f;
    // How high off the ground the ball sits while dribbling
    public float dribbleHeight = 0.2f;
    // How smooth the ball follows the player (Higher this value the Snappier)
    public float dribbleSmoothSpeed = 12f;

    // Goal manager
    public GoalManager goalManager;
    
    // ----- Private State ------
    // For Animations
    private Animator anim;
    private Rigidbody defender_body;
    private Rigidbody ballRb;
    private Vector3 ballVelocity;
    private bool isResetting = false;
    
    // Start of the game function
    void Awake() {
	// Grab the transform of the Ball
	GameObject b = GameObject.FindGameObjectWithTag("Ball");
	if (b != null) {
	    ball = b.transform;
	}

    }

    // Start Animations
    void Start() {
        anim = GetComponentInChildren<Animator>();
	defender_body = GetComponent<Rigidbody>();

	// Get the balls rigid body when the game starts
	if (ball != null) {
	    ballRb = ball.GetComponent<Rigidbody>();
	}
    }

    // Update is called once per frame
    void Update() {
	if (ball == null || playerController == null) {
	    return;
	}

	// Move Towards the target
	Vector3 direction = (ball.position - transform.position).normalized;
	float distance = Vector3.Distance(transform.position, ball.position);
	
	// Move to the desired postion based 
	if (distance > 1.4f) {
	    Vector3 newPos = transform.position + direction * speed;
	    newPos.y = transform.position.y;

	    // Clamp the defender to the field
	    newPos.x = Mathf.Clamp(newPos.x, -46f, 46f);
	    newPos.z = Mathf.Clamp(newPos.z, -133f, 0f);

	    // Move to target
	    defender_body.MovePosition(newPos);

	    // lock so defeder only rotates on Y axis
	    Vector3 lookDir = direction;
	    lookDir.y = 0;

	    if (lookDir != Vector3.zero) {
		    // Face the Ball
		    Quaternion targetRotation = Quaternion.LookRotation(lookDir);
		    transform.rotation = Quaternion.RotateTowards(
			transform.rotation,
			targetRotation,
			5f
		    );
		}
	} else {
	    // Tackling logic

	    if (isResetting == false) {
		isResetting = true;
		goalManager.BallSteal();
	    }
	    
	    // Tell the player script they lost the
	    // use the reference to the Player controller
	    if (playerController.HasShot == false) {
		playerController.ResetShot();
	    }

	    if (ballRb != null) {
		// Freeze the ball Physics while dribbling so the ball does not roll away
		ballRb.isKinematic = true;
	    }
	    
	    // Move the ball to the Defendered feet
	    Vector3 targetPos = transform.position + transform.forward * dribbleDistance + transform.right * 0.2f + Vector3.up * dribbleHeight;

	    // Noe ref ball was missing, use a dummy vector
	    // Smooth Damp for natural ball movement that lags slightly behind the player (You can define a velocity at the top)
	    ball.position = Vector3.SmoothDamp(
		ball.position,
		targetPos,
		ref ballVelocity,
		1f / dribbleSmoothSpeed
	    );

	}
	    
	// // Animation
	if (anim != null) {
	    anim.SetFloat("isRunning", distance);
	}
    }
}
