// Unity Classes such as:
// MonoBehaviour, Vector3 and Transform
using UnityEngine;

// Unities MonoBehaviour behavior Class
// MonoBehaviour == allows a person to attach the script to a game object
public class CameraFollow : MonoBehaviour
{
    // Player is a reference to another object within the game/schene 
    // Transform is like a stuct that contains Position, rotation and scale for an object
    public Transform player;
    // offset is a 3D vector that tells the app how far the camera should be from the player
    public Vector3 offset;

    // Another special unity function that is called after all Update() calls are done
    void LateUpdate()
    {
        // Sets the cameras positon based on the players position and the offset value
        transform.position = player.position + offset;
        // Rotates the camera to always face the player
        transform.LookAt(player);
    }
}