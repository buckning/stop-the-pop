using UnityEngine;
using System.Collections;

public class PlatformForPressureSwitch : MonoBehaviour {
	public PressureSwitch pressureSwitch;

	public Vector3 pressedPosition;
	public Vector3 depressedPosition;
	
	Vector3 globalPressedPosition;
	Vector3 globalDepressedPosition;

	public Vector3 dustSpawnPosition;

	public float speedOfPlatformUp = 10f;
	public float speedOfPlatformDown = 3f;

	bool resetDustSpawn = true;

	float dustScale = 1.5f;

	SpriteRenderer myRenderer;

	void Start () {
		ResourceCache.Load ("Effects/LandingDustParent");
		globalPressedPosition = pressedPosition + transform.position;
		globalDepressedPosition = depressedPosition + transform.position;
		dustSpawnPosition = dustSpawnPosition + transform.position;
		myRenderer = GetComponent<SpriteRenderer> ();
	}

	void Update () {
		Vector3 position = transform.position;
		if (pressureSwitch.isPressed ()) {
			position.y = Mathf.MoveTowards (transform.position.y, globalPressedPosition.y, speedOfPlatformUp * Time.deltaTime);
		} else {
			position.y = Mathf.MoveTowards (transform.position.y, globalDepressedPosition.y, speedOfPlatformDown * Time.deltaTime);
		}
			
		if (position.y > (globalPressedPosition.y - 0.5f)) {
			resetDustSpawn = true;
		}

		if (position.y < (globalDepressedPosition.y + 0.5f)) {
			if (resetDustSpawn) {
				resetDustSpawn = false;

				if(myRenderer.isVisible) {
					GameObject dust = (GameObject)Instantiate (ResourceCache.Get ("Effects/LandingDustParent"), dustSpawnPosition, Quaternion.identity);
					dust.transform.localScale = new Vector3 (dustScale, dustScale, dustScale);

					AudioManager.PlaySound ("platform-bang", Random.Range(0.9f, 1.1f));
				}
			}
		}

		transform.position = position;
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
