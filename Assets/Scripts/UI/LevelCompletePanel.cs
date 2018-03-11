using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletePanel : MonoBehaviour {

	public TextFieldNumberAnimator coinsTextAnimator;
	public TextFieldNumberAnimator livesLostTextAnimator;
	public TextFieldNumberAnimator timeTakenTextAnimator;

	public Image starCoinsCollectedImage;
	public Image starLivesLost;
	public Image starTimeTaken;

	public ScreenFader whiteFlash;
	public GameObject starBurstParticleSystem;
	public GameObject spinningStars;
	public Image spinningLines;
	public GameObject buttonsPanel;
	public GameObject sparkles;

	private int numberOfCoins = 0;
	private int numberOfLivesLost = 0;
	private int timeTaken = 0;

	private const int NUMBER_OF_COINS_GOAL = 100;
	private const int NUMBER_OF_LIVES_LOST_GOAL = 0;
	private const int TIME_TAKEN_GOAL = 120;

	void Start() {
		if (GameObject.Find ("Canvas").GetComponent<Canvas> ().renderMode 
				!= RenderMode.ScreenSpaceCamera) {
			Debug.LogError ("The LevelCompletePanel requires the canvas render mode to be set to " +
				"Screen Space - Camera. This might not function as expected.");
		}

		InitialiseTextAnimator (coinsTextAnimator);
		InitialiseTextAnimator (livesLostTextAnimator);
		InitialiseTextAnimator (timeTakenTextAnimator);

		coinsTextAnimator.animationCompleteListeners += AnimateCoinsCollectedStar;
		livesLostTextAnimator.animationCompleteListeners += AnimateLivesLostStar;
		timeTakenTextAnimator.animationCompleteListeners += AnimateTimeTakenStar;
	}

	public void StartCoinCollectAnimation(int numberOfCoins) {
		this.numberOfCoins = numberOfCoins;
		coinsTextAnimator.SetNumber (numberOfCoins);
	}

	public void StartTimeTakenAnimation(int timeTaken) {
		this.timeTaken = timeTaken;
		timeTakenTextAnimator.SetNumber (timeTaken);
	}

	public void StartLivesLostAnimation(int numberOfLivesLost) {
		this.numberOfLivesLost = numberOfLivesLost;
		livesLostTextAnimator.initialNumber = 999;
		livesLostTextAnimator.currentNumber = 999;
		livesLostTextAnimator.SetNumber (numberOfLivesLost);
	}

	private void InitialiseTextAnimator(TextFieldNumberAnimator textAnimator) {
		textAnimator.initialNumber = 0.0f;
		textAnimator.currentNumber = 0.0f;
		textAnimator.desiredNumber = 0.0f;
	}

	private void AnimateCoinsCollectedStar() {
		if (numberOfCoins == NUMBER_OF_COINS_GOAL) {
			AnimateStar (starCoinsCollectedImage);
		}
	}

	private void AnimateTimeTakenStar() {
		if (timeTaken < TIME_TAKEN_GOAL) {
			AnimateStar (starTimeTaken);
		}
	}

	private void AnimateLivesLostStar() {
		if (numberOfLivesLost == NUMBER_OF_LIVES_LOST_GOAL) {
			AnimateStar (starLivesLost);
			AnimateLevelCompleteScreen ();
		}
		EnableButtonPanel ();
	}

	private void AnimateStar(Image starImage) {
		starImage.gameObject.SetActive (true);
		Vector3 positionOfStar = starImage.rectTransform.TransformPoint (Vector3.zero);
		Instantiate (starBurstParticleSystem, positionOfStar, Quaternion.identity);
		whiteFlash.StartFadingIn ();
	}

	private void EnableButtonPanel() {
		buttonsPanel.SetActive (true);
	}

	private void AnimateLevelCompleteScreen() {
		if (starCoinsCollectedImage.IsActive () && starTimeTaken.IsActive () 
			&& starLivesLost.IsActive ()) {

			spinningStars.SetActive (true);
			spinningLines.gameObject.SetActive (true);

			sparkles.SetActive(true);

			// enable confetti
		}
	}
}
