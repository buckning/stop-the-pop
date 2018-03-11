using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.SocialPlatforms;

public class LevelCompletePanelBehaviour : MonoBehaviour {
	public HudListener mainHud;

	public Image starCoinsCollectedImage;
	public Image starLivesLostImage;
	public Image starTimeTakenImage;

	public Image whiteFlashPanel; 

	public Share shareInfo;

	public Text textCoinsCollectedImage;
	public Text textLivesLostImage;
	public Text textTimeTakenImage;
	private TextFieldNumberAnimator textCoinsAnimator;
	private TextFieldNumberAnimator textLivesAnimator;
	private TextFieldNumberAnimator textTimeAnimator;

	public Image spinningImage;
	public GameObject spinningStars;

	public GameObject videoAdButton;
	public GameObject nextLevelButton;
	public GameObject restartLevelButton;
	public GameObject mainMenuButton;
	public GameObject shareButton;
	public GameObject leaderboardButton;

	public GameObject starBurstParticleSystem;

	public Image confettiPos1;
	public Image confettiPos2;

	public GameObject confettiParticleSystem1;
	public GameObject confettiParticleSystem2;

	GameObject confettiParticleSystem1Cached;
	GameObject confettiParticleSystem2Cached;

	public GAui[] sparkles;

	float timeActive = 0.0f;

	int numOfCoins = 0;
	int lengthOfTimeInLevel = 0;
	int numberOfLivesLost = 0;

	bool coinAnimationPlaying = false;
	bool livesLostAnimationPlaying = false;
	bool timeTakenAnimationPlaying = false;

	float lastSoundPlay = 0.0f;

	int numberOfStarsVisible = 0;

	public PlayerRewardPanel playerRewardScreen;

	private string[] googlePlayServiceLevelAchievementIds = {	
		GPGSIds.achievement_level_1_pro,
		GPGSIds.achievement_level_2_pro,
		GPGSIds.achievement_level_3_pro,
		GPGSIds.achievement_level_4_pro,
		GPGSIds.achievement_level_5_pro,
		GPGSIds.achievement_level_6_pro,
		GPGSIds.achievement_level_7_pro,
		GPGSIds.achievement_level_8_pro,
		GPGSIds.achievement_level_9_pro,
		GPGSIds.achievement_level_10_pro
	};

	private string[] googlePlayServiceLevelLeaderboardIds = {
		GPGSIds.leaderboard_best_time__level_1,
		GPGSIds.leaderboard_best_time__level_2,
		GPGSIds.leaderboard_best_time__level_3,
		GPGSIds.leaderboard_best_time__level_4,
		GPGSIds.leaderboard_best_time__level_5,
		GPGSIds.leaderboard_best_time__level_6,
		GPGSIds.leaderboard_best_time__level_7,
		GPGSIds.leaderboard_best_time__level_8,
		GPGSIds.leaderboard_best_time__level_9,
		GPGSIds.leaderboard_best_time__level_10
	};

	public void SpawnCoinCollectedStarBurst() {
		Vector3 positionOfStar = starCoinsCollectedImage.rectTransform.TransformPoint (Vector3.zero);
		Instantiate (starBurstParticleSystem, positionOfStar, Quaternion.identity);
		AudioManager.PlaySound ("Star");
	}

	public void SpawnLivesLostStarBurst() {
		Vector3 positionOfStar = starLivesLostImage.rectTransform.TransformPoint (Vector3.zero);
		Instantiate (starBurstParticleSystem, positionOfStar, Quaternion.identity);
		AudioManager.PlaySound ("Star", 1.1f);
	}

	public void SpawnTimeTakenStarBurst() {
		Vector3 positionOfStar = starTimeTakenImage.rectTransform.TransformPoint (Vector3.zero);
		Instantiate (starBurstParticleSystem, positionOfStar, Quaternion.identity);
		AudioManager.PlaySound ("Star", 1.05f);
	}

	void PlayCoinSound() {
		if ((Time.unscaledTime - lastSoundPlay) > 0.05f) {
			AudioManager.PlaySound ("coin-new", 1.0f);
			lastSoundPlay = Time.unscaledTime;
		}
	}

