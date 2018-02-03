using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class BiscuitBehaviour : MonoBehaviour {

	public bool collapseOnCollision = true;

	private bool hadCollision = false;

	private Rigidbody2D rigidBody2d;
	private float shakeAmplitude = 0.15f;
	private Animator myAnimator;

	void Start() {
		myAnimator = GetComponent<Animator> ();
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if(coll.gameObject.tag == "Player" && !hadCollision && collapseOnCollision) {
			StartCoroutine (ShakeAndDrop ());
			hadCollision = true;
			myAnimator.SetTrigger ("Surprised");
		}
	}

	IEnumerator ShakeAndDrop() {
		yield return new WaitForSeconds (0.15f);

		AudioManager.PlaySound ("Biscuit");
		Hashtable ht = new Hashtable ();
		ht.Add ("x", shakeAmplitude);
		ht.Add ("y", shakeAmplitude);
		ht.Add ("time", 0.25f);

		iTween.ShakePosition (gameObject, ht);
		yield return new WaitForSeconds (0.1f);
		AudioManager.PlaySound ("Biscuit-Worried");
		yield return new WaitForSeconds (0.35f);
		myAnimator.SetTrigger ("Worried");
		rigidBody2d = gameObject.AddComponent<Rigidbody2D> ();
		rigidBody2d.isKinematic = false;
	}
}
