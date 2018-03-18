using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent (typeof (BoxCollider2D))]
public class LevelCompleteTrigger : MonoBehaviour {
	public string nextLevelName;

	bool alreadyTriggered = false;

	private LevelManager levelManager;

	public void SetLevelManager(LevelManager levelManager) {
		this.levelManager = levelManager;
	}

	void OnTriggerEnter2D(Collider2D otherObject) {
		// seen some bugs here since the player can have multiple 
		// colliders attached, causing multiple triggers
		if (alreadyTriggered) {
			return;
		}

		if (otherObject.gameObject.tag == Strings.PLAYER) {
			levelManager.LevelComplete (nextLevelName);
			alreadyTriggered = true;
		}
	}

	public void Reset() {
		alreadyTriggered = false;
	}
}
