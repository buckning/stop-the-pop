using UnityEngine;
using System.Collections;

public class MagnetPowerup : MonoBehaviour {
	bool collected = false;

	Animator animator;
	void Start() {
		animator = GetComponent<Animator> ();
	}
	/***
	 * If there is a collision with the player, make the player invincibile for a set amount of time
	 * and delete this object
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if (collected) {
			return;
		}
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			collected = true;
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.magnetEnabled = true;
			AudioManager.PlaySound ("magnet-new");
			animator.SetTrigger ("Pickup");
			player.inputManager.ShowWhiteFlash ();

			#if UNITY_IOS
			SocialServiceManager.GetInstance ().UnlockAchievement ("lawofattraction");
			#endif
			#if UNITY_ANDROID
			SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_law_of_attraction);
			#endif
		}
	}

	public void ResetAnimation() {
		animator.SetTrigger ("Reset");
	}

	public void Reset() {
		collected = false;
	}
}
