using UnityEngine;
using System.Collections;

/***
 * Gas Dispenser has multiple flames. The flames have an off and on period and this toggles class 
 * toggles them on or off. 
 */
public class GasDispenser : SoundEffectPlayer {

	public GameObject[] flames;

	public float flameOnTime = 2f;	//the amount of time that the flames are active
	public float flameOffTime = 2f;	//the amount of time that the flames are deactivated

	public float animationStartTime = 0.0f;

	void Start() {
		GetComponent<Animator>().Play("GasDispenserFace", 0, animationStartTime);
	}

	/***
	 * Check if the flames are active. This is assuming if the first in the list is 
	 * active, they all are
	 */
	bool AreFlamesActive() {
		foreach (GameObject flame in flames) {
			return flame.activeInHierarchy;
		}
		return false;
	}

	public void TurnFlamesOn() {
		SetFlamesOn (true);
	}

	public void TurnFlamesOff() {
		SetFlamesOn (false);
	}

	/***
	 * Set the flames either on or off. 
	 */
	void SetFlamesOn(bool flamesOn) {
		foreach (GameObject flame in flames) {
			if (flamesOn && playSound) {
				AudioManager.PlaySound ("flame-dispenser");
			}
			flame.SetActive (flamesOn);
		}
	}
}
