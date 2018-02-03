using UnityEngine;
using System.Collections;

public class MarshmallowBehaviour : MonoBehaviour {
	private Animator animator;

	public Vector2 reboundForce;

	public SpriteRenderer bodySprite;

	float timeSinceLastBlink = 53f;

	void Start () {
		animator = GetComponent<Animator> ();

		timeSinceLastBlink = Random.Range (0, 3f);
	}

	void Update() {
		timeSinceLastBlink += Time.deltaTime;

		if (timeSinceLastBlink > 3f) {
			animator.SetTrigger("Blink");
			timeSinceLastBlink = 0f;

			if (bodySprite.isVisible) {
				PlayBlinkSound ();
			}
		}
	}

	void PlayBlinkSound() {
		AudioManager.PlaySound("blink");
	}

	void OnTriggerEnter2D(Collider2D coll) {
		if(coll.gameObject.tag == "Player") {
			PlayerController player = coll.gameObject.GetComponent<PlayerController> ();
			if(player.GetVelocity().y < -3f) {
				player.SetVelocity(reboundForce);
				animator.SetTrigger("Bounce");
				AudioManager.PlaySound("boing");
				AudioManager.PlaySound("marshmallowHit");
				timeSinceLastBlink = 0f;
			}
		}
	}
}
