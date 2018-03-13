using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCountPanel : MonoBehaviour {
	public Text coinCountText;
	public float coinCountTextScaleMax = 2f;
	public int coinCount = 0;

	public void SetCoinCount(int coinCount) {
		if (this.coinCount != coinCount) {
			this.coinCount = coinCount;
			coinCountText.gameObject.transform.localScale = Vector3.one * coinCountTextScaleMax;
		}
	}

	void Update () {
		//don't want to run the lerp on every frame for performance reasons, so adding this if statement for protection
		if (coinCountText.gameObject.transform.localScale.x > 1.1f) {
			
			coinCountText.gameObject.transform.localScale = 
				Vector3.Lerp (coinCountText.gameObject.transform.localScale, Vector3.one, Time.deltaTime * 5f);
		}
		coinCountText.text = coinCount.ToString();
	}
}
