using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftRewardGenerator : MonoBehaviour {
	
	public static PlayerCustomisation GenerateReward() {
		List<PlayerCustomisation> lockedItems = StoreInventory.GetAllLockedPlayerCustomisations ();

		PlayerCustomisation gift = lockedItems[Random.Range (0, lockedItems.Count)];
		Settings.lastRewardGivenTime = new TimeDiff().TimeNow();
		Store.PurchasePlayerReward (gift);

		return gift;
	}
}
