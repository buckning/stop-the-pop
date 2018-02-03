using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * The Checkpoint system in this game is very simple. When a level starts, 
 * it checks if LastCheckpoint.lastCheckpoint == Application.loadedLevelName
 * if it is, the players position is updated to the position of the last checkpoint.
 * player.setPosition(LastCheckpoint.lastCheckpoint);
 * 
 * The value of loadedLevelName is then cached in LastCheckpoint.checkpointName.
 * 
 * If the value of checkpointName is not equal to Application.loadedLevelName,
 * a checkpoint is not loaded and the default position of the player is 
 * 
 * Levels are restarted by using Application.loadLevel(currentLevelName).
 * This resets all objects in the scene to the default location. 
 * As it stands, only the player position gets updated. This could be expanded so
 * all objects in the game get stored here and get dynamically instantiated during load time.
 * This does not seem very necessary at this point in the game design.
 */
public class LastCheckpoint : MonoBehaviour {

	public static string checkpointName = "";

	public static Vector2 lastCheckpoint = new Vector2(0.0f,0.0f);

	static List<int> collectedCoins = new List<int> ();

	public static void AddCollectedCoin(int coinId) {
		if (!collectedCoins.Contains (coinId)) {
			collectedCoins.Add (coinId);
		}
	}

	public static List<int> GetCollectedCoins() {
		return collectedCoins;
	}

	public static void Reset() {
		collectedCoins.Clear ();
		checkpointName = "";
		lastCheckpoint = Vector2.zero;
	}
}
