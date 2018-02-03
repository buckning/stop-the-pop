using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyCane : MonoBehaviour {
	
	public Animator myAnimator;

	float timeOfLastCollision = 0.0f;

	void OnCollisionEnter2D(Collision2D otherObject) {
		if ((Time.time - timeOfLastCollision) > 1) {
			if (otherObject.gameObject.tag == Strings.PLAYER) {
				PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();	
				if (player.GetVelocity ().y <= 3.0f && player.IsGrounded()) {
					myAnimator.SetTrigger ("Bounce");
					timeOfLastCollision = Time.time;
				}
			}
		}
	}
}
