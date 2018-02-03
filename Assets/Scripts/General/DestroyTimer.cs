using UnityEngine;
using System.Collections;

public class DestroyTimer : MonoBehaviour {

	public float delay;

	void Start () {
		StartCoroutine (DestroyAfterTime (delay));
	}
	
	private IEnumerator DestroyAfterTime(float timeTillDestroy) {
		yield return new WaitForSeconds (timeTillDestroy);
		Destroy (gameObject);
	}
}
