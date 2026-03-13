// Unity Classes such as:
// MonoBehaviour, Vector3 and Transform
using UnityEngine;

// Goal Trigger sees if the ball collides with the space to make a goal
public class GoalTrigger : MonoBehaviour
{
    // goal Manager Variable
    public GoalManager goalManager;
    // What happens when ball touches this space logic
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            // Print goal in the logs
            Debug.Log("Goal!!");

            // call out goal scored function
            goalManager.GoalScored();

            // Eventually score logic will go here
        }
    }
}
