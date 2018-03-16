using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public UiManager uiManager;
	public PopcornKernelController popcornKernel;

	private InputManager inputManager;

	// Use this for initialization
	void Start () {
		if (IsMobile ()) {
			inputManager = InitialiseSoftKeyInputManager ();
		} else {
			inputManager = gameObject.AddComponent<KeyboardInputManager> ();
		}
		popcornKernel.inputManager = inputManager;
		popcornKernel.Init ();

		popcornKernel.popcornKernelHurtListeners += uiManager.hud.TriggerDamageIndicator;
		popcornKernel.popcornKernelHealListeners += uiManager.hud.TriggerFlash;
	}
	
	// Update is called once per frame
	void Update () {
		uiManager.SetTemperature ((int) popcornKernel.GetTemperature ());
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

	private bool IsMobile() {
		return Application.platform == RuntimePlatform.Android 
			|| Application.platform == RuntimePlatform.IPhonePlayer;
	}
}
