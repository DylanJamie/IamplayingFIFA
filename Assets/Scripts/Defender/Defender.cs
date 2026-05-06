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
    // public PlayerController playerController;
    
    // ----- Private State ------
    // For Animations
    private Animator anim;
    private Rigidbody defender_body;

    // Start of the game function
    void Awake() {
	// follow the Ball
	GameObject b = GameObject.FindGameObjectWithTag("Ball");
	if (b != null) {
	    ball = b.transform;
	}    
    }

    // Start Animations
    void Start() {
        anim = GetComponentInChildren<Animator>();
	defender_body = GetComponent<Rigidbody>();
    }


    // Update is called once per frame
    void Update() {
	if (ball == null) {
	    return;
	}

	// Move Towards the target
	Vector3 direction = (ball.position - transform.position).normalized;
	float distance = Vector3.Distance(transform.position, ball.position);
	
	// Move to the desired postion based 
	if (distance > 0.3f) {
	    Vector3 newPos = transform.position + direction * speed;
	    newPos.y = transform.position.y;

	    // Clamp the defender to the field
	    newPos.x = Mathf.Clamp(newPos.x, -46f, 46f);
	    newPos.z = Mathf.Clamp(newPos.z, -133f, 0f);

	    // Move to target
	    defender_body.MovePosition(newPos);

	    // Face the Ball
	    Quaternion targetRotation = Quaternion.LookRotation(direction);
	    transform.rotation = Quaternion.RotateTowards(
		transform.rotation,
		targetRotation,
		5f
	    );

	}

	// // Animation
	if (anim != null) {
	    anim.SetFloat("isRunning", distance);
	}
    }
}
