using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CommonUiOps : MonoBehaviour {

	public float rotationSpeed = 1.0f;
	RectTransform rectTransform;

	void Start () {
		rectTransform = gameObject.GetComponent <RectTransform> ();
	}
	
	void Update () {
		rectTransform.Rotate (0,0, rotationSpeed);
	}

	void Disable() {
		gameObject.SetActive (false);
	}
}
