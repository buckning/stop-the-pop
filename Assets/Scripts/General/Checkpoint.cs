using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour {
	private LevelManager levelManager;

	public void SetLevelManager(LevelManager levelManager) {
		this.levelManager = levelManager;
	}

	public void OnTriggerEnter2D(Collider2D coll) {
		if(coll.gameObject.tag == "Player") {
			levelManager.SetCheckpoint (this);
			gameObject.SetActive(false);
		}
	}
}
