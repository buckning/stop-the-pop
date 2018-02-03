using UnityEngine;
using System.Collections;

public class Strings : MonoBehaviour {
	public const string PLAYER = "Player";
	public const string POPCORN = "Popcorn";
	public const string COLLECTOR = "Collector";
	public const string MOVING_PLATFORM = "MovingPlatform";
	public const string DESTROY_ALL_OBJECTS = "destroyAll";
	public const string BREAKABLE = "Breakable";
	public const string TITLE_SCREEN = "TitleScreen";
	public const string TERRAIN = "Terrain";
	public const string BISCUIT_PLATFORM = "BiscuitPlatform";


	//Tags that are used in the save file
	public const string TOTAL_COINS_COLLECTED_TAG = "TotalCoinsCollected: ";
	public const string NUMBER_OF_CAPES_TAG = "NumberOfCapes: ";
	public const string LOCKED_TAG = "Locked: ";
	public const string BEST_TIME_TAG = "BestTime: ";
	public const string LIVES_LOST_TAG = "LivesLost: ";
	public const string COINS_COLLECTED_TAG = "CoinsCollected: ";
	public const string LEVEL_NAME_TAG = "LevelName: ";
	public const string LEVEL_START_TAG = "---LevelStart---";
	public const string LEVEL_END_TAG = "";

	public const string PLAYER_NAME_TAG = "PlayerName: ";
	public const string PLAYER_LOCKED_TAG = "PlayerLocked: ";
	public const string PLAYER_START_TAG = "---PlayerStart---";
	public const string PLAYER_END_TAG = "---PlayerEnd---";
	public const string PLAYER_CUSTOMISATION_START_TAG = "---PlayerCustomisationStart---";
	public const string PLAYER_CUSTOMISATION_END_TAG = "---PlayerCustomisationEnd---";
	public const string PLAYER_CUSTOMISATION_NAME_TAG = "PlayerCustomisationName: ";
	public const string PLAYER_CUSTOMISATION_LOCKED_TAG = "PlayerCustomisationLocked: ";
	public const string PLAYER_CUSTOMISATION_TYPE_TAG = "PlayerCustomisationType: "; 

	public const string COLLECTABLES_START_TAG = "---CollectablesStart---";
	public const string COLLECTABLES_END_TAG = "---CollectablesEnd---";

	public const string SETTINGS_START_TAG = "---SettingsStart---";
	public const string SETTINGS_SFX_TAG = "SfxEnabled: ";
	public const string SETTINGS_MUSIC_TAG = "MusicEnabled: ";
	public const string SETTINGS_LAST_RATE_US_DIALOG_SNOOZE_TIME = "rateUsDialogSnoozeTime: ";
	public const string SETTINGS_LAST_RATE_US_DIALOG_SHOW_TIME = "lastRateUsDialogShowTime: ";
	public const string SETTINGS_LAST_REWARD_GIVEN_TIME = "lastRewardGiveTime: ";
	public const string SETTINGS_LAST_LEVEL_SKIP_TIME = "LastLevelSkipTime: ";
	public const string SETTINGS_SELECTED_HAT = "selectedHat: ";
	public const string SETTINGS_SELECTED_GLASSES = "selectedGlasses: ";
	public const string SETTINGS_SELECTED_FACIAL_HAIR = "selectedFacialHair: ";
	public const string SETTINGS_SELECTED_SHOES = "selectedShoes: ";
	public const string SETTINGS_END_TAG = "---SettingsEnd---";

	public const string SNOWFLAKE = "Snowflake";
	public const string CAPE = "Cape";
	public const string MAGNET = "Magnet";
	public const string SHIELD = "Shield";

	public const string LEPRECHAUN_HAT = "LeprechaunHat";
	public const string SANTA_HAT = "SantaHat";
	public const string VIKING_HAT = "VikingHat";
	public const string ELF_HAT = "ElfHat";
	public const string PIRATE_HAT = "PirateHat";
	public const string CLOWN_HAIR = "ClownHair";

