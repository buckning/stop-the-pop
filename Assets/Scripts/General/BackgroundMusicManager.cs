using UnityEngine;
using System.Collections;

public class BackgroundMusicManager : MonoBehaviour {

	public string backgroundMusic = "Music/main-verse";
	AudioClip clip;
	AudioSource audioSource;

	void Start () {
		audioSource = GetComponent<AudioSource> ();
		audioSource.clip = ResourceCache.LoadAudioClip (backgroundMusic);
		audioSource.volume = 0.7f;
		if (Settings.musicEnabled) {
			audioSource.Play ();
		}
	}

}
