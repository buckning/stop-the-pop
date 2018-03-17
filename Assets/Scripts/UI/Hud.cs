using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour {

	public CoinCountPanel coinCountPanel;
	public ThermometerBehaviour thermometer;
	public PlayerControlPanel playerControlPanel;
	public ScreenFader damageIndicator;
	public ScreenFader screenFader;
	public ScreenFader flashPanel;
	public Button pauseButton;
	public Button retryButton;

	public delegate void NotifyEvent ();
	public event NotifyEvent pauseButtonPressedListeners;
	public event NotifyEvent retryButtonPressedListeners;
	public event NotifyEvent fadeOutCompletedListeners;

	void Start () {
		playerControlPanel.gameObject.SetActive (IsMobile ());
		retryButton.gameObject.SetActive (false);
		screenFader.fadeInCompleteListeners += FadeInCompleted;
		screenFader.fadeOutCompleteListeners += FadeOutCompleted;

		SetUpRetryButtonListener ();
		SetUpPauseButtonListener ();
	}

	private void FadeOutCompleted() {
		if (fadeOutCompletedListeners != null) {
			fadeOutCompletedListeners ();
		}
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
		playerControlPanel.gameObject.SetActive (false);
		retryButton.gameObject.SetActive (false);
		thermometer.gameObject.SetActive (false);
		screenFader.StartFadingOut ();
	}

	public void FadeIn() {
		thermometer.gameObject.SetActive (false);
		retryButton.gameObject.SetActive (false);
		pauseButton.gameObject.SetActive (false);
		playerControlPanel.gameObject.SetActive (false);
		screenFader.StartFadingIn ();
	}

	private void FadeInCompleted() {
		thermometer.gameObject.SetActive (true);
		pauseButton.gameObject.SetActive(true);
		retryButton.gameObject.SetActive (false);
		playerControlPanel.gameObject.SetActive (IsMobile ());
		coinCountPanel.gameObject.SetActive (true);
	}

	private bool IsMobile() {
		return Application.platform == RuntimePlatform.Android
			|| Application.platform == RuntimePlatform.IPhonePlayer;
	}

	public void ShakeScreen() {
		Camera.main.gameObject.GetComponent<CameraShaker> ().StartShaking();
		StartCoroutine (StopShakingAfterTime (0.2f));
	}

	IEnumerator StopShakingAfterTime(float stopAfterTime) {
		yield return new WaitForSeconds(stopAfterTime);
		Camera.main.gameObject.GetComponent<CameraShaker> ().StopShaking();
	}
}
