using UnityEngine;
using System.Collections;

/***
 * Respawn (move) the player when they interact with this object.
 * This is useful for bottomless areas of the map. If the player hits the trigger,
 * they are positioned at the respawn position. 
 */
public class PlayerRespawnTrigger : MonoBehaviour {
	public HudListener hud;

	public void Start() {
		if (hud == null) {
			hud = GameObject.Find (HudListener.gameObjectName).GetComponent<HudListener>();
		}
	}

	/***
	 * Reposition the player when they hit this trigger
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			CurrentLevel.AddLivesLost (1);
			hud.RetryLevel();
		}
	}
}
