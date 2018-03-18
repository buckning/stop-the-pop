﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * Script to control a platform moving between different points (or path definitions).
 * A path definition is defined and passed in and the platform will follow the path. 
 */
public class PlatformWaypointController : MonoBehaviour {

	public float speed = 0.5f;
	public float maxDistanceToGoal = 0.2f;
	public bool onlyStartAfterCollision = false;
	public float initialDelay = 0.0f;
	public float waitTime = 0;		//time that the platform waits at when waypoint has been reached
	float delay;
	float nextMoveTime;
	public bool cyclic = false;
	int fromWaypointIndex;
	float percentBetweenWaypoints;
	public float easeAmount;

	public bool movePassengers = true;

	public Vector3[] localWaypoints;
	Vector3[] globalWaypoints;

	//keep track of how many player collisions have taken place.
	//there was a situation where two collisions could take place and
	//one collision is removed, then player is not moved with the platform
	//even though one collisions still remained
	int playerCollisionCount = 0;	

	List<PopcornKernelController> passengers = new List<PopcornKernelController>();

	private bool hadCollision = false;
	
	public void Start() {
		delay = initialDelay;

		globalWaypoints = new Vector3[localWaypoints.Length];

		for(int i = 0; i < localWaypoints.Length; i++) {
			globalWaypoints[i] = localWaypoints[i] + transform.position;
		}
	}
	
	public void Update() {
		//initial delay of the platform starting
		if (delay > 0.0f) {
			delay -= Time.deltaTime;
			return;
		}

		//only start moving the platform if the player has landed on it
		if(onlyStartAfterCollision && !hadCollision) {
			return;	
		}

		if (globalWaypoints.Length == 0) {
			return;
		}

		//recalculate our position (we are always moving towards the next point)
		Vector2 velocity = CalculatePlatformMovement ();
		transform.Translate (velocity);

		//update passengers
		foreach (PopcornKernelController passenger in passengers) {
			passenger.Translate(velocity);
		}
	}

	/***
	 * Calculate the movement that the platform will move this frame
	 */
	Vector3 CalculatePlatformMovement() {
		//if there is to be a delay at the time the platform reaches a waypoint
		if (Time.time < nextMoveTime) {
			return Vector3.zero;
		}

		fromWaypointIndex %= globalWaypoints.Length;
		
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex]);
		percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
		
		percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);

		//this line calculates the next velocity, the rest this method is to update the next waypoint
		Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);

		if (percentBetweenWaypoints >= 1) {
			percentBetweenWaypoints = 0;
			fromWaypointIndex++;
			
			if(!cyclic) {
				if(fromWaypointIndex >= globalWaypoints.Length-1) {
					fromWaypointIndex = 0;
					System.Array.Reverse(globalWaypoints);
				}
			}
			nextMoveTime = Time.time + waitTime;
		}

		return newPos - transform.position;
	}

	/***
	 * When the player collides with the platform, add it to the passengers list
	 */
	public void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == Strings.PLAYER) {
			hadCollision = true;
		}

		if (!movePassengers) {
			return;
		}
		if(other.gameObject.tag == Strings.PLAYER) {
			playerCollisionCount++;

			PopcornKernelController player = other.gameObject.GetComponent<PopcornKernelController> ();

//			this is commented out to see if this is still exists in the refactor
//			//this if statement is for a bug where the player sometimes lands on a platform 
//			//and the trigger is called but the player controller isn't updated so it seems like the
//			//player isn't on the ground, when it is, so the player isn't moved with the platform
//			if (!player.IsGrounded ()) {
//				player.FixedUpdate ();
//			}
			if (!passengers.Contains (player) && player != null && player.IsGrounded()) {
				passengers.Add(player);
			}
		}
	}

	/***
	 * When the player stops colliding with the platform, remove it from the passengers list
	 */
	public void OnTriggerExit2D(Collider2D other) {
		if (!movePassengers) {
			return;
		}
		if (other.gameObject.tag == Strings.PLAYER) {
			playerCollisionCount--;
			PopcornKernelController player = other.gameObject.GetComponent<PopcornKernelController> ();
			if (passengers.Contains (player)) {
				//only remove the player when all of its colliders have been removed
				//there was a situation where two collisions could take place and
				//one collision is removed, then player is not moved with the platform
				//even though one collisions still remained
				if(playerCollisionCount == 0) {
					passengers.Remove(player);
				}
			}
		}
	}

	float Ease(float x) {
		float a = easeAmount + 1;
		
		return Mathf.Pow (x, a) / (Mathf.Pow (x, a) + Mathf.Pow (1 - x, a));
	}

	void OnDrawGizmos() {
		if (localWaypoints != null) {
			Gizmos.color = Color.red;
			float size = 0.3f;
			
			for(int i = 0; i < localWaypoints.Length; i++) {
				Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
				Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
}
