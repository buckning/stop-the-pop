using UnityEngine;
using System.Collections;

/***
 * Destroy the game object that interacts with this object. 
 * The name of the object to destroy is passed in.
 */
public class GameObjectDestructor : MonoBehaviour {
	public string destroyName;

	//this is an optional game object, when an object is getting destroyed, the new
	//game object is instantiated in its place. This is useful for spawning a death animation
	public GameObject newGameObject;

	void OnTriggerEnter2D(Collider2D col) {
		//if this is to destroy everything, we simply delete the game object and return
		if (destroyName == Strings.DESTROY_ALL_OBJECTS) {
			OnDestroy ();
			Destroy(col.gameObject);
			return;
		}

		if (col.gameObject.name == (destroyName)) {
			if(newGameObject != null) {
				Instantiate (newGameObject, col.gameObject.transform.position, Quaternion.identity);
			}
			OnDestroy ();
			Destroy(col.gameObject);
		}
	}

	public virtual void OnDestroy() {

	}
}
