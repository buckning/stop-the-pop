using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	private UiManager uiManager;
	private PopcornKernelController player;

	private InputManager inputManager;
	private LevelManager levelManager;

	void Start () {
		levelManager = GameObject.FindObjectOfType<LevelManager> ();
		uiManager = GameObject.FindObjectOfType<UiManager> ();

		if (levelManager == null) {
			Debug.LogError ("Level manager is null");
		}

		if (IsMobile ()) {
			inputManager = InitialiseSoftKeyInputManager ();
		} else {
			inputManager = gameObject.AddComponent<KeyboardInputManager> ();
		}
		InitialisePlayer ();

		uiManager.hud.fadeOutCompletedListeners += RestartLevel;
		uiManager.restartLevelButtonPressedListeners += RestartLevel;
		uiManager.levelCompletePanel.nextLevelButtonListeners += NextLevel;
		uiManager.hud.pauseButtonPressedListeners += Pause;
		levelManager.levelCompleteListeners += LevelComplete;
		uiManager.pausePanel.continueButtonListeners += Unpause;
	}

	private void Pause() {
		Time.timeScale = 0.0f;
	}

	private void Unpause() {
		Time.timeScale = 1.0f;
	}

	private void NextLevel() {
		uiManager.ShowLoadingPanel ();
		SceneManager.LoadScene (levelManager.GetNextLevel ());
	}

	private void LevelComplete() {
		DisablePlayerInput ();
		uiManager.LevelComplete (levelManager.GetCoinCount (), 
			levelManager.GetLengthOfTimeInLevel (), 
			levelManager.GetLivesLost ());
	}

	private void RestartLevel() {
		uiManager.hud.FadeIn ();
		Destroy (player.gameObject);
		InitialisePlayer ();
		levelManager.ResetLevel ();
	}

	private void Update () {
		uiManager.SetTemperature ((int) player.GetTemperature ());
		uiManager.hud.UpdateCoinCount (levelManager.GetCoinCount ());
	}

	private SoftKeyInputManager InitialiseSoftKeyInputManager() {
		SoftKeyInputManager softKeyInputManager = gameObject.AddComponent<SoftKeyInputManager> ();
		softKeyInputManager.jumpButton = uiManager.hud.playerControlPanel.jumpButton;
		softKeyInputManager.leftDirectionButton = uiManager.hud.playerControlPanel.leftButton;
		softKeyInputManager.rightDirectionButton = uiManager.hud.playerControlPanel.rightButton;
		softKeyInputManager.attackButton = uiManager.hud.playerControlPanel.attackButton;
		softKeyInputManager.SetUp ();
		return softKeyInputManager;
	}

	private void InitialisePlayer() {
		GameObject loadedPopcornKernel = (GameObject) Resources.Load ("Prefabs/PopcornKernelController", typeof(GameObject));
		GameObject instance = Instantiate(loadedPopcornKernel, levelManager.GetPlayerDropPoint()) as GameObject;
		player = (PopcornKernelController) instance.GetComponent<PopcornKernelController> ();
		player.transform.parent = null;

		player.inputManager = inputManager;
		player.Init ();
		inputManager.Enable ();
		player.popcornKernelHurtListeners += uiManager.hud.TriggerDamageIndicator;
		player.popcornKernelHurtListeners += uiManager.hud.ShakeScreen;
		player.popcornKernelHealListeners += uiManager.hud.TriggerFlash;
		player.popcornKernelRestartListeners += uiManager.hud.FadeOut;
		player.popcornKernelStartPoppingListeners += uiManager.ShowRetryButton;
		player.popcornKernelStartPoppingListeners += DisablePlayerInput;
		player.popcornKernelStartPoppingListeners += levelManager.IncrementLivesLost;
		player.popcornKernelInstantDeathListeners += InstantDeath;
	}

	private void InstantDeath() {
		levelManager.IncrementLivesLost();
		uiManager.hud.FadeOut ();
	}

	private void DisablePlayerInput() {
		player.inputManager.Disable ();
	}

	private bool IsMobile() {
		return Application.platform == RuntimePlatform.Android 
			|| Application.platform == RuntimePlatform.IPhonePlayer;
	}
}
