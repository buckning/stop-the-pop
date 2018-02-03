using System;
using System.Collections;
using System.Collections.Generic;

/***
 * This class contains all of the global data in the game in RAM, including, number of coins that the player has collected,
 * what characters are unlocked, what levels are unlocked, etc.
 */
public class GameStats {
	static GameStats instance;

	public List<LevelStats> levels;
	public List<PlayerStatistics> players;	//the available characters
	public List<PlayerCustomisation> playerCustomisations;	//the available customisations, including hats, shoes, glasses and facial hair

	public int totalNumberOfCoins;			//the total number of coins collected in the game so far
	public int numberOfCapes = 0;			//the total number of capes that the player has purchased

	private GameStats() {
		levels = new List<LevelStats>();
		players = new List<PlayerStatistics>();
		playerCustomisations = new List<PlayerCustomisation> ();
	}

	public static GameStats GetInstance() {
		if (instance == null) {
			instance = new GameStats();
		}
		return instance;
	}

	/***
	 * 
	 */
	public void Initialise(GameData data) {
		//read in the file, split each line
		//check for ---LevelStart--- and from there build up all the levels

		string[] lines = data.gameData.Split('\n');
		
		List<int> indexOfLevelStartInFile = new List<int>();
		for(int i = 0; i < lines.Length; i++) {
			string line = lines[i];
			if(line.StartsWith(Strings.LEVEL_START_TAG)) {
				indexOfLevelStartInFile.Add(i);
			}
		}

		List<int> indexOfPlayerStartInFile = new List<int>();
		for(int i = 0; i < lines.Length; i++) {
			string line = lines[i];
			if(line.StartsWith(Strings.PLAYER_START_TAG)) {
				indexOfPlayerStartInFile.Add(i);
			}
		}

		List<int> indexOfPlayerCustomisationsInFile = new List<int> ();
		for (int i = 0; i < lines.Length; i++) {
			string line = lines [i];
			if (line.StartsWith (Strings.PLAYER_CUSTOMISATION_START_TAG)) {
				indexOfPlayerCustomisationsInFile.Add (i);
			}
		}

		//loop through each level in the file
		foreach(int levelStartLine in indexOfLevelStartInFile) {
			LevelStats level = new LevelStats();
			
			//---LevelStart---
			//LevelName: levelName
			//Locked: true
			//BestTime: 100
			//LivesLost: 0
			//CoinsCollected: 0
			level.levelName = lines[levelStartLine + 1].Replace(Strings.LEVEL_NAME_TAG, "");
			level.locked = Boolean.Parse(lines[levelStartLine + 2].Replace(Strings.LOCKED_TAG, ""));
			level.bestTimeToCompleteLevel = Int32.Parse(lines[levelStartLine + 3].Replace(Strings.BEST_TIME_TAG, ""));
			level.minNumberOfLivesLost = Int32.Parse(lines[levelStartLine + 4].Replace(Strings.LIVES_LOST_TAG, ""));
			level.maxNumberOfCoinsCollected = Int32.Parse(lines[levelStartLine + 5].Replace(Strings.COINS_COLLECTED_TAG, ""));
			AddStatsForLevel(level);
		}

		//loop through each player in the file and load them into RAM
		foreach(int playerStartLine in indexOfPlayerStartInFile) {
			PlayerStatistics player = new PlayerStatistics();

			//---LevelStart---
			//LevelName: levelName
			//Locked: true
			//BestTime: 100
			//LivesLost: 0
			//CoinsCollected: 0
			player.name = lines[playerStartLine + 1].Replace(Strings.PLAYER_NAME_TAG,"");
			player.locked = Boolean.Parse(lines[playerStartLine + 2].Replace(Strings.PLAYER_LOCKED_TAG, ""));
			AddStatsForPlayer(player);
		}

		//loop through each player customisation in the file and load them into RAM
		foreach(int playerCustomisationStartLine in indexOfPlayerCustomisationsInFile) {
			PlayerCustomisation playerCustomisation = new PlayerCustomisation();

			//---PlayerCustomisation---
			//PlayerCustomisationName: playerCustomisationName
			//PlayerCustomisationLocked: true
			//PlayerCustomisationType: HAT_OR_HAIR
			playerCustomisation.name = lines[playerCustomisationStartLine + 1].Replace(Strings.PLAYER_CUSTOMISATION_NAME_TAG,"");
			playerCustomisation.locked = Boolean.Parse(lines[playerCustomisationStartLine + 2].Replace(Strings.PLAYER_CUSTOMISATION_LOCKED_TAG, ""));
			string playerCustomisationType = lines [playerCustomisationStartLine + 3].Replace (Strings.PLAYER_CUSTOMISATION_TYPE_TAG, "");
			playerCustomisation.type = (PlayerCustomisationType) System.Enum.Parse (typeof(PlayerCustomisationType), playerCustomisationType);

			AddStatsForPlayerCustomisation(playerCustomisation);
		}


		foreach (string line in lines) {
			if (line.StartsWith (Strings.TOTAL_COINS_COLLECTED_TAG)) {
				totalNumberOfCoins = Int32.Parse (line.Replace (Strings.TOTAL_COINS_COLLECTED_TAG, ""));
			}

			if (line.StartsWith (Strings.NUMBER_OF_CAPES_TAG)) {
				numberOfCapes = Int32.Parse (line.Replace (Strings.NUMBER_OF_CAPES_TAG, ""));
			}
			if (line.StartsWith (Strings.SETTINGS_SFX_TAG)) {
				Settings.sfxEnabled = Boolean.Parse (line.Replace (Strings.SETTINGS_SFX_TAG, ""));
			}
			if (line.StartsWith (Strings.SETTINGS_MUSIC_TAG)) {
				Settings.musicEnabled = Boolean.Parse (line.Replace (Strings.SETTINGS_MUSIC_TAG, ""));
			}
			if (line.StartsWith (Strings.SETTINGS_LAST_LEVEL_SKIP_TIME)) {
				Settings.lastSkipLevelTime = line.Replace (Strings.SETTINGS_LAST_LEVEL_SKIP_TIME, "");
			}
			if (line.StartsWith (Strings.SETTINGS_LAST_RATE_US_DIALOG_SNOOZE_TIME)) {
				Settings.rateUsDialogSnoozeTime = Int32.Parse (line.Replace (Strings.SETTINGS_LAST_RATE_US_DIALOG_SNOOZE_TIME, ""));
			}
			if (line.StartsWith (Strings.SETTINGS_LAST_RATE_US_DIALOG_SHOW_TIME)) {
				Settings.lastRateUsDialogShowTime = line.Replace (Strings.SETTINGS_LAST_RATE_US_DIALOG_SHOW_TIME, "");
			}
			if (line.StartsWith (Strings.SETTINGS_LAST_REWARD_GIVEN_TIME)) {
				Settings.lastRewardGivenTime = line.Replace (Strings.SETTINGS_LAST_REWARD_GIVEN_TIME, "");
			}
			if (line.StartsWith (Strings.SETTINGS_SELECTED_HAT)) {
				Settings.selectedHat = line.Replace (Strings.SETTINGS_SELECTED_HAT, "");
			}
			if (line.StartsWith (Strings.SETTINGS_SELECTED_GLASSES)) {
				Settings.selectedGlasses = line.Replace (Strings.SETTINGS_SELECTED_GLASSES, "");
			}
			if (line.StartsWith (Strings.SETTINGS_SELECTED_FACIAL_HAIR)) {
				Settings.selectedFacialHair = line.Replace (Strings.SETTINGS_SELECTED_FACIAL_HAIR, "");
			}
			if (line.StartsWith (Strings.SETTINGS_SELECTED_SHOES)) {
				Settings.selectedShoes = line.Replace (Strings.SETTINGS_SELECTED_SHOES, "");
			}
		}
	}

