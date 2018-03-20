using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JazzieBehaviour : MonoBehaviour {

	public SpriteRenderer myRenderer;
	bool wasVisible = false;

	void Update () {
		if (myRenderer.isVisible && !wasVisible) {
			AudioManager.PlaySound("Jazzie-Fall", Random.Range(0.8f, 1.2f));
			wasVisible = true;
		}
	}

	void OnCollisionEnter2D(Collision2D otherObject) {
		if (myRenderer.isVisible) {
			if (otherObject.gameObject.tag == Strings.PLAYER) {
				PopcornKernelController player = otherObject.gameObject.GetComponent<PopcornKernelController> ();
				AudioManager.PlaySound ("Jazzie-Hit");
				player.InstantDeath ();
			}
		}
		Destroy (gameObject);
	}
}
