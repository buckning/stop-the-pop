using UnityEngine;
using System.Collections;

public class PressureSwitch : MonoBehaviour {
	public Vector3 pressedPosition;
	public Vector3 depressedPosition;

	Vector3 globalPressedPosition;
	Vector3 globalDepressedPosition;

	int collisionCount = 0;

	bool pressed = false;
	
	void Start () {
		globalPressedPosition = pressedPosition + transform.position;
		globalDepressedPosition = depressedPosition + transform.position;
	}

	void Update() {
		pressed = collisionCount > 0 ? true : false;

		Vector3 position = transform.position;

		if (pressed) {
			position.y = Mathf.Lerp (transform.position.y, globalPressedPosition.y, 5*Time.deltaTime);
			//move to pressed position
		} else {
			//move to depressed position
			position.y = Mathf.Lerp (transform.position.y, globalDepressedPosition.y, 5*Time.deltaTime);
		}
		transform.position = position;
	}

	public bool isPressed() {
		return pressed;
	}

	/***
	 * Count the amount of collisions on this object. If there is more than 1, this is pressed
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			collisionCount++;
		}
	}

	void OnTriggerExit2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			collisionCount--;
		}
	}

	void OnDrawGizmos() {
		float size = 0.3f;
		Gizmos.color = Color.green;
		//draw pressedPosition
		Vector3 globalWaypointPos = (Application.isPlaying) ? globalPressedPosition : pressedPosition + transform.position;
		Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
		Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
		//draw depressed position
		Gizmos.color = Color.red;
		globalWaypointPos = (Application.isPlaying) ? globalDepressedPosition : depressedPosition + transform.position;
		Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
		Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
	}
}