	public void ShowLevelCompletePanel(string levelName) {

		#if UNITY_IOS
		SocialServiceManager.GetInstance ().GetLeaderboardScores ("testbesttimelevel" + mainHud.levelName.Substring (10), InitialLeaderboardScoresCallback);
		#endif
		#if UNITY_ANDROID
		int leaderboardIndex = Int32.Parse(mainHud.levelName.Substring (10)) - 1;
		SocialServiceManager.GetInstance ().GetLeaderboardScores (googlePlayServiceLevelLeaderboardIds[leaderboardIndex], InitialLeaderboardScoresCallback);
		#endif

		//don't show the video ad button if there were no coins collected
		numOfCoins = 100 - (GameObject.FindObjectsOfType(typeof(CoinBehaviour)) as CoinBehaviour[]).Length;
		starCoinsCollectedImage.gameObject.SetActive (false);
		starLivesLostImage.gameObject.SetActive (false);
		starTimeTakenImage.gameObject.SetActive (false);

		coinAnimationPlaying = false;
		timeTakenAnimationPlaying = false;
		livesLostAnimationPlaying = false;

		spinningStars.SetActive (false);
		spinningStars.SetActive (false);

		foreach (GAui sparkle in sparkles) {
			sparkle.gameObject.SetActive (false);
		}

		numberOfLivesLost = CurrentLevel.GetLivesLost();
		lengthOfTimeInLevel = (int)CurrentLevel.GetLengthOfTimeInLevel();

		#if UNITY_IOS
		SocialServiceManager.GetInstance().PostToLeaderboard(lengthOfTimeInLevel, "testbesttimelevel" + mainHud.levelName.Substring (10));
		#endif
		#if UNITY_ANDROID
		SocialServiceManager.GetInstance().PostToLeaderboard(lengthOfTimeInLevel, googlePlayServiceLevelLeaderboardIds[leaderboardIndex]);
		#endif

		textCoinsCollectedImage.text = "";
		textTimeTakenImage.text = "";
		textLivesLostImage.text = "";

		textCoinsAnimator = textCoinsCollectedImage.GetComponent<TextFieldNumberAnimator> ();
		textLivesAnimator = textLivesLostImage.GetComponent<TextFieldNumberAnimator> ();
		textTimeAnimator = textTimeTakenImage.GetComponent<TextFieldNumberAnimator> ();

		textTimeAnimator.Reset ();
		textCoinsAnimator.Reset ();
		textLivesAnimator.Reset ();

		textLivesAnimator.initialNumber = 0;
		textLivesAnimator.currentNumber = 0;
		textLivesAnimator.desiredNumber = 0;

		textCoinsAnimator.initialNumber = 0.0f;
		textCoinsAnimator.currentNumber = 0.0f;
		textCoinsAnimator.desiredNumber = 0.0f;

		textTimeAnimator.initialNumber = 0.0f;
		textTimeAnimator.currentNumber = 0.0f;
		textTimeAnimator.desiredNumber = 0.0f;

		textCoinsAnimator.SetNumber (numOfCoins);
		textCoinsAnimator.animationCompleteListeners = AnimateCoinsCollectedStar;
		textCoinsAnimator.valueIncrementedListeners = CoinIncrementedCallback;

		AnalyticsManager.SendLevelCompleteEvent(levelName, numOfCoins, numberOfLivesLost, lengthOfTimeInLevel);
	}

	void CoinIncrementedCallback(float value) {
		PlayCoinSound ();
	}

	/****
	 * This gets called back when the text field animator is finished animating 
	 */
	public void AnimateCoinsCollectedStar() {
		//animate the coins star when goal score has been reached
		if (numOfCoins >= LevelHighScoreDefaults.goalTotalCoins && !coinAnimationPlaying) {
			starCoinsCollectedImage.gameObject.GetComponent<GAui> ().m_FadeIn.CallbackWhenFinished.m_Method = "StartAnimatingTimeTakenTextField";
			starCoinsCollectedImage.gameObject.GetComponent<GAui> ().m_FadeIn.CallbackWhenFinished.m_Reciever = gameObject;
			starCoinsCollectedImage.gameObject.SetActive (true);
			starCoinsCollectedImage.gameObject.GetComponent<GAui> ().MoveIn ();

			numberOfStarsVisible++;
			coinAnimationPlaying = true;
		} else {
			StartAnimatingTimeTakenTextField ();
		}
	}

	public void StartAnimatingTimeTakenTextField() {
		if(numOfCoins >= LevelHighScoreDefaults.goalTotalCoins) {
			ShowWhiteFlash ();
		}
		StartCoroutine (AnimateTimeTakenAfterDelay());
	}

