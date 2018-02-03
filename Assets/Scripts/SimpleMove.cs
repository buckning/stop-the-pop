using UnityEngine;
using System.Collections;

/***
 * Script to simply move a game object in the x-axis either to the left or right.
 */
public class SimpleMove : MonoBehaviour {

	
	/***
	 * The speed of the game object in the x-axis. Moving left by default.
	 */
	public float xAxisSpeed = -0.01f;
	
	/***
	 * The multiplier for the speed. 
	 */
	public float speedMultiplier = 1;

	void Update () {
		if (Time.deltaTime != 0.0f) {
			transform.position = new Vector3 (transform.position.x + xAxisSpeed * speedMultiplier, transform.position.y, transform.position.z);
		}
	}
}
