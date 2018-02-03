using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * This class contains everything that can be purchased.
 * This firstly has hardcoded values for items in the inventory, 
 * then it reads in some values from disk to see if any objects were already purchased 
 */
public class StoreInventory {

	static List<StoreItem> inventory;

	public static void Init() {
		inventory = new List<StoreItem>();

		CollectableStoreItem capeStoreItem = new CollectableStoreItem ();
		capeStoreItem.locked = false;
		capeStoreItem.name = "Cape";
		capeStoreItem.cost = 250;
		capeStoreItem.quantity = 1;
		inventory.Add (capeStoreItem);

		CollectableStoreItem snowflakeStoreItem = new CollectableStoreItem ();
		snowflakeStoreItem.locked = false;
		snowflakeStoreItem.name = "Snowflake";
		snowflakeStoreItem.cost = 20;
		snowflakeStoreItem.quantity = 1;
		inventory.Add (snowflakeStoreItem);

		CollectableStoreItem magnetStoreItem = new CollectableStoreItem ();
		magnetStoreItem.locked = false;
		magnetStoreItem.name = Strings.MAGNET;
		magnetStoreItem.cost = 350;
		magnetStoreItem.quantity = 1;
		inventory.Add (magnetStoreItem);

		CollectableStoreItem shieldStoreItem = new CollectableStoreItem ();
		shieldStoreItem.locked = false;
		shieldStoreItem.name = Strings.SHIELD;
		shieldStoreItem.cost = 450;
		shieldStoreItem.quantity = 1;
		inventory.Add (shieldStoreItem);

		AddPlayerCustomisation (null, 0, false, PlayerCustomisationType.HAT_OR_HAIR);
		AddPlayerCustomisation (Strings.LEPRECHAUN_HAT, 300, true, PlayerCustomisationType.HAT_OR_HAIR);
		AddPlayerCustomisation (Strings.VIKING_HAT, 400, true, PlayerCustomisationType.HAT_OR_HAIR);
		AddPlayerCustomisation (Strings.PIRATE_HAT, 350, true, PlayerCustomisationType.HAT_OR_HAIR);
		AddPlayerCustomisation (Strings.ELF_HAT, 250, true, PlayerCustomisationType.HAT_OR_HAIR);
		AddPlayerCustomisation (Strings.SANTA_HAT, 200, true, PlayerCustomisationType.HAT_OR_HAIR);

		AddPlayerCustomisation (null, 0, false, PlayerCustomisationType.FACIAL_HAIR);
		AddPlayerCustomisation (Strings.LEPRECHAUN_BEARD, 200, true, PlayerCustomisationType.FACIAL_HAIR);
		AddPlayerCustomisation (Strings.VIKING_BEARD, 350, true, PlayerCustomisationType.FACIAL_HAIR);
		AddPlayerCustomisation (Strings.PIRATE_BEARD, 300, true, PlayerCustomisationType.FACIAL_HAIR);
		AddPlayerCustomisation (Strings.SANTA_BEARD, 250, true, PlayerCustomisationType.FACIAL_HAIR);

		AddPlayerCustomisation (null, 0, false, PlayerCustomisationType.SHOES);
		AddPlayerCustomisation (Strings.LEPRECHAUN_SHOE, 100, true, PlayerCustomisationType.SHOES);
		AddPlayerCustomisation (Strings.VIKING_SHOE, 250, true, PlayerCustomisationType.SHOES);
		AddPlayerCustomisation (Strings.PIRATE_SHOE, 300, true, PlayerCustomisationType.SHOES);
		AddPlayerCustomisation (Strings.ELF_SHOE, 150, true, PlayerCustomisationType.SHOES);
		AddPlayerCustomisation (Strings.SANTA_SHOE, 200, true, PlayerCustomisationType.SHOES);

		//loop through all the game characters, if the character already exists in the file, 
		//set the locked flag to whatever is in the file.
		foreach (StoreItem item in inventory) {
			PlayerStatistics itemFromFile = GameStats.GetInstance ().GetPlayerStats (item.name);
			if (itemFromFile != null) {
				item.locked = itemFromFile.locked;
			} 
		}

		//loop through all the player customisations, if the customisation already exists in the file, 
		//set the locked flag to whatever is in the file.
		foreach (StoreItem item in inventory) {
			PlayerCustomisation itemFromFile = GameStats.GetInstance ().GetPlayerCustomisation (item.name);
			if (itemFromFile != null) {
				item.locked = itemFromFile.locked;
			} 
		}
	}

	private static void AddPlayerCustomisation(string name, int cost, bool locked, PlayerCustomisationType type) {
		PlayerCustomisation customisation = new PlayerCustomisation ();
		customisation.type = type;
		customisation.name = name;
		customisation.cost = cost;
		customisation.locked = locked;
		inventory.Add (customisation);
	}

	public static List<StoreItem> GetAllItems() {
		return inventory;
	}

	/***
	 * Look for a type of player customisation in the inventory. 
	 * Can search for hat/hair, glasses, shoes, facial hair
	 */
	public static List<PlayerCustomisation> GetAllPlayerCustomisationsOfType(PlayerCustomisationType type) {
		List<PlayerCustomisation> playerCustomisationItems = new List<PlayerCustomisation> ();
		foreach (StoreItem item in inventory) {
			if (item is PlayerCustomisation) {
				PlayerCustomisation playerCustomisation = (PlayerCustomisation) item;
				if (playerCustomisation.type == type) {
					playerCustomisationItems.Add (playerCustomisation);
				}
			}
		}

		return playerCustomisationItems;
	}

	public static List<PlayerCustomisation> GetAllLockedPlayerCustomisations() {
		List<PlayerCustomisation> lockedPlayerCustomisations = new List<PlayerCustomisation> ();
		foreach (StoreItem item in inventory) {
			if (item is PlayerCustomisation && item.locked) {
				//bug here, where we don't actually have glasses to give to the player but 
				//there was one hidden one in the save file in a release, so it always exists in the file but it is never used
				//but it still can give undesireable results so we just block it here by not adding it.
				if (((PlayerCustomisation)item).type != PlayerCustomisationType.GLASSES) {
					lockedPlayerCustomisations.Add ((PlayerCustomisation)item);
				}
			}
		}

		return lockedPlayerCustomisations;
	}

	/***
	 * Get a reference to an item in the store.
	 * Returns null if could not find it.
	 */
	public static StoreItem GetItemFromInventory(string itemName) {
		//loop through the inventory list and return the store item for that name
		foreach(StoreItem item in inventory) {
			if (itemName == item.name) {
				return item;
			}
		}
		return null;
	}
}
