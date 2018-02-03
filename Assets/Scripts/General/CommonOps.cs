using UnityEngine;
using System.Collections;

/***
 * Class to be used mainly by the animator. It contains useful methods that the animator can call upon, like destroying the game object
 */
public class CommonOps : MonoBehaviour {
	public void DeactivateMyself() {
		gameObject.SetActive(false);
	}

	public void DestroyMyself() {
		Destroy (gameObject);
	}

	public void DestroyParent() {
		Destroy (transform.parent.gameObject);
	}
}
