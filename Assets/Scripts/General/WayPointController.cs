using UnityEngine;
using System.Collections;

public class WayPointController : MonoBehaviour {
	public float speed = 0.5f;
	public float maxDistanceToGoal = 0.2f;
	public float initialDelay = 0.0f;
	public float waitTime = 0;		//time that the platform waits at when waypoint has been reached
	float delay;
	float nextMoveTime;
	public bool cyclic = false;
	int fromWaypointIndex;
	float percentBetweenWaypoints;
	public float easeAmount;

	public Vector3[] localWaypoints;
	Vector3[] globalWaypoints;

	public float[] speedForWaypoint;

	SpriteRenderer spriteRenderer;

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

		if (globalWaypoints.Length == 0) {
			return;
		}

		//recalculate our position (we are always moving towards the next point)
		Vector2 velocity = CalculatePlatformMovement ();
		transform.Translate (velocity);
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
		percentBetweenWaypoints += Time.deltaTime * speedForWaypoint[fromWaypointIndex] / distanceBetweenWaypoints;

		percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);

		//this line calculates the next velocity, the rest this method is to update the next waypoint
		Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);

		if (percentBetweenWaypoints >= 1) {
			percentBetweenWaypoints = 0;
			fromWaypointIndex++;
			nextMoveTime = Time.time + waitTime;
		}

		return newPos - transform.position;
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
