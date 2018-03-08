using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Advertisements;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

/***
 * This class acts like an input manager.
 */
public class HudListener : MonoBehaviour {

	private Vector2 startPos;

	public const string gameObjectName = "LevelHUD";

	public GAui menuBackgroundFader;

	public GameObject pauseMenu;

	public Text debugText;

	//the control panel that contains the forward, backward, and jump buttons
	public GameObject playerControlPanel;
	public Image jumpButton;
	public Image attackButton;
	public Image leftDirectionButton;
	public Image rightDirectionButton;

	public GameObject pauseButton;

	public GameObject loadingPanel;

	public Button skipCutSceneButton;

	public StorePanel storePanel;

	public InformationPanel infoPanel;

	public TutorialImagePanel tutorialPanel;

	public GameObject errorPanel;

	public DialogBox dialogBox;

	public DialogBox watchVideoAdPanel;

	public GameObject leftButton;

	public ScreenFader whiteFlashPanel;

	public GameObject rightButton;

	public Button skipLevelButton;	//need a reference to this to disable if it is the last level

	//this button is used to skip the death animation. It is only shown during the pop sequence. 
	public GameObject reloadButton;

	public CoinCountPanel coinCountPanel;

	public ScreenFader screenFader;
	public ScreenFader damageIndicator;

	public Image dialogBoxBackgroundFader;

	public Image levelLoadingBar;

	//the actual player in the game
	public PlayerController player;
	public Transform initialPlayerPosition;

	private float playerSpeed = 0f;

	public string levelName;
	
	public ThermometerBehaviour thermometer;

	public LevelCompletePanelBehaviour levelCompletePanel;

	private string nextLevel;

	private Vector2 directionalInput;

	private bool dedicatedAchievementUnlocked = false;

	private bool gameOverTriggered = false;

	private Image leftButtonImage;
	private Image rightButtonImage;

	//Localised text fields
	public Text continueText;
	public Text restartLevelText;
	public Text pausedText;
	public Text skipLevelText;
	public Text storeText;
	public Text mainMenuText;
	public Text videoMakeEasierText;
	public Text videoStoreText;

	public Text coinsText;
	public Text livesText;
	public Text timeText;
	public Text loadingText;

	private InputManager inputManager;

	public void debug(string message) {
		if (debugText != null && Debug.isDebugBuild) {
			debugText.text += message + "\n";
		}
	}

	public void clearDebug() {
		if (debugText != null && Debug.isDebugBuild) {
			debugText.text = "";
		}
	}

