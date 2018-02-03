using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour {

	private static Dictionary<string, object> metricsDictionary = new Dictionary <string, object>();

	public static void SendLevelStartEvent(string levelName) {
		metricsDictionary.Clear ();
		metricsDictionary.Add ("levelName", levelName);
	
		postEvent("levelStart");
	}

	public static void SendLevelCompleteEvent(string levelName, int coins, int lives, int time) {
		metricsDictionary.Clear ();
		metricsDictionary.Add ("levelName", levelName);
		metricsDictionary.Add ("coins", coins);
		metricsDictionary.Add ("lives", lives);
		metricsDictionary.Add ("time", time);
		postEvent("levelComplete");
	}

	public static void SendDeathEvent(string levelName, Vector2 position, string objectCausingDeath) {
		metricsDictionary.Clear ();
		metricsDictionary.Add ("levelName", levelName);
		metricsDictionary.Add ("posX", position.x);
		metricsDictionary.Add ("posY", position.y);
		metricsDictionary.Add ("objectCausingDeath", objectCausingDeath);
		postEvent("playerDeath");
	}

	public static void SendInGameStorePurchase(string levelName, string itemType, Vector2 playerPosition) {
		metricsDictionary.Clear ();
		metricsDictionary.Add ("levelName", levelName);
		metricsDictionary.Add ("itemType", itemType);
		metricsDictionary.Add ("playerPosX", playerPosition.x);
		metricsDictionary.Add ("playerPosY", playerPosition.y);
		postEvent("inGameStorePurchase");
	}

	public static void SendPlayerCustomisationPurchasedEvent(string itemType, int itemCost, int coinCount) {
		metricsDictionary.Clear ();
		metricsDictionary.Add ("itemType", itemType);
		metricsDictionary.Add ("itemCost", itemCost);
		metricsDictionary.Add ("remainingCoins", coinCount);
		postEvent("playerCustomiseStorePurchase");
	}

	public static void SendAdWatchEvent (string levelName, string adType, int livesLost, int timeInLevel) {
		metricsDictionary.Clear ();
		metricsDictionary.Add ("levelName", levelName);
		metricsDictionary.Add ("adType", adType);
		metricsDictionary.Add ("livesLost", livesLost);
		metricsDictionary.Add ("timeInLevel", timeInLevel);
		postEvent("adWatched");
	}

	public static void SendQuitLevelEvent(string levelName, Vector2 playerPosition, int livesLost, int timeInLevel) {
		metricsDictionary.Clear ();
		metricsDictionary.Add ("levelName", levelName);
		metricsDictionary.Add ("playerPosX", playerPosition.x);
		metricsDictionary.Add ("playerPosY", playerPosition.y);
		metricsDictionary.Add ("lives", livesLost);
		metricsDictionary.Add ("time", timeInLevel);
		postEvent("quitLevel");

	}

	public static void SendLikeGameEvent(bool likeGame) {
		metricsDictionary.Clear ();
		metricsDictionary.Add ("result", likeGame);
		postEvent("likeGame");
	}

	public static void SendRateGameEvent(bool rateGame) {
		metricsDictionary.Clear ();
		metricsDictionary.Add ("result", rateGame);
		postEvent("rateGame");
	}

	public static void postEvent(string eventName) {
		#if !UNITY_EDITOR	
			Analytics.CustomEvent (eventName, metricsDictionary);
		#endif
	}
}
