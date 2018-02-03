using UnityEngine;
using System.Collections;

public class PlayerCustomisation : StoreItem {
	public PlayerCustomisationType type;

	//these static sprite arrays are a cache mainly to speed up loading of levels and 
	//to improve the performance of the generate gift behaviour. These are set in the
	//PlayerController class when the player is getting reskinned. The first time it is
	//called the load operation takes place but then it is cached for the rest of the game
	public static Sprite[] hatSprites;
	public static Sprite[] facialHairSprites;
	public static Sprite[] shoesSprites;
}
