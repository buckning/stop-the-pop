using UnityEngine;
using System.Collections;

/***
 * Idea of this class is that another class would extend it and use the playSound variable.
 * The SoundEffectTrigger class can then call this class to enable or disable sounds
 */
public class SoundEffectPlayer : MonoBehaviour {

	protected bool playSound = false;

	public void EnableSfx() {
		playSound = true;
	}

	public void DisableSfx() {
		playSound = false;
	}
}
