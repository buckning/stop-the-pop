using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialPanel : MonoBehaviour {

	public Text informationText;
	public Image informationImage;

	public void SetText(string text) {
		informationText.text = text;
	}

	public void SetImage(Image img) {
		informationImage = img;
	}

	public void ShowTutorialPanel() {
		Time.timeScale = 0.0f;
		gameObject.SetActive (true);
	}

	public void QuitButtonPressed() {
		Time.timeScale = 1.0f;
		gameObject.SetActive (false);
	}
}
