using UnityEngine;
using System.Collections;

public class CameraShakeTrigger : MonoBehaviour {

	public bool startShakeTrigger = true;
	AudioSource audioSource;
	HudListener hud;

	public AudioClip audioClip;
	public AudioClip musicClip;	//the clip of music that should be played for this section

	public bool disableSfxAfterTime = false;
	public float timeTillSfxDisabled = 0.0f;

	AudioSource sceneCameraAudio;

	SpriteRenderer spriteRenderer;

	void Start() {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		hud = GameObject.Find ("LevelHUD").GetComponent<HudListener> ();

		audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = audioClip;
	}

	void Update() {
		if (!Settings.sfxEnabled) {
			audioSource.Stop ();
		}

		if (spriteRenderer == null) {
			return;
		}
		if (!spriteRenderer.isVisible && audioSource.isPlaying) {
			audioSource.Stop ();
		}
	}

	public void OnTriggerEnter2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			if (startShakeTrigger) {
				hud.StartShakingScreen ();

				if (disableSfxAfterTime) {
					StartCoroutine (StopSfx ());
				}

				if (audioClip != null) {
					audioSource.Play ();
				}
			} else {
				hud.StopShakingScreen ();
				audioSource.Stop ();
			}
		}
	}


	public IEnumerator StopSfx() {
		yield return new WaitForSeconds(timeTillSfxDisabled);
		if (audioSource.isPlaying) {
			audioSource.Stop ();
		}
	}
}
