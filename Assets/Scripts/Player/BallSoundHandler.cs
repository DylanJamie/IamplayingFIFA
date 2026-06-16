// BallSoundHandler.cs
// this will change and allow the ball to make noises as it is being kicked

using UnityEngine;

public class BallSoundHandler : MonoBehaviour {
    // audio clips and sources
    public AudioClip ballHitSound;
    private AudioSource audioSource;

    void Start() {
	audioSource = GetComponent<AudioSource>();
    }

    // when the ball hits anything we play a soccer ball sound
    void OnCollisionEnter(Collision collision) {
	Debug.Log("Ball Hit Something: " + collision.relativeVelocity.magnitude);
	if (collision.relativeVelocity.magnitude > 2f) {
	    audioSource.PlayOneShot(ballHitSound);
	}
    }
}
