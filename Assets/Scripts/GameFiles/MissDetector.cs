// Unity class
// MonoBehavior
using UnityEngine;

// Miss Detector that mirrors the Goal detector
public class MissDetector : MonoBehaviour {
    // Goal Manager Variable
    public GoalManager goalManager;

    // Logic for when the ball touches the miss space
    private void OnTriggerEnter(Collider other) {
	if (other.CompareTag("Ball")) {
	    Debug.Log("Shot Missed!");
	    goalManager.GoalMissed();
	}
    }
}
