using UnityEngine;
using System.Collections;

public class EyeFollow : MonoBehaviour {
	PopcornKernelController player;
	public float eyeRadius = 1.0f;
	public GameObject pupil;

	SpriteRenderer myRenderer;
	// Use this for initialization
	void Start () {
		myRenderer = pupil.GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		//no need to run the calculations in this method if the yes are not visible
		if (!myRenderer.isVisible) {
			return;
		}
		if (player == null) {
			player = GameObject.FindObjectOfType <PopcornKernelController>();
		}

		// first, find the distance from the center of the eye to the target 
		Vector3 distanceToTarget = player.transform.position - transform.position;


		// clamp the distance so it never exceeds the size of the eyeball 
		distanceToTarget = Vector3.ClampMagnitude( distanceToTarget, eyeRadius );


		// place the pupil at the desired position relative to the eyeball 
		Vector3 finalPupilPosition = transform.position + distanceToTarget; 
		pupil.transform.position = finalPupilPosition; 
	}
}
