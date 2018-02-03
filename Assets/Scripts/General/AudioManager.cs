using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * Audio Manager has a list of all files that are loaded 
 */
public class AudioManager : MonoBehaviour{

	const int audioPoolSize = 10;

	List<AudioFile> audioFiles = new List<AudioFile> ();

	static AudioClip backgroundMusic;

	static AudioManager instance;

	static List<AudioSource> audioSourcePool;

	bool initialized = false;

	void Initialize() {
		//to cut down on loading times, only load up sound effects once and not every time the audio manager is instantiated
		if (initialized) {
			return;
		}

		backgroundMusic = LoadBackgroundMusic ("Music/main-verse");

		LoadSound("SoundEffects/Collectables/", "coin-new");
		LoadSound("SoundEffects/Collectables/", "snowflake-new");
		LoadSound("SoundEffects/Collectables/", "magnet-new");
		LoadSound("SoundEffects/Collectables/", "cape-new");

		//load in all files from the pop directory
		//the numbers 1 and 23 correspond to the integer appended on to the pop filename
		for (int i = 1; i <= 14; i++) {
			LoadSound ("SoundEffects/PopFx/", "pop" + i);
		}
	
		LoadSound("SoundEffects/UI/", "Click");
		LoadSound("SoundEffects/UI/", "Star");
		LoadSound ("SoundEffects/UI/", "InfoPanelSlideIn");
		LoadSound ("SoundEffects/UI/", "Reward");
		LoadSound ("SoundEffects/UI/", "RewardBox");
		LoadSound("SoundEffects/Environmental/", "splash1");
		LoadSound("SoundEffects/Environmental/", "electricshock");
		LoadSound("SoundEffects/Environmental/", "splash");
		LoadSound("SoundEffects/Environmental/", "wall-break-new");
		LoadSound("SoundEffects/Environmental/", "platform-bang");
		LoadSound("SoundEffects/Environmental/", "toaster");
		LoadSound("SoundEffects/Environmental/", "saw");	
		LoadSound("SoundEffects/Environmental/", "sizzle");
		LoadSound("SoundEffects/Environmental/", "flame-spawn");
		LoadSound("SoundEffects/Environmental/", "kernel-hit-floor2");
		LoadSound("SoundEffects/Environmental/", "flame-destroy");
		LoadSound("SoundEffects/Environmental/", "flame-dispenser");
		LoadSound ("SoundEffects/Environmental/", "cork-pop");
		LoadSound("SoundEffects/Environmental/", "Biscuit");
		LoadSound("SoundEffects/Environmental/", "Biscuit-Worried");
		LoadSound("SoundEffects/Environmental/", "Jazzie-Hit");
		LoadSound("SoundEffects/Environmental/", "Candle-Death");
		LoadSound("SoundEffects/Environmental/", "Jazzie-Fall");
		LoadSound ("SoundEffects/Environmental/", "boing");
		LoadSound ("SoundEffects/Environmental/", "marshmallowHit");
		LoadSound("SoundEffects/Enemies/", "squash");
		LoadSound("SoundEffects/Enemies/", "butter-death");
		LoadSound("SoundEffects/Enemies/", "salt-collision");
		LoadSound("SoundEffects/Enemies/", "salt-shaker-explode");
		LoadSound("SoundEffects/Enemies/", "salt-shaker-spit");

		LoadSound("SoundEffects/Player/", "jump");
		LoadSound("SoundEffects/Player/", "blink");
		LoadSound("SoundEffects/Player/", "landing");
		LoadSound("SoundEffects/Player/", "player-death");
		LoadSound("SoundEffects/Player/", "popping");

		initialized = true;
	}

	public void Start() {
		instance = this;
		audioSourcePool = new List<AudioSource>();

		for(int i = 0; i < audioPoolSize; i++) {
			audioSourcePool.Add(gameObject.AddComponent<AudioSource>());
		}
		Initialize ();
	}

	public static AudioClip GetBackgroundMusic() {
		if (backgroundMusic == null) {
			LoadBackgroundMusic ("Music/main-verse");
		}
		return backgroundMusic;
	}

	public static AudioClip LoadBackgroundMusic(string filename) {
		return Resources.Load (filename) as AudioClip;
	}

	public static void LoadSound(string directory, string filename) {
		//quit if the file is already in memory
		foreach(AudioFile file in instance.audioFiles) {
			if (file.filename == filename) {
				return;
			}
		}

		AudioFile audioFile = new AudioFile ();
		audioFile.filename = filename;

		audioFile.clip = Resources.Load (directory + audioFile.filename) as AudioClip;

		instance.audioFiles.Add (audioFile);
	}
		
	public static void PlaySoundAfterTime(string filename, float delay) {
		instance.StartCoroutine (instance.PlayAfterTime (filename, delay));
	}

	public IEnumerator PlayAfterTime(string filename, float delay) {
		yield return new WaitForSeconds(delay);
		PlaySound (filename);
	}

	public static void PlaySound(string filename) {
		PlaySound (filename, 1.0f);
	}

	public static void DisablePlayingSounds() {
		foreach(AudioSource audio in audioSourcePool) {
			if(audio.isPlaying) {
				audio.Stop ();
				return;
			}
		}
	}

	public static void PlaySound(string filename, float pitch) {
		PlaySound (filename, pitch, 1.0f);
	}

	public static void PlaySound(string filename, float pitch, float volume) {
		if (!Settings.sfxEnabled) {
			return;
		}

		AudioFile fileToPlay = null;
		foreach(AudioFile file in instance.audioFiles) {
			if (file.filename == filename) {
				fileToPlay = file;
			}
		}

		if (fileToPlay != null) {
			//look for an available audio source
			foreach(AudioSource audio in audioSourcePool) {
				if(!audio.isPlaying) {
					audio.clip = fileToPlay.clip;
					audio.pitch = pitch;
					audio.volume = volume;
					audio.Play ();
					return;
				}
			}

			//we haven't got an available audio source so pick a random one from the pool to play the sound on
			int audioSourceIndex = Random.Range(0, audioPoolSize - 1);
			audioSourcePool[audioSourceIndex].clip = fileToPlay.clip;
			audioSourcePool [audioSourceIndex].volume = volume;
			audioSourcePool[audioSourceIndex].Play ();
		}
	}
}
