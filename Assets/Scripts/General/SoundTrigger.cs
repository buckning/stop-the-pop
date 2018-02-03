using UnityEngine;
using System.Collections;

public class SoundTrigger : MonoBehaviour {

	public string soundToPlay;
	public string otherObjectName;		//the tag of the other object that would cause a sound to be played

	public bool oneShot = true;

	bool wasTriggered = false;

	public void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.name == otherObjectName && !wasTriggered) {
			AudioManager.PlaySound (soundToPlay);
			wasTriggered = true;
		}
	}

	public void Reset() {
		wasTriggered = false;
	}
}
