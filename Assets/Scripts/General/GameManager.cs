using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public UiManager uiManager;
	private PopcornKernelController player;

	private InputManager inputManager;
	private LevelManager levelManager;

	void Start () {
		levelManager = GameObject.FindObjectOfType<LevelManager> ();

		if (levelManager == null || levelManager.playerDropPoint == null) {
			Debug.LogError ("Level manager or player drop point is null");
		}

		if (IsMobile ()) {
			inputManager = InitialiseSoftKeyInputManager ();
		} else {
			inputManager = gameObject.AddComponent<KeyboardInputManager> ();
		}
		InitialisePlayer ();

		uiManager.hud.fadeOutCompletedListeners += RestartLevel;
		uiManager.restartLevelButtonPressedListeners += RestartLevel;
	}

	void RestartLevel() {
		uiManager.hud.FadeIn ();
		Destroy (player.gameObject);
		InitialisePlayer ();
		levelManager.ResetLevel ();
	}

	void Update () {
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
		GameObject instance = Instantiate(loadedPopcornKernel, levelManager.playerDropPoint) as GameObject;
		player = (PopcornKernelController) instance.GetComponent<PopcornKernelController> ();

		player.inputManager = inputManager;
		player.Init ();
		inputManager.Enable ();
		player.popcornKernelHurtListeners += uiManager.hud.TriggerDamageIndicator;
		player.popcornKernelHurtListeners += uiManager.hud.ShakeScreen;
		player.popcornKernelHealListeners += uiManager.hud.TriggerFlash;
		player.popcornKernelRestartListeners += uiManager.hud.FadeOut;
		player.popcornKernelStartPoppingListeners += uiManager.ShowRetryButton;
		player.popcornKernelStartPoppingListeners += DisablePlayerInput;
	}

	private void DisablePlayerInput() {
		player.inputManager.Disable ();
	}

	private bool IsMobile() {
		return Application.platform == RuntimePlatform.Android 
			|| Application.platform == RuntimePlatform.IPhonePlayer;
	}
}
