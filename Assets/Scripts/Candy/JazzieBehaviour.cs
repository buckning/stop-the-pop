using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JazzieBehaviour : MonoBehaviour {

	HudListener hud;
	public SpriteRenderer myRenderer;
	bool wasVisible = false;

	void Start () {
		hud = GameObject.Find ("LevelHUD").GetComponent<HudListener> ();
	}

	void Update () {
		if (myRenderer.isVisible && !wasVisible) {
			AudioManager.PlaySound("Jazzie-Fall", Random.Range(0.8f, 1.2f));
			wasVisible = true;
		}
	}

	void OnCollisionEnter2D(Collision2D otherObject) {
		if (myRenderer.isVisible) {
			if (otherObject.gameObject.tag == Strings.PLAYER) {
				PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
				AnalyticsManager.SendDeathEvent (player.inputManager.levelName, player.transform.position, gameObject.name);
				AudioManager.PlaySound ("Jazzie-Hit");
				player.AddLifeLost ();
				player.inputManager.RetryLevel ();
			}

			hud.ShakeForDuration (0.2f);
		}
		Destroy (gameObject);
	}
}
