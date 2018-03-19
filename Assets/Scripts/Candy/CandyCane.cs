using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyCane : MonoBehaviour {
	
	public Animator myAnimator;

	float timeOfLastCollision = 0.0f;

	void OnCollisionEnter2D(Collision2D otherObject) {
		if ((Time.time - timeOfLastCollision) > 1) {
			if (otherObject.gameObject.tag == Strings.PLAYER) {
				PopcornKernelController player = otherObject.gameObject.GetComponent<PopcornKernelController> ();
				Rigidbody2D rigidbody2d = otherObject.gameObject.GetComponent<Rigidbody2D> ();
				if (rigidbody2d.velocity.y <= 3.0f && player.IsGrounded()) {
					myAnimator.SetTrigger ("Bounce");
					timeOfLastCollision = Time.time;
				}
			}
		}
	}
}
