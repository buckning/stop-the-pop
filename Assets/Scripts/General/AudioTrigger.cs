using UnityEngine;
using System.Collections;

/***
 * Class that plays an audio clip when the player is colliding with this object.
 * When the player stops colliding with this object, the audio clip is stopped
 */
[RequireComponent (typeof(AudioSource))]
public class AudioTrigger : MonoBehaviour {

	AudioSource audioSource;

	void Start() {
		audioSource = GetComponent<AudioSource> ();
	}

	public void OnTriggerEnter2D(Collider2D other) {
		audioSource.Play ();
	}

	public void OnTriggerExit2D(Collider2D other) {
		audioSource.Stop ();
	}
}
