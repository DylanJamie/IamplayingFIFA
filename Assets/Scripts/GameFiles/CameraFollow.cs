// Unity Classes such as:
// MonoBehaviour, Vector3 and Transform
using UnityEngine;

// Unities MonoBehaviour behavior Class
// MonoBehaviour == allows a person to attach the script to a game object
public class CameraFollow : MonoBehaviour
{
    [Header("References")]
    // Ball is a reference to another object within the game/schene 
    // Transform is like a stuct that contains Position, rotation and scale for an object
    public Transform ball;
    public PlayerSwitcher playerSwitcher;

    [Header("Celebrations")]
    public bool showCelebration = false;

    [Header("Offsets")]
    // offset is a 3D vector that tells the app how far the camera should be from the player
    public Vector3 ballOffset;
    public Vector3 playerOffset;

    [Header("Settings")]
    public float followSmoothSpeed = 8f;
    
    // Another special unity function that is called after all Update() calls are done
    void LateUpdate() {
	PlayerController activePlayer = playerSwitcher.ActivePlayer;

	// once the player has shot lock on to the target so we can see the celebration
	if (activePlayer != null && showCelebration) {
            FollowTarget(activePlayer.transform, playerOffset);
	} else {
	    FollowTarget(ball.transform, ballOffset);
	}
    }

    // Called by GoalManager once the playerScores
    public void SetCelebrationView(bool active) {
	showCelebration = active;
    }
    
    // Follow target function
    void FollowTarget(Transform target, Vector3 offset) {
        Vector3 desiredPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPos, followSmoothSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
}
	    

