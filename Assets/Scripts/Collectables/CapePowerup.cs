using UnityEngine;
using System.Collections;

public class CapePowerup : MonoBehaviour {
	bool collected = false;

	private string coinAudioClipPath = "SoundEffects/Collectables/cape-new";

	bool movingUp = false;	//for internal animation
	float moveDistance = 0.25f;
	float upperYThreshold;
	float lowerYThreshold;
	float animationSpeed = 0.75f;
	Vector2 originalPosition;
	HudListener hud;

	void Start() {
		ResourceCache.LoadAudioClip (coinAudioClipPath);
		originalPosition = transform.position;
		upperYThreshold = originalPosition.y + moveDistance;
		lowerYThreshold = originalPosition.y - moveDistance;

		if (hud == null) {
			hud = GameObject.Find (HudListener.gameObjectName).GetComponent<HudListener>();
		}
	}

	void Update() {
		//don't need to enable all the capes if the player already has it enabled
		if (hud.GetPlayer ().glidingEnabled) {
			gameObject.SetActive (false);
		}

		float newYPos = 0.0f;
		if (movingUp) {
			newYPos = Mathf.MoveTowards(transform.position.y, 
				upperYThreshold,
				Time.deltaTime * animationSpeed);	
			if (newYPos > (upperYThreshold - 0.1f)) {
				movingUp = false;
			}
		}
		else {
			newYPos = Mathf.MoveTowards(transform.position.y, 
				lowerYThreshold,
				Time.deltaTime * animationSpeed);	
			if (newYPos < lowerYThreshold + 0.1f) {
				movingUp = true;
			}
		}

		transform.position = new Vector2 (transform.position.x, newYPos);
	}

	public void Reset() {
		collected = false;
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
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.cape.gameObject.SetActive (true);
			player.glidingEnabled = true;
			AudioManager.PlaySound ("cape-new");
			gameObject.SetActive (false);
		}
	}
}
