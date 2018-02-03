using UnityEngine;
using System.Collections;

/***
 * Spawn a game object at a fixed interval.
 * The object will get spawned from the position of the Spawner object. 
 */
public class Spawner : MonoBehaviour {

	/***
	 * The object that will be repeatedly spawned. 
	 */
	public GameObject spawnObject;
	
	/***
	 * The initial start time of the first spawn
	 */
	public float startTime = 0f;

	/***
	 * The repeat interval of the spawning. 
	 */
	public float interval = 1f;

	/***
	 * The x axis offset to spawn objects from the spawners current position
	 */
	public float xOffset = 0f;

	/***
	 * The y axis offset to spawn objects from the spawners current position
	 */
	public float yOffset = 0f;

	/***
	 * The z axis offset to spawn objects from the spawners current position
	 */
	public float zOffset = 0f;

	public bool randomStartTime = false;

	public bool recurring = true;	//set to true to repeatedly spawn objects

	public int numberOfItemsToSpawn = 0;	//the number of items to spawn if recurring is false, so when the spawner is created

	public bool startSpawningWhenFirstSeen = false;

	public bool randomRotation = false;

	SpriteRenderer myRenderer;
	bool wasVisible = false;
	int numberOfSpawnedItems = 0;

	void Start () {
		if (randomStartTime) {
			startTime = Random.value * startTime;
		}

		if (recurring) {
			InvokeRepeating ("Spawn", startTime, interval);
		} else {
			if (!startSpawningWhenFirstSeen) {
				for (int i = 0; i < numberOfItemsToSpawn; i++) {
					Spawn ();
					numberOfSpawnedItems++;
				}
			}
		}

		if (startSpawningWhenFirstSeen) {
			myRenderer = GetComponent<SpriteRenderer> ();
		}
	}

	void Update() {
		if (!startSpawningWhenFirstSeen) {
			return;
		}
		if (myRenderer.isVisible) {
			wasVisible = true;
		}

		if (wasVisible) {
			while (numberOfSpawnedItems < numberOfItemsToSpawn) {
				Spawn ();
				numberOfSpawnedItems++;
			}
		}
	}

	/***
	 * Spawn a game object
	 */
	void Spawn () {
		if (gameObject.activeSelf) {
			//get a random value between -1 and 1
			float randomX = (Random.value - 0.5f) * 2;
			float randomY = (Random.value - 0.5f) * 2;
			float randomZ = (Random.value - 0.5f) * 2;

			//spawn the new game object from the Spawners current position +-xOffset and +-yOffset
			Vector3 spawnPosition = new Vector3 (transform.position.x + xOffset * randomX, 
				                       transform.position.y + yOffset * randomY,
				                       transform.position.z + zOffset * randomZ);

			GameObject spawnedObject = (GameObject)Instantiate (spawnObject, spawnPosition, spawnObject.transform.rotation);

			if (randomRotation) {
				float rotation = Random.value * 360;
				spawnedObject.transform.localEulerAngles = new Vector3 (0, 0, rotation);
			}
		}
	}

	public void Reset() {
		wasVisible = false;
		numberOfSpawnedItems = 0;
	}
}
