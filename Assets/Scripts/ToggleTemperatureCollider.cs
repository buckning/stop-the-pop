using UnityEngine;
using System.Collections;

public class ToggleTemperatureCollider : MonoBehaviour {

	/***
	 * If there is a collision with the player, decrease the temperature
	 * and delete this object
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.updateTemperature = true;	
		}
	}
}
