using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCustomiseBuyButton : MonoBehaviour {
	public Text buttonText;
	public Image coinImage;
	public Button button;

	public void SetInteractable(bool interactable) {
		button.interactable = interactable;
	}

	public void EnableCoinImage(bool enable) {
		coinImage.gameObject.SetActive (enable);
	}

	public void SetText(string text) {
		buttonText.text = text;
	}
}
