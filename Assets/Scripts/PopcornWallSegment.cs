using UnityEngine;
using System.Collections;

public class PopcornWallSegment : MonoBehaviour {
	private Rigidbody2D rigidbody2d;
	Renderer myRenderer;	
	public Vector2 destroyVector;	//the velocity that this section will have when it is broken

	void Start () {
		myRenderer = GetComponent<Renderer> ();
		rigidbody2d = GetComponent<Rigidbody2D> ();
	}

	void Update() {
		if (!rigidbody2d.isKinematic && !myRenderer.isVisible) {
			Destroy (gameObject);
		}
	}

	public void BreakWall() {
		rigidbody2d.velocity = destroyVector;
		//apply velocity
		rigidbody2d.isKinematic = false;
	}
}
