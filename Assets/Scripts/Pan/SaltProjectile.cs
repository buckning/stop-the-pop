using UnityEngine;
using System.Collections;

[RequireComponent (typeof (TrailRenderer))]
public class SaltProjectile : MonoBehaviour {

	TrailRenderer trailRenderer;
	SpriteRenderer spriteRenderer;

	public GameObject collisionDust;

	public float damageToPlayer = 50f;

	void Start() {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	/***
	 * If there is a collision with the player, increase the temperature
	 * and delete this object
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			PopcornKernelController player = otherObject.gameObject.GetComponent<PopcornKernelController> ();
			player.CollisionWithEnemy(damageToPlayer);
		}
		if (otherObject.gameObject.tag == Strings.PLAYER || otherObject.gameObject.tag == Strings.TERRAIN) {
			if (spriteRenderer.isVisible) {
				AudioManager.PlaySound ("salt-collision", 1.4f);
			}
			gameObject.SetActive (false);

			Instantiate (collisionDust, transform.position, Quaternion.identity);
		}
	}

	public void Reset() {
		if (trailRenderer == null) {
			trailRenderer = GetComponent<TrailRenderer> ();
			trailRenderer.sortingLayerName = "Foreground";
		}
		trailRenderer.Clear ();
	}
}
