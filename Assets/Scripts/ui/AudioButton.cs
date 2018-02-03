using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AudioButton : MonoBehaviour {
	public enum BUTTON_TYPE {SFX, MUSIC};

	public BUTTON_TYPE buttonType;
	public Image disabledImage;

	void OnEnable() {
		if (buttonType == BUTTON_TYPE.MUSIC) {
			disabledImage.gameObject.SetActive(!Settings.musicEnabled);
		} else if (buttonType == BUTTON_TYPE.SFX) {
			disabledImage.gameObject.SetActive(!Settings.sfxEnabled);
		}
	}

	public void ToggleMusic() {
		if (Settings.musicEnabled) {
			Settings.musicEnabled = false;
			GameObject child = GameObject.FindGameObjectWithTag ("MainCamera");
			child.transform.parent.gameObject.GetComponent<AudioSource>().Stop ();
			disabledImage.gameObject.SetActive (true);
		} else {
			Settings.musicEnabled = true;
			GameObject child = GameObject.FindGameObjectWithTag ("MainCamera");
			child.transform.parent.gameObject.GetComponent<AudioSource>().Play ();
			disabledImage.gameObject.SetActive (false);
		}
		GameDataPersistor.Save(GameStats.GetInstance().GetGameData());
	}

	public void ToggleSfx() {
		if (Settings.sfxEnabled) {
			Settings.sfxEnabled = false;
			disabledImage.gameObject.SetActive (true);
		} else {
			Settings.sfxEnabled = true;
			disabledImage.gameObject.SetActive (false);
			AudioManager.DisablePlayingSounds ();
		}
		GameDataPersistor.Save(GameStats.GetInstance().GetGameData());
	}
}