	void Start() {
		continueText.text = Strings.UI_CONTINUE;
		restartLevelText.text = Strings.UI_RESTART_LEVEL;
		pausedText.text = Strings.UI_PAUSED;
		skipLevelText.text = Strings.UI_SKIP_LEVEL;
		storeText.text = Strings.UI_STORE;
		mainMenuText.text = Strings.UI_MAIN_MENU;
		videoMakeEasierText.text = Strings.UI_DO_YOU_WANT_TO_WATCH_A_VIDEO_TO_MAKE_THIS_LEVEL_EASIER;
		videoStoreText.text = Strings.UI_WATCH_VIDEO_FOR_MORE_COINS;
		coinsText.text = Strings.UI_COINS;
		timeText.text = Strings.UI_TIME;
		livesText.text = Strings.UI_LIVES;
		loadingText.text = Strings.UI_LOADING;

		GSui.Instance.m_AutoAnimation = false;
		leftButtonImage = GameObject.Find("Canvas/LevelHUD/PlayerControlPanel/ButtonPlayerLeft").GetComponent<Image> ();
		rightButtonImage = GameObject.Find("Canvas/LevelHUD/PlayerControlPanel/ButtonPlayerRight").GetComponent<Image> ();

		if(initialPlayerPosition == null) {
			initialPlayerPosition = GameObject.Find ("PlayerDropPoint").GetComponent<Transform>();
		}
		if (player == null) {
			GameObject instance = Instantiate(Resources.Load("Prefabs/" + SelectedPlayer.selectedPlayer, typeof(GameObject))) as GameObject;
			player = (PlayerController)instance.GetComponent<PlayerController> ();
			Vector2 playerPosition = initialPlayerPosition.position;
			player.setPosition (playerPosition);
		}
			
		player.hud = this;

		reloadButton.SetActive (false);	//this button should only be shown during the pop sequence so the player can skip it

		directionalInput = new Vector2 ();
		Time.timeScale = 1.0f;
		pauseMenu.SetActive(false);
		thermometer.gameObject.SetActive (true);
		levelName = SceneManager.GetActiveScene ().name;
		AnalyticsManager.SendLevelStartEvent(levelName);


		if (isMobile ()) {
			inputManager = InitialiseSoftKeyInputManager ();
		} else {
			inputManager = gameObject.AddComponent<KeyboardInputManager> ();
		}

		player.inputManager = inputManager;
		playerControlPanel.SetActive (isMobile ());

		SimpleSpawner[] spawners = Resources.FindObjectsOfTypeAll (typeof(SimpleSpawner)) as SimpleSpawner[];
		foreach (SimpleSpawner spawner in spawners) {
			spawner.Reset ();
			spawner.Spawn ();
		}

		screenFader.fadeOutCompleteListeners += FadeOutCompleted;
		screenFader.fadeInCompleteListeners += FadeInCompleted;

		FadeIn ();

		string nextLevelName = GameObject.Find("LevelCompleteTrigger").GetComponent<LevelCompleteTrigger>().nextLevelName;
		if (nextLevelName == "GameComplete") {
			skipLevelButton.GetComponent<GAui> ().enabled = false;
			skipLevelText.GetComponent<GAui> ().enabled = false;
			skipLevelButton.gameObject.SetActive (false);
			skipLevelText.gameObject.SetActive (false);
		}
	}

	private SoftKeyInputManager InitialiseSoftKeyInputManager() {
		SoftKeyInputManager softKeyInputManager = gameObject.AddComponent<SoftKeyInputManager> ();
		softKeyInputManager.jumpButton = jumpButton;
		softKeyInputManager.leftDirectionButton = leftDirectionButton;
		softKeyInputManager.rightDirectionButton = rightDirectionButton;
		softKeyInputManager.attackButton = attackButton;
		softKeyInputManager.SetUp ();
		return softKeyInputManager;
	}

	void FadeIn() {
		pauseButton.SetActive (false);
		playerControlPanel.SetActive (false);
		//TODO see if the fix for the autopilot bug is still required with new input managers
		gameOverTriggered = false;
		screenFader.StartFadingIn ();
	}

	void FadeOut() {
		coinCountPanel.gameObject.SetActive (false);
		pauseButton.SetActive (false);
		playerControlPanel.SetActive (false);
		screenFader.StartFadingOut ();
	}

	void FadeOutCompleted() {
		Retry();
	}

	void FadeInCompleted() {
		pauseButton.SetActive(true);
		if (isMobile ()) {
			playerControlPanel.SetActive (true);
		} else {
			playerControlPanel.SetActive (false);
		}
		coinCountPanel.gameObject.SetActive (true);
		coinCountPanel.coinCount = player.GetCollectedCoins ().Count;
	}


	void Update() {
		//skip button is only active when a cutscene is running
		//so only add to the length of time in the level when the skip button isn't active
		if (!skipCutSceneButton.gameObject.activeInHierarchy) {
			CurrentLevel.AddLengthOfTimeInLevel (Time.deltaTime);
		}

		thermometer.currentTemperature = player.GetTemperature() / 100.0f;

		if (player.GetTemperature() >= 100.0f && !gameOverTriggered) {
			directionalInput = Vector2.zero;
			GameOver();
			gameOverTriggered = true;

			return;
		}

		if (inputManager.BackButtonPressed ()) {
			BackButtonPressed ();
		}
	}

