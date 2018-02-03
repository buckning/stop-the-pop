using UnityEngine;
using System.Collections;

/***
 * Class to destroy the Oil Splash after enough time has elapsed. 
 */
public class OilSplashBehaviour : MonoBehaviour {
	
	void Start () {
		StartCoroutine(DestroyThis());
	}

	private IEnumerator DestroyThis() {
		yield return new WaitForSeconds(0.4f);
		Destroy(gameObject);
	}
}