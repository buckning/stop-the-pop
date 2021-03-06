﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopcornBehaviour : Breakable {

	private float kickForce = 500f;	//the amount of force applied to this object when it is kicked
	public GameObject collisionDust;
	private float timeNotInViewport;
	private Rigidbody2D rigidbody2d;
	PolygonCollider2D myCollider;
	public bool causesHarmToPlayer = true;	//if this is set to true, then the resulting popcorn will hurt the player when it falls on them
	List<Vector2> velocityHistory;
	int velocitySamples = 5;

	bool destroying = false;

	public bool destroyAfterCollision = true;

	float forceToTriggerBreak = 3.0f;	//threshold for the break method to be triggered. 

	void Start () {
		ResourceCache.Load ("Effects/LandingDustParent");
		rigidbody2d = GetComponent<Rigidbody2D> ();
		velocityHistory = new List<Vector2>();
		myCollider = GetComponent<PolygonCollider2D> ();
	}

	public override void Break(Vector3 positionOfOriginator) {
		if (positionOfOriginator.x < transform.position.x) {
			rigidbody2d.AddForce (new Vector2(kickForce, 0f));
		} else {
			rigidbody2d.AddForce (new Vector2(kickForce * -1, 0f));
		}
	}

	void OnCollisionEnter2D(Collision2D otherObject) {
		if (destroying) {
			//only execute this once
			return;	
		}

		if (otherObject.gameObject.tag == Strings.POPCORN) {
			return;
		}
			
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			//only conflict damage to the player if this velocity is over a certain speed
			Vector2 avgVelocity = AverageVelocityHistory() / velocitySamples;
			if (Mathf.Abs (avgVelocity.x) > forceToTriggerBreak ||
			   						Mathf.Abs (avgVelocity.y) > forceToTriggerBreak) {
				PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();

				if (rigidbody2d.velocity.y < 0 && causesHarmToPlayer) {
					player.CollisionWithEnemy (gameObject.name);
				}
			}
		}
		if (destroyAfterCollision) {
			StartCoroutine (DisableCollider ());
		}
	}

	/***
	 * Bounce forwards or backwards
	 */
	IEnumerator ZBounce() { 
		while (true) {
			
			Vector3 newPos = transform.position;
			newPos.z = newPos.z - .1f;
			transform.position = newPos;
			yield return new WaitForSeconds(0.1f);
		}
	}

	IEnumerator DisableCollider() {
		//only destroy collider after this has been falling and has a collision
		if (rigidbody2d.velocity.y > 0) {
			yield return false;
		}
		GameObject dustObject = (GameObject)Instantiate (ResourceCache.Get("Effects/LandingDustParent"), 
			transform.position, Quaternion.identity);
		dustObject.transform.localScale = transform.localScale;

		destroying = true;
		yield return new WaitForSeconds(0.15f);
		myCollider.enabled = false;
		yield return new WaitForSeconds(10f);

		Destroy (gameObject);
	}

	Vector2 AverageVelocityHistory() {
		Vector2 overallVelocity = Vector2.zero;

		foreach (Vector2 velocity in velocityHistory) {
			overallVelocity += velocity;
		}

		return overallVelocity;
	}


	/***
	 * Delete this object if it has been out of the view port for over
	 * deleteAfterTime seconds. This behaviour only gets triggered when
	 * this has been seen. If this object has not been seen at all, 
	 * it does not delete itself
	 */
	void FixedUpdate () {
		//keep a history of the velocity over the last 5 updates.
		//this is useful when trying to assess the velocity in onCollisionEnter
		velocityHistory.Insert (0, rigidbody2d.velocity);
		if (velocityHistory.Count >= velocitySamples) {
			velocityHistory.RemoveAt (velocityHistory.Count - 1);
		}
	}
}
