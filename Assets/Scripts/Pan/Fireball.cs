using UnityEngine;
using System.Collections;

/***
 * Fireball that keeps moving in the x-direction until the player or collector has been hit
 */
public class Fireball : MonoBehaviour {

	public float speed = 10f;
	public float ySpeed = 0f;
	Rigidbody2D rigidbody2d;
	public SpriteRenderer myRenderer;
	public GameObject explosion;

	bool playedSpawnSfx = false;

	void Start () {
		rigidbody2d = GetComponent<Rigidbody2D> ();
	}

	void Update () {
		if (myRenderer.isVisible && !playedSpawnSfx) {
			AudioManager.PlaySound ("flame-spawn", Random.Range(1.0f, 1.3f));
			playedSpawnSfx = true;
		}

		rigidbody2d.velocity = new Vector2 (speed, ySpeed);
	}

	/***
	 * If there is a collision with the player, increase the temperature
	 * and delete this object
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			PopcornKernelController player = otherObject.gameObject.GetComponent<PopcornKernelController> ();

			player.CollisionWithEnemy (50);
			Instantiate (explosion, transform.position, Quaternion.identity);
			AudioManager.PlaySound ("flame-destroy", Random.Range(0.8f, 1.2f));

			speed = 0;
			ySpeed = 0f;
			Destroy(gameObject);
		}
		else if (otherObject.gameObject.tag == Strings.COLLECTOR) {
			Instantiate (explosion, transform.position, Quaternion.identity);
			if (myRenderer.isVisible) {
				AudioManager.PlaySound ("flame-destroy", Random.Range(0.8f, 1.2f));
			}
			speed = 0;
			ySpeed = 0f;
			Destroy(gameObject);
		}
			
	}
}

