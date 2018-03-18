using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
	public Transform playerDropPoint;

	public delegate void NotifyEvent ();
	public event NotifyEvent levelCompleteListeners;

	private List<int> collectedCoinsSinceCheckpoint = new List<int> ();
	private List<int> collectedCoins = new List<int> ();

	private Checkpoint lastCheckpoint;
	private Checkpoint[] checkpoints;

	private LevelCompleteTrigger levelCompleteTrigger;

	private int livesLost = 0;
	private string nextLevelName;

	void Start() {
		levelCompleteTrigger = GameObject.Find("LevelCompleteTrigger").GetComponent<LevelCompleteTrigger>();
		levelCompleteTrigger.SetLevelManager (this);

		checkpoints = Resources.FindObjectsOfTypeAll (typeof(Checkpoint)) as Checkpoint[];
		foreach (Checkpoint checkpoint in checkpoints) {
			checkpoint.SetLevelManager (this);
		}

		CoinBehaviour[] coins = Resources.FindObjectsOfTypeAll(typeof(CoinBehaviour)) as CoinBehaviour[];
		for (int i = 0; i < coins.Length; i++) {
			coins [i].id = i;
			coins [i].SetLevelManager (this);
		}
	}

	public string GetNextLevel() {
		return nextLevelName;
	}

	public void LevelComplete(string nextLevel) {
		this.nextLevelName = nextLevel;
		if (levelCompleteListeners != null) {
			levelCompleteListeners ();
		}
	}

	public int GetLengthOfTimeInLevel() {
		return 90;
	}

	public int GetLivesLost() {
		return livesLost;
	}

	public void IncrementLivesLost() {
		livesLost++;
	}

	public void SetCheckpoint(Checkpoint checkpoint) {
		this.lastCheckpoint = checkpoint;
		foreach (int coinId in collectedCoinsSinceCheckpoint) {
			collectedCoins.Add (coinId);
		}
		collectedCoinsSinceCheckpoint.Clear ();
		playerDropPoint = checkpoint.transform;
	}

	public void AddCollectedCoin(int id) {
		collectedCoinsSinceCheckpoint.Add (id);
	}

	public int GetCoinCount() {
		return collectedCoinsSinceCheckpoint.Count + collectedCoins.Count;
	}

	public void ResetLevel() {
		SnowflakeBehaviour[] snowflakes = Resources.FindObjectsOfTypeAll (typeof(SnowflakeBehaviour)) as SnowflakeBehaviour[];
		foreach (SnowflakeBehaviour snowflake in snowflakes) {
			snowflake.Reset ();
			snowflake.gameObject.transform.parent.gameObject.SetActive (true);
		}

		CoinBehaviour[] coins = Resources.FindObjectsOfTypeAll(typeof(CoinBehaviour)) as CoinBehaviour[];
		foreach(CoinBehaviour coin in coins) {
			// don't want to re-enable coins that were already saved off in a checkpoint
			if (!collectedCoins.Contains(coin.id)) {
				coin.Reset ();
				coin.gameObject.transform.localScale = Vector3.one;
				coin.gameObject.SetActive (true);
			}
		}

		MagnetPowerup[] magnets = Resources.FindObjectsOfTypeAll(typeof(MagnetPowerup)) as MagnetPowerup[];
		foreach(MagnetPowerup magnet in magnets) {
			magnet.Reset ();
			magnet.gameObject.SetActive(true);
		}

		collectedCoinsSinceCheckpoint.Clear ();
	}
}