	public void AddStatsForLevel(LevelStats levelStats) {
		bool levelStatsInList = false;

		//check if we have stats for this level already
		foreach (LevelStats level in levels) {
			if (level.levelName == levelStats.levelName) {
				levelStatsInList = true;
			}
		}

		if (!levelStatsInList) {
			levels.Insert (0, levelStats);
		} else {
			foreach (LevelStats level in levels) {
				if (level.levelName == levelStats.levelName) {
					int coinsCollected = levelStats.maxNumberOfCoinsCollected;

					if(coinsCollected > level.maxNumberOfCoinsCollected && coinsCollected != -1) {
						level.maxNumberOfCoinsCollected = coinsCollected;
					}

					int livesLost = levelStats.minNumberOfLivesLost;
					if(livesLost < level.minNumberOfLivesLost || level.minNumberOfLivesLost == -1 && livesLost != -1) {
						level.minNumberOfLivesLost = livesLost;
					}

					float lengthOfTimeInLevel = levelStats.bestTimeToCompleteLevel;
					if(lengthOfTimeInLevel < level.bestTimeToCompleteLevel || level.bestTimeToCompleteLevel <= 0f && lengthOfTimeInLevel > 0f) {
						level.bestTimeToCompleteLevel = levelStats.bestTimeToCompleteLevel;
					}
				}
			}
		}
	}

	/***
	 * Save a player to our cache if it does not exist already
	 */
	public void AddStatsForPlayer(PlayerStatistics playerStats) {
		bool playerStatsInList = false;

		//check if we have stats for this player already
		foreach (PlayerStatistics player in players) {
			if (player.name == playerStats.name) {
				playerStatsInList = true;
			}
		}

		if (!playerStatsInList) {
			players.Insert (0, playerStats);
		} 
	}


	/****
	 * Add a player customisation to our cache if it does not exist in the cache already
	 */
	public void AddStatsForPlayerCustomisation(PlayerCustomisation playerCustomisation) {
		bool playerCustomisationStatsInList = false;

		//check if we have stats for this customisation already
		foreach (PlayerCustomisation customisation in playerCustomisations) {
			if (customisation.name == playerCustomisation.name) {
				playerCustomisationStatsInList = true;
			}
		}

		if (!playerCustomisationStatsInList) {
			playerCustomisations.Insert (0, playerCustomisation);
		}
	}