	public void BackButtonPressed() {
		//only allow to unpause if the level complete panel is not shown
		if(!levelCompletePanel.gameObject.activeInHierarchy 
			&& !dialogBox.gameObject.activeInHierarchy 
			&& !storePanel.gameObject.activeInHierarchy) {

			PauseButtonPressed ();
		} 

		if (levelCompletePanel.gameObject.activeInHierarchy && errorPanel.activeInHierarchy) {
			HideErrorPanel ();
		}

		if (storePanel.gameObject.activeInHierarchy) {
			if (errorPanel.activeInHierarchy) {
				HideErrorPanel ();
			} else {
				storePanel.BackButtonPressed ();
			}
		}

		if (dialogBox.gameObject.activeInHierarchy) {
			dialogBoxBackgroundFader.GetComponent<GAui> ().MoveOut();
			dialogBox.gameObject.GetComponent<GAui> ().MoveOut ();
			AudioManager.PlaySound ("InfoPanelSlideIn", 0.9f);
			Pause();
		}
		if (infoPanel.gameObject.activeInHierarchy) {
			infoPanel.QuitButtonPressed ();
		}
	}

	public void ShowErrorPanel() {
		errorPanel.gameObject.SetActive (true);
		errorPanel.GetComponent<GAui> ().MoveIn ();
	}

	public void HideErrorPanel() {
		AudioManager.PlaySound ("Click");
		errorPanel.GetComponent<GAui> ().MoveOut ();
	}

	public void ShowDamageIndicator() {
		damageIndicator.StartFadingIn ();
	}

	public void SetCoinCount(int coinCount) {
		coinCountPanel.SetCoinCount (coinCount);
	}

	public void PauseButtonPressed() {
		if (Time.timeScale >= 1.0f) {
			AudioManager.PlaySound ("Click");
			Pause ();
			menuBackgroundFader.gameObject.SetActive (true);
			menuBackgroundFader.MoveIn (GSui.eGUIMove.SelfAndChildren);
			pauseMenu.SetActive (true);

			Component[] levelButtons = pauseMenu.gameObject.transform.GetComponentsInChildren(typeof(GAui), true);

			foreach(GAui button in levelButtons) {
				if (button.enabled) {	//this if statement is for the skip level button on the last level
					//only show skip button and text if ad is ready
					if (button.gameObject == skipLevelButton.gameObject || button.gameObject == skipLevelText.gameObject) {	
						ShowSkipButtonIfAdIsReadyAnd24HoursHasElapsedSinceLastSkip(button);
					} else {
						button.gameObject.SetActive (true);
						button.MoveIn (GSui.eGUIMove.SelfAndChildren);
					}
				}
			}
		} else {
			AudioManager.PlaySound ("Click", 0.9f);
			ResumeGame ();
			coinCountPanel.gameObject.SetActive (true);
		}
	}

	void ShowSkipButtonIfAdIsReadyAnd24HoursHasElapsedSinceLastSkip(GAui element) {
		if (Advertisement.IsReady ("rewardedVideo") && Has24HoursSinceLastLevelSkip()) {
			element.gameObject.SetActive (true);
			element.MoveIn (GSui.eGUIMove.SelfAndChildren);
		} else {
			element.enabled = false;
			element.gameObject.SetActive (false);
		}
	}

	bool Has24HoursSinceLastLevelSkip() {
		DateTime lastSkipTime = DateTime.Parse(Settings.lastSkipLevelTime);

		DateTime currentTime = DateTime.Now;
		TimeSpan ts = currentTime - lastSkipTime;

		return ts.TotalHours >= 24;
	}

