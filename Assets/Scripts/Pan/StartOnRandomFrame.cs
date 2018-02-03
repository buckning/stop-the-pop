using UnityEngine;
using System.Collections;

/***
 * Script to start an animation from a random frame. 
 * Useful when you want a lot of identical objects to not be in sync
 */
public class StartOnRandomFrame : MonoBehaviour {
	public string animationName;

	//start the animation from a random point
	void Start () {
		Animator animator = GetComponent<Animator>();
		float startPoint = Random.Range(0f, 1f);
		animator.Play(animationName, -1, startPoint);
	}
}