	public const string STAR_GLASSES = "StarGlasses";
	public const string AVIATOR_GLASSES = "AviatorGlasses";

	public const string LEPRECHAUN_BEARD = "LeprechaunBeard";
	public const string SANTA_BEARD = "SantaBeard";
	public const string PIRATE_BEARD = "PirateBeard";
	public const string VIKING_BEARD = "VikingBeard";
	public const string CLOWN_NOSE = "ClownNose";

	public const string LEPRECHAUN_SHOE = "leprechaunShoe";
	public const string SANTA_SHOE = "santaShoe";
	public const string ELF_SHOE = "elfShoe";
	public const string PIRATE_SHOE = "pirateShoe";
	public const string VIKING_SHOE = "vikingShoe";
	public const string CLOWN_SHOE = "clownShoe";

	public const string INSUFFICIENT_FUNDS_PURCHASE = "Insufficient funds!\nYou need more coins to purchase this item";
	public const string ITEM_ALREADY_PURCHASED = "You already have this item. You don't need to purchase it again.";

	public const string CAPE_PURCHASED = "Cape purchased! To glide, press and hold jump button while in the air.";
	public const string SHIELD_PURCHASED = "Shield purchased! You will now be invulnerable for 15 seconds!";
	public const string SNOWFLAKE_PURCHASED = "Snowflake Purchased! Cooling down...";
	public const string MAGNET_PURCHASED = "Magnet purchased! You will now attract coins and other items.";
	public const string SNOWFLAKE_NOT_NEEDED = "You don't need to cool down right now.";
	public const string CONGRATULATIONS = "Congratulations!";
	public const string TRIPLE_COIN_REWARD = "You received triple coins!";
	public const string WATCH_VIDEO_BONUS = "You received 50 coins!";

	public const string QUIT_TO_MAIN_MENU = "Are you sure you want to exit to the main menu?";
	public const string CONFIRM_RESTART = "Are you sure you want to restart?";

	public static string APP_STORE_LINK;

	public const string NEW_LINE = "\n";

	public static string UI_SELECT_LEVEL;
	public static string UI_PURCHASED;
	public static string UI_GAME_BY;
	public static string UI_THANKS_TO;
	public static string UI_THEME_BY;
	public static string UI_SOUND_EFFECTS;
	public static string UI_LOGO_DESIGN;
	public static string UI_LOADING;
	public static string UI_PAUSED;
	public static string UI_SKIP_LEVEL;
	public static string UI_CONTINUE;
	public static string UI_RESTART_LEVEL;
	public static string UI_STORE;
	public static string UI_MAIN_MENU;
	public static string UI_ARE_YOU_SURE_YOU_WANT_TO_RESTART;
	public static string UI_WATCH_VIDEO_FOR_MORE_COINS;
	public static string UI_YOU_DONT_NEED_TO_COOL_DOWN_RIGHT_NOW;
	public static string UI_YOU_ALREADY_PURCHASED_THIS_ITEM;
	public static string UI_ARE_YOU_SURE_YOU_WANT_TO_EXIT_TO_THE_MAIN_MENU;
	public static string UI_INSUFFICIENT_FUNDS;
	public static string UI_COINS;
	public static string UI_TIME;
	public static string UI_LIVES;
	public static string UI_GAME_COMPLETE;
	public static string UI_MORE_LEVELS_COMING_SOON;
	public static string UI_DO_YOU_WANT_TO_WATCH_A_VIDEO_TO_MAKE_THIS_LEVEL_EASIER;
	public static string UI_ARE_YOU_SURE_YOU_WANT_TO_QUIT;
	public static string UI_COULD_NOT_PLAY_THE_VIDEO;
	public static string UI_DO_YOU_LIKE_STOP_THE_POP;
	public static string UI_DO_YOU_WANT_TO_RATE_STOP_THE_POP;

