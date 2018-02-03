using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InformationPanel : MonoBehaviour {

	public string text;

	public GameObject previousScreen;
	public Text informationText;

	public void SetText(string text) {
		informationText.text = text;
	}

	public void QuitButtonPressed() {
		gameObject.SetActive (false);
	}
}
