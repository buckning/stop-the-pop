using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ThermometerBehaviour : MonoBehaviour {

	public float currentTemperature = 0;

	public Image redFill;
	public Image alertGlow;

	float currentFill = 0f;
	float animationSpeed = 5f;	//the speed of the lerp 
	Animator animator;

	void Start() {
		alertGlow.gameObject.SetActive (false);
		animator = GetComponent<Animator> ();
	}

	/***
	 * 
	 */
	void Update () {
		//this is purely cosmetic so the fill is always gradually moving and there is no sudden jumps
		currentFill = Mathf.Lerp(currentFill, currentTemperature, Time.deltaTime * animationSpeed);
		if (currentTemperature > 0.8f) {
			alertGlow.gameObject.SetActive (true);
		} else {
			alertGlow.gameObject.SetActive (false);
		}
		animator.SetFloat ("temperature", currentTemperature);
		redFill.fillAmount = currentFill;
	}

	public void Reset() {
		currentTemperature = 0;
		currentFill = 0;
	}
}
