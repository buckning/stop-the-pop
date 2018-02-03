using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent (typeof(AudioSource))]
public class TitleScreenMusic : MonoBehaviour {

	private static TitleScreenMusic instance = null;

	public static TitleScreenMusic Instance {
		get { return instance; }
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
		}
		DontDestroyOnLoad(this.gameObject);

		AudioSource audioSource = GetComponent<AudioSource> ();

		GameData gameData = GameDataPersistor.Load ();

		if (gameData != null) {
			GameStats.GetInstance ().Initialise (gameData);

			//the saved music and sfx settings are loaded into RAM here, so we can enable or disable them here

			if (Settings.musicEnabled) {
				audioSource.Play ();
			} else {
				audioSource.Stop ();
			}
		} else {
			audioSource.Play ();
		}

	}
}
