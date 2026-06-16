// BallSoundHandler.cs
// this will change and allow the ball to make noises as it is being kicked

using UnityEngine;

public class BallSoundHandler : MonoBehaviour {
    // audio clips and sources
    public AudioClip ballHitSound;
    public AudioClip netSound;
    public AudioClip postSound;
    
    private AudioSource audioSource;

    void Start() {
	audioSource = GetComponent<AudioSource>();
    }

    // when the ball hits anything we play a soccer ball sound
    void OnCollisionEnter(Collision collision) {
	if (collision.relativeVelocity.magnitude > 2f) {
	    if (collision.gameObject.CompareTag("Net")) {
		audioSource.PlayOneShot(netSound);
	    } else if (collision.gameObject.CompareTag("Post")) {
		audioSource.PlayOneShot(postSound);
	    } else {
		audioSource.PlayOneShot(ballHitSound);
	    }
	}
    }
}
