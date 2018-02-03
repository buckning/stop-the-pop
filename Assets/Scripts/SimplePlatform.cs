using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimplePlatform : PlatformWaypointController {
	public void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == Strings.COLLECTOR) {
			Destroy (gameObject);
			return;
		}
		base.OnTriggerEnter2D (other);
	}
}
