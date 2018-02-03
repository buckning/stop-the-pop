using UnityEngine;
using System.Collections;

public class SteamBehaviour : MonoBehaviour {

	/***
	 * If there is a collision with the player, make their glide speed opposite their default glide speed
	 * So colliding with this object will make the player float upwards
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.glideSpeed = PlayerController.defaultGlideSpeed * -1;
		}
	}

	/***
	 * If there is a collision with the player, make their glide speed opposite their default glide speed
	 * So colliding with this object will make the player float upwards
	 */
	void OnTriggerExit2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.glideSpeed = PlayerController.defaultGlideSpeed;
		}
	}
}
