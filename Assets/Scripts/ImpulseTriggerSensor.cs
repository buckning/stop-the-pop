using UnityEngine;
using System.Collections;

public class ImpulseTriggerSensor : MonoBehaviour {

	public string triggerName;		//the name of the object that gets an impulse applied when they collide with the trigger
	public float impulseVelocity = 10f;
	public float angularVelocity = 20f;

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.name == (triggerName)) {
			GameObject gameObject = col.gameObject;
			gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, impulseVelocity);
			gameObject.GetComponent<Rigidbody2D>().angularVelocity = angularVelocity;
		}
	}
}
