using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleBehaviour : Breakable {
	public GameObject normalFace;	//the face that the candle usually has
	public GameObject deathFace;	//the face that will be shown when the candle dies
	public GameObject flame;		//this is the normal low flame when the candle
	public GameObject bigFlame;		//this is the big flame when the candle is blowing
	public SpriteRenderer bodySprite;
	public float initialDelay = 0.0f;
	Rigidbody2D myRigidbody2d;
	BoxCollider2D myCollider;

	void Start() {
		myRigidbody2d = GetComponent<Rigidbody2D> ();
		myCollider = GetComponent<BoxCollider2D> ();
		InvokeRepeating ("ToggleFlames", initialDelay, 1.5f);
	}

	public void ToggleFlames() {
		if(normalFace.activeInHierarchy) {	//this check is to prevent a unwanted flame toggles when the candle is killed
			if (flame.activeInHierarchy) {
				flame.SetActive (false);
				bigFlame.SetActive (true);
				if (bodySprite.isVisible) {
					AudioManager.PlaySound ("flame-dispenser", Random.Range(1.1f, 1.3f), 0.5f);
				}
			} else {
				flame.SetActive (true);
				bigFlame.SetActive (false);
			}
		}
	}

	public override void Break(Vector3 positionOfOriginator) {
		normalFace.SetActive (false);
		deathFace.SetActive (true);
		myRigidbody2d.isKinematic = false;
		myCollider.enabled = false;
		flame.SetActive (false);
		bigFlame.SetActive (false);
		float directionOfOriginator = Mathf.Sign (transform.position.x - positionOfOriginator.x);
		myRigidbody2d.AddForce (new Vector2 (400 * directionOfOriginator, 400));
		myRigidbody2d.AddTorque (directionOfOriginator * -30);
		myRigidbody2d.gravityScale = 2f;
		AudioManager.PlaySound ("Candle-Death");
		Destroy (gameObject, 3f);
	}
}
