using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class StorePanel : MonoBehaviour {

	public HudListener mainHud;
	public Text coinCountText;
	public Text snowflakeCostText;
	public Text capeCostText;
	public Text magnetCostText;

	GameStats stats;
	TextFieldNumberAnimator textFieldAnimator;
	static float numberOfItemsPurchased = 0;	//this is used purely for the achievement system

	float lastSoundPlay = 0.0f;

	public void BackButtonPressed() {
		AudioManager.PlaySound ("Click", 0.9f);
		Component[] storeItems = transform.GetComponentsInChildren(typeof(GAui), true);

		foreach(GAui button in storeItems) {
			button.MoveOut (GSui.eGUIMove.SelfAndChildren);
		}

		StartCoroutine (DelayAndGoBackToPauseScreen (0.4f));
	}

	IEnumerator DelayAndGoBackToPauseScreen(float timeToDelay) {
		float start = Time.realtimeSinceStartup;

		while (Time.realtimeSinceStartup < start + timeToDelay) {
			yield return null;
		}
		Component[] pauseItems = mainHud.pauseMenu.gameObject.transform.GetComponentsInChildren(typeof(GAui), true);
		foreach(GAui button in pauseItems) {
			button.MoveIn (GSui.eGUIMove.SelfAndChildren);
		}
		mainHud.Pause ();	//return back to pause menu
	}

	public void InitStorePanel() {
		stats = GameStats.GetInstance ();
		snowflakeCostText.text = "" + StoreInventory.GetItemFromInventory (Strings.SNOWFLAKE).cost;
		capeCostText.text = "" + StoreInventory.GetItemFromInventory (Strings.CAPE).cost;
		magnetCostText.text = "" + StoreInventory.GetItemFromInventory (Strings.MAGNET).cost;

		textFieldAnimator = coinCountText.GetComponent<TextFieldNumberAnimator> ();
		textFieldAnimator.initialNumber = GameStats.GetInstance ().totalNumberOfCoins;
		textFieldAnimator.currentNumber = GameStats.GetInstance ().totalNumberOfCoins;
		textFieldAnimator.desiredNumber = GameStats.GetInstance ().totalNumberOfCoins;
		textFieldAnimator.SetNumber (GameStats.GetInstance ().totalNumberOfCoins);

		textFieldAnimator.valueDecrementedListeners = PlayCoinSound;
		textFieldAnimator.valueIncrementedListeners = PlayCoinSound;
	}

	void PlayCoinSound(float value) {
		if ((Time.unscaledTime - lastSoundPlay) > 0.05f) {
			AudioManager.PlaySound ("coin-new");
			lastSoundPlay = Time.unscaledTime;
		}
	}

	public void SnowflakeButtonPressed() {
		StoreItem snowflake = StoreInventory.GetItemFromInventory (Strings.SNOWFLAKE);
		if (stats.totalNumberOfCoins >= snowflake.cost) {

			if (mainHud.GetPlayer ().GetTemperature() < 20f) {
				mainHud.infoPanel.SetText (Strings.UI_YOU_DONT_NEED_TO_COOL_DOWN_RIGHT_NOW);
				mainHud.infoPanel.gameObject.SetActive (true);
				return;
			}

			mainHud.GetPlayer ().ResetTemperature ();
			stats.totalNumberOfCoins -= snowflake.cost;
			textFieldAnimator.AddToNumber (-snowflake.cost);
			stats.SaveToDisk ();
			AudioManager.PlaySound ("snowflake-new");
			AnalyticsManager.SendInGameStorePurchase (mainHud.levelName, Strings.SNOWFLAKE, mainHud.player.transform.position);
			numberOfItemsPurchased++;

			if (numberOfItemsPurchased == 3) {
				#if UNITY_IOS
				SocialServiceManager.GetInstance ().UnlockAchievement ("shopaholic");
				#endif
				#if UNITY_ANDROID
				SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_shopaholic);
				#endif
			}
		} else {
			mainHud.infoPanel.SetText (Strings.UI_INSUFFICIENT_FUNDS);
			mainHud.infoPanel.gameObject.SetActive (true);
		}
	}

	public void CapeButtonPressed() {
		StoreItem cape = StoreInventory.GetItemFromInventory (Strings.CAPE);
		if (stats.totalNumberOfCoins >= cape.cost) {
			if (mainHud.GetPlayer ().glidingEnabled) {
				mainHud.infoPanel.SetText (Strings.UI_YOU_ALREADY_PURCHASED_THIS_ITEM);
				mainHud.infoPanel.gameObject.SetActive (true);
				return;
			}

			mainHud.GetPlayer ().glidingEnabled = true;
			mainHud.GetPlayer ().cape.gameObject.SetActive (true);
			stats.totalNumberOfCoins -= cape.cost;
			textFieldAnimator.AddToNumber (-cape.cost);
			stats.SaveToDisk ();
			AudioManager.PlaySound ("cape-new");
			AnalyticsManager.SendInGameStorePurchase (mainHud.levelName, Strings.CAPE, mainHud.player.transform.position);
			numberOfItemsPurchased++;

			if (numberOfItemsPurchased == 3) {
				#if UNITY_IOS
				SocialServiceManager.GetInstance ().UnlockAchievement ("shopaholic");
				#endif
				#if UNITY_ANDROID
				SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_shopaholic);
				#endif
			}
		} else {
			mainHud.infoPanel.SetText (Strings.UI_INSUFFICIENT_FUNDS);
			mainHud.infoPanel.gameObject.SetActive (true);
		}
	}

	public void MagnetButtonPressed() {
		StoreItem magnet = StoreInventory.GetItemFromInventory (Strings.MAGNET);

		if (stats.totalNumberOfCoins >= magnet.cost) {
			if (mainHud.GetPlayer ().IsMagnetEnabled ()) {
				mainHud.infoPanel.SetText (Strings.UI_YOU_ALREADY_PURCHASED_THIS_ITEM);
				mainHud.infoPanel.gameObject.SetActive (true);
				return;
			}

			mainHud.GetPlayer ().EnableMagnet(true);
			stats.totalNumberOfCoins -= magnet.cost;
			textFieldAnimator.AddToNumber (-magnet.cost);
			stats.SaveToDisk ();
			AudioManager.PlaySound ("magnet-new");
			AnalyticsManager.SendInGameStorePurchase (mainHud.levelName, Strings.MAGNET, mainHud.player.transform.position);
			numberOfItemsPurchased++;

			if (numberOfItemsPurchased == 3) {
				#if UNITY_IOS
				SocialServiceManager.GetInstance ().UnlockAchievement ("shopaholic");
				#endif
				#if UNITY_ANDROID
				SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_shopaholic);
				#endif
			}
		} else {
			mainHud.infoPanel.SetText (Strings.UI_INSUFFICIENT_FUNDS);
			mainHud.infoPanel.gameObject.SetActive (true);
		}
	}

	public void WatchVideoButtonPressed() {
		if (Advertisement.IsReady ("rewardedVideo")) {
			AnalyticsManager.SendAdWatchEvent (mainHud.levelName, "InGameStoreAd", CurrentLevel.GetLivesLost (), (int)CurrentLevel.GetLengthOfTimeInLevel ());
			var options = new ShowOptions { resultCallback = GetCoins };
			Advertisement.Show ("rewardedVideo", options);
		} else {
			mainHud.ShowErrorPanel ();
		}
	}		

	private void GetCoins(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			stats.totalNumberOfCoins += 300;
			stats.SaveToDisk ();
			textFieldAnimator.AddToNumber (300);

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
