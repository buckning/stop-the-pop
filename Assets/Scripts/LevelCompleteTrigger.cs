using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/***
 * End of the level trigger. This triggers the next level to be loaded. 
 * The stats of the level are saved to the file system if they are better than the previous stats.
 */
[RequireComponent (typeof (BoxCollider2D))]
public class LevelCompleteTrigger : MonoBehaviour {
	HudListener levelHud;
	public string nextLevelName;

	bool alreadyTriggered = false;

	public void Start() {
		if (levelHud == null) {
			levelHud = GameObject.Find (HudListener.gameObjectName).GetComponent<HudListener>();
		}
	}

	void OnTriggerEnter2D(Collider2D otherObject) {
		//seen some bugs here since the player can have multiple 
		//colliders attached, causing multiple triggers
		if (alreadyTriggered) {
			return;
		}

		if (otherObject.gameObject.tag == Strings.PLAYER) {
			int numberOfCoins = 100 - (GameObject.FindObjectsOfType(typeof(CoinBehaviour)) as CoinBehaviour[]).Length;
			int numberOfLivesLost = CurrentLevel.GetLivesLost();
			int lengthOfTimeInLevel = (int)CurrentLevel.GetLengthOfTimeInLevel();

			CurrentLevel.AddCoins(numberOfCoins);

			LevelStats thisLevel = new LevelStats();
			thisLevel.levelName = SceneManager.GetActiveScene().name;
			thisLevel.maxNumberOfCoinsCollected = numberOfCoins;
			thisLevel.minNumberOfLivesLost = numberOfLivesLost;
			thisLevel.bestTimeToCompleteLevel = lengthOfTimeInLevel;
			
			GameStats stats = GameStats.GetInstance();

			stats.AddCoinsToTotal (numberOfCoins);			//update the total amount of coins that we have
			stats.AddStatsForLevel(thisLevel);

			if(nextLevelName != Strings.TITLE_SCREEN) {
				//unlock the next level from the level select screen but don't do anything if the next level is the title screen

				//check if we already have the next level saved
				LevelStats nextLevel = stats.GetLevelStats (nextLevelName);

				//we don't have the next level saved, so we just create placeholder info for it
				if (nextLevel == null) {
					nextLevel = new LevelStats ();
					nextLevel.levelName = nextLevelName;
					nextLevel.locked = false;
					nextLevel.maxNumberOfCoinsCollected = -1;
					nextLevel.minNumberOfLivesLost = -1;
					nextLevel.bestTimeToCompleteLevel = -1f;
				} else {
					//if we have some data for the next level, we just make sure that the next level is unlocked
					nextLevel.locked = false;
				}

				stats.AddStatsForLevel(nextLevel);
			}

			//save our stats
			GameDataPersistor.Save(stats.GetGameData());
			levelHud.levelComplete (nextLevelName);
			CurrentLevel.ResetAll ();
			LastCheckpoint.Reset ();
			alreadyTriggered = true;
			Settings.numberOfLevelsCompletedThisSession++;
		}
	}

	public void Reset() {
		alreadyTriggered = false;
	}
}
