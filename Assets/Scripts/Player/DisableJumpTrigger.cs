using UnityEngine;
using System.Collections;

public class DisableJumpTrigger : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			PopcornKernelController player = otherObject.gameObject.GetComponent<PopcornKernelController> ();
			player.SetJumpEnabled (false);
		}
	}

	void OnTriggerExit2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			PopcornKernelController player = otherObject.gameObject.GetComponent<PopcornKernelController> ();
			player.SetJumpEnabled (true);
		}
	}
}
