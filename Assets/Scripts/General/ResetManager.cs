using UnityEngine;
using System.Collections;

public class ResetManager : MonoBehaviour {

	public void Reset() {
		
		foreach(Transform child in transform) {
			Resettable resetable = child.gameObject.GetComponent<Resettable> ();
			resetable.Reset ();
		}
	}
}