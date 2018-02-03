using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VideoErrorPanelLanguageChanger : MonoBehaviour {

	void OnEnable () {
		GetComponent<Text> ().text = Strings.UI_COULD_NOT_PLAY_THE_VIDEO;
	}
}
