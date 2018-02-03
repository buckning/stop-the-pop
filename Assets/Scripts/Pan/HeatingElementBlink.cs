using UnityEngine;
using System.Collections;

public class HeatingElementBlink : MonoBehaviour {
	Animator myAnimator;

	void Start () {
		myAnimator = GetComponent<Animator> ();
		InvokeRepeating ("Blink", Random.Range(0f, 3f), 3f);
	}

	void Blink () {
		myAnimator.SetTrigger ("Blink");
	}
}