	/***
	 * The the information for a player customisation
	 */
	public PlayerCustomisation GetPlayerCustomisation(string playerCustomisationName) {
		foreach(PlayerCustomisation playerCustomisation in playerCustomisations) {
			if(playerCustomisation.name == playerCustomisationName) {
				return playerCustomisation;
			}
		}
		return null;
	}

	/***
	 * Get the information for a character
	 */
	public PlayerStatistics GetPlayerStats(string playerName) {
		foreach(PlayerStatistics player in players) {
			if(player.name == playerName) {
				return player;
			}
		}
		return null;
	}

	/***
	 * Get the information for a level
	 */
	public LevelStats GetLevelStats(string levelName) {
		foreach(LevelStats level in levels) {
			if(level.levelName == levelName) {
				return level;
			}
		}
		return null;
	}

	public void AddCoinsToTotal(int amountOfCoins) {
		totalNumberOfCoins += amountOfCoins;
	}

	/***
	 * Get the data from the game that can be saved to disk.
	 * This does not save to disk, but reads from all of the caches in this class
	 */
	public GameData GetGameData() {
		GameData data = new GameData ();
		string gameData = "\n###GameData### \n";

		foreach(LevelStats level in levels) {
			gameData += Strings.LEVEL_START_TAG + "\n";
			gameData += Strings.LEVEL_NAME_TAG + level.levelName + "\n";
			gameData += Strings.LOCKED_TAG + level.locked + "\n";
			gameData += Strings.BEST_TIME_TAG + level.bestTimeToCompleteLevel + "\n";
			gameData += Strings.LIVES_LOST_TAG + level.minNumberOfLivesLost + "\n";			
			gameData += Strings.COINS_COLLECTED_TAG + level.maxNumberOfCoinsCollected + "\n";
			gameData += Strings.LEVEL_END_TAG + "\n";
		}

		foreach(PlayerStatistics player in players) {
			gameData += Strings.PLAYER_START_TAG + "\n";
			gameData += Strings.PLAYER_NAME_TAG + player.name + "\n";
			gameData += Strings.PLAYER_LOCKED_TAG + player.locked + "\n";
			gameData += Strings.PLAYER_END_TAG + "\n";
		}

		foreach (PlayerCustomisation playerCustomisation in playerCustomisations) {
			gameData += Strings.PLAYER_CUSTOMISATION_START_TAG + "\n";
			gameData += Strings.PLAYER_CUSTOMISATION_NAME_TAG + playerCustomisation.name + "\n";
			gameData += Strings.PLAYER_CUSTOMISATION_LOCKED_TAG + playerCustomisation.locked + "\n";
			gameData += Strings.PLAYER_CUSTOMISATION_TYPE_TAG + playerCustomisation.type + "\n";
			gameData += Strings.PLAYER_CUSTOMISATION_END_TAG + "\n";
		}

		gameData += Strings.COLLECTABLES_START_TAG + "\n";
		gameData += Strings.TOTAL_COINS_COLLECTED_TAG + totalNumberOfCoins + "\n";
		gameData += Strings.NUMBER_OF_CAPES_TAG + numberOfCapes + "\n";
		gameData += Strings.COLLECTABLES_END_TAG + "\n";

		gameData += Strings.SETTINGS_START_TAG + "\n";
		gameData += Strings.SETTINGS_SFX_TAG + Settings.sfxEnabled + "\n";
		gameData += Strings.SETTINGS_MUSIC_TAG + Settings.musicEnabled + "\n";
		gameData += Strings.SETTINGS_LAST_RATE_US_DIALOG_SNOOZE_TIME + Settings.rateUsDialogSnoozeTime + "\n";
		gameData += Strings.SETTINGS_LAST_REWARD_GIVEN_TIME + Settings.lastRewardGivenTime + "\n";
		gameData += Strings.SETTINGS_LAST_RATE_US_DIALOG_SHOW_TIME + Settings.lastRateUsDialogShowTime + "\n";
		gameData += Strings.SETTINGS_LAST_LEVEL_SKIP_TIME + Settings.lastSkipLevelTime + "\n";
		gameData += Strings.SETTINGS_SELECTED_HAT + Settings.selectedHat + "\n";
		gameData += Strings.SETTINGS_SELECTED_GLASSES + Settings.selectedGlasses + "\n";
		gameData += Strings.SETTINGS_SELECTED_FACIAL_HAIR + Settings.selectedFacialHair + "\n";
		gameData += Strings.SETTINGS_SELECTED_SHOES + Settings.selectedShoes + "\n";
		gameData += Strings.SETTINGS_END_TAG + "\n";
		data.gameData = gameData;
		return data;
	}

	public void SaveToDisk() {
		GameDataPersistor.Save(GetGameData());
	}
}
