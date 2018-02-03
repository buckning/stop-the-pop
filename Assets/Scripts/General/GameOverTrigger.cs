using UnityEngine;
using System.Collections;

public class GameOverTrigger : MonoBehaviour {

	HudListener hud;

	void Start () {
		if (hud == null) {
			hud = GameObject.Find (HudListener.gameObjectName).GetComponent<HudListener>();
		}
	}

	void OnTriggerEnter2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.AddLifeLost ();
			hud.RetryLevel ();
		}
	}
}
