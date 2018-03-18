using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour {
	public Hud hud;
	public StorePanel storePanel;
	public PausePanel pausePanel;
	public LevelCompletePanel levelCompletePanel;
	public Image backgroundFader;
	public Image loadingPanel;
	public InformationPanel infoPanel;
	public DialogBox dialogBox;

	public delegate void NotifyEvent ();
	public event NotifyEvent skipLevelButtonPressedListeners;
	public event NotifyEvent restartLevelButtonPressedListeners;
	public event NotifyEvent quitGameButtonPressedListeners;

	void Start() {
		hud.pauseButtonPressedListeners += Pause;
		hud.retryButtonPressedListeners += Retry;
		pausePanel.continueButtonListeners += Resume;
		pausePanel.storeButtonListeners += OpenStore;
		pausePanel.restartButtonListeners += Restart;
		pausePanel.skipLevelButtonListeners += SkipLevel;
		pausePanel.mainMenuButtonListeners += QuitGame;
		storePanel.backButtonPressedListeners += Pause;
	}

	public void ShowRetryButton() {
		hud.retryButton.gameObject.SetActive (true);
	}

	public void QuitGame() {
		SetUpDialogBox (Strings.UI_ARE_YOU_SURE_YOU_WANT_TO_EXIT_TO_THE_MAIN_MENU, 
			quitGameButtonPressedListeners);
	}

	public void Restart() {
		SetUpDialogBox (Strings.UI_ARE_YOU_SURE_YOU_WANT_TO_RESTART, 
			restartLevelButtonPressedListeners);
	}

	public void SetTemperature(int temperature) {
		hud.UpdateTemperature (temperature);
	}

	void SkipLevel() {
		SetUpDialogBox ("Do you want to watch a video to skip to the next level?\n" +
		"You will not be able to skip another level for 24 hours", 
			skipLevelButtonPressedListeners);
	}

	private void SetUpDialogBox(string dialogText, NotifyEvent notifyEvent) {
		pausePanel.gameObject.SetActive (false);
		dialogBox.dialogText.text = dialogText;
		dialogBox.confirmButton.onClick.RemoveAllListeners ();
		dialogBox.returnButton.onClick.RemoveAllListeners ();
		dialogBox.confirmButton.onClick.AddListener (() => {
			if(notifyEvent != null) {
				notifyEvent();
			}
		});
		dialogBox.returnButton.onClick.AddListener (() => {
			Pause();
		});
		dialogBox.gameObject.SetActive (true);
	}

	public void TriggerLevelComplete() {
		levelCompletePanel.gameObject.SetActive (true);
	}

	public void Pause() {
		pausePanel.gameObject.SetActive (true);
		storePanel.gameObject.SetActive (false);
		dialogBox.gameObject.SetActive (false);
		backgroundFader.gameObject.SetActive (true);
		hud.gameObject.SetActive (false);
	}

	public void Resume() {
		pausePanel.gameObject.SetActive (false);
		hud.gameObject.SetActive (true);
		backgroundFader.gameObject.SetActive (false);
	}

	public void OpenStore() {
		pausePanel.gameObject.SetActive (false);
		storePanel.gameObject.SetActive (true);
	}

	public void Retry() {
		hud.FadeOut ();
	}

	public void LevelComplete(int numberOfCoins, int timeTaken, int numberOfLivesLost) {
		levelCompletePanel.gameObject.SetActive (true);
		pausePanel.gameObject.SetActive (false);
		storePanel.gameObject.SetActive (false);
		dialogBox.gameObject.SetActive (false);
		backgroundFader.gameObject.SetActive (false);
		hud.gameObject.SetActive (false);
		levelCompletePanel.ShowLevelCompleteScreen (numberOfCoins, timeTaken, numberOfLivesLost);
	}

	public void ShowLoadingPanel() {
		pausePanel.gameObject.SetActive (false);
		storePanel.gameObject.SetActive (false);
		dialogBox.gameObject.SetActive (false);
		backgroundFader.gameObject.SetActive (false);
		hud.gameObject.SetActive (false);
		levelCompletePanel.gameObject.SetActive (false);
		loadingPanel.gameObject.SetActive (true);
	}
}
