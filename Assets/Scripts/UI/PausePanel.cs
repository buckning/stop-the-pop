using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour {

	public delegate void NotifyEvent ();
	public event NotifyEvent continueButtonListeners;
	public event NotifyEvent restartButtonListeners;
	public event NotifyEvent storeButtonListeners;
	public event NotifyEvent skipLevelButtonListeners;
	public event NotifyEvent mainMenuButtonListeners;

	public Button[] continueButtons;
	public Button[] restartButtons;
	public Button[] storeButtons;
	public Button[] skipLevelButtons;
	public Button[] mainMenuButtons;

	void Start () {
		SetUpButtonListeners (continueButtons, continueButtonListeners);
		SetUpButtonListeners (restartButtons, restartButtonListeners);
		SetUpButtonListeners (storeButtons, storeButtonListeners);
		SetUpButtonListeners (skipLevelButtons, skipLevelButtonListeners);
		SetUpButtonListeners (mainMenuButtons, mainMenuButtonListeners);
	}

	private void SetUpButtonListeners(Button[] buttons, NotifyEvent listener) {
		foreach (Button button in buttons) {
			button.onClick.AddListener (() => {
				if(listener != null) {
					listener();
				}
			});
		}
	}
}
