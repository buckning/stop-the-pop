using UnityEngine;
using System.Collections;

/**
 * Class that destroys a game object after the countdown time reaches zero
 */
public class GameObjectDestroyTimer : MonoBehaviour {
	public float timeToDestruct = 1.0f;		//the amount of time until this game object is destroyed
	
	void Update () {
		timeToDestruct -= Time.deltaTime;
		if (timeToDestruct <= 0.0f) {
			Destroy (gameObject);	
		}
	}
}
