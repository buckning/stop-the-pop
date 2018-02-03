using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioSourcePool : MonoBehaviour {

	static List<AudioSource> audioSourcePool;
	static int audioPoolSize = 10;

	void Start () {
		audioSourcePool = new List<AudioSource>();

		for(int i = 0; i < audioPoolSize; i++) {
			audioSourcePool.Add(gameObject.AddComponent<AudioSource>());
		}
	}
	
	public static void PlayClip(AudioClip clip) {
		//look for an available audio source
		foreach(AudioSource audio in audioSourcePool) {
			if(!audio.isPlaying) {
				audio.clip = clip;
				audio.Play ();
				return;
			}
		}

		//we haven't got an available audio source so pick a random one from the pool to play the sound on
		int audioSourceIndex = Random.Range(0, audioPoolSize - 1);
		audioSourcePool[audioSourceIndex].clip = clip;
		audioSourcePool[audioSourceIndex].Play ();
	}
}
