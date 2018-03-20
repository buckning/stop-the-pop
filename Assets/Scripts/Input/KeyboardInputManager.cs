using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputManager : MonoBehaviour, InputManager {

	private bool jumpKeyPressed;
	private bool jumpKeyReleased;

	private bool attackKeyPressed;
	private bool attackKeyReleased;

	private bool escKeyPressed;

	private Vector2 directionalInput;

	void Start () {
		directionalInput = new Vector2 ();
		jumpKeyPressed = false;
		jumpKeyReleased = false;
		attackKeyPressed = false;
		attackKeyReleased = false;
	}

	void Update () {
		directionalInput = new Vector2 ();
		jumpKeyPressed = false;
		jumpKeyReleased = false;
		attackKeyPressed = false;
		escKeyPressed = false;

		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown(KeyCode.Joystick1Button9)) {
			escKeyPressed = true;
		}

		if (!enabled) {
			return;
		}

		if(Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1)) {
			jumpKeyPressed = true;
		}
		if (Input.GetKeyUp (KeyCode.Space) || Input.GetKeyUp(KeyCode.Joystick1Button1)) {
			jumpKeyReleased = true;
		}

		if(Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Joystick1Button0)) {
			attackKeyPressed = true;
		}
		if(Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.Joystick1Button0)) {
			attackKeyReleased = true;
		}

		directionalInput.x = Input.GetAxisRaw ("Horizontal");
		directionalInput.y = Input.GetAxisRaw ("Vertical");
	}

	public void Enable() {
		enabled = true;
	}

	public void Disable() {
		enabled = false;
	}

	public bool BackButtonPressed() {
		return escKeyPressed;
	}

	public bool JumpKeyDown() {
		return jumpKeyPressed;
	}

	public bool JumpKeyUp() {
		return jumpKeyReleased;
	}

	public bool AttackKeyPressed() {
		return attackKeyPressed;
	}

	public bool AttackKeyReleased() {
		return attackKeyReleased;
	}

	public float GetXAxis() {
		return directionalInput.x;
	}
}
