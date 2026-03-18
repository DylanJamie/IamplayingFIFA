// Unity Classes such as:
// MonoBehaviour, Vector3 and Transform
using UnityEngine;

// Script to control the logic of the Goalie!
public class KeeperController : MonoBehaviour {

    // -------- Inspector Settings -------
    [Header("Patrol")]
    // Side to side patrol speed when ball is far
    public float patrolSpeed = 2.5f;
    // How far left/right the keeper patrols from their start position
    public float patrolRange = 2.5f;

    [Header("Reaction")]
    // How close the ball needs to be before the keeper reacts
    public float reactDistance = 12f;
    // Delay before the keeper starts tracking the ball (simulates human reaction time)
    public float reactionDelay = 0.2f;
    // How fast the keeper moves to save once reacting
    public float saveSpeed = 6f;

    [Header("Difficulty")]
    // Max random offset added to keeper guess (higher = easier for player)
    public float maxGuessError = 1.2f;

    [Header("References")]
    public Transform ball;
    public PlayerController player;

    [Header("Animation")]
    public Animator keeperAnimator;

    // ---------- Private State ----------

    // The keeper's original position saved at game start
    private Vector3 startPosition;
 
    // Patrol direction: 1 = right, -1 = left
    private int movementDirection = 1;
 
    // Reaction timer counts up once ball is close
    private float reactionTimer = 0f;
 
    // The keeper's guess for where the ball is going
    // Set once when the shot is taken, not updated every frame
    private float shotGuessX = 0f;
    private bool hasGuessed = false;
 
    // Keeper state machine
    private enum KeeperState { Patrolling, Reacting, Saving }
    private KeeperState state = KeeperState.Patrolling;

    // Unity Life Cycle
    void Start() {
        startPosition = transform.position;
    }
 
    void Update() {
        UpdateState();
 
        switch (state) {
            case KeeperState.Patrolling: Patrol(); break;
            case KeeperState.Reacting:  React();   break;
            case KeeperState.Saving:    Save();    break;
        }
	Debug.Log($"Keeper State: {state}"); // Bro is always reacting
    }

    // ------------ State Machine ------------

    void UpdateState() {
        bool ballIsClose = ball.position.z >= reactDistance;
        bool playerHasShot = player != null && player.HasShot;
	
        if (playerHasShot) {
            // Once the player shoots lock in a guess and go to saving
            if (!hasGuessed) {
                shotGuessX = ball.position.x + Random.Range(-maxGuessError, maxGuessError);
                hasGuessed = true;
            }
            state = KeeperState.Saving;
        } else if (ballIsClose) {
            state = KeeperState.Reacting;
        } else {
            // Ball far away go back to patrol
            state = KeeperState.Patrolling;
            reactionTimer = 0f;
            hasGuessed = false;
        }
	// Push state to Animator every Frame
	UpdateAnimator();
    }

    // All Animation Logic
    void UpdateAnimator() {
	if (keeperAnimator == null) return;

	// Speed drives idle vs shuffle animation
	float directionalSpeed = 0f;

	// if the goalie is in patrol state
	if (state == KeeperState.Patrolling || state == KeeperState.Reacting) {
	    // Patrol direction is 1 right or -1 left
	    directionalSpeed = movementDirection * patrolSpeed;
	}
	
	keeperAnimator.SetFloat("Speed", directionalSpeed);
	keeperAnimator.SetBool("IsSaving", state == KeeperState.Saving);
    }

    // --------- Keeper Behaviours -----------
    // side to side patrol for the goalie.
    // this is the distance that the goalie can run back and forth
    void Patrol() {
        float newX = transform.position.x + movementDirection * patrolSpeed * Time.deltaTime;
 
        // Flip direction at patrol range limits
        if (Mathf.Abs(newX - startPosition.x) >= patrolRange) {
            movementDirection *= -1;
        }
	
        MoveToX(newX);
    }

    // Tracks the ball slowly when it gets close but player hasnt shot yet
    // Simulates the keeper setting their feet
    void React() {
        reactionTimer += Time.deltaTime;

	float targetX = 0f;
	
        if (reactionTimer >= reactionDelay) {
            // Gradually track the balls X, slower than saving to feel natural
            targetX = Mathf.Lerp(transform.position.x, ball.position.x, Time.deltaTime * 3f);
            MoveToX(targetX);
        }

	// Calculate the diffence in position to see which way the goalie needs to move.
	float difference_in_position = targetX - transform.position.x;

	// See which way the player is moving and update movement direction
	Debug.Log("Target X: " + targetX);
	Debug.Log("difference in pos: " + difference_in_position);
	Debug.Log("X pos: " + transform.position.x);
	
	// If the ball is moving to the left move to the left so side step right because of inverse
	// if ball is moving right move right side step left
	// otherwise == 0
	if (difference_in_position > 0) {
	    movementDirection = 1;
		} else if (difference_in_position < 0) {
	    movementDirection = -1;
	} else {
	    movementDirection = 0;
	}

	Debug.Log(movementDirection);
    }

    // Dives to the guessed save position after player shoots
    void Save() {
        float newX = Mathf.MoveTowards(transform.position.x, shotGuessX, saveSpeed * Time.deltaTime);
        MoveToX(newX);
    }

    // ----------- Helpers -------------
    // Moves the keeper only on the X axis, preserving the Y and Z axis
    void MoveToX(float targetX) {
        transform.position = new Vector3(
            targetX,
            transform.position.y,
            transform.position.z
        );
    }

    // Called by GoalManager when resetting positions
    // Resets keeper back to start and clears all state
    public void ResetKeeper() {
        transform.position = startPosition;
        state = KeeperState.Patrolling;
        reactionTimer = 0f;
        hasGuessed = false;
        shotGuessX = 0f;
        movementDirection = 1;

	// reset the animation states
	if (keeperAnimator != null) {
	    keeperAnimator.SetFloat("Speed", 0f);
	    keeperAnimator.SetBool("IsSaving", false);
	}
    }
}
 
    
