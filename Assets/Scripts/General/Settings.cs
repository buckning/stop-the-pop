using UnityEngine;
using System.Collections;

/***
 * Class containing global settings of the game
 */
public class Settings : MonoBehaviour {
	public static bool musicEnabled = true;
	public static bool sfxEnabled = true;
	public static bool touchInputEnabled = false;
	public static int rateUsDialogSnoozeTime = 1;
	public static string lastSkipLevelTime = "2000-01-01 00:00:00";
	public static string lastRateUsDialogShowTime = "2000-01-01 00:00:00";
	public static string lastRewardGivenTime = "2000-01-01 00:00:00";
	public static string launchTimeOfGame = null;
	public static int numberOfLevelsCompletedThisSession = 0;

	public static string selectedHat = null;
	public static string selectedGlasses = null;
	public static string selectedFacialHair = null;
	public static string selectedShoes = null;
}
