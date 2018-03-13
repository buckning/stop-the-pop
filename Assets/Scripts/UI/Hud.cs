using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour {

	public CoinCountPanel coinCountPanel;
	public ThermometerBehaviour thermometer;
	public GameObject playerControlPanel;
	public ScreenFader damageIndicator;
	public ScreenFader screenFader;
	public ScreenFader flashPanel;
	public Button pauseButton;
	public Button retryButton;

	public delegate void NotifyEvent ();
	public event NotifyEvent pauseButtonPressedListeners;
	public event NotifyEvent retryButtonPressedListeners;

	void Start () {
		playerControlPanel.SetActive (IsMobile ());
		retryButton.gameObject.SetActive (false);
		screenFader.fadeInCompleteListeners += FadeInCompleted;

		SetUpRetryButtonListener ();
		SetUpPauseButtonListener ();
	}

	private void SetUpRetryButtonListener() {
		retryButton.onClick.AddListener (() => {
			if(retryButtonPressedListeners != null) {
				retryButtonPressedListeners();
			}
		});
	}

	private void SetUpPauseButtonListener() {
		pauseButton.onClick.AddListener (() => {
			if(pauseButtonPressedListeners != null) {
				pauseButtonPressedListeners();
			}
		});
	}

	public void UpdateTemperature(int temperature) {
		thermometer.currentTemperature = temperature / 100.0f;
	}

	public void UpdateCoinCount(int coinCount) {
		coinCountPanel.SetCoinCount (coinCount);
	}

	public void TriggerDamageIndicator() {
		damageIndicator.StartFadingIn ();
	}

	public void TriggerFlash() {
		flashPanel.StartFadingIn ();
	}

	public void FadeOut() {
		coinCountPanel.gameObject.SetActive (false);
		pauseButton.gameObject.SetActive (false);
		playerControlPanel.SetActive (false);
		retryButton.gameObject.SetActive (false);
		thermometer.gameObject.SetActive (false);
		screenFader.StartFadingOut ();
	}

	public void FadeIn() {
		thermometer.gameObject.SetActive (false);
		retryButton.gameObject.SetActive (false);
		pauseButton.gameObject.SetActive (false);
		playerControlPanel.SetActive (false);
		screenFader.StartFadingIn ();
	}

	private void FadeInCompleted() {
		thermometer.gameObject.SetActive (true);
		pauseButton.gameObject.SetActive(true);
		retryButton.gameObject.SetActive (true);
		playerControlPanel.SetActive (IsMobile ());
		coinCountPanel.gameObject.SetActive (true);
	}

	private bool IsMobile() {
		if (Application.platform == RuntimePlatform.Android || 
			Application.platform == RuntimePlatform.IPhonePlayer) {
			return true;
		}
		return false;
	}
}
