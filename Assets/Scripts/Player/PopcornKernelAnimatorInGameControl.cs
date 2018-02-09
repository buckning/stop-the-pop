using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PopcornKernelAnimator))]
public class PopcornKernelAnimatorInGameControl : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		PopcornKernelAnimator kernel = (PopcornKernelAnimator)target;

		if (GUILayout.Button ("Kick")) {
			kernel.Kick ();
		}

		if (GUILayout.Button ("Pop")) {
			kernel.StartPopping ();
		}

		if (GUILayout.Button ("Run")) {
			kernel.SetVelocityX (1f);
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
	}
}
