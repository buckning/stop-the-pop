using UnityEngine;
using System.Collections;

public class FlameSequencer : MonoBehaviour {

	public float onTime = 1.0f;
	public float offTime = 1.0f;
	public float initialDelay = 0.0f;

	float timeInCurrentState = 0.0f;

	public GameObject flame;

	float delay = 0.0f;

	enum State {
		ON,
		OFF
	}

	State currentState;

	void Start () {
		currentState = State.OFF;
	}

	void Update () {
		if (delay < initialDelay) {
			delay += Time.deltaTime;
			return;
		}
		timeInCurrentState += Time.deltaTime;

		if (currentState == State.ON && timeInCurrentState > onTime) {
			currentState = State.OFF;
			flame.SetActive (false);
			timeInCurrentState = 0.0f;
		} 

		if (currentState == State.OFF && timeInCurrentState > offTime) {
			currentState = State.ON;
			flame.SetActive (true);
			timeInCurrentState = 0.0f;
		} 
	}
}
