using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialImagePanel : MonoBehaviour {
	public Image tutorialImage;
	public Image tutorialPanel;
	public Image tutorialPanelTouchPanel;

	public void ShowTutorialImage(Sprite tutorialImage, float showAfterDelay) {
		this.tutorialImage.sprite = tutorialImage;
		this.tutorialImage.preserveAspect = true;
		StartCoroutine (ShowTutorialAfterDelay (showAfterDelay));

	}

	void Update() {
		//this is only for desktop 
		if (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.LeftControl)) {
			Dismiss ();
		}
	}

	IEnumerator ShowTutorialAfterDelay(float showAfterDelay) {
		yield return new WaitForSeconds(showAfterDelay);
		gameObject.SetActive (true);
	}

	public void Pause() {
		Time.timeScale = 0.0f;
	}

	public void Dismiss() {
		gameObject.SetActive (false);
		EventSystem.current.SetSelectedGameObject(null);
		Time.timeScale = 1.0f;
	}
}
