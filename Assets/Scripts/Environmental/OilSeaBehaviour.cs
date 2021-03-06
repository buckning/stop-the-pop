﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class OilSeaBehaviour : MonoBehaviour {
	
	public GameObject splash;
	HudListener hud;

	Renderer myRenderer;				//the renderer - this is used to see if the sprite is visible, if it is, it does more calculations

	void Start() {
		myRenderer = GetComponent<Renderer> ();

		if (hud == null) {
			hud = GameObject.Find (HudListener.gameObjectName).GetComponent<HudListener>();
		}
	}

	void OnTriggerEnter2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			AudioManager.PlaySound ("splash");
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.AddLifeLost ();
			AnalyticsManager.SendDeathEvent (hud.levelName, player.transform.position, gameObject.name);
			otherObject.gameObject.SetActive (false);
			Instantiate (splash, otherObject.transform.position, Quaternion.identity);
			hud.RetryLevel ();
		} else if (otherObject.gameObject.name.StartsWith (Strings.BISCUIT_PLATFORM)) {
			AudioManager.PlaySound ("splash");
			otherObject.gameObject.SetActive (false);

			Vector2 splashPosition = new Vector2 (otherObject.transform.position.x, otherObject.transform.position.y - 1.5f);
			GameObject splashGameObject = (GameObject)Instantiate (splash, splashPosition, Quaternion.identity);
			splashGameObject.transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);
		} else if (otherObject.gameObject.name.StartsWith ("Jazzie")) {
			if (myRenderer.isVisible) {
				AudioManager.PlaySound ("splash", Random.Range(0.7f, 1.4f));
				Vector2 splashPosition = new Vector2 (otherObject.transform.position.x, otherObject.transform.position.y - 1.5f);
				GameObject splashGameObject = (GameObject)Instantiate (splash, splashPosition, Quaternion.identity);
				splashGameObject.transform.localScale = new Vector3 (1.5f, 1.5f, 1.5f);
				hud.ShakeForDuration (0.2f);
			}
			Destroy (otherObject.gameObject);
		}
	}
}
