using UnityEngine;
using System.Collections;

public class Sawblade : MonoBehaviour {

	public GameObject particles;
	public bool stationary = false;
	GameObject spawnedParticles;

	void OnCollisionEnter2D(Collision2D otherObject) {
		CameraFollow camera = GameObject.Find ("Main Camera").GetComponent<CameraFollow>();
		if (otherObject.gameObject.tag == Strings.PLAYER && 
			!camera.gameOver) {	//this check of gameOver stops two objects from triggering the death animation again
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			AudioManager.PlaySound ("saw");
			AnalyticsManager.SendDeathEvent (player.inputManager.levelName, player.transform.position, gameObject.name);
			player.AddLifeLost ();

			if (spawnedParticles == null) {
				spawnedParticles = (GameObject)Instantiate (particles);
				spawnedParticles.transform.position = otherObject.contacts [0].point;
				camera.gameOver = true;	//this is used to start zooming in on the player
				player.inputManager.GameOver(false);	//don't show the retry button for this death animation
				player.playerMovementEnabled = false;

				float shakeDuration = .2f;
				if (stationary) {
					shakeDuration = 1.5f;
					player.PopLeftLegSprite ();
					player.PopRightLegSprite ();
					player.DisableCollider ();
					player.EnableBodyCollider ();
					player.PlaySawBladeDeathAnimation ();
					player.jumpEnabled = false;
				} else {
					player.jumpEnabled = false;
					player.PlayMovingSawBladeDeathAnimation ();
				}

				player.inputManager.ShakeForDuration (shakeDuration);
				StartCoroutine (RestartAfterDelay (player, shakeDuration));	
			}
		}
	}

	IEnumerator RestartAfterDelay(PlayerController player, float delay) {
		yield return new WaitForSeconds (delay);
		player.inputManager.RetryLevel ();
	}

	public void Reset() {
		Destroy (spawnedParticles);
		spawnedParticles = null;
	}
}
