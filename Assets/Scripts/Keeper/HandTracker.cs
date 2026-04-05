using UnityEngine;

// Attach to an empty GameObject parented to Keeper_Mesh
// Drag the hand bone from the Keeper's armature into handBone
public class HandTracker : MonoBehaviour {
    
    [Header("References")]
    // Drag the hand bone from Keeper armature
    public Transform handBone;

    // Late update runs after the animation update
    void LateUpdate() {
        // LateUpdate runs AFTER animations update
        // So we always get the correct hand position this frame
        if (handBone != null) {
            transform.position = handBone.position;
            transform.rotation = handBone.rotation;
        }
    }
}
