using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameDispenser : MonoBehaviour {

	Animator myAnimator;

	public Transform spawnPoint;
	public GameObject spawnObject;
	public float interval;
	public float initialDelay;

	void Start() {
		myAnimator = GetComponent<Animator> ();
		InvokeRepeating ("TriggerFlame", initialDelay, interval);
	}
		
	void Spawn () {
		Instantiate (spawnObject, spawnPoint.position, spawnObject.transform.rotation);
	}

	void TriggerFlame() {
		myAnimator.SetTrigger ("Fire");
	}
}
