using UnityEngine;
using System.Collections;

/***
 * Triggers if the player is colliding with the terrain. The idea is to have a collider on each side of the
 * player, so when the player collides with a wall, a flag in the player is set. 
 * When the player is not colliding with a wall, the flag is cleared.
 */
public class PlayerWallTrigger : MonoBehaviour {

	private bool isColliding = false;

	void OnTriggerEnter2D(Collider2D coll) {

		if(coll.gameObject.tag == "Terrain" || coll.gameObject.tag == Strings.MOVING_PLATFORM || coll.gameObject.tag == "Marshmallow") {
			isColliding = true;
		}
	}

	void OnTriggerExit2D(Collider2D coll) {
		if(coll.gameObject.tag == "Terrain" || coll.gameObject.tag == Strings.MOVING_PLATFORM || coll.gameObject.tag == "Marshmallow") {
			isColliding = false;
		}
	}

	public void Reset() {
		isColliding = false;
	}

	public bool isCollidingWithWall() {
		return isColliding;
	}
}
