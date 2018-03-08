using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InputManager {
	bool JumpKeyDown ();
	bool JumpKeyUp ();
	bool AttackKeyPressed();
	bool BackButtonPressed();
	float GetXAxis ();
}
