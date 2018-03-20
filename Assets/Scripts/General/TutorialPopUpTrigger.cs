using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopUpTrigger : MonoBehaviour {

	public float showAfterDelay = 0.0f;
	public Sprite tutorialImage;

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == Strings.PLAYER) {
			UiManager uiManager = GameObject.FindObjectOfType<UiManager> ();
			uiManager.tutorialPanel.gameObject.SetActive (true);
			uiManager.tutorialPanel.ShowTutorialImage(tutorialImage, showAfterDelay);
			Destroy (gameObject);
		}
	}
}
