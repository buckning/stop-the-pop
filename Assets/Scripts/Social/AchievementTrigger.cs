using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementTrigger : MonoBehaviour {
	public string achievementName;
	public string androidAchievementName;
	void OnTriggerEnter2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			#if UNITY_IOS
			SocialServiceManager.GetInstance ().UnlockAchievement (achievementName);
			#endif
			#if UNITY_ANDROID
			SocialServiceManager.GetInstance ().UnlockAchievement (androidAchievementName);
			#endif
		}
	}
}