	void ResumeGame() {
		AudioManager.PlaySound ("Click", 0.9f);
		menuBackgroundFader.MoveOut (GSui.eGUIMove.SelfAndChildren);
		Component[] levelButtons = pauseMenu.gameObject.transform.GetComponentsInChildren(typeof(GAui), true);

		foreach(GAui button in levelButtons) {
			if (button.enabled) {
				button.gameObject.SetActive (true);
				button.MoveOut (GSui.eGUIMove.SelfAndChildren);
			}
		}
	}

	public void RetryButtonPressed() {
		reloadButton.SetActive (false);
		FadeOut ();
		Time.timeScale = 1.0f;
		levelCompletePanel.Reset ();
	}

	public void RestartButtonPressed() {
		dialogBox.dialogText.text = (Strings.UI_ARE_YOU_SURE_YOU_WANT_TO_RESTART);
		dialogBox.confirmButton.onClick.RemoveAllListeners ();
		dialogBox.returnButton.onClick.RemoveAllListeners ();
		dialogBox.confirmButton.onClick.AddListener (() => {
			dialogBoxBackgroundFader.GetComponent<GAui> ().MoveOut();
			dialogBox.gameObject.GetComponent<GAui> ().MoveOut ();
			AudioManager.PlaySound ("Click");
			Restart ();
		});
		dialogBox.returnButton.onClick.AddListener (() => {
			dialogBoxBackgroundFader.GetComponent<GAui> ().MoveOut();
			dialogBox.gameObject.GetComponent<GAui> ().MoveOut ();
			AudioManager.PlaySound ("InfoPanelSlideIn", 0.95f);
			Pause();
		});
		dialogBox.gameObject.SetActive (true);
		dialogBoxBackgroundFader.gameObject.SetActive (true);
		dialogBoxBackgroundFader.GetComponent<GAui> ().MoveIn();
		dialogBox.gameObject.GetComponent<GAui> ().MoveIn ();
		AudioManager.PlaySound ("InfoPanelSlideIn");

	}

	public void DisableDialogBoxBackground() {
		dialogBoxBackgroundFader.gameObject.SetActive (false);
	}
	
	public void ResumeButtonPressed() {
		AudioManager.PlaySound ("ButtonClick");
		ResumeGame ();
		coinCountPanel.gameObject.SetActive (true);
	}
	
	public void QuitButtonPressed() {
		AnalyticsManager.SendQuitLevelEvent(levelName, player.transform.position, CurrentLevel.GetLivesLost(), 
			(int) CurrentLevel.GetLengthOfTimeInLevel());
		QuitGame ();
	}
		
	/***
	 * Restart the level. Counters and checkpoints are reset.
	 */
	public void Restart() {
		menuBackgroundFader.MoveOut (GSui.eGUIMove.SelfAndChildren);
		CurrentLevel.Reset ();		//reset counters
		LastCheckpoint.Reset ();	//reset checkpoint
		levelCompletePanel.Reset();
		Retry();
	}

	/***
	 * Retry from the last checkpoint
	 */
	public void Retry() {
		Resume ();
		reloadButton.SetActive (false);

		FadeIn ();

		if(initialPlayerPosition == null) {
			initialPlayerPosition = GameObject.Find ("PlayerDropPoint").GetComponent<Transform>();
		}

		GameObject oldPlayer = player.gameObject;

		GameObject instance = Instantiate(Resources.Load("Prefabs/" + SelectedPlayer.selectedPlayer, typeof(GameObject))) as GameObject;
		player = (PlayerController)instance.GetComponent<PlayerController> ();
		Vector2 playerPosition = initialPlayerPosition.position;
		player.setPosition (playerPosition);
		CameraFollow camera = GameObject.Find ("Main Camera").GetComponent<CameraFollow>();
		camera.isLocked = false;
		camera.target = player;
		player.hud = this;

		thermometer.Reset ();

		CameraZoomTrigger zoomTrigger = GameObject.Find ("InitialCameraZoomTrigger").GetComponent<CameraZoomTrigger>();

		camera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, zoomTrigger.zoomAmount);
		StopShakingScreen ();
		Checkpoint[] checkpoints = Resources.FindObjectsOfTypeAll (typeof(Checkpoint)) as Checkpoint[];
		foreach (Checkpoint checkpoint in checkpoints) {
			checkpoint.gameObject.SetActive (true);
		}

