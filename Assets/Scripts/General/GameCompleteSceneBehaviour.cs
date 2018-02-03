using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameCompleteSceneBehaviour : MonoBehaviour {
	public Text gameCompleteText;

	void Start () {
		Time.timeScale = 1.0f;
		gameCompleteText.text = Strings.UI_MORE_LEVELS_COMING_SOON;
		StartCoroutine (StartTitleScreen ());

		#if UNITY_IOS
		SocialServiceManager.GetInstance ().UnlockAchievement ("readyformore");
		#endif
		#if UNITY_ANDROID
		SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_ready_for_more);
		#endif
	}

	IEnumerator StartTitleScreen() {
		yield return new WaitForSeconds(90.5f);
		SceneManager.LoadScene ("TitleScreen");
	}

	public void ReturnToMainMenu() {
		SceneManager.LoadScene ("TitleScreen");
	}

	public void ShowLeaderboards() {
		if (SocialServiceManager.GetInstance ().IsAuthenticated ()) {
			SocialServiceManager.GetInstance ().ShowLeaderboard ();
		} else {
			SocialServiceManager.GetInstance ().Authenticate ();
		}
	}

	public void OpenFacebook() {
		SocialServiceManager.GetInstance ().OpenFacebook ();
	}

	public void OpenTwitter() {
		SocialServiceManager.GetInstance ().OpenTwitter ();
	}
		
	public void RateUs() {
		SocialServiceManager.GetInstance ().RateUs ();
	}
}
