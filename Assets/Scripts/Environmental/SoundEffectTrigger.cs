using UnityEngine;
using System.Collections;

/***
 * Trigger that enables or disables sfx 
 */
public class SoundEffectTrigger : MonoBehaviour {

	public SoundEffectPlayer sfxPlayer;

	public void OnTriggerEnter2D(Collider2D coll) {
		if (coll.gameObject.tag == "Player") {
			sfxPlayer.EnableSfx ();
		}
	}

	public void OnTriggerExit2D(Collider2D coll) {
		if (coll.gameObject.tag == "Player") {
			sfxPlayer.DisableSfx ();
		}
	}
}
