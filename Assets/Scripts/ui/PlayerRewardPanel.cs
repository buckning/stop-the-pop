using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRewardPanel : MonoBehaviour {
	public Image backgroundFader;
	public Image rewardImage;
	public GameObject giftButton;
	public GAui spinningLines;
	public GAui spinningStars;

	public void ShowGift() {
		giftButton.SetActive (true);
		backgroundFader.gameObject.SetActive (true);
	}

	public void SetRewardImage(Sprite sprite) {
		rewardImage.sprite = sprite;
		rewardImage.preserveAspect = true;
	}

	public void GiftBoxPressed() {
		giftButton.SetActive(false);
		rewardImage.gameObject.SetActive(true);
		rewardImage.gameObject.GetComponent<GAui> ().MoveIn ();
		spinningLines.gameObject.SetActive (true);
		spinningStars.gameObject.SetActive (true);
		spinningLines.MoveIn ();
		spinningStars.MoveIn ();
		AudioManager.PlaySound ("Reward");
	}

	public void RewardPressed() {
		rewardImage.gameObject.SetActive (false);
		backgroundFader.gameObject.SetActive (false);
		spinningLines.gameObject.SetActive (false);
		spinningStars.gameObject.SetActive (false);

		Settings.selectedHat = SelectedPlayerCustomisations.selectedHat;
		Settings.selectedGlasses = SelectedPlayerCustomisations.selectedGlasses;
		Settings.selectedFacialHair = SelectedPlayerCustomisations.selectedFacialHair;
		Settings.selectedShoes = SelectedPlayerCustomisations.selectedShoes;

		GameDataPersistor.Save (GameStats.GetInstance ().GetGameData ());
	}

	public void StartRewardFlashPulse() {
		Animator rewardImageAnimator = rewardImage.GetComponent<Animator> ();
		rewardImageAnimator.enabled = true;
		rewardImageAnimator.SetTrigger ("StartPulse");
	}
}
