using UnityEngine;
using System.Collections;

/***
 * Zoom the camera in or out if the player collides with this trigger.
 * The camera does not actually zoom, but the z position is moved
 */
public class CameraZoomTrigger : MonoBehaviour {
	public CameraFollow sceneCamera;		//the camera from the scene
	public float zoomAmount = -20f;	//the new z-position
	public float zoomSpeed = 2f;	//the speed of the zoom
	public bool zoomIn = false;		//is this a zoom in trigger or zoom out trigger?

	private bool collisionDetected = false;

	public void Start() {
		if (sceneCamera == null) {
			sceneCamera = GameObject.Find ("Main Camera").GetComponent<CameraFollow>();
		}
	}

	public void Update() {
		//if a collision was detected, zoom the camera
		if (collisionDetected && !sceneCamera.isLocked) {
			//get the new zoom position
			float zoom = Mathf.Lerp (sceneCamera.transform.position.z, zoomAmount, zoomSpeed * Time.deltaTime);
			//update the cameras position
			sceneCamera.transform.position = new Vector3 (sceneCamera.transform.position.x, sceneCamera.transform.position.y, zoom);

			if(zoomIn) {
				if((Mathf.Abs(sceneCamera.transform.position.z) - Mathf.Abs(zoomAmount)) < 0.5f) {
					collisionDetected = false;
				}
			}

			else {
				if((Mathf.Abs(sceneCamera.transform.position.z) - Mathf.Abs(zoomAmount)) > -0.5f) {
					collisionDetected = false;
				}
			}
		}
	}

	/***
	 * Detect a collision and update the collision flag
	 */
	public void OnTriggerEnter2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			collisionDetected = true;
		}
	}
}
