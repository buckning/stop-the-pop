using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint : MonoBehaviour {
	private string levelName;

	void Start () {
		levelName = SceneManager.GetActiveScene().name;	
	}

	/*
	 * save the checkpoint if there has been a collision with the player, 
	 * then remove the checkpoint from the game
	 */
	public void OnTriggerEnter2D(Collider2D coll) {
		if(coll.gameObject.tag == "Player") {
			PlayerController player = coll.gameObject.GetComponent<PlayerController> ();
			List<int> collectedCoins = player.GetCollectedCoins();
			LastCheckpoint.checkpointName = levelName;
			LastCheckpoint.lastCheckpoint = transform.position;

			foreach(int coinId in collectedCoins) {
				LastCheckpoint.AddCollectedCoin(coinId);
			}

			gameObject.SetActive(false);
		}
	}
}
