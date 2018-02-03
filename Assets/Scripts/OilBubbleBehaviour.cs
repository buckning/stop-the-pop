using UnityEngine;
using System.Collections;

/***
 * 
 */
public class OilBubbleBehaviour : MonoBehaviour {

	/***
	 * Start the animation from a random frame 
	 * so multiple oil bubbles won't look the exact same
	 */
	void Start () {
		Animator animator = GetComponent<Animator>();
		float startPoint = Random.Range(0f, 1f);
		animator.Play("OilBubble", -1, startPoint);
	}
}
