using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PopcornKernelAnimator))]
public class PopcornKernelAnimatorInGameControl : Editor {
	int selectedOption = 0;

	private string selectedSkin = "Skins/Player/normal";

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		PopcornKernelAnimator kernel = (PopcornKernelAnimator)target;

		string[] skinOptions = new string[] {"normal", "leprechaun", "viking", "pirate", "santa", "elf"};
		selectedOption = EditorGUILayout.Popup("Costume", selectedOption, skinOptions);
		string newSkin = skinOptions[selectedOption];

		if (newSkin != selectedSkin) {
			selectedSkin = newSkin;
			kernel.CustomisePlayer (selectedSkin, selectedSkin, selectedSkin);
		}

		if (GUILayout.Button ("Kick")) {
			kernel.Kick ();
		}

		if (GUILayout.Button ("Pop")) {
			kernel.StartPopping ();
		}

		if (GUILayout.Button ("Run")) {
			kernel.SetVelocityX (2f);
		}

		if (GUILayout.Button ("Stop")) {
			kernel.SetVelocityX (0f);
		}

		if (GUILayout.Button ("Jump")) {
			kernel.SetVelocityY (10f);
			kernel.SetGrounded (false);
			kernel.Jump ();
		}

		if (GUILayout.Button ("Land")) {
			kernel.SetVelocityY (0f);
			kernel.SetGrounded (true);
			kernel.Land ();
		}

		if (GUILayout.Button ("Fall")) {
			kernel.SetVelocityY (-10f);
			kernel.SetGrounded (false);
		}

		if (GUILayout.Button ("Enable Cape")) {
			kernel.EnableCape (true);
		}

		if (GUILayout.Button ("Start Gliding")) {
			kernel.SetGliding (true);
		}

		if (GUILayout.Button ("Title Screen Animation")) {
			kernel.PlayTitleScreenAnimation ();
		}
	}
}
