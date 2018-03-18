using UnityEngine;
using System.Collections;

public class Sawblade : MonoBehaviour {

	public GameObject particles;
	public bool stationary = false;
	GameObject spawnedParticles;

	void OnCollisionEnter2D(Collision2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			PopcornKernelController player = otherObject.gameObject.GetComponent<PopcornKernelController> ();
			AudioManager.PlaySound ("saw");

			if (spawnedParticles == null) {
				spawnedParticles = (GameObject)Instantiate (particles);
				spawnedParticles.transform.position = otherObject.contacts [0].point;


				player.SetJumpEnabled (false);
				player.inputManager.Disable ();
				StartCoroutine(TriggerPlayerDeath(player, 0.3f));

				if (stationary) {
					PopcornKernelAnimator popcornKernelAnimator = GameObject.FindObjectOfType<PopcornKernelAnimator> ();
					popcornKernelAnimator.PopLeftLeg ();
					popcornKernelAnimator.PopRightLeg ();
					player.DisableCollider ();
//					player.EnableBodyCollider ();
					player.PlaySawBladeDeathAnimation ();
				} else {
					player.PlayMovingSawBladeDeathAnimation ();
				}
			}
		}
	}

	IEnumerator TriggerPlayerDeath(PopcornKernelController player, float delay) {
		yield return new WaitForSeconds (delay);
		player.InstantDeath ();
	}

	public void Reset() {
		Destroy (spawnedParticles);
		spawnedParticles = null;
	}
}
