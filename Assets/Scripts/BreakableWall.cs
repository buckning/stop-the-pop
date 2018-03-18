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
}
