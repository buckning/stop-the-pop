using UnityEngine;
using System.Collections;

public class SnowflakeBehaviour : MonoBehaviour {
	Animator snowflakeAnimator;
	bool collected = false;

	public void Start() {
		snowflakeAnimator = GetComponent<Animator> ();
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
			PopcornKernelController player = otherObject.gameObject.GetComponent<PopcornKernelController> ();

			player.ResetTemperature();

			snowflakeAnimator.SetTrigger("PickedUp");
			AudioManager.PlaySound ("snowflake-new");
			#if UNITY_IOS
			SocialServiceManager.GetInstance ().UnlockAchievement ("cooldown");
			#elif UNITY_ANDROID
			SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_cool_down);
			#endif
			StartCoroutine(DestroyAfterDelay());
		}
	}

	public void Reset() {
		collected = false;
	}

	IEnumerator DestroyAfterDelay() {
		yield return new WaitForSeconds(0.2f);
		//this is wrapped in a container object, delete the container, including this
		transform.parent.gameObject.SetActive(false);
	}
}
