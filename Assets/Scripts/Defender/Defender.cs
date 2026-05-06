// Use Unity classes within C Sharp
using UnityEngine;

// using unity monobehavior
public class Defender : MonoBehaviour {
    // Difficulty
    [Header("Difficulty")]
    public float speed = 4.0f;

    // References to obj in the game
    [Header("Reference")]
    // Reference for ball
    public Transform ball;
    public PlayerController playerController;

    [Header("Patrol")]
    public Vector3 patrolCenter = new Vector3(-1.0f, 1.3f, -10.3f);
    public float chaseRange = 6f; 
    
    // ----- Private State ------
    // For Animations
    private Animator anim;
    private Rigidbody defender_body;
    private Rigidbody ballRb;

    // Start Animations
    void Start() {
        anim = GetComponentInChildren<Animator>();
	ballRb = ball.GetComponent<Rigidbody>();
	defender_body = GetComponent<Rigidbody>();
    }

    // Start of the game function
    void Awake() {
	// follow the Ball
	GameObject b = GameObject.FindGameObjectWithTag("Ball");
	if (b != null) {
	    ball = b.transform;
	}

	GameObject p = GameObject.FindGameObjectWithTag("Player");
	if (p != null)
	    playerController = p.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update() {
	if (ball == null || playerController == null)
	    return;

	// Only chase when the ball has been shot
	bool ballIsLive = playerController.HasShot;
	float distToBall = Vector3.Distance(transform.position, ball.position);

	// Variable for the target position for the defender
	Vector3 targetPos;

	// Chase logic
	if (ballIsLive && distToBall < chaseRange) {
	    // Chase the ball
	    targetPos = ball.position;
	} else {
	    // Return to patrol center (penalty spot) when the player is dribbling
	    targetPos = patrolCenter;

	}
	
	// Move Towards the target
	Vector3 direction = (targetPos - transform.position).normalized;
	float distance = Vector3.Distance(transform.position, targetPos);

	Debug.Log("dist" + distance);
	Debug.Log("Target" + targetPos);
	
	// Move to the desired postion based 
	if (distance > 0.3f) {
	    defender_body.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);

	    // Face the Ball
	    Quaternion targetRotation = Quaternion.LookRotation(direction);
	    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
	}

	// Animation
	// if (anim != null) {
	//     anim.Setbool("isRunning", distance > 0.3f);
	// }
    }
}
