using UnityEngine;
using System.Collections;

public class DisableJumpTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.jumpEnabled = false;
		}
	}

	void OnTriggerExit2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.jumpEnabled = true;
		}
	}
}
