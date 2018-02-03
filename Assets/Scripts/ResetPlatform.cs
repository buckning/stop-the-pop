using UnityEngine;
using System.Collections;

/***
 * Reset a platforms angle to 0
 */
public class ResetPlatform : MonoBehaviour {
	Renderer myRenderer;
	void Start () {
		myRenderer = GetComponent<Renderer> ();
	}

	void Update () {
		if (!myRenderer.isVisible) {
			//reset platform rotation to 0
			transform.eulerAngles = new Vector3(0, 0, 0);
		}
	}
}