	IEnumerator AnimateTimeTakenAfterDelay() {
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + 0.2f) {
			yield return null;
		}
		textTimeAnimator.SetNumber (lengthOfTimeInLevel);
		textTimeAnimator.animationCompleteListeners = AnimateTimeTakenStar;
		textTimeAnimator.valueIncrementedListeners = CoinIncrementedCallback;
	}

	/***
	 * This gets called back by the text field animator on the time taken text box
	 */
	public void AnimateTimeTakenStar() {
		//animate the star for level time 
		if (lengthOfTimeInLevel <= LevelHighScoreDefaults.goalLevelCompleteTime && !timeTakenAnimationPlaying) {
			starTimeTakenImage.gameObject.GetComponent<GAui> ().m_FadeIn.CallbackWhenFinished.m_Method = "StartAnimatingLivesLostTextField";
			starTimeTakenImage.gameObject.GetComponent<GAui> ().m_FadeIn.CallbackWhenFinished.m_Reciever = gameObject;

			starTimeTakenImage.gameObject.SetActive (true);
			starTimeTakenImage.gameObject.GetComponent<GAui> ().MoveIn ();

			numberOfStarsVisible++;
			timeTakenAnimationPlaying = true;
		} else {
			StartAnimatingLivesLostTextField ();
		}
	}

	public void StartAnimatingLivesLostTextField() {
		if (lengthOfTimeInLevel <= LevelHighScoreDefaults.goalLevelCompleteTime) {
			ShowWhiteFlash ();
		}
		StartCoroutine (AnimateLivesLostAfterDelay ());
	}

	IEnumerator AnimateLivesLostAfterDelay() {
		float start = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup < start + 0.2f) {
			yield return null;
		}

		textLivesAnimator.initialNumber = 999;
		textLivesAnimator.currentNumber = 999;
		textLivesAnimator.SetNumber (numberOfLivesLost);
		textLivesAnimator.animationCompleteListeners = AnimateLivesLostStar;
		textLivesAnimator.valueDecrementedListeners = CoinIncrementedCallback;
	}

	public void AnimateLivesLostStar() {
		if (numberOfLivesLost <= LevelHighScoreDefaults.goalNumberOfLivesLost && !livesLostAnimationPlaying) {
			if (ShouldShowGameLikeDialog ()) {
				ShowLikeGameDialog ();
			} else {
				starLivesLostImage.gameObject.GetComponent<GAui> ().m_FadeIn.CallbackWhenFinished.m_Method = "AnimateTheRestOfTheLevelCompleteScreen";
			}
			starLivesLostImage.gameObject.GetComponent<GAui> ().m_FadeIn.CallbackWhenFinished.m_Reciever = gameObject;

			starLivesLostImage.gameObject.SetActive (true);
			starLivesLostImage.gameObject.GetComponent<GAui> ().MoveIn ();

			numberOfStarsVisible++;
			livesLostAnimationPlaying = true;
		} else {
			if (ShouldShowGameLikeDialog ()) {
				ShowLikeGameDialog ();
			} else {
				AnimateTheRestOfTheLevelCompleteScreen ();
			}
		}
	}

	public void ShowWhiteFlash() {
		whiteFlashPanel.gameObject.SetActive (true);
		whiteFlashPanel.gameObject.GetComponent<GAui> ().MoveOut(GSui.eGUIMove.SelfAndChildren);
	}

	public bool ShouldShowGiftReward() {
		if (StoreInventory.GetAllLockedPlayerCustomisations ().Count == 0) {
			return false;
		}

		if (Settings.lastRewardGivenTime == "2000-01-01 00:00:00") {	//initial play
			return true;
		}
		if (new TimeDiff ().MinutesSince (Settings.lastRewardGivenTime) >= 20) {
			return true;
		}

		return false;
	}

	public void AnimateTheRestOfTheLevelCompleteScreen() {
		if (numberOfLivesLost <= LevelHighScoreDefaults.goalNumberOfLivesLost) {
			ShowWhiteFlash ();
		}
		DialogBox dialogBox = mainHud.dialogBox;
		Image dialogBoxBackgroundFader = mainHud.dialogBoxBackgroundFader;
		if (dialogBox.gameObject.activeInHierarchy) {
			dialogBoxBackgroundFader.GetComponent<GAui> ().MoveOut();
			dialogBox.gameObject.GetComponent<GAui> ().MoveOut ();
			AudioManager.PlaySound ("InfoPanelSlideIn", 0.9f);
		}

		if (numberOfStarsVisible == 3) {
			//enable the spinner
			if (!spinningImage.gameObject.activeInHierarchy) {
				spinningImage.gameObject.SetActive (true);
				spinningImage.gameObject.GetComponent<GAui> ().MoveIn ();

				spinningStars.SetActive (true);
				spinningStars.gameObject.GetComponent<GAui> ().MoveIn ();

				//enable sparkles
				foreach (GAui sparkle in sparkles) {
					sparkle.gameObject.SetActive (true);
				}

				Vector3 positionOfConfetti1 = confettiPos1.rectTransform.TransformPoint (Vector3.zero);
				confettiParticleSystem1Cached = (GameObject) Instantiate (confettiParticleSystem1, positionOfConfetti1, Quaternion.identity);

				Vector3 positionOfConfetti2 = confettiPos2.rectTransform.TransformPoint (Vector3.zero);
				confettiParticleSystem2Cached = (GameObject)Instantiate (confettiParticleSystem2, positionOfConfetti2, Quaternion.identity);

				#if UNITY_IOS
				SocialServiceManager.GetInstance ().UnlockAchievement ("level" + mainHud.levelName.Substring (10) + "pro");
				#endif
				#if UNITY_ANDROID
				int achievementIndex = Int32.Parse(mainHud.levelName.Substring (10)) - 1;
				SocialServiceManager.GetInstance ().UnlockAchievement (googlePlayServiceLevelAchievementIds[achievementIndex]);
				#endif

			}
		}

		if (!videoAdButton.activeInHierarchy) {
			videoAdButton.gameObject.SetActive (true);
			videoAdButton.gameObject.GetComponent<GAui> ().MoveIn ();
		}

		if (!restartLevelButton.activeInHierarchy) {
			restartLevelButton.gameObject.SetActive (true);
			restartLevelButton.gameObject.GetComponent<GAui> ().MoveIn ();
		}

		if (!mainMenuButton.activeInHierarchy) {
			mainMenuButton.gameObject.SetActive (true);
			mainMenuButton.gameObject.GetComponent<GAui> ().MoveIn ();
		}

		if (!nextLevelButton.activeInHierarchy) {
			nextLevelButton.gameObject.SetActive (true);
			nextLevelButton.gameObject.GetComponent<GAui> ().MoveIn ();
		}

		if (!shareButton.activeInHierarchy) {
			shareButton.gameObject.SetActive (true);
			shareButton.gameObject.GetComponent<GAui> ().MoveIn ();

			shareInfo.title = "I completed level " + mainHud.levelName.Substring (10) +
				" of Stop the Pop!";
			shareInfo.bodyText = "I completed level " + mainHud.levelName.Substring (10) +
			" of Stop the Pop! Can you do better?\nDownload for free: " + Strings.APP_STORE_LINK + "\n#stopthepop";
		}

		if (!leaderboardButton.activeInHierarchy) {
			leaderboardButton.gameObject.SetActive (true);
			leaderboardButton.gameObject.GetComponent<GAui> ().MoveIn ();
		}

		DisplayRewardScreen ();
	}

	public void ShowLeaderboards() {
		if(SocialServiceManager.GetInstance().IsAuthenticated()) {
			#if UNITY_IOS
			SocialServiceManager.GetInstance ().ShowLeaderboard ("testbesttimelevel" + mainHud.levelName.Substring (10));
			#endif
			#if UNITY_ANDROID
			int achievementIndex = Int32.Parse(mainHud.levelName.Substring (10)) - 1;
			SocialServiceManager.GetInstance ().ShowLeaderboard (googlePlayServiceLevelLeaderboardIds[achievementIndex]);
			#endif

		} else {
			SocialServiceManager.GetInstance ().Authenticate ();
		}
	}

	void InitialLeaderboardScoresCallback(List<IScore> leaderboard) {
		Debug.Log ("got leaderboard of size" + leaderboard.Count);

		IScore myBestScore = GetMyLowestScore (leaderboard);
		IScore bestScore = GetLowestScore (leaderboard);

		if (bestScore == null || lengthOfTimeInLevel < bestScore.value) {
			Debug.Log ("Congratulations you are on top of the leaderboard");
			return;
		} 

		if (myBestScore != null) {
			if (lengthOfTimeInLevel < myBestScore.value) {
				Debug.Log ("Congratulations, you bet your best time! You are number " + myBestScore.rank + " on the leaderboard");
			} else {
				Debug.Log ("Not good enough to move up the leaderboard!");
			}
		} else {
			//I'm not on the leaderboard and not good enough to be on the leaderboard
		}
	}

	IScore GetLowestScore(List<IScore> scores) {
		foreach (IScore score in scores) {
			if (score.rank == 1) {
				return score;
			}
		}
		return null;
	}

	IScore GetMyLowestScore(List<IScore> scores) {
		IScore bestScore = null;
		foreach (IScore score in scores) {
			print ("GetMyLowestScore: " + score.userID + " my userId = " + SocialServiceManager.GetInstance ().GetMyUserId () +
			" score " + score.value);
			if (score.userID == SocialServiceManager.GetInstance ().GetMyUserId ()) {
				if (bestScore == null) {
					bestScore = score;
				} else {
					if (score.value < bestScore.value) {
						bestScore = score;
					}
				}
			}
		}
		return bestScore;
	}

	public void DisplayRewardScreen() {
		if (ShouldShowGiftReward ()) {
			PlayerCustomisation gift = GiftRewardGenerator.GenerateReward ();
			if (gift.type == PlayerCustomisationType.GLASSES) {
				print ("glasses not supported yet, not providing a reward...");
				return;
			}
			playerRewardScreen.gameObject.SetActive(true);	
			playerRewardScreen.SetRewardImage(GetSpriteImageOfReward(gift));
			playerRewardScreen.ShowGift ();

			//this should not be the case because the player should be able to choose if they want to equip the new
			//customisation. This could be considered a bug. The correct way would be to register a callback with
			//PlayerRewardPanel for when an equip button is pressed. This might be fine for the time being
			ActivateRewardForPlayer (gift);	
		}
	}

	public void ActivateRewardForPlayer(PlayerCustomisation reward) {
		if (reward.type == PlayerCustomisationType.FACIAL_HAIR) {
			SelectedPlayerCustomisations.selectedFacialHair = reward.name;
		} else if (reward.type == PlayerCustomisationType.SHOES) {
			SelectedPlayerCustomisations.selectedShoes = reward.name;
		} else if (reward.type == PlayerCustomisationType.HAT_OR_HAIR) {
			SelectedPlayerCustomisations.selectedHat = reward.name;
		} 
	}

	public Sprite GetSpriteImageOfReward(PlayerCustomisation gift) {
		Sprite[] sprites = null;

		//now load in the right sprite sheet
		if (gift.type == PlayerCustomisationType.FACIAL_HAIR) {
			sprites = PlayerCustomisation.facialHairSprites;
		} else if (gift.type == PlayerCustomisationType.SHOES) {
			sprites = PlayerCustomisation.shoesSprites;
		} else if (gift.type == PlayerCustomisationType.HAT_OR_HAIR) {
			sprites = PlayerCustomisation.hatSprites;
		} 

		Sprite rewardImage = null;
		foreach (Sprite sprite in sprites) {
			if (gift.type == PlayerCustomisationType.SHOES) {
				if (sprite.name == gift.name + "Left") {
					rewardImage = sprite;
					break;
				}
			} else {
				if (sprite.name == gift.name) {
					rewardImage = sprite;
					break;
				}
			}
		}
		return rewardImage;
	}

	public void ShowLikeGameDialog() {
		if (ShouldShowGameLikeDialog ()) {
			DialogBox dialogBox = mainHud.dialogBox;
			Image dialogBoxBackgroundFader = mainHud.dialogBoxBackgroundFader;
			dialogBoxBackgroundFader.gameObject.SetActive (true);
			dialogBoxBackgroundFader.GetComponent<GAui> ().MoveIn();

			dialogBox.dialogText.text = Strings.UI_DO_YOU_LIKE_STOP_THE_POP;
			dialogBox.confirmButton.onClick.RemoveAllListeners ();
			dialogBox.returnButton.onClick.RemoveAllListeners ();
			dialogBox.confirmButton.onClick.AddListener (() => {
				AnalyticsManager.SendLikeGameEvent(true);
				ShowRateDialog ();
			});
			dialogBox.returnButton.onClick.AddListener (() => {
				AnimateTheRestOfTheLevelCompleteScreen ();
				AnalyticsManager.SendLikeGameEvent(false);
				Settings.rateUsDialogSnoozeTime = 24 * 7;	//if they are still playing in 7 days, they must like it, so show again
				string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
				string currentTime = DateTime.Now.ToString(dateTimeFormat);
				Settings.lastRateUsDialogShowTime = currentTime;
				GameDataPersistor.Save(GameStats.GetInstance().GetGameData());
			});
			dialogBox.gameObject.SetActive (true);
			dialogBox.gameObject.GetComponent<GAui> ().MoveIn ();
			AudioManager.PlaySound ("InfoPanelSlideIn");
		} else {
			AnimateTheRestOfTheLevelCompleteScreen ();
		}
	}

	public void ShowRateDialog() {
		DialogBox dialogBox = mainHud.dialogBox;

		dialogBox.dialogText.text = Strings.UI_DO_YOU_WANT_TO_RATE_STOP_THE_POP;
		dialogBox.confirmButton.onClick.RemoveAllListeners ();
		dialogBox.returnButton.onClick.RemoveAllListeners ();
		dialogBox.confirmButton.onClick.AddListener (() => {
			RateUs();
			AnimateTheRestOfTheLevelCompleteScreen();
			AnalyticsManager.SendRateGameEvent(true);
			Settings.rateUsDialogSnoozeTime = -1;	//snooze indefinitely
			string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
			string currentTime = DateTime.Now.ToString(dateTimeFormat);
			Settings.lastRateUsDialogShowTime = currentTime;
			GameDataPersistor.Save(GameStats.GetInstance().GetGameData());
		});
		dialogBox.returnButton.onClick.AddListener (() => {
			AnimateTheRestOfTheLevelCompleteScreen();
			AnalyticsManager.SendRateGameEvent(false);
			Settings.rateUsDialogSnoozeTime = 24 * 3;	//snooze for 3 days
			string dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
			string currentTime = DateTime.Now.ToString(dateTimeFormat);
			Settings.lastRateUsDialogShowTime = currentTime;
			GameDataPersistor.Save(GameStats.GetInstance().GetGameData());
		});
		dialogBox.gameObject.SetActive (true);
		dialogBox.gameObject.GetComponent<GAui> ().MoveIn ();
		AudioManager.PlaySound ("InfoPanelSlideIn");
	}

	public void RateUs() {
		SocialServiceManager.GetInstance ().RateUs ();
	}

	bool ShouldShowGameLikeDialog() {
		if (Settings.rateUsDialogSnoozeTime == -1) {
			return false;
		}

		//if we should show rate dialog (only show if android or iOS)
		TimeDiff timeDiff = new TimeDiff();

		//don't show the rate dialog if the game is being played less than 10 minutes
		if(timeDiff.MinutesSince(Settings.launchTimeOfGame) < 10) {
			return false;
		}

		//don't show the rate dialog if the player has not finished at least 2 levels
		if (Settings.numberOfLevelsCompletedThisSession < 2) {
			return false;
		}

		int hoursSinceLastLikeUsDialog = (int)timeDiff.HoursSince (Settings.lastRateUsDialogShowTime);

		if(hoursSinceLastLikeUsDialog > Settings.rateUsDialogSnoozeTime) {
			return true;
		}

		return false;
	}

	public void NextLevelButtonPressed() {
		mainHud.NextLevel ();
	}

	public void PlayAdForReward() {
		if (Advertisement.IsReady ("rewardedVideo")) {
			var options = new ShowOptions { resultCallback = GetTripleCoins };
			Advertisement.Show ("rewardedVideo", options);
		} else {
			mainHud.ShowErrorPanel ();
		}
	}

	private void GetTripleCoins(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			textCoinsAnimator.animationCompleteListeners = null;
			textCoinsAnimator.Reset ();
			textCoinsAnimator.AddToNumber (300);

			GameStats stats = GameStats.GetInstance ();
			GameStats.GetInstance ().AddCoinsToTotal (300);
			GameDataPersistor.Save(stats.GetGameData());

			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}

	public void Reset() {
		textCoinsCollectedImage.text = "";
		textTimeTakenImage.text = "";
		textLivesLostImage.text = "";
		numberOfStarsVisible = 0;
		spinningImage.gameObject.SetActive(false);
		timeActive += Time.unscaledDeltaTime;
		Destroy(confettiParticleSystem1Cached);
		Destroy(confettiParticleSystem2Cached);
	}

	IEnumerator ShowAdWhenReady() {
		return null;
	}
}
