using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class KernelBehaviour : SpriteColourChanger {

	public PopcornBehaviour poppedGameObject;	//the game object that will spawn when Pop() is called

	public bool randomPop = true;
	public bool popcornGetsCleanedUp = true;
	public Vector2 nonRandomPopVelocity = new Vector2 (150, 150);	//if random pop is false, use this velocity

	public float cameraShakeDuration = 0.1f;
	public float cameraShakeMagnitude = 0.1f;

	public bool causesHarmToPlayer = true;	//if this is set to true, then the resulting popcorn will hurt the player when it falls on them

	protected override void MaxColourReached() {
		GetComponent<Animator> ().SetTrigger ("Pop");
	}

	public void Pop() {
		float minX = 100f;
		float maxX = 200f;

		float minY = 300f;
		float maxY = 800f;
		float xForce = Random.Range (minX, maxX);
		float yForce = Random.Range (minY, maxY);

		string soundToPlay = "pop" + Random.Range(1, 14);	//this is hardcoded, not good practice and should be changed.
		AudioManager.PlaySound(soundToPlay);

		//have the kernel pop randomly either positively or negatively in the x-axis
		float dir = (Random.Range (0f, 1f) < 0.5f) ? -1f : 1f;
		xForce = xForce * dir;

		if (!randomPop) {
			xForce = nonRandomPopVelocity.x;
			yForce = nonRandomPopVelocity.y;
		}

		//delete this object and instantiate the Popped popcorn prefab
		PopcornBehaviour poppedObject = (PopcornBehaviour)Instantiate (poppedGameObject, gameObject.transform.position, Quaternion.identity);
		poppedObject.causesHarmToPlayer = causesHarmToPlayer;
		if (popcornGetsCleanedUp) {
			poppedObject.destroyAfterCollision = true;
		} else {
			poppedObject.destroyAfterCollision = false;
		}
		poppedObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (xForce, yForce));
		poppedObject.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (0f, 100f));
		Destroy (gameObject);
	}
}
