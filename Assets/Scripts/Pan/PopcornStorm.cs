using UnityEngine;
using System.Collections;

public class PopcornStorm : MonoBehaviour {
	public PopcornBehaviour popcornTemplate;

	public float xOffset = 0.0f;
	public float yOffset = 0.0f;
	public float zOffset = 0.0f;
	public int numberOfPopcorns = 10;

	public bool recurring = false;
	public float timeBetweenStorms = 2.0f;

	public Vector2 popcornVelocity;
	bool wasTriggered = false;
	bool shakeCamera = false;
	HudListener hud;

	public float shakeDuration = 0.5f;
	public float shakeMagnitude = 0.4f;

	float cameraShakeDuration = 0.15f;
	 float cameraShakeMagnitude = 0.4f;

	public void Start() {
		hud = GameObject.Find ("LevelHUD").GetComponent<HudListener> ();
	}

	public void StartStorm() {
		if (wasTriggered) {
			return;
		}
		StartCoroutine (TriggerStorm ());
		wasTriggered = true;


	}

	IEnumerator TriggerStorm() {
		if (recurring) {
			while (true) {
				for (int i = 0; i < numberOfPopcorns; i++) {
					StartCoroutine (SpawnPopcorn());
				}
				yield return new WaitForSeconds(timeBetweenStorms);
			}
		}
	}

	IEnumerator SpawnPopcorn() {
		yield return new WaitForSeconds(Random.value);

		//only play the audio when the player is colliding with the collider
		if (shakeCamera) {
			hud.ShakeForDuration (cameraShakeDuration, cameraShakeMagnitude);
			string soundToPlay = "pop" + Random.Range (1, 14);	//this is hardcoded, not good practice and should be changed.
			AudioManager.PlaySound (soundToPlay);
		}

		//get a random value between -1 and 1
		float randomX = (Random.value - 0.5f) * 2;
		float randomY = (Random.value - 0.5f) * 2;
		float randomZ = (Random.value - 0.5f) * 2;

		//spawn the new game object from the Spawners current position +-xOffset and +-yOffset
		Vector3 spawnPosition = new Vector3 (transform.position.x + xOffset * randomX, 
			transform.position.y + yOffset * randomY,
			transform.position.z + zOffset * randomZ);


		PopcornBehaviour popcorn = (PopcornBehaviour)Instantiate (popcornTemplate, spawnPosition, Quaternion.Euler(transform.rotation.x, transform.rotation.y, (Random.value * 360)));
		Rigidbody2D popcornRigidbody = popcorn.gameObject.GetComponent<Rigidbody2D> ();
		popcornRigidbody.velocity = new Vector2(popcornVelocity.x, popcornVelocity.y);
	}



	void OnTriggerEnter2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			shakeCamera = true;
			StartStorm ();
		}
	}
	void OnTriggerExit2D(Collider2D otherObject) {
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			shakeCamera = false;
		}
	}
}
