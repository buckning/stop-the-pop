using UnityEngine;
using System.Collections;

public class Sparkle : MonoBehaviour {

	public float delayBetweenSparkles = 1.0f;
	public float initialDelay = 0.0f;

	GAui animator;

	float timeBetweenSparkles = 0.0f;

	void Start () {
		animator = GetComponent<GAui> ();
		timeBetweenSparkles = delayBetweenSparkles - initialDelay;
	}

	void Update () {
		timeBetweenSparkles += Time.unscaledDeltaTime;

		if (timeBetweenSparkles > delayBetweenSparkles) {
			//trigger sparkle effect
			animator.MoveIn(GSui.eGUIMove.SelfAndChildren);
			timeBetweenSparkles = 0.0f;
		}
	}
		
	public void FadeOut() {
		animator.MoveOut(GSui.eGUIMove.SelfAndChildren);
	}
}
