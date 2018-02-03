using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class SocialServiceManager : MonoBehaviour {
	ISocialService socialService = null;

	private static SocialServiceManager instance;

	bool appWentInBackground = false;

	bool twitterOpened = false;
	bool facebookOpened = false;

	AchievementCache cache;

	void Awake() {
		DontDestroyOnLoad(gameObject);
		cache = new AchievementCache ();
	}

	public void Authenticate () {
		Debug.Log ("Authenticate");
		#if UNITY_IOS
		socialService = new GameCenterService();
		#endif
		#if UNITY_ANDROID
		socialService = new GooglePlayService();
		((GooglePlayService)socialService).Init();
		#endif

		if (socialService != null) {
			socialService.Authenticate ();
		}
	}

	public bool IsAuthenticated () {
		if (socialService != null) {
			return socialService.IsAuthenticated ();
		}
		return false;
	}

	public void PostToLeaderboard(int score, string leaderboardId) {
		if (socialService != null) {
			socialService.PostScoreToLeaderboard (score, leaderboardId);
		}
	}

	public string GetMyUserId() {
		if (socialService != null) {
			return socialService.GetMyUserId ();
		}
		return null;
	}

	public void ShowLeaderboard(string leaderboardId) {
		if (socialService != null) {
			socialService.ShowLeaderboard (leaderboardId);
		}
	}

	public void ShowLeaderboard() {
		if (socialService != null) {
			socialService.ShowLeaderboard ();
		}
	}

	public void ShowAchievements() {
		if (socialService != null) {
			socialService.ShowAchievements ();
		}
	}

	public void UnlockAchievement(string achievementId) {
		if (socialService != null && !cache.Contains(achievementId)) {
			socialService.UnlockAchievement (achievementId);
			cache.Add (achievementId);
		}
	}

	public static SocialServiceManager GetInstance() {
		if(instance == null) {
			instance = (SocialServiceManager)GameObject.FindObjectOfType (typeof(SocialServiceManager));
		}

		return instance;
	}

	public void GetLeaderboardScores(string leaderboardId, Action<List<IScore>> callback) {
		if (socialService != null) {
			Debug.Log ("Loading leaderboard " + leaderboardId);
			socialService.GetLeaderboardScores (leaderboardId, callback);
		}
	}


	IEnumerator OpenFacebookPage(){
		//try to open the native facebook app
		appWentInBackground = false;
		Application.OpenURL("fb://page/629199253921499");
		yield return new WaitForSeconds(0.1f);

		//we should be paused if starting the native facebook app took focus. So just reset 
		if(appWentInBackground){
			appWentInBackground = false;
		}
		else {
			//fall back to opening facebook in the browser
			Application.OpenURL("https://www.facebook.com/stopthepopgame/");
		}
		facebookOpened = true;

		if (twitterOpened) {
			#if UNITY_IOS
			SocialServiceManager.GetInstance ().UnlockAchievement ("socialmediaaddict");
			#elif UNITY_ANDROID
			SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_social_media_addict);
			#endif
		}
	}

	IEnumerator OpenTwitterPage(){
		//try to open the native twitter app
		appWentInBackground = false;
		Application.OpenURL("twitter://user?screen_name=stopthepopgame");
		yield return new WaitForSeconds(0.1f);

		//we should be paused if starting the native twitter app took focus. So just reset 
		if(appWentInBackground){
			appWentInBackground = false;
		}
		else {
			//fall back to opening facebook in the browser
			Application.OpenURL("https://twitter.com/stopthepopgame");
		}
		twitterOpened = true;

		if (facebookOpened) {
			#if UNITY_IOS
			SocialServiceManager.GetInstance ().UnlockAchievement ("socialmediaaddict");
			#elif UNITY_ANDROID
			SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_social_media_addict);
			#endif
		}
	}

	public void OpenFacebook() {
		StartCoroutine (OpenFacebookPage ());
	}

	public void OpenTwitter() {
		StartCoroutine(OpenTwitterPage());
	}

	//callback which is called when the app is paused (used when facebook link is pressed)
	void OnApplicationPause() {
		appWentInBackground = true;
	}

	public void RateUs() {
		#if UNITY_ANDROID
		Application.OpenURL("market://details?id=com.stopthepopgame.stopthepop");
		#elif UNITY_IPHONE
		Application.OpenURL("itms-apps://itunes.apple.com/app/stop-the-pop/id1166315634");
		#endif
		StartCoroutine(UnlockRateAchievement());
	}

	IEnumerator UnlockRateAchievement() {
		yield return new WaitForSeconds (5f);
		#if UNITY_IOS
		SocialServiceManager.GetInstance ().UnlockAchievement ("critic");
		#elif UNITY_ANDROID
		SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_critic);
		#endif
	}
}
