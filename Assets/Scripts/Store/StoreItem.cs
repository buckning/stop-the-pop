using UnityEngine;
using System.Collections;

/***
 * This class is used by the store to keep its inventory.
 */
public class StoreItem {
	public string name;				//the internal name of the item
	public string displayName;		//the name that will be shown on the UI
	public int cost;				//the cost to purchase this item
	public bool locked;				//states whether this item is accessible to the player
	public Sprite thumbnail;		//the sprite of the thumbnail
}
