using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
	public Transform playerDropPoint;

	private List<int> collectedCoins = new List<int>();

	// checkpoints

	void Start() {
		CoinBehaviour[] coins = Resources.FindObjectsOfTypeAll(typeof(CoinBehaviour)) as CoinBehaviour[];
		for (int i = 0; i < coins.Length; i++) {
			coins [i].id = i;
			coins [i].SetLevelManager (this);
		}
	}

	public void AddCollectedCoin(int id) {
		collectedCoins.Add (id);
	}

	public int GetCoinCount() {
		return collectedCoins.Count;
	}

	public void ResetLevel() {
		SnowflakeBehaviour[] snowflakes = Resources.FindObjectsOfTypeAll (typeof(SnowflakeBehaviour)) as SnowflakeBehaviour[];
		foreach (SnowflakeBehaviour snowflake in snowflakes) {
			snowflake.Reset ();
			snowflake.gameObject.transform.parent.gameObject.SetActive (true);
		}

		CoinBehaviour[] coins = Resources.FindObjectsOfTypeAll(typeof(CoinBehaviour)) as CoinBehaviour[];
		foreach(CoinBehaviour coin in coins) {
			coin.Reset ();
			coin.gameObject.transform.localScale = Vector3.one;
			coin.gameObject.SetActive(true);
		}

		collectedCoins.Clear ();
	}
}
