using UnityEngine;
using System.Collections;

public class SaltShaker : Breakable  {

	public float spitFrequency = 4.0f;
	public GameObject saltProjectile;
	public GameObject mouth;
	public Vector2 projectileForce;
	public GameObject destroyAnimation;
	public SpriteRenderer spriteRenderer;

	Animator anim;
	float timeToSpit;

	bool dying = false;

	Rigidbody2D salt;	//we cache the rigidbody2d. When we need to spawn it again, we just re-enable it and set the position (for performance)
	private Vector2 reboundVector = new Vector2(0f, 12f);	//force applied to the player when he interacts with this object
	private Vector2 collisionVector = new Vector2(30f, 8f);	//force applied to the player when he gets hit with this object

	void Start () {
		anim = GetComponent<Animator> ();
		timeToSpit = spitFrequency;
		salt = ((GameObject)Instantiate (saltProjectile, mouth.transform.position, Quaternion.identity)).GetComponent<Rigidbody2D> ();
		salt.gameObject.SetActive (false);
	}
	
	void Update () {
		timeToSpit -= Time.deltaTime;

		if (timeToSpit <= 0.0f) {
			Spit ();
			timeToSpit = spitFrequency;
		}
	}

	void Spit() {
		anim.SetTrigger ("Spit");
	}

	/***
	 * This is called back by the animator when it is time to spawn the salt
	 */
	public void SpawnSaltProjectile() {
		salt.gameObject.transform.position = mouth.transform.position;
		salt.gameObject.GetComponent<SaltProjectile> ().Reset ();
		salt.gameObject.SetActive (true);
		salt.velocity = Vector2.zero;
		salt.AddForce (projectileForce);
		if (spriteRenderer.isVisible) {
			AudioManager.PlaySound ("salt-shaker-spit", 0.75f);
		}
	}

	void OnTriggerEnter2D(Collider2D otherObject) {
		if (dying) {
			return;
		}
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			if (player.GetVelocity ().y <= 0.0f) {
				DestroyMyself (player);
			}
		}
	}

	public override void Break(Vector3 positionOfOriginator) {
		dying = true;
		AudioManager.PlaySound ("wall-break-new");
		AudioManager.PlaySound ("salt-shaker-explode", Random.Range (1.5f, 1.75f));
		anim.SetTrigger ("Die");
		Instantiate (destroyAnimation, transform.position, Quaternion.identity);

	}

	void OnCollisionEnter2D(Collision2D otherObject) {
		if (dying) {
			return;
		}
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			if (ShouldPlayerDestroyMe (player)) {
				DestroyMyself (player);
			} else {
				int direction = (player.transform.position.x < transform.position.x) ? -1 : 1;
				player.SetVelocity (new Vector2 (collisionVector.x * direction, collisionVector.y));

				player.CollisionWithEnemy (gameObject.name);
			}
		}
	}

	public bool ShouldPlayerDestroyMe(PlayerController player) {
		//the purpose of this method is for cases when the player lands on a small area where the collider is larger than the
		//trigger. Without this, the player would continuously bounce on the one spot since the collision event would jump the player
		//up and the player would not think that he is grounded so it would jump forever.
		if (player.groundCheck[0].transform.position.y >= transform.position.y) {
			if (player.GetVelocity ().y <= 1.0f) {	//this value of 1.0 seems to give the best experience
				return true;
			}
		}

		return false;
	}

	public void SpawnDustAndDestroy() {
		Destroy (gameObject);
	}

	void DestroyMyself(PlayerController player) {
		player.SetVelocity (new Vector2 (player.GetVelocity ().x, reboundVector.y));
		dying = true;
		AudioManager.PlaySound ("salt-shaker-explode");
		player.hud.ShakeForDuration (0.2f);
		anim.SetTrigger ("Die");
		Instantiate (destroyAnimation, transform.position, Quaternion.identity);
	}
}
