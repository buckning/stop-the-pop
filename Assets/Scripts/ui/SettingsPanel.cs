using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SettingsPanel : MonoBehaviour {
	public Button sfxButton;
	public Button musicButton;
	public Image sfxCheckboxTick;
	public Image musicCheckboxTick;

	void Start () {
		sfxCheckboxTick.gameObject.SetActive (!Settings.sfxEnabled);
		musicCheckboxTick.gameObject.SetActive (!Settings.musicEnabled);
	}

	public void SfxCheckboxClicked() {
		Settings.sfxEnabled = !Settings.sfxEnabled;	
		sfxCheckboxTick.gameObject.SetActive (!Settings.sfxEnabled);

		GameDataPersistor.Save(GameStats.GetInstance().GetGameData());
	}

	public void MusicCheckboxClicked() {
		Settings.musicEnabled = !Settings.musicEnabled;
		musicCheckboxTick.gameObject.SetActive (!Settings.musicEnabled);
		AudioSource audioSource = GameObject.Find("TitleScreenAudio").GetComponent<AudioSource>();

		if (Settings.musicEnabled) {
			if (!audioSource.isPlaying) {
				audioSource.Play ();
			}
		} else {
			audioSource.Stop ();
		}

		GameDataPersistor.Save(GameStats.GetInstance().GetGameData());
	}

	public void SlideIn() {
		transform.parent.gameObject.GetComponent<TitleScreenListener>().PlayButtonClickSound ();
	}

	public void SlideOut() {
		AudioManager.PlaySound ("GoBack");
		GameDataPersistor.Save(GameStats.GetInstance().GetGameData());
	}
}
