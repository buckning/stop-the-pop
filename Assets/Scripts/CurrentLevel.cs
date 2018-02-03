using UnityEngine;
using System.Collections;

public class CurrentLevel : MonoBehaviour {

	public enum LevelDifficulty { EASY, NORMAL };

	static int numberOfLivesLost = 0;
	static int numberOfCoins = 0;
	static float lengthOfTimeOnLevel = 0.0f;

	static int livesLostSinceLastAd = 0;

	static LevelDifficulty levelDifficulty = LevelDifficulty.NORMAL;

	public static void Reset() {
		numberOfCoins = 0;
		numberOfLivesLost = 0;
		lengthOfTimeOnLevel = 0.0f;
		livesLostSinceLastAd = 0;
	}

	public static void ResetAll() {
		Reset ();
		levelDifficulty = LevelDifficulty.NORMAL;
	}

	public static LevelDifficulty GetLevelDifficulty() {
		return levelDifficulty;
	}

	public static void SetLevelDifficulty(LevelDifficulty difficulty) {
		levelDifficulty = difficulty;
	}

	public static void AddCoins(int coins) {
		numberOfCoins += coins;
	}

	public static void AddLivesLost(int livesLost) {
		numberOfLivesLost += livesLost;
		livesLostSinceLastAd += livesLost;
	}

	public static void ResetLivesLostSinceLastAd() {
		livesLostSinceLastAd = 0;
	}

	public static void AddLengthOfTimeInLevel(float time) {
		lengthOfTimeOnLevel += time;
	}

	public static int GetNumberOfCoins() {
		return numberOfCoins;
	}

	public static int GetLivesLost() {
		return numberOfLivesLost;
	}

	public static float GetLengthOfTimeInLevel() {
		return lengthOfTimeOnLevel;
	}

	public static int GetNumberOfLivesLostSinceLastAd() {
		return livesLostSinceLastAd;
	}
}
