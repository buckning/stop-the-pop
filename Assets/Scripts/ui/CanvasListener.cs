using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class CanvasListener : MonoBehaviour {

	void Start() {
		Time.timeScale = 1.0f;
		DisableAutoRotation ();
	}



	public void DisableAutoRotation() {
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.orientation = ScreenOrientation.Landscape;
	}

	public void QuitButtonPressed() {
		QuitGame ();
	}

	public void QuitGame() {
		SceneManager.LoadScene ("TitleScreen");
	}

	public void StartGame() {
		SceneManager.LoadScene ("PanLevel");
	}

}
