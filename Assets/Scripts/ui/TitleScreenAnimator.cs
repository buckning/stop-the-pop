using UnityEngine;
using System.Collections;

public class TitleScreenAnimator : MonoBehaviour {
	public GAui playButton;
	public GAui informationButton;
	public GAui facebookButton;
	public GAui twitterButton;
	public GAui rateUsButton;
	public GAui leaderboardButton;
	public GAui playerCustomiseButton;

	public GAui backgroundFader;

	public GameObject levelSelectScreen;

	public GAui informationPanel;

	public GAui musicButton;
	public GAui sfxButton;

	public GAui quitDialog;

	public GAui errorPanel;

	public GAui logo;

	public TitleScreenListener titleScreen;

	void Start () {
		titleScreen.titleScreenActiveListeners += TitleScreenActive;
		titleScreen.titleScreenStartButtonPressedListeners += StartButtonPressed;
		titleScreen.titleScreenLevelSelectBackButtonPressedListeners += LevelSelectBackButtonPressed;
		titleScreen.titleScreenInformationButtonPressedListeners += InformationButtonPressed;
		titleScreen.titleScreenInformationBackButtonPressedListeners += InformationBackButtonPressed;

		titleScreen.titleScreenQuitDialogEnabledListeners += QuitButtonPressed;
		titleScreen.titleScreenQuitDialogCancelListeners += QuitCancelButtonPressed;

		titleScreen.titleScreenPlayerCustomisationButtonPressedListeners += ZoomOutTitleScreen;

		titleScreen.titleScreenShowErrorPanelListeners += ShowErrorPanel;
		titleScreen.titleScreenHideErrorPanelListeners += HideErrorPanel;

		facebookButton.gameObject.SetActive (true);
		twitterButton.gameObject.SetActive (true);
		rateUsButton.gameObject.SetActive (true);
		leaderboardButton.gameObject.SetActive (true);
		musicButton.gameObject.SetActive (true);
		sfxButton.gameObject.SetActive (true);
		logo.gameObject.SetActive (true);
		ZoomInTitleScreen ();
	}

	void ShowErrorPanel() {
		errorPanel.gameObject.SetActive (true);
		errorPanel.MoveIn (GSui.eGUIMove.SelfAndChildren);
	}

	void HideErrorPanel() {
		errorPanel.MoveOut (GSui.eGUIMove.SelfAndChildren);
	}

	void QuitButtonPressed() {
		ZoomOutTitleScreen ();
		backgroundFader.gameObject.SetActive (true);
		backgroundFader.MoveIn (GSui.eGUIMove.SelfAndChildren);
		quitDialog.MoveIn(GSui.eGUIMove.SelfAndChildren);
	}

	void QuitCancelButtonPressed() {
		ZoomInTitleScreen ();
		backgroundFader.MoveOut (GSui.eGUIMove.SelfAndChildren);
		quitDialog.MoveOut(GSui.eGUIMove.SelfAndChildren);
	}
		
	void StartButtonPressed() {
		ZoomOutTitleScreen ();

		levelSelectScreen.gameObject.SetActive (true);

		Component[] levelButtons = levelSelectScreen.gameObject.transform.GetComponentsInChildren(typeof(GAui), true);

		foreach(GAui button in levelButtons) {
			button.gameObject.SetActive (true);
			button.MoveIn (GSui.eGUIMove.SelfAndChildren);
		}
	}

	void InformationButtonPressed() {
		ZoomOutTitleScreen ();
		backgroundFader.gameObject.SetActive (true);
		backgroundFader.MoveIn (GSui.eGUIMove.SelfAndChildren);
		informationPanel.MoveIn(GSui.eGUIMove.SelfAndChildren);
	}

	void InformationBackButtonPressed() {
		ZoomInTitleScreen ();
		backgroundFader.MoveOut (GSui.eGUIMove.SelfAndChildren);
		informationPanel.MoveOut(GSui.eGUIMove.SelfAndChildren);
	}
	
	void TitleScreenActive() {
		ZoomInTitleScreen ();
	}

	void LevelSelectBackButtonPressed() {
		Component[] levelButtons = levelSelectScreen.gameObject.transform.GetComponentsInChildren(typeof(GAui), true);

		foreach(GAui button in levelButtons) {
			button.MoveOut (GSui.eGUIMove.SelfAndChildren);
		}
		ZoomInTitleScreen ();
	}

	void ZoomOutTitleScreen() {
		playButton.MoveOut(GSui.eGUIMove.SelfAndChildren);
		informationButton.MoveOut(GSui.eGUIMove.SelfAndChildren);
		facebookButton.MoveOut(GSui.eGUIMove.SelfAndChildren);
		twitterButton.MoveOut (GSui.eGUIMove.SelfAndChildren);
		rateUsButton.MoveOut (GSui.eGUIMove.SelfAndChildren);
		leaderboardButton.MoveOut (GSui.eGUIMove.SelfAndChildren);
		playerCustomiseButton.MoveOut(GSui.eGUIMove.SelfAndChildren);
		musicButton.MoveOut (GSui.eGUIMove.SelfAndChildren);
		sfxButton.MoveOut (GSui.eGUIMove.SelfAndChildren);
		logo.MoveOut (GSui.eGUIMove.SelfAndChildren);
	}

	void ZoomInTitleScreen() {
		playButton.MoveIn(GSui.eGUIMove.SelfAndChildren);
		informationButton.MoveIn(GSui.eGUIMove.SelfAndChildren);
		facebookButton.MoveIn(GSui.eGUIMove.SelfAndChildren);
		twitterButton.MoveIn (GSui.eGUIMove.SelfAndChildren);
		rateUsButton.MoveIn(GSui.eGUIMove.SelfAndChildren);
		leaderboardButton.MoveIn(GSui.eGUIMove.SelfAndChildren);
		playerCustomiseButton.MoveIn(GSui.eGUIMove.SelfAndChildren);
		musicButton.MoveIn (GSui.eGUIMove.SelfAndChildren);
		sfxButton.MoveIn (GSui.eGUIMove.SelfAndChildren);
		logo.MoveIn (GSui.eGUIMove.SelfAndChildren);
	}
}
