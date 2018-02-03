using UnityEngine;
using System.Collections;

public class ImpulseTrigger : MonoBehaviour {
	public float popInterval = 3f;
	public float xForce = 0f;
	public float yForce = 1000f;

	private float timeToNextPop = 0;
	private Rigidbody2D rigidbody2d;

	void Start () {
		rigidbody2d = GetComponent<Rigidbody2D>();
	}

	void Update () {
		print (timeToNextPop);
		timeToNextPop -= Time.deltaTime;
		if (timeToNextPop <= 0f) {
			rigidbody2d.AddForce (new Vector2 (xForce, yForce));
			timeToNextPop = popInterval;
		}
	}
}