		CoinBehaviour[] coins = Resources.FindObjectsOfTypeAll(typeof(CoinBehaviour)) as CoinBehaviour[];
		foreach(CoinBehaviour coin in coins) {
			coin.Reset ();
			coin.gameObject.transform.localScale = Vector3.one;
			coin.gameObject.SetActive(true);
		}

		MagnetPowerup[] magnets = Resources.FindObjectsOfTypeAll(typeof(MagnetPowerup)) as MagnetPowerup[];
		foreach(MagnetPowerup magnet in magnets) {
			magnet.Reset ();
			magnet.gameObject.SetActive(true);
		}

		SnowflakeBehaviour[] snowflakes = Resources.FindObjectsOfTypeAll (typeof(SnowflakeBehaviour)) as SnowflakeBehaviour[];
		foreach (SnowflakeBehaviour snowflake in snowflakes) {
			snowflake.Reset ();
			snowflake.gameObject.transform.parent.gameObject.SetActive (true);
		}

		CapePowerup[] capes = Resources.FindObjectsOfTypeAll (typeof(CapePowerup)) as CapePowerup[];
		foreach (CapePowerup cape in capes) {
			cape.Reset ();
			cape.gameObject.SetActive (true);
		}

		SimpleSpawner[] spawners = Resources.FindObjectsOfTypeAll (typeof(SimpleSpawner)) as SimpleSpawner[];
		foreach (SimpleSpawner spawner in spawners) {
			spawner.Reset ();
			spawner.Spawn ();
		}

		LevelCompleteTrigger levelCompleteTrigger = GameObject.FindObjectOfType(typeof(LevelCompleteTrigger)) as LevelCompleteTrigger;
		levelCompleteTrigger.Reset ();
			
		KernelBehaviour[] spawnedKernels = Resources.FindObjectsOfTypeAll (typeof(KernelBehaviour)) as KernelBehaviour[];
		foreach (KernelBehaviour kernel in spawnedKernels) {
			if (kernel.gameObject.name == "Kernel(Clone)" || kernel.gameObject.name == "Kernel1(Clone)" ) {
				if (kernel.transform.parent != null) {
					if (!kernel.transform.parent.gameObject.name.StartsWith ("KernelSpawner")) {
						print (kernel.transform.parent.gameObject.name);
						Destroy (kernel.gameObject);
					}
				} else {
					Destroy (kernel.gameObject);
				}
			}
		}

		Spawner[] genericSpawners = Resources.FindObjectsOfTypeAll (typeof(Spawner)) as Spawner[];
		foreach (Spawner spawner in genericSpawners) {
			spawner.Reset ();
		}

		SoundTrigger[] soundTriggers = Resources.FindObjectsOfTypeAll (typeof(SoundTrigger)) as SoundTrigger[];
		foreach (SoundTrigger soundTrigger in soundTriggers) {
			soundTrigger.Reset ();
		}

		Sawblade[] sawblades = Resources.FindObjectsOfTypeAll (typeof(Sawblade)) as Sawblade[];
		foreach (Sawblade sawblade in sawblades) {
			sawblade.Reset ();
		}

		GameObject[] popcorn = GameObject.FindGameObjectsWithTag ("Popcorn");
		foreach (GameObject pop in popcorn) {
			if (pop.name == "Popcorn(Clone)") {
				Destroy (pop);
			}
		}

