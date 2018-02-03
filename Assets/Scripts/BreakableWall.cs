using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class BreakableWall : Breakable {

	List<PopcornWallSegment> wallSegments;

	public void Start() {
		wallSegments = new List<PopcornWallSegment> ();
		foreach (Transform child in transform) {
			PopcornWallSegment segment = child.gameObject.GetComponent<PopcornWallSegment> ();
			wallSegments.Add(segment);
		}
	}

	public override void Break(Vector3 positionOfOriginator) {
		AudioManager.PlaySound ("wall-break-new");
		foreach (PopcornWallSegment segment in wallSegments) {
			segment.BreakWall();
			segment.gameObject.transform.parent = null;
		}

		Destroy (gameObject);
	}

	void OnCollisionEnter2D(Collision2D otherObject) {
		//this code is here to allow the player to break the wall if they glide into it
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			if (player.IsGliding ()) {
				Break (otherObject.transform.position);
				//this reset is for the scenario where the player is running into a wall and have stopped.
				//the wall breaks but the physics event to say the player has stopped colliding with the wall doesn't trigger
				//so the player is stuck in a situation where they cannot move after a wall is broken
				player.wallCollider.Reset ();
			}
		}
	}
}
