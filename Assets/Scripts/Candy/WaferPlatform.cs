using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaferPlatform : Breakable {

	float initialAngle;
	bool startRotation = false;
	bool rotationFinished = false;
	public float desiredAngle;
	public float rotationSpeed = 1.0f;

	HudListener hud;

	void Start () {
		hud = GameObject.Find ("LevelHUD").GetComponent<HudListener> ();
	}

	void Update() {
		if (startRotation && !rotationFinished) {
			transform.eulerAngles = new Vector3 (0, 0, 
				Mathf.MoveTowardsAngle (transform.eulerAngles.z, desiredAngle, Time.deltaTime * rotationSpeed));
			if (transform.eulerAngles.z - desiredAngle < 0.1f) {
				rotationFinished = true;
				AudioManager.PlaySound ("platform-bang", Random.Range(0.9f, 1.1f));
				hud.ShakeForDuration (0.2f, 0.5f);
			}
		}
	}

	public override void Break(Vector3 positionOfOriginator) {
		startRotation = true;
		AudioManager.PlaySound ("Biscuit", Random.Range(0.9f, 1.1f));
	}
}
