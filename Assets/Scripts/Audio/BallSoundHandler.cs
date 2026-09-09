// BallSoundHandler.cs
// this will change and allow the ball to make noises as it is being kicked

using UnityEngine;

public class BallSoundHandler : MonoBehaviour {
    // audio clips and sources
    public AudioClip ballHitSound;
    public AudioClip netSound;
    public AudioClip postSound;

    // Net sound cooldown
    private float lastGoal = -10f;
    public float NetsoundCooldown = 1.0f;
    
    private AudioSource audioSource;

    void Start() {
	audioSource = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision) {
	// If the magnitude is greater than 2 play a sound
	if (collision.relativeVelocity.magnitude > 2f) {
	    GameObject hit = collision.gameObject;

	    // did it hit the post or just the ground or defender/goalie
	    if (hit.CompareTag("Post")) {
		audioSource.PlayOneShot(postSound);
	    } else {
		audioSource.PlayOneShot(ballHitSound);
	    }
	}
    }

    // Trigger if it hits the net
    void OnTriggerEnter(Collider other) {	
	if (other.CompareTag("Net")) {
	    if (Time.time >= lastGoal + NetsoundCooldown) {
		audioSource.PlayOneShot(netSound);
		lastGoal = Time.time;
	    }
	}
    }
}
