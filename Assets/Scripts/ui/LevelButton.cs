using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms;

public class LevelButton : MonoBehaviour {

	public Image lockImage;
	public Image starImage1;
	public Image starImage2;
	public Image starImage3;
	public Text levelText;
	public Button button;

	public TitleScreenListener titleScreenListener;

	private string worldName;

	private string levelName;
	private bool achievementUnlocked = false;

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

	void Start() {
		//we register for the start button event. When the start button is pressed we get called back
		titleScreenListener.titleScreenStartButtonPressedListeners += OnEnable;
	}

	// Use this for initialization
	void OnEnable () {
		worldName = "PanLevel1_";
		levelName = worldName + levelText.text;
		//read in stats object for this level
		LevelStats level = GameStats.GetInstance ().GetLevelStats (levelName);

		if (level == null) {	//if there is no data saved for this level already
			lockImage.enabled = !shouldUnlockThisLevel();
			button.interactable = shouldUnlockThisLevel();
			starImage1.enabled = false;
			starImage2.enabled = false;
			starImage3.enabled = false;
		} else {
			lockImage.enabled = level.locked;
			button.interactable = !level.locked;

			starImage1.enabled = false;
			starImage2.enabled = false;
			starImage3.enabled = false;

			int stars = 0;
			if(level.maxNumberOfCoinsCollected >= LevelHighScoreDefaults.goalTotalCoins) {
				stars++;
			}
			if(level.minNumberOfLivesLost == LevelHighScoreDefaults.goalNumberOfLivesLost) {
				stars++;
			}
			if(level.bestTimeToCompleteLevel <= LevelHighScoreDefaults.goalLevelCompleteTime &&
			   level.bestTimeToCompleteLevel > 0f) {
				stars++;
			}

			if(stars >= 1) {
				starImage1.enabled = true;
			}
			if(stars >=2) {
				starImage2.enabled = true;
			}
			if(stars >= 3) {
				starImage3.enabled = true;
				if (!achievementUnlocked) {
					#if UNITY_IOS
					SocialServiceManager.GetInstance ().UnlockAchievement ("level" + levelText.text + "pro");
					#endif
					#if UNITY_ANDROID
					int achievementIndex = Int32.Parse(levelName.Substring (10)) - 1;
					SocialServiceManager.GetInstance ().UnlockAchievement (googlePlayServiceLevelAchievementIds[achievementIndex]);
					#endif
					achievementUnlocked = true;
				}
			}
		}
	}

	/***
	 * This method was added since during the time of the initial release of Stop the Pop, backwards data storage wasn't considered. 
	 * The problem is that a player can complete the game e.g. Level 8. When the game upgrades, a new level will be added but there 
	 * will be no data for the level saved so this level will be shown as locked. 
	 */
	bool shouldUnlockThisLevel() {
		//check if there is a previous level that was completed?
		int previousLevelNumber = int.Parse(levelName.Substring(worldName.Length)) - 1;

		if (previousLevelNumber > 1) {
			LevelStats previousLevel = GameStats.GetInstance ().GetLevelStats (worldName + previousLevelNumber);

			if (previousLevel != null) {	
				//there could be a case where the previous level stats might not exist. 
				//A typical example of this is if two new levels were created at once or for the first time the game is started
				if (previousLevel.bestTimeToCompleteLevel > -1) {
					button.interactable = true;
					return true;
				}
			}
		}

		return false;
	}
}