	//Localised Strings for UI
	public static void LoadLocalisedStrings(LocalisedStrings localisedStrings) {
		localisedStrings.LoadStrings ();

		UI_SELECT_LEVEL = localisedStrings.UI_SELECT_LEVEL;
		UI_SOUND_EFFECTS = localisedStrings.UI_SOUND_EFFECTS;
		UI_LOGO_DESIGN = localisedStrings.UI_LOGO_DESIGN;
		UI_PURCHASED = localisedStrings.UI_PURCHASED;
		UI_GAME_BY = localisedStrings.UI_GAME_BY;
		UI_THANKS_TO = localisedStrings.UI_THANKS_TO;
		UI_THEME_BY = localisedStrings.UI_THEME_BY;
		UI_LOADING = localisedStrings.UI_LOADING;
		UI_PAUSED = localisedStrings.UI_PAUSED;
		UI_SKIP_LEVEL = localisedStrings.UI_SKIP_LEVEL;
		UI_CONTINUE = localisedStrings.UI_CONTINUE;
		UI_RESTART_LEVEL = localisedStrings.UI_RESTART_LEVEL;
		UI_STORE = localisedStrings.UI_STORE;
		UI_MAIN_MENU = localisedStrings.UI_MAIN_MENU;
		UI_ARE_YOU_SURE_YOU_WANT_TO_RESTART = localisedStrings.UI_ARE_YOU_SURE_YOU_WANT_TO_RESTART;
		UI_WATCH_VIDEO_FOR_MORE_COINS = localisedStrings.UI_WATCH_VIDEO_FOR_MORE_COINS;
		UI_YOU_DONT_NEED_TO_COOL_DOWN_RIGHT_NOW = localisedStrings.UI_YOU_DONT_NEED_TO_COOL_DOWN_RIGHT_NOW;
		UI_YOU_ALREADY_PURCHASED_THIS_ITEM = localisedStrings.UI_YOU_ALREADY_PURCHASED_THIS_ITEM;
		UI_ARE_YOU_SURE_YOU_WANT_TO_EXIT_TO_THE_MAIN_MENU = localisedStrings.UI_ARE_YOU_SURE_YOU_WANT_TO_EXIT_TO_THE_MAIN_MENU;
		UI_INSUFFICIENT_FUNDS = localisedStrings.UI_INSUFFICIENT_FUNDS;
		UI_COINS = localisedStrings.UI_COINS;
		UI_TIME = localisedStrings.UI_TIME;
		UI_LIVES = localisedStrings.UI_LIVES;
		UI_GAME_COMPLETE = localisedStrings.UI_GAME_COMPLETE;
		UI_MORE_LEVELS_COMING_SOON = localisedStrings.UI_MORE_LEVELS_COMING_SOON;
		UI_DO_YOU_WANT_TO_WATCH_A_VIDEO_TO_MAKE_THIS_LEVEL_EASIER = localisedStrings.UI_DO_YOU_WANT_TO_WATCH_A_VIDEO_TO_MAKE_THIS_LEVEL_EASIER;
		UI_ARE_YOU_SURE_YOU_WANT_TO_QUIT = localisedStrings.UI_ARE_YOU_SURE_YOU_WANT_TO_QUIT;
		UI_COULD_NOT_PLAY_THE_VIDEO = localisedStrings.UI_COULD_NOT_PLAY_THE_VIDEO;
		UI_DO_YOU_LIKE_STOP_THE_POP = localisedStrings.UI_DO_YOU_LIKE_STOP_THE_POP;
		UI_DO_YOU_WANT_TO_RATE_STOP_THE_POP = localisedStrings.UI_DO_YOU_WANT_TO_RATE_STOP_THE_POP;

		#if UNITY_IOS
		APP_STORE_LINK = "https://goo.gl/dsb3r9";
		#endif

		#if UNITY_ANDROID
		APP_STORE_LINK = "https://goo.gl/RK6Poh";
		#endif
	}
}
