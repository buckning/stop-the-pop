using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine.Advertisements;

public class TitleScreenListener : MonoBehaviour {
	static bool socialSignInAttempted = false;
	//Event Listeners for notifying the animator
	public delegate void TitleScreenEvent ();
	public event TitleScreenEvent titleScreenActiveListeners;
	public event TitleScreenEvent titleScreenStartButtonPressedListeners;
	public event TitleScreenEvent titleScreenLevelSelectBackButtonPressedListeners;
	public event TitleScreenEvent titleScreenInformationButtonPressedListeners;
	public event TitleScreenEvent titleScreenInformationBackButtonPressedListeners;
	public event TitleScreenEvent titleScreenQuitDialogEnabledListeners;
	public event TitleScreenEvent titleScreenQuitDialogCancelListeners;
	public event TitleScreenEvent titleScreenPlayerCustomisationButtonPressedListeners;
	public event TitleScreenEvent titleScreenShowErrorPanelListeners;
	public event TitleScreenEvent titleScreenHideErrorPanelListeners;

	public GameObject titleScreen;
	public GameObject levelSelectScreen;
	public GameObject loadingScreen;
	public GameObject quitDialogBox;
	public GameObject informationPanel;
	public GameObject errorPanel;
	public PlayerCustomiseScreen playerCustomiseScreen;

	//localised text
	public Text loadingScreenText;
	public Text selectLevelText;

	private GameObject currentScreen;

	public TitleScreenCamera titleScreenCamera;

	public String androidAdvertisementId = "";
	public String iOSAdvertisementId = "";

	void Start() {
		#if UNITY_ANDROID
		Advertisement.Initialize (androidAdvertisementId, false);
		#elif UNITY_IPHONE
		Advertisement.Initialize (iOSAdvertisementId, false);
		#endif

		LocalisationManager.LoadLanguageFile ();
		loadingScreenText.text = Strings.UI_LOADING;
		selectLevelText.text = Strings.UI_SELECT_LEVEL;

		currentScreen = titleScreen;
		Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

		GSui.Instance.m_AutoAnimation = false;

		GameData gameData = GameDataPersistor.Load ();

		GameStats stats = GameStats.GetInstance ();

		if (gameData != null) {
			GameStats.GetInstance ().Initialise (gameData);

			//the saved music and sfx settings are loaded into RAM here, so we can enable or disable them here
			AudioSource audioSource = GameObject.Find("TitleScreenAudio").GetComponent<AudioSource>();
			if (Settings.musicEnabled) {
				if (!audioSource.isPlaying) {
					audioSource.Play ();
				}
			} else {
				audioSource.Stop ();
			}
		} else {
			//we have not found our saved game data, going to recreate it in RAM
			//this is a new game
			//initialise the GameStats to a new game
			LevelStats firstLevel = new LevelStats();
			firstLevel.levelName = "PanLevel1_1";
			firstLevel.locked = false;
			firstLevel.maxNumberOfCoinsCollected = -1;
			firstLevel.minNumberOfLivesLost = -1;
			firstLevel.bestTimeToCompleteLevel = -1;
			stats.AddStatsForLevel(firstLevel);

			PlayerStatistics initialPlayer = new PlayerStatistics ();
			initialPlayer.name = SelectedPlayer.selectedPlayer;	//use the default player
			initialPlayer.locked = false;
			stats.AddStatsForPlayer (initialPlayer);

			//save our game data to the file system
			GameDataPersistor.Save(stats.GetGameData());
		}

		Store.LoadStore ();
		if(Settings.launchTimeOfGame == null) {
			Settings.launchTimeOfGame = new TimeDiff ().TimeNow ();
		}
		Time.timeScale = 1.0f;

		if (!socialSignInAttempted) {
			SocialServiceManager.GetInstance ().Authenticate ();
			socialSignInAttempted = true;
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			BackButtonPressed();
		}
	}

	public void EnableTitleScreen() {
		titleScreenCamera.target = titleScreenCamera.titleScreenCameraPosition;
		currentScreen = titleScreen;
		titleScreenActiveListeners ();
	}

	public void ShowLeaderboards() {
		if (SocialServiceManager.GetInstance ().IsAuthenticated ()) {
			SocialServiceManager.GetInstance ().ShowAchievements ();
		} else {
			SocialServiceManager.GetInstance ().Authenticate ();
		}
	}

