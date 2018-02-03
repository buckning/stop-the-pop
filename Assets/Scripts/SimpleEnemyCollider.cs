using UnityEngine;
using System.Collections;

/***
 * Simple collider script that notifies the player that there has been a collision.
 * After the notification occurs, this object is destroyed. 
 */
public class SimpleEnemyCollider : MonoBehaviour {
	public float damageToPlayer = 30f;

	public bool destroyAfterCollision = false;
	public bool disableAfterCollision = false;

	/***
	 * If there is a collision with the player, increase the temperature
	 * and delete this object
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.CollisionWithEnemy(gameObject.name, damageToPlayer, false);	
			if (destroyAfterCollision) {
				Destroy (gameObject);
			}
		}
		if (disableAfterCollision && otherObject.gameObject.tag == Strings.PLAYER || otherObject.gameObject.tag == Strings.TERRAIN) {
			gameObject.SetActive (false);
		}
	}
}
