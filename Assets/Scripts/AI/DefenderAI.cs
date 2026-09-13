// DefenderAI.cs

// File doesn't do anything but file and give instruction based on what DefenseManager Tells this AI Defender todo

using UnityEngine;

public class DefenderAI : MonoBehaviour {

    // variables
    //
    public enum AIState { Zonal, Marking, Pressing }

    [Header("Role")]
    public PlayerRole role;
    // formation spot this defender holds by default
    public Transform homeAnchor;
    public float zonalShiftFactor = 0.3f;
    public float markRadius = 8f;

    [Header("Movement")]
    public float baseSpeed = 8f;
    public float maxSpeed = 14f;

    [Header("References")]
    public Transform ball;
    public GoalManager goalManager;
    public PlayerSwitcher playerSwitcher;
    public float dribbleDistance = 0.85f;
    public float dribbleHeight = 0.2f;
    public float dribbleSmoothSpeed = 12f;

    public Vector3 HomePosition => homeAnchor != null ? homeAnchor.position : transform.position;

    private AIState state = AIState.Zonal;
    private PlayerController markTarget;
    private Animator anim;
    private Rigidbody body;
    private Rigidbody ballRb;
    private Vector3 ballVelocity;
    private bool isResetting = false;

    // Execute on start up of obj
    // defender will find the ball which is tagged
    void Awake() {
        if (ball == null) {
            GameObject b = GameObject.FindGameObjectWithTag("Ball");

	    if (b != null)
		ball = b.transform;
        }
    }

    // called on start of the game
    // basic get the animator and rigidbody
    void Start() {
        anim = GetComponentInChildren<Animator>();
        body = GetComponent<Rigidbody>();
        if (ball != null)
	    ballRb = ball.GetComponent<Rigidbody>();
    }
    
    // Set the state machine up and set the marked target as variables
    public void SetState(AIState newState) => state = newState;
    public void SetMarkTarget(PlayerController target) => markTarget = target;

    // Update every frame
    // check if the defender is close enough to the ball to make a tackle
    // have the defender move toward the target position based on its current state
    void Update() {
        if (ball == null || isResetting)
	    return;

        if (state == AIState.Pressing && Vector3.Distance(transform.position, ball.position) <= 1.4f) {
            TackleBall();
            return;
        }

        Vector3 targetPos = GetTargetPositionForState();
        MoveTowards(targetPos);
    }

    // state machine set
    // defender will be in a pressing, marking, or zonal state
    // pressing - the defender is close enough to the ball where he should be trying to activaly take the ball
    // marking - defender is not closest to the ball but is near an attacker and marks him to block off pass
    // zonal - default stick to position wait for state to change travel with team
    // Home position is their positions point where they sit on the field
    Vector3 GetTargetPositionForState() {
        switch (state) {
            case AIState.Pressing:
                return ball.position;
            case AIState.Marking:
                if (markTarget != null)
                    return markTarget.transform.position + (HomePosition - markTarget.transform.position).normalized * 1.5f;
                return HomePosition;
            default: // Zonal
                Vector3 shift = (ball.position - HomePosition) * zonalShiftFactor;
                return HomePosition + shift;
        }
    }

    // Move towards
    // move the defender in the desired position based on state
    void MoveTowards(Vector3 targetPos) {

	// here calculate the distance again (redundent but will fix in future) 
        float distance = Vector3.Distance(transform.position, targetPos);
        if (distance < 0.3f) {
            if (anim != null) anim.SetFloat("isRunning", 0f);
            return;
        }

	// see if pressing state and if so max speed at the ball
        Vector3 direction = (targetPos - transform.position).normalized;
        float speed = state == AIState.Pressing ? maxSpeed : baseSpeed;

	// limits on where defenders can run
        Vector3 newPos = transform.position + direction * speed * Time.deltaTime;
        newPos.y = transform.position.y;
        newPos.x = Mathf.Clamp(newPos.x, -46f, 46f);
        newPos.z = Mathf.Clamp(newPos.z, -133f, 0f);
        body.MovePosition(newPos);

	// have the defender rotate as he chases the player
        Vector3 lookDir = direction; lookDir.y = 0;
        if (lookDir != Vector3.zero) {
            Quaternion targetRotation = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 5f);
        }

	// set the animation to running
        if (anim != null)
	    anim.SetFloat("isRunning", 1f);
    }

    // tackle and take the ball off the player
    void TackleBall() {
        if (!isResetting) {
            isResetting = true;
            if (playerSwitcher != null && playerSwitcher.ActivePlayer != null)
                playerSwitcher.ActivePlayer.SetPossession(false);
            if (goalManager != null)
		goalManager.BallSteal();
        }

	// set kinematic on the ball
        if (ballRb != null)
	    ballRb.isKinematic = true;

	// set the target position for after talking the ball
        Vector3 targetPos = transform.position + transform.forward * dribbleDistance + transform.right * 0.2f + Vector3.up * dribbleHeight;
        ball.position = Vector3.SmoothDamp(ball.position, targetPos, ref ballVelocity, 1f / dribbleSmoothSpeed);

	// push the animation to idle after taking the ball
        if (anim != null)
	    anim.SetFloat("isRunning", 0f);
    }

    // Reset the defender
    // set the state back to zonal 
    public void ResetDefender() {
	isResetting = false;
	ballVelocity = Vector3.zero;
	state = AIState.Zonal;
	markTarget = null;

	if (homeAnchor != null) {
	    transform.position = homeAnchor.position;
	    transform.rotation = homeAnchor.rotation;
	}

	if (body != null) {
	    body.linearVelocity = Vector3.zero;
	    body.angularVelocity = Vector3.zero;
	}
    }
}