	public void InformationButtonPressed() {
		AudioManager.PlaySound ("Click");
		AudioManager.PlaySound ("InfoPanelSlideIn");
		informationPanel.gameObject.SetActive (true);
		titleScreenInformationButtonPressedListeners ();
		currentScreen = informationPanel;
	}

	public void TitleMenuStartButtonPressed() {
		PlayButtonClickSound();

		if (titleScreenStartButtonPressedListeners != null) {
			titleScreenStartButtonPressedListeners ();
		}
		currentScreen = levelSelectScreen;
	}

	public void PlayButtonClickSound() {
		AudioManager.PlaySound ("Click");
	}
		
	public void LevelButtonPressed(int level) {
		PlayButtonClickSound();
		SetActiveScreen (loadingScreen);
		CurrentLevel.ResetAll ();	//reset the counters and difficulty for the current level 
		LoadNextLevelAsync("PanLevel1_" + level);
	}

	public void PlayerCustomisationButtonPressed() {
		PlayButtonClickSound();
		playerCustomiseScreen.SetActive (true);
		currentScreen = playerCustomiseScreen.gameObject;
		titleScreenPlayerCustomisationButtonPressedListeners();
		titleScreenCamera.target = titleScreenCamera.titleScreenPlayerCustomisationCameraPosition;
	}

	public void BackButtonPressed() {
		if (currentScreen.Equals (quitDialogBox)) {
			AudioManager.PlaySound ("InfoPanelSlideIn", 0.95f);
			titleScreenQuitDialogCancelListeners ();
			currentScreen = titleScreen;
			return;
		} else if (currentScreen.Equals (informationPanel)) {
			AudioManager.PlaySound ("InfoPanelSlideIn", 0.95f);
			titleScreenInformationBackButtonPressedListeners ();
			currentScreen = titleScreen;
			return;
		} else if (currentScreen.Equals (titleScreen)) {
			quitDialogBox.SetActive (true);
			titleScreenQuitDialogEnabledListeners ();
			AudioManager.PlaySound ("InfoPanelSlideIn");
			currentScreen = quitDialogBox;
		} else if (currentScreen.Equals (levelSelectScreen)) {
			AudioManager.PlaySound ("Click", 0.9f);
			titleScreenLevelSelectBackButtonPressedListeners ();
			currentScreen = titleScreen;
		} else if (currentScreen.Equals (errorPanel)) {
			AudioManager.PlaySound ("Click", 0.9f);
			titleScreenHideErrorPanelListeners ();
			currentScreen = playerCustomiseScreen.gameObject;
		}
	}

	public void ShowErrorPanel() {
		titleScreenShowErrorPanelListeners ();
		currentScreen = errorPanel;
	}

	public void QuitGame() {
		Application.Quit ();
	}

	public void ReturnToTitleScreen() {
		AudioManager.PlaySound ("GoBack");

		if (titleScreenActiveListeners != null) {
			titleScreenActiveListeners ();	//call back our listeners
		}
	}

	private void SetActiveScreen(GameObject activeScreen) {
		titleScreen.SetActive (false);
		loadingScreen.SetActive (false);
		playerCustomiseScreen.SetActive (false);
		activeScreen.SetActive (true);
		currentScreen = activeScreen;
	}
		
	public void OpenFacebook() {
		SocialServiceManager.GetInstance ().OpenFacebook ();
	}

	public void OpenTwitter() {
		SocialServiceManager.GetInstance ().OpenTwitter ();
	}

	public void LoadNextLevelAsync(String nextLevel) {
		AsyncOperation levelLoadJob = SceneManager.LoadSceneAsync (nextLevel);
		levelLoadJob.allowSceneActivation = false;
		StartCoroutine(LoadNextLevel(levelLoadJob));
	}

	IEnumerator LoadNextLevel(AsyncOperation levelLoadJob) {
		while (levelLoadJob.progress < 0.9f) {
			//if loading bar is needed, update it here
			yield return null;
		}
		AudioSource audioSource = GameObject.Find("TitleScreenAudio").GetComponent<AudioSource>();
		audioSource.Stop ();
		levelLoadJob.allowSceneActivation = true;
	}

	public void OnRateUs() {
		SocialServiceManager.GetInstance ().RateUs ();
	}
}
