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

	public Button nextLevelButton;
	public Button restartLevelButton;
	public Button quitButton;

	public delegate void NotifyEvent ();
	public event NotifyEvent quitButtonListeners;
	public event NotifyEvent restartButtonListeners;
	public event NotifyEvent nextLevelButtonListeners;

	private int numberOfCoins = 0;
	private int numberOfLivesLost = 0;
	private int timeTaken = 0;

	private const int NUMBER_OF_COINS_GOAL = 100;
	private const int NUMBER_OF_LIVES_LOST_GOAL = 0;
	private const int TIME_TAKEN_GOAL = 120;

	private bool initialised = false;

	void Start() {
		Initialise ();
	}

	// this initialisation needs to be in a seperate method to the Start method since the 
	// ShowLevelCompleteScreen can fire inside the same update loop, resulting in the 
	// animations not running.
	private void Initialise() {
		if (initialised) {
			return;
		}

		SetUpButtonListeners (quitButton, quitButtonListeners);
		SetUpButtonListeners (restartLevelButton, restartButtonListeners);
		SetUpButtonListeners (nextLevelButton, nextLevelButtonListeners);

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
		initialised = true;
	}

	public void ShowLevelCompleteScreen(int numberOfCoins, int timeTaken, int numberOfLivesLost) {
		Initialise ();
		this.numberOfCoins = numberOfCoins;
		this.numberOfLivesLost = numberOfLivesLost;
		this.timeTaken = timeTaken;

		coinsTextAnimator.SetNumber (numberOfCoins);

		coinsTextAnimator.animationCompleteListeners += StartTimeTakeAnimation;
		timeTakenTextAnimator.animationCompleteListeners += StartLivesLostAnimation;
	}

	private void StartTimeTakeAnimation() {
		timeTakenTextAnimator.SetNumber (timeTaken);
	}

	private void StartLivesLostAnimation() {
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
		buttonsPanel.SetActive (true);
	}

	private void AnimateStar(Image starImage) {
		starImage.gameObject.SetActive (true);
		Vector3 positionOfStar = starImage.rectTransform.TransformPoint (Vector3.zero);
		Instantiate (starBurstParticleSystem, positionOfStar, Quaternion.identity);
		whiteFlash.StartFadingIn ();
	}

	private void AnimateLevelCompleteScreen() {
		if (starCoinsCollectedImage.IsActive () && starTimeTaken.IsActive () 
			&& starLivesLost.IsActive ()) {

			spinningStars.SetActive (true);
			spinningLines.gameObject.SetActive (true);

			sparkles.SetActive(true);
		}
	}

	private void SetUpButtonListeners(Button button, NotifyEvent listener) {
		button.onClick.AddListener (() => {
			if (listener != null) {
				listener ();
			}
		});
	}
}
