using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;

public class PlayerCustomiseScreen : MonoBehaviour {
	public PlayerCustomiseBuyButton hatButton;			// the hat button, we need to update a reference of this to update the text on the button
	public PlayerCustomiseBuyButton facialHairButton;	// the facial hair button, we need to update a reference of this to update the text on the button
	public PlayerCustomiseBuyButton shoesButton;		// the shoes button, we need to update a reference of this to update the text on the button

	public Button selectButton;

	public PopcornKernelAnimator player;

	public event Event backButtonListeners;
	public delegate void Event ();

	private int playerCustomisationHatIndex = 0;
	private int playerCustomisationFacialHairIndex = 0;
	private int playerCustomisationShoesIndex = 0;

	public TextFieldNumberAnimator coinCountTextFieldAnimator;

	private float lastSoundPlay = 0.0f;

	private Color DISABLED_COLOUR = new Color (0.25f, 0.25f, 0.25f, 1.0f);
	private Color ENABLED_COLOUR = new Color (1.0f, 1.0f, 1.0f, 1.0f);

	private List<PlayerCustomisation> facialHairInventory;
	private List<PlayerCustomisation> hatInventory;
	private List<PlayerCustomisation> shoesInventory;

	void Start() {
		Store.LoadStore ();
		int totalNumberOfCoins = Store.GetWalletBalance ();
		coinCountTextFieldAnimator.initialNumber = totalNumberOfCoins;
		coinCountTextFieldAnimator.currentNumber = totalNumberOfCoins;
		coinCountTextFieldAnimator.desiredNumber = totalNumberOfCoins;
		coinCountTextFieldAnimator.SetNumber (totalNumberOfCoins);

		coinCountTextFieldAnimator.valueDecrementedListeners = PlayCoinSound;
		coinCountTextFieldAnimator.valueIncrementedListeners = PlayCoinSound;

		Store.LoadStore ();

		SetActive (true);

		RefreshPlayerCustomisationScreen ();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			BackButtonPressed();
		}
	}

	void PlayCoinSound(float value) {
		if ((Time.unscaledTime - lastSoundPlay) > 0.05f) {
			// TODO play sound here
			lastSoundPlay = Time.unscaledTime;
		}
	}

	public void BackButtonPressed() {
		player.CustomisePlayer (SelectedPlayerCustomisations.selectedHat,
			SelectedPlayerCustomisations.selectedFacialHair,
			SelectedPlayerCustomisations.selectedShoes
		);
		if (backButtonListeners != null) {
			backButtonListeners ();
		}
	}

	public void SetActive(bool active) {
		gameObject.SetActive (active);

		playerCustomisationHatIndex = 0;
		playerCustomisationFacialHairIndex = 0;
		playerCustomisationShoesIndex = 0;

		facialHairInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.FACIAL_HAIR);
		hatInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.HAT_OR_HAIR);
		shoesInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.SHOES);

		RefreshPlayerCustomisationScreen ();
	}

	public void PurchaseEquipOrUnequipHat() {
		PurchaseItem (hatInventory [playerCustomisationHatIndex]);
	}

	public void PurchaseEquipOrUnequipFacialHair() {
		PurchaseItem (facialHairInventory [playerCustomisationFacialHairIndex]);
	}

	public void PurchaseEquipOrUnequipShoes() {
		PurchaseItem (shoesInventory [playerCustomisationShoesIndex]);
	}

	/***
	 * This is called back by the Select button
	 */
	public void SelectHighlightedPlayerCustomisation() {
		PlayerCustomisation highlightedHat = hatInventory [playerCustomisationHatIndex];
		SelectedPlayerCustomisations.selectedHat = highlightedHat.name;

		PlayerCustomisation highlightedFacialHair = facialHairInventory [playerCustomisationFacialHairIndex];
		SelectedPlayerCustomisations.selectedFacialHair = highlightedFacialHair.name;

		PlayerCustomisation highlightedShoes = shoesInventory [playerCustomisationShoesIndex];
		SelectedPlayerCustomisations.selectedShoes = highlightedShoes.name;

		//save to settings
		Settings.selectedHat = SelectedPlayerCustomisations.selectedHat;
		Settings.selectedFacialHair = SelectedPlayerCustomisations.selectedFacialHair;
		Settings.selectedShoes = SelectedPlayerCustomisations.selectedShoes;
		GameDataPersistor.Save (GameStats.GetInstance ().GetGameData ());

		BackButtonPressed ();
	}

	public void PlayerHatCustomisationPrevButtonPressed() {
		playerCustomisationHatIndex = GetRolledOverIndex(playerCustomisationHatIndex - 1, 
			hatInventory.Count);

		RefreshPlayerCustomisationScreen ();
	}

	public void PlayerHatCustomisationNextButtonPressed() {
		playerCustomisationHatIndex = GetRolledOverIndex(playerCustomisationHatIndex + 1, 
			hatInventory.Count);

		RefreshPlayerCustomisationScreen ();
	}

	public void PlayerFacialHairCustomisationPrevButtonPressed() {
		playerCustomisationFacialHairIndex = GetRolledOverIndex(playerCustomisationFacialHairIndex - 1, 
			facialHairInventory.Count);

		RefreshPlayerCustomisationScreen ();
	}

	public void PlayerFacialHairCustomisationNextButtonPressed() {
		playerCustomisationFacialHairIndex = GetRolledOverIndex(playerCustomisationFacialHairIndex + 1, 
			facialHairInventory.Count);

		RefreshPlayerCustomisationScreen ();
	}

	public void PlayerShoesCustomisationPrevButtonPressed() {
		playerCustomisationShoesIndex = GetRolledOverIndex(playerCustomisationShoesIndex - 1, 
			shoesInventory.Count);
		RefreshPlayerCustomisationScreen ();
	}

	public void PlayerShoesCustomisationNextButtonPressed() {
		playerCustomisationShoesIndex = GetRolledOverIndex(playerCustomisationShoesIndex + 1, 
			shoesInventory.Count);
		RefreshPlayerCustomisationScreen ();
	}

	public void WatchVideoForMoreCoins() {
		if (Advertisement.IsReady ("rewardedVideo")) {
			AnalyticsManager.SendAdWatchEvent ("TitleScreen", "PlayerCustomiseScreenAd", 0, 0);
			var options = new ShowOptions { resultCallback = GetMoreCoins };
			Advertisement.Show ("rewardedVideo", options);
		} else {
			// TODO - handle error case here
		}
	}

	private void GetMoreCoins(ShowResult result) {
		switch (result) {
		case ShowResult.Finished:

			GameStats stats = GameStats.GetInstance ();
			stats.totalNumberOfCoins += 300;
			stats.SaveToDisk ();

			coinCountTextFieldAnimator.AddToNumber (300);

			RefreshPlayerCustomisationScreen ();
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}

	private void RefreshPlayerCustomisationScreen() {
		PlayerCustomisation highlightedHat = hatInventory [playerCustomisationHatIndex];
		PlayerCustomisation highlightedFacialHair = facialHairInventory [playerCustomisationFacialHairIndex];
		PlayerCustomisation highlightedShoes = shoesInventory [playerCustomisationShoesIndex];

		UpdatePurchaseButton (playerCustomisationHatIndex, hatButton, PlayerCustomisationType.HAT_OR_HAIR);
		UpdatePurchaseButton (playerCustomisationFacialHairIndex, facialHairButton, PlayerCustomisationType.FACIAL_HAIR);
		UpdatePurchaseButton (playerCustomisationShoesIndex, shoesButton, PlayerCustomisationType.SHOES);

		GreyOutPlayerSprite (highlightedShoes.locked, "Shoe");
		GreyOutPlayerSprite (highlightedHat.locked, "Hat");
		GreyOutPlayerSprite (highlightedFacialHair.locked, "FacialHair");

		player.CustomisePlayer (hatInventory [playerCustomisationHatIndex].name,
			facialHairInventory [playerCustomisationFacialHairIndex].name,
			shoesInventory [playerCustomisationShoesIndex].name
		);

		if (!highlightedHat.locked && !highlightedFacialHair.locked && !highlightedShoes.locked) {
			selectButton.interactable = true;
		} else {
			selectButton.interactable = false;
		}
	}

	/***
	 * Update a purchase button in the store based on the status of item in the store 
	 * and on if the player can afford the item or not.
	 */
	private void UpdatePurchaseButton(int index, PlayerCustomiseBuyButton playerCustomiseBuyButton, PlayerCustomisationType playerCustomisationType) {
		List<PlayerCustomisation> inventory = Store.GetPlayerCustomisationItemsOfType (playerCustomisationType);

		PlayerCustomisation highlightedItem = inventory [index];

		if (highlightedItem.locked) {
			playerCustomiseBuyButton.SetInteractable (Store.CanAfford (highlightedItem));
			playerCustomiseBuyButton.EnableCoinImage (true);
			playerCustomiseBuyButton.SetText ("" + highlightedItem.cost);
		} else {
			playerCustomiseBuyButton.SetInteractable (false);
			playerCustomiseBuyButton.EnableCoinImage (false);
			playerCustomiseBuyButton.SetText (Strings.UI_PURCHASED);
		}
	}

	private void PurchaseItem(PlayerCustomisation itemBeingPurchased) {
		if (itemBeingPurchased.locked) {
			if (Store.Purchase (itemBeingPurchased)) {
				coinCountTextFieldAnimator.AddToNumber (itemBeingPurchased.cost * -1);
				AnalyticsManager.SendPlayerCustomisationPurchasedEvent (itemBeingPurchased.name, itemBeingPurchased.cost, GameStats.GetInstance ().totalNumberOfCoins);
			}
		} 

		RefreshPlayerCustomisationScreen ();
	}

	/***
	 * Grey out a sprite on the player or set to the original colour. 
	 */
	private void GreyOutPlayerSprite(bool greyOut, string spriteToGreyOutName) {
		SpriteRenderer[] playerSprites = player.GetComponentsInChildren<SpriteRenderer> ();

		foreach(SpriteRenderer sprite in playerSprites) {
			if (sprite.name == spriteToGreyOutName) {
				if (greyOut) {
					sprite.color = DISABLED_COLOUR;
				} else {
					sprite.color = ENABLED_COLOUR;
				}
			}
		}
	}

	/***
	 * This method is to get the new index within the bounds of 0 - limit. 
	 * If it falls outside of these values, it is rolled over.
	 * If the current index is less than 0, it is set to the limit - 1.
	 * If the current index is greater or equal to the limit, it is set to 0.
	 */
	private int GetRolledOverIndex(int currentIndex, int limit) {
		int newIndex = currentIndex;
		if (currentIndex < 0) {
			newIndex = limit - 1;
		}
		if (currentIndex >= limit) {
			newIndex = 0;
		}

		return newIndex;
	}
}
