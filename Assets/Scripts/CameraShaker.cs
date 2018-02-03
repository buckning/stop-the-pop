using UnityEngine;
using System.Collections;

/***
 * Adapted from http://unitytipsandtricks.blogspot.ie/2013/05/camera-shake.html
 */
public class CameraShaker : MonoBehaviour {
	public float shakeMagnitude = 0.1f;
	public float shakeDuration = 0.5f;
	bool startShaking = false;

	private bool shaking = false;

	void Update () {
		if (startShaking && shaking == false) {
			StartCoroutine (Shake ());
		} 
	}

	/***
	 * Start Shaking the camera
	 */
	public void StartShaking() {
		startShaking = true;
	}

	/***
	 * 
	 */
	public void StopShaking() {
		startShaking = false;
	}

	IEnumerator Shake() {
		shaking = true;

		Vector3 originalCamPos = transform.localPosition;

		while (startShaking) {
			float damper = 1.0f - Mathf.Clamp(4.0f *  - 3.0f, 0.0f, 1.0f);

			// map value to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= shakeMagnitude * damper + originalCamPos.x;
			y *= shakeMagnitude * damper + originalCamPos.y;

			if (Time.timeScale >= 1.0f) {	//don't shake the camera if the game is paused
				transform.localPosition = new Vector3 (x, y, originalCamPos.z);
			}
			yield return null;
		}
		shaking = false;
	}
}
