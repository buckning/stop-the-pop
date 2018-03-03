using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * This is the main store of the game. This handles the purchasing of levels, players or other items
 */
public class Store {

	/***
	 * Load the inventory using the passed in inventory list and update it from the file
	 */
	public static void LoadStore() {
		//read in our inventory. This reads in the hard coded information and 
		//updates the information from the file on disk
		StoreInventory.Init ();		
	}

	public static bool CanAfford(StoreItem item) {
		int balance = GetWalletBalance ();
		if (item.cost <= balance) {
			return true;
		}
		return false;
	}

	public static int GetWalletBalance() {
		return GameStats.GetInstance ().totalNumberOfCoins;
	}

	/***
	 * Purchase an item from the store.
	 * Returns true if the transaction completed successfully. 
	 */
	public static bool Purchase(StoreItem item) {
		if (GetWalletBalance () < item.cost) {
			return false;
		}

		item.locked = false;

		//check if a player is being bought
		if (item is PlayerStoreItem) {
			PurchasePlayer ((PlayerStoreItem) item);
		} else if (item is CollectableStoreItem) {
			//save the collectable to the game stats
			PurchaseCollectable ((CollectableStoreItem) item);
		} else if (item is PlayerCustomisation) {
			PurchasePlayerCustomisation ((PlayerCustomisation) item);
		}

		GameStats.GetInstance().totalNumberOfCoins -= item.cost;

		//write to game stats and save to file
		GameDataPersistor.Save (GameStats.GetInstance ().GetGameData ());

		return true;
	}

	public static void PurchasePlayerReward(PlayerCustomisation item) {
		PurchasePlayerCustomisation (item);
		GameDataPersistor.Save (GameStats.GetInstance ().GetGameData ());
	}

	/***
	 * Purchase a player
	 */
	public static bool PurchasePlayer(PlayerStoreItem playerItem) {
		PlayerStatistics playerStats = new PlayerStatistics();
		playerStats.name = playerItem.name;
		playerStats.locked = false;
		GameStats.GetInstance ().AddStatsForPlayer (playerStats);	//save our purchase to RAM

		return true;
	}

	public static bool PurchasePlayerCustomisation(PlayerCustomisation playerCustomisation) {
		Logger.Log ("purchased  " + playerCustomisation.name);

		PlayerCustomisation purchasedItem = new PlayerCustomisation ();
		purchasedItem.name = playerCustomisation.name;
		purchasedItem.type = playerCustomisation.type;
		purchasedItem.locked = false;
		StoreInventory.GetItemFromInventory (playerCustomisation.name).locked = false;
		GameStats.GetInstance ().AddStatsForPlayerCustomisation (purchasedItem);

		return true;
	}

	public static bool PurchaseCollectable(CollectableStoreItem collectableItem) {
		if (collectableItem.name == "Cape") {
			GameStats.GetInstance ().numberOfCapes += collectableItem.quantity;
		}
		return true;
	}

	public static List<PlayerCustomisation> GetPlayerCustomisationItemsOfType(PlayerCustomisationType type) {
		return StoreInventory.GetAllPlayerCustomisationsOfType (type);
	}

	/***
	 * Purchase a level
	 */
	public static void PurchaseLevel(LevelStoreItem levelItem) {

	}

	/***
	 * Get a reference to an item in the store.
	 * Returns null if could not find it.
	 */
	public static StoreItem GetStoreItem(string itemName) {
		return StoreInventory.GetItemFromInventory (itemName);
	}

	public static List<StoreItem> GetAllStoreItems() {
		return StoreInventory.GetAllItems ();
	}


	public static List<PlayerStoreItem> GetPlayableCharacters() {
		List<StoreItem> inventory = StoreInventory.GetAllItems ();
		List<PlayerStoreItem> playableCharacters = new List<PlayerStoreItem> ();
		foreach (StoreItem item in inventory) {
			if(item is PlayerStoreItem) {
				playableCharacters.Add ((PlayerStoreItem)item);
			}
		}

		return playableCharacters;
	}
}
