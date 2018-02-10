using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour {

	public LayerMask collectableMask;
	float magnetRadius = 6.0f;
	float magnetAttractForce = 20f;

	void FixedUpdate () {
		Collider2D[] collidingObjects = Physics2D.OverlapCircleAll (transform.position, 
			magnetRadius  * transform.localScale.x, collectableMask);
		
		foreach(Collider2D collider in collidingObjects) {
			if (collider.gameObject.name.StartsWith("Coin")) {
				collider.gameObject.transform.position = 
					Vector3.MoveTowards (collider.gameObject.transform.position, 
											transform.position, 
											Time.deltaTime * magnetAttractForce);
			}
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color (0f, 0.5f, 0f, 0.1f);
		Gizmos.DrawSphere (transform.position, magnetRadius * transform.localScale.x);
	}
}
