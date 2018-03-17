using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class CoinBehaviour : MonoBehaviour {

	public int id = 0;

	bool collected = false;

	static float lastSoundPlay;
	static float pitch;
	Animator coinAnimator;

	private string coinAudioClipPath = "SoundEffects/Collectables/coin-new";

	Vector2 initialPosition;

	private LevelManager levelManager;

	public void Start() {
		ResourceCache.LoadAudioClip (coinAudioClipPath);
		coinAnimator = GetComponent<Animator> ();
		lastSoundPlay = Time.time;
		pitch = 1.0f;
		initialPosition = transform.position;
	}

	public void SetLevelManager(LevelManager levelManager) {
		this.levelManager = levelManager;
	}

	/***
	 * If there is a collision with the player, decrease the temperature
	 * and delete this object
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if (collected) {
			return;
		}

		if(otherObject.gameObject.tag == Strings.PLAYER) {
			collected = true;
//			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
//			player.IncrementCoinCount(id);

			levelManager.AddCollectedCoin (id);
			coinAnimator.SetTrigger("PickedUp");

//			PlaySound (0.05f);

			StartCoroutine(DisableAfterDelay());
			//this is wrapped in a container object, delete the container, including this
		}
	}

	public static void PlaySound(float ignoreSoundsLessThan) {
		//reset the pitch if a coin hasn't been collected in 2 seconds
		if ((Time.time - lastSoundPlay) > 1.2f) {
			pitch = 1.0f;
		} else {
			pitch += 0.03f;
		}

		if ((Time.time - lastSoundPlay) > ignoreSoundsLessThan) {
			AudioManager.PlaySound ("coin-new", pitch);
		}

		lastSoundPlay = Time.time;
	}

	public void Reset() {
		collected = false;
		transform.position = initialPosition;
	}

	IEnumerator DisableAfterDelay() {
		yield return new WaitForSeconds(0.2f);
		coinAnimator.SetTrigger ("Reset");
		gameObject.SetActive (false);
	}
}
