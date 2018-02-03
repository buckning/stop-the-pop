using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Advertisements;

public class PlayerCustomiseScreen : MonoBehaviour {
	public PlayerCustomiseBuyButton hatButton;
	public PlayerCustomiseBuyButton facialHairButton;
	public PlayerCustomiseBuyButton shoesButton;

	public GAui[] customisationPanels;

	public Button selectButton;

	public Text customisationScreenCoinCountText;
	public TitleScreenPlayerAnimation playerAvatar;

	public TitleScreenListener titleScreen;

	private int playerCustomisationHatIndex = 0;
	private int playerCustomisationFacialHairIndex = 0;
	private int playerCustomisationShoesIndex = 0;

	private TextFieldNumberAnimator textFieldAnimator;

	float lastSoundPlay = 0.0f;

	void Start() {
		playerAvatar.CustomisePlayer();
		textFieldAnimator = customisationScreenCoinCountText.GetComponent<TextFieldNumberAnimator> ();
		textFieldAnimator.initialNumber = GameStats.GetInstance ().totalNumberOfCoins;
		textFieldAnimator.currentNumber = GameStats.GetInstance ().totalNumberOfCoins;
		textFieldAnimator.desiredNumber = GameStats.GetInstance ().totalNumberOfCoins;
		textFieldAnimator.SetNumber (GameStats.GetInstance ().totalNumberOfCoins);

		textFieldAnimator.valueDecrementedListeners = PlayCoinSound;
		textFieldAnimator.valueIncrementedListeners = PlayCoinSound;
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			BackButtonPressed();
		}
	}

	void PlayCoinSound(float value) {
		if ((Time.unscaledTime - lastSoundPlay) > 0.05f) {
			AudioManager.PlaySound ("coin-new");
			lastSoundPlay = Time.unscaledTime;
		}
	}

	public void BackButtonPressed() {
		AudioManager.PlaySound ("Click", 0.9f);
		Component[] buttons = gameObject.transform.GetComponentsInChildren(typeof(GAui), true);

		foreach(GAui button in buttons) {
			button.MoveOut (GSui.eGUIMove.SelfAndChildren);
		}
		playerAvatar.CustomisePlayer (SelectedPlayerCustomisations.selectedHat,
			SelectedPlayerCustomisations.selectedFacialHair,
			SelectedPlayerCustomisations.selectedShoes
		);

		titleScreen.EnableTitleScreen ();
	}

	public void SetActive(bool active) {
		gameObject.SetActive (active);

		playerCustomisationHatIndex = 0;
		playerCustomisationFacialHairIndex = 0;
		playerCustomisationShoesIndex = 0;

		Component[] buttons = gameObject.transform.GetComponentsInChildren(typeof(GAui), true);

		foreach(GAui button in buttons) {
			button.gameObject.SetActive (true);
			button.MoveIn (GSui.eGUIMove.SelfAndChildren);
		}
		RefreshPlayerCustomisationScreen ();

		ReskinPlayer ();
	}

	/****************
	 * 
	 * Player Hat actions
	 * 
	 * ********************/

	public void PlayerHatCustomisationPrevButtonPressed() {
		AudioManager.PlaySound ("Click", 0.9f);
		playerCustomisationHatIndex--;
		RefreshPlayerCustomisationScreen ();
	}

	public void PlayerHatCustomisationNextButtonPressed() {
		AudioManager.PlaySound ("Click");
		playerCustomisationHatIndex++;
		RefreshPlayerCustomisationScreen ();
	}

	public void PurchaseEquipOrUnequipHat() {
		AudioManager.PlaySound ("Click");
		List<PlayerCustomisation> hatInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.HAT_OR_HAIR);
		PlayerCustomisation itemBeingPurchased = hatInventory [playerCustomisationHatIndex];

		if (itemBeingPurchased.locked) {
			if (Store.Purchase (itemBeingPurchased)) {
				textFieldAnimator.AddToNumber (itemBeingPurchased.cost * -1);
				AnalyticsManager.SendPlayerCustomisationPurchasedEvent (itemBeingPurchased.name, itemBeingPurchased.cost, GameStats.GetInstance ().totalNumberOfCoins);
			}
		} 

		RefreshPlayerCustomisationScreen ();
	}

	/****************
	 * 
	 * Player Facial Hair actions
	 * 
	 * ********************/
	public void PlayerFacialHairCustomisationPrevButtonPressed() {
		AudioManager.PlaySound ("Click", 0.9f);
		playerCustomisationFacialHairIndex--;
		RefreshPlayerCustomisationScreen ();
	}

	public void PlayerFacialHairCustomisationNextButtonPressed() {
		AudioManager.PlaySound ("Click");
		playerCustomisationFacialHairIndex++;
		RefreshPlayerCustomisationScreen ();
	}

	public void PurchaseEquipOrUnequipFacialHair() {
		AudioManager.PlaySound ("Click");
		List<PlayerCustomisation> facialHairInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.FACIAL_HAIR);
		PlayerCustomisation itemBeingPurchased = facialHairInventory [playerCustomisationFacialHairIndex];

		if (itemBeingPurchased.locked) {
			if (Store.Purchase (itemBeingPurchased)) {
				textFieldAnimator.AddToNumber (itemBeingPurchased.cost * -1);
				AnalyticsManager.SendPlayerCustomisationPurchasedEvent (itemBeingPurchased.name, itemBeingPurchased.cost, GameStats.GetInstance ().totalNumberOfCoins);
			}
		}

		RefreshPlayerCustomisationScreen ();
	}


	/****************
	 * 
	 * Player shoes actions
	 * 
	 * ********************/
	public void PlayerShoesCustomisationPrevButtonPressed() {
		AudioManager.PlaySound ("Click", 0.9f);
		playerCustomisationShoesIndex--;
		RefreshPlayerCustomisationScreen ();
	}

	public void PlayerShoesCustomisationNextButtonPressed() {
		AudioManager.PlaySound ("Click");
		playerCustomisationShoesIndex++;
		RefreshPlayerCustomisationScreen ();
	}

	public void PurchaseEquipOrUnequipShoes() {
		AudioManager.PlaySound ("Click");
		List<PlayerCustomisation> shoesInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.SHOES);
		PlayerCustomisation itemBeingPurchased = shoesInventory [playerCustomisationShoesIndex];

		if (itemBeingPurchased.locked) {
			if (Store.Purchase (itemBeingPurchased)) {
				textFieldAnimator.AddToNumber (itemBeingPurchased.cost * -1);
				AnalyticsManager.SendPlayerCustomisationPurchasedEvent (itemBeingPurchased.name, itemBeingPurchased.cost, GameStats.GetInstance ().totalNumberOfCoins);
			}
		} 

		RefreshPlayerCustomisationScreen ();
	}

	public void RefreshPlayerCustomisationScreen() {
		List<PlayerCustomisation> hatInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.HAT_OR_HAIR);

		if (playerCustomisationHatIndex < 0) {
			playerCustomisationHatIndex = hatInventory.Count - 1;
		}
		if (playerCustomisationHatIndex >= hatInventory.Count) {
			playerCustomisationHatIndex = 0;
		}


		UpdateHighlightedItem (playerCustomisationHatIndex, hatButton, PlayerCustomisationType.HAT_OR_HAIR);
		PlayerCustomisation highlightedHat = hatInventory [playerCustomisationHatIndex];

		//Refresh the facial hair
		List<PlayerCustomisation> facialHairInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.FACIAL_HAIR);
		if (playerCustomisationFacialHairIndex < 0) {
			playerCustomisationFacialHairIndex = facialHairInventory.Count - 1;
		}
		if (playerCustomisationFacialHairIndex >= facialHairInventory.Count) {
			playerCustomisationFacialHairIndex = 0;
		}
			
		PlayerCustomisation highlightedFacialHair = facialHairInventory [playerCustomisationFacialHairIndex];

		UpdateHighlightedItem (playerCustomisationFacialHairIndex, facialHairButton, PlayerCustomisationType.FACIAL_HAIR);

		List<PlayerCustomisation> shoesInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.SHOES);
		if (playerCustomisationShoesIndex < 0) {
			playerCustomisationShoesIndex = shoesInventory.Count - 1;
		}
		if (playerCustomisationShoesIndex >= shoesInventory.Count) {
			playerCustomisationShoesIndex = 0;
		}

		PlayerCustomisation highlightedShoes = shoesInventory [playerCustomisationShoesIndex];

		UpdateHighlightedItem (playerCustomisationShoesIndex, shoesButton, PlayerCustomisationType.SHOES);

		playerAvatar.CustomisePlayer (hatInventory [playerCustomisationHatIndex].name,
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
	 * Update a purchase button in the store based on the highlighted item in the store.
	 * 
	 */
	void UpdateHighlightedItem(int index, PlayerCustomiseBuyButton playerCustomiseBuyButton, PlayerCustomisationType playerCustomisationType) {
		List<PlayerCustomisation> inventory = Store.GetPlayerCustomisationItemsOfType (playerCustomisationType);

		PlayerCustomisation highlightedItem = inventory [index];
		if (highlightedItem.locked) {
			playerCustomiseBuyButton.EnableCoinImage (true);
			playerCustomiseBuyButton.GetComponentInChildren<Text> ().text = "" + highlightedItem.cost;
			if (highlightedItem.cost > GameStats.GetInstance ().totalNumberOfCoins) {
				playerCustomiseBuyButton.SetInteractable(false);
			} else {
				playerCustomiseBuyButton.SetInteractable(true);
			}
		} else {
			playerCustomiseBuyButton.SetInteractable (false);
			playerCustomiseBuyButton.EnableCoinImage (false);
			playerCustomiseBuyButton.SetText (Strings.UI_PURCHASED);
		}
	}

	public void SelectHighlightedPlayerCustomisation() {
		List<PlayerCustomisation> hatInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.HAT_OR_HAIR);
		PlayerCustomisation highlightedHat = hatInventory [playerCustomisationHatIndex];
		SelectedPlayerCustomisations.selectedHat = highlightedHat.name;

		List<PlayerCustomisation> facialHairInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.FACIAL_HAIR);
		PlayerCustomisation highlightedFacialHair = facialHairInventory [playerCustomisationFacialHairIndex];
		SelectedPlayerCustomisations.selectedFacialHair = highlightedFacialHair.name;

		List<PlayerCustomisation> shoesInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.SHOES);
		PlayerCustomisation highlightedShoes = shoesInventory [playerCustomisationShoesIndex];
		SelectedPlayerCustomisations.selectedShoes = highlightedShoes.name;

		//save to settings
		Settings.selectedHat = SelectedPlayerCustomisations.selectedHat;
		Settings.selectedFacialHair = SelectedPlayerCustomisations.selectedFacialHair;
		Settings.selectedShoes = SelectedPlayerCustomisations.selectedShoes;
		GameDataPersistor.Save (GameStats.GetInstance ().GetGameData ());

		BackButtonPressed ();
	}

	public void WatchVideoForMoreCoins() {
		if (Advertisement.IsReady ("rewardedVideo")) {
			AnalyticsManager.SendAdWatchEvent ("TitleScreen", "PlayerCustomiseScreenAd", 0, 0);
			var options = new ShowOptions { resultCallback = GetMoreCoins };
			Advertisement.Show ("rewardedVideo", options);
		} else {
			titleScreen.ShowErrorPanel();
		}
	}

	private void GetMoreCoins(ShowResult result) {
		switch (result) {
		case ShowResult.Finished:

			GameStats stats = GameStats.GetInstance ();
			stats.totalNumberOfCoins += 300;
			stats.SaveToDisk ();

			textFieldAnimator.AddToNumber (300);

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

	public void ReskinPlayer() {
		SelectedPlayerCustomisations.selectedHat = Settings.selectedHat;
		SelectedPlayerCustomisations.selectedFacialHair = Settings.selectedFacialHair;
		SelectedPlayerCustomisations.selectedShoes = Settings.selectedShoes;

		playerAvatar.CustomisePlayer (SelectedPlayerCustomisations.selectedHat,
			SelectedPlayerCustomisations.selectedFacialHair,
			SelectedPlayerCustomisations.selectedShoes
		);

		//this is needed so the counter will be correctly set when the game is returned to the title screen
		//and a customisation is selected. If there was something previously selected, the index should be updated to 
		//the correct one. This should be in its own method but meh...
		if (SelectedPlayerCustomisations.selectedHat != null) {
			List<PlayerCustomisation> hatInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.HAT_OR_HAIR);
			int index = 0;
			foreach (PlayerCustomisation customisation in hatInventory) {
				if (SelectedPlayerCustomisations.selectedHat == hatInventory [index].name) {
					playerCustomisationHatIndex = index;
				}
				index++;
			}
		}

		if (SelectedPlayerCustomisations.selectedFacialHair != null) {
			List<PlayerCustomisation> facialHairInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.FACIAL_HAIR);
			int index = 0;
			foreach (PlayerCustomisation customisation in facialHairInventory) {
				if (SelectedPlayerCustomisations.selectedFacialHair == facialHairInventory [index].name) {
					playerCustomisationFacialHairIndex = index;
				}
				index++;
			}
		}

		if (SelectedPlayerCustomisations.selectedShoes != null) {
			List<PlayerCustomisation> shoesInventory = Store.GetPlayerCustomisationItemsOfType (PlayerCustomisationType.SHOES);
			int index = 0;
			foreach (PlayerCustomisation customisation in shoesInventory) {
				if (SelectedPlayerCustomisations.selectedShoes == shoesInventory [index].name) {
					playerCustomisationShoesIndex = index;
				}
				index++;
			}
		}
	}
}
