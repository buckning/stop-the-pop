using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreenQuitLanguageChanger : MonoBehaviour {
	public Text quitDialogBoxText;

	void OnEnable () {
		quitDialogBoxText.text = Strings.UI_ARE_YOU_SURE_YOU_WANT_TO_QUIT;
	}
}
