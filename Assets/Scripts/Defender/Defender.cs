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

    // ----- Private State ------
    // For Animations
    private Animator anim;
    private Rigidbody ballRb;

    // Start Animations
    void Start() {
        anim = GetComponentInChildren<Animator>();
	ballRb = ball.GetComponent<Rigidbody>();
    }

    // Start of the game function
    void Awake() {
	// follow the Ball
	GameObject b = GameObject.FindGameObjectWithTag("Ball");
	if (b != null) {
	    ball = b.transform;
	}
    }

    // Update is called once per frame
    void Update() {
	if (ball == null)
	    return;

	// Move Towards the ball
	Vector3 direction = (ball.position - transform.position).normalized;
	Vector3 newPos = transform.position + direction * speed * Time.deltaTime;
	ballRb.MovePosition(newPos);

	// Face the Ball
	if (direction != Vector3.zero) {
	    Quaternion targetRotation = Quaternion.LookRotation(direction);
	    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
	}

	// Animation
	bool isMoving = direction.magnitude > 0.1f;
	// anim.Setbool("isRunning", isMoving);
    }
}
