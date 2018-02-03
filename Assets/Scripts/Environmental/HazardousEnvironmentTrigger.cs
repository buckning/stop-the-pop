using UnityEngine;
using System.Collections;

public class HazardousEnvironmentTrigger : MonoBehaviour {
	/***
	 * If there is a collision with the player, increase the temperature
	 * and delete this object
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.CollisionWithHazardousEnvironment (gameObject.name);
			Destroy(gameObject);	
		}
	}
}
