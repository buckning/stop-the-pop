using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PopcornKernelAnimator))]
public class PopcornKernelAnimatorInGameControl : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		if (GUILayout.Button ("Kick")) {
			PopcornKernelAnimator kernel = (PopcornKernelAnimator)target;
			kernel.Kick ();
		}

		if (GUILayout.Button ("Pop")) {
			PopcornKernelAnimator kernel = (PopcornKernelAnimator)target;
			kernel.StartPopping ();
		}

		if (GUILayout.Button ("Run")) {
			PopcornKernelAnimator kernel = (PopcornKernelAnimator)target;
			kernel.SetVelocityX (1f);
		}

		if (GUILayout.Button ("Stop")) {
			PopcornKernelAnimator kernel = (PopcornKernelAnimator)target;
			kernel.SetVelocityX (0f);
		}

		if (GUILayout.Button ("Jump")) {
			PopcornKernelAnimator kernel = (PopcornKernelAnimator)target;
			kernel.SetVelocityY (10f);
			kernel.SetGrounded (false);
			kernel.Jump ();
		}

		if (GUILayout.Button ("Land")) {
			PopcornKernelAnimator kernel = (PopcornKernelAnimator)target;
			kernel.SetVelocityY (0f);
			kernel.SetGrounded (true);
			kernel.Land ();
		}

		if (GUILayout.Button ("Fall")) {
			PopcornKernelAnimator kernel = (PopcornKernelAnimator)target;
			kernel.SetVelocityY (-10f);
			kernel.SetGrounded (false);
		}
	}
}
