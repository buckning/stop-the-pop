using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScreenFader))]
public class ScreenFaderEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		ScreenFader screenFader = (ScreenFader) target;

		if (GUILayout.Button ("Fade In")) {
			screenFader.StartFadingIn ();
		}

		if (GUILayout.Button ("Fade Out")) {
			screenFader.StartFadingOut ();
		}
	}
}
