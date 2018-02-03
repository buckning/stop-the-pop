using UnityEngine;
using System.Collections;

public class TempFallingObject : MonoBehaviour {
	PolygonCollider2D mycollider;
	// Use this for initialization
	void Start () {
		mycollider = GetComponent<PolygonCollider2D> ();
	}

	void OnCollisionEnter2D(Collision2D otherObject) {
		StartCoroutine(ChangeToTrigger());
	}

	IEnumerator ChangeToTrigger() {
		yield return new WaitForSeconds(0.1f);
		mycollider.isTrigger = true;
	}
}