		if (LastCheckpoint.checkpointName == levelName) {
			//move player to the last check point
			Camera.main.gameObject.transform.parent.transform.position = new Vector3(LastCheckpoint.lastCheckpoint.x, LastCheckpoint.lastCheckpoint.y, Camera.main.gameObject.transform.position.z);

			player.setPosition(LastCheckpoint.lastCheckpoint);
			List<int> collectedCoins = LastCheckpoint.GetCollectedCoins();
			//remove all the collected coins from the level

			foreach(CoinBehaviour coin in coins) {
				if(collectedCoins.Contains(coin.id)) {
					coin.gameObject.SetActive (false);
				}
			}
		}

		Destroy (oldPlayer);

		//resetting the state of the player input. Without this, there can be some strange behaviour like where the player would move when the player is not touching the screen
		// TODO - need to implement a similar solution with new input managers
		directionalInput.x = 0.0f;
		directionalInput.y = 0.0f;
		camera.gameOver = false;
	}
	
	public void QuitGame() {
		dialogBox.gameObject.SetActive (false);
		loadingPanel.SetActive (true);
		CurrentLevel.ResetAll ();		//reset counters and difficulty
		LastCheckpoint.Reset ();	//reset checkpoint
		Resume ();	//want to resume, otherwise other scenes could lock up
		SceneManager.LoadScene ("TitleScreen");
	}

	public void GameOver() {
		GameOver (true);
	}

	public void GameOver(bool showReloadButton) {
		if(showReloadButton) {
			reloadButton.SetActive (true);	//button to allow skip of death animation
		}
		playerSpeed = 0f;
		if (isMobile()) {
			playerControlPanel.SetActive (false);
		}
		pauseMenu.SetActive (false);
		thermometer.gameObject.SetActive (true);
		pauseButton.SetActive (false);
		levelCompletePanel.gameObject.SetActive (false);
		watchVideoAdPanel.gameObject.SetActive (false);
	}

	public void StoreButtonPressed() {
		AudioManager.PlaySound ("Click");

		Component[] levelButtons = pauseMenu.gameObject.transform.GetComponentsInChildren(typeof(GAui), true);

		foreach(GAui button in levelButtons) {
			button.MoveOut (GSui.eGUIMove.SelfAndChildren);
		}

		storePanel.gameObject.SetActive (true);

		Component[] storeItems = storePanel.gameObject.transform.GetComponentsInChildren(typeof(GAui), true);

		foreach(GAui button in storeItems) {
			button.MoveIn (GSui.eGUIMove.SelfAndChildren);
		}

		storePanel.InitStorePanel ();
	}

	/***
	 * SHOULD ONLY BE USED BY THE ANIMATOR AT THE END OF A POP SEQUENCE
	 */
	public void TriggerFadeOut() {
		FadeOut ();
	}
	
	public void Pause() {
		Time.timeScale = 0.0f;
		pauseMenu.SetActive(true);
		levelCompletePanel.gameObject.SetActive (false);
		storePanel.gameObject.SetActive (false);
		thermometer.gameObject.SetActive (false);
		infoPanel.gameObject.SetActive (false);
		coinCountPanel.gameObject.SetActive (false);
		watchVideoAdPanel.gameObject.SetActive (false);
		if (isMobile()) {
			playerControlPanel.SetActive (false);
		}
		pauseButton.SetActive (false);
	}
	
	public void Resume() {
		pauseMenu.SetActive(false);
		thermometer.gameObject.SetActive (true);
		levelCompletePanel.gameObject.SetActive (false);
		storePanel.gameObject.SetActive (false);
		infoPanel.gameObject.SetActive (false);
		watchVideoAdPanel.gameObject.SetActive (false);
		if (isMobile()) {
			playerControlPanel.SetActive (true);
		}

		pauseButton.SetActive (true);
		Time.timeScale = 1.0f;
	}

	public void EnableControlPanel(bool enable) {
		if (isMobile()) {
			playerControlPanel.SetActive (enable);
		}
	}

	/***
	 * Retry the level without doing the pop animation
	 */
	public void RetryLevel() {
		FadeOut ();
	}

	public void levelComplete(string nextLevel) {
		this.nextLevel = nextLevel;
		Pause ();
		pauseMenu.SetActive(false);
		thermometer.gameObject.SetActive (false);
		coinCountPanel.gameObject.SetActive (false);
		levelCompletePanel.gameObject.SetActive (true);
		levelCompletePanel.ShowLevelCompletePanel (levelName);

		if (isMobile()) {
			playerControlPanel.SetActive (false);
		}
		pauseButton.SetActive (false);
	}

	public void NextLevel() {
		LoadNextLevelAsync();
	}

	public void LoadNextLevelAsync() {
		levelCompletePanel.gameObject.SetActive (false);
		loadingPanel.SetActive (true);
		AsyncOperation levelLoadJob = SceneManager.LoadSceneAsync (nextLevel);
		levelLoadJob.allowSceneActivation = false;
		StartCoroutine(LoadNextLevel(levelLoadJob));
	}

	IEnumerator LoadNextLevel(AsyncOperation levelLoadJob) {
		while (levelLoadJob.progress < 0.9f) {
			levelLoadingBar.fillAmount = levelLoadJob.progress;
			yield return null;
		}
		levelLoadJob.allowSceneActivation = true;
	}

	private bool isMobile() {
		if (Application.platform == RuntimePlatform.Android || 
		    Application.platform == RuntimePlatform.IPhonePlayer) {
			return true;
		}
		return false;
	}

	public void MainMenuButtonPressed() {
		dialogBox.dialogText.text = (Strings.UI_ARE_YOU_SURE_YOU_WANT_TO_EXIT_TO_THE_MAIN_MENU);
		dialogBox.confirmButton.onClick.RemoveAllListeners ();
		dialogBox.returnButton.onClick.RemoveAllListeners ();
		dialogBox.confirmButton.onClick.AddListener (() => {
			AudioManager.PlaySound ("Click");

			AnalyticsManager.SendQuitLevelEvent(levelName, player.transform.position, CurrentLevel.GetLivesLost(), 
				(int) CurrentLevel.GetLengthOfTimeInLevel());

			QuitGame();
		});
		dialogBox.returnButton.onClick.AddListener (() => {
			dialogBoxBackgroundFader.gameObject.GetComponent<GAui> ().MoveOut ();
			dialogBox.gameObject.GetComponent<GAui> ().MoveOut ();
			AudioManager.PlaySound ("InfoPanelSlideIn", 0.95f);
			Pause();
		});
		dialogBox.gameObject.SetActive (true);
		dialogBoxBackgroundFader.gameObject.SetActive (true);
		dialogBoxBackgroundFader.gameObject.GetComponent<GAui> ().MoveIn ();
		dialogBox.gameObject.GetComponent<GAui> ().MoveIn ();
		AudioManager.PlaySound ("InfoPanelSlideIn");
	}

	public void SkipLevelButtonPressed() {
		//this needs to be localized. While it is not, the skip level button only pops up a confirmation dialog when English is selected
		if (Application.systemLanguage == SystemLanguage.English) {
			SkipLevelInEnglish ();
		} else {
			SkipLevel ();
		}
	}

	void SkipLevelInEnglish() {
		dialogBox.dialogText.text = ("Do you want to watch a video to skip to the next level?\n" +
			"You will not be able to skip another level for 24 hours");
		dialogBox.confirmButton.onClick.RemoveAllListeners ();
		dialogBox.returnButton.onClick.RemoveAllListeners ();
		dialogBox.confirmButton.onClick.AddListener (() => {
			dialogBoxBackgroundFader.GetComponent<GAui> ().MoveOut();
			dialogBox.gameObject.GetComponent<GAui> ().MoveOut ();
			SkipLevel();
		});
		dialogBox.returnButton.onClick.AddListener (() => {
			dialogBoxBackgroundFader.GetComponent<GAui> ().MoveOut();
			dialogBox.gameObject.GetComponent<GAui> ().MoveOut ();
			AudioManager.PlaySound ("InfoPanelSlideIn", 0.95f);
			Pause();
		});
		dialogBox.gameObject.SetActive (true);
		dialogBoxBackgroundFader.gameObject.SetActive (true);
		dialogBoxBackgroundFader.GetComponent<GAui> ().MoveIn();
		dialogBox.gameObject.GetComponent<GAui> ().MoveIn ();
		AudioManager.PlaySound ("InfoPanelSlideIn");
	}

	void SkipLevel() {
		AudioManager.PlaySound ("Click");
		AnalyticsManager.SendAdWatchEvent (levelName, "SkipLevelAd", CurrentLevel.GetLivesLost (), (int)CurrentLevel.GetLengthOfTimeInLevel ());
		var options = new ShowOptions { resultCallback = HandleSkipLevel };
		Advertisement.Show ("rewardedVideo", options);
	}

	public void StartShakingScreen() {
		Camera.main.gameObject.GetComponent<CameraShaker> ().StartShaking();
	}

	public void StopShakingScreen() {
		Camera.main.gameObject.GetComponent<CameraShaker> ().StopShaking();
	}

	public void ShakeForDuration(float shakeLength, float shakeMagnitude) {
		CameraShaker shaker = Camera.main.gameObject.GetComponent<CameraShaker> ();
		shaker.shakeMagnitude = shakeMagnitude;
		shaker.StartShaking ();
		StartCoroutine (StopShakingAfterTime (shakeLength));
	}

	public void ShakeForDuration(float shakeLength) {
		Camera.main.gameObject.GetComponent<CameraShaker> ().StartShaking();
		StartCoroutine (StopShakingAfterTime (shakeLength));
	}

	IEnumerator StopShakingAfterTime(float stopAfterTime) {
		yield return new WaitForSeconds(stopAfterTime);
		Camera.main.gameObject.GetComponent<CameraShaker> ().StopShaking();
	}

	public PlayerController GetPlayer() {
		return player;
	}
		
	public void LevelCompleteWatchAdButtonPress() {
		AnalyticsManager.SendAdWatchEvent (levelName, "EndOfLevelRewardAd", CurrentLevel.GetLivesLost(), (int) CurrentLevel.GetLengthOfTimeInLevel());
		levelCompletePanel.PlayAdForReward ();
	}

	void SkipToNextLevel() {
		string nextLevelName = GameObject.Find("LevelCompleteTrigger").GetComponent<LevelCompleteTrigger>().nextLevelName;
		this.nextLevel = nextLevelName;
		GameStats stats = GameStats.GetInstance ();

		LevelStats nextLevelStats = stats.GetLevelStats (nextLevelName);

		//we don't have the next level saved, so we just create placeholder info for it
		if (nextLevelStats == null) {
			nextLevelStats = new LevelStats ();
			nextLevelStats.levelName = nextLevelName;
			nextLevelStats.locked = false;
			nextLevelStats.maxNumberOfCoinsCollected = -1;
			nextLevelStats.minNumberOfLivesLost = -1;
			nextLevelStats.bestTimeToCompleteLevel = -1f;
		} else {
			//if we have some data for the next level, we just make sure that the next level is unlocked
			nextLevelStats.locked = false;
		}

		//save the last level skip as the current time
		String dateTimeFormat = "yyyy-MM-dd HH:mm:ss";
		string currentTime = DateTime.Now.ToString(dateTimeFormat);
		Settings.lastSkipLevelTime = currentTime;	//this is written to file by stats.GetGameData()

		stats.AddStatsForLevel(nextLevelStats);
		GameDataPersistor.Save(stats.GetGameData());
		LastCheckpoint.Reset ();
		NextLevel ();
	}

	public void ShowWhiteFlash() {
		whiteFlashPanel.StartFadingIn ();
	}

	private void HandleSkipLevel(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			SkipToNextLevel ();
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}
}
