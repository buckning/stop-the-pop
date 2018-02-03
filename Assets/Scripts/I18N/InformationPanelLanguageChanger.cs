using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InformationPanelLanguageChanger : MonoBehaviour {
	public Text gameByText;
	public Text thanksToText;
	public Text themeByText;
	public Text soundEffectsText;
	public Text logoDesignText;

	void OnEnable () {
		gameByText.text = Strings.UI_GAME_BY;
		themeByText.text = Strings.UI_THEME_BY;
		soundEffectsText.text = Strings.UI_SOUND_EFFECTS;
		logoDesignText.text = Strings.UI_LOGO_DESIGN;
		thanksToText.text = Strings.UI_THANKS_TO;
	}
}
