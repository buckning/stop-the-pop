using UnityEngine;
using System.Collections;

public class Breakable : MonoBehaviour {

	public float timeUntilDestroy = 0.1f;	//the amount of time from when this is triggered to be destroyed before the game object is destroyed

	/***
	 * positionOfOriginator is the position of the gameobject who triggered this Break method
	 */
	public virtual void Break(Vector3 positionOfOriginator) {
		Destroy (gameObject, timeUntilDestroy);
	}
}
