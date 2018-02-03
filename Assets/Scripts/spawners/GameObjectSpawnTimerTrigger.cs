using UnityEngine;
using System.Collections;

/***
 * Trigger the generation of game objects at a recurring time
 */
public class GameObjectSpawnTimerTrigger : MonoBehaviour {

	public float spawnInterval = 1.0f;		//the amount of time between objects getting spawned
	public GameObject spawnObject;			//the game object that will be spawned
	public float initialSpawnDelay = 1f;	//the delay from which the spawn cycle starts
	public float yOffset = 0.0f;			//the offset from the current position from which the object will be spawned
	/// <summary>
	/// internal counter to monitor the time since the last spawn of an object. 
	/// When this reaches 0.0f, a new object is spawned and this is reset to spawnInterval
	/// </summary>
	private float timeSinceLastSpawn = 0.0f;	

	void Start() {
		timeSinceLastSpawn = initialSpawnDelay;
	}

	void Update () {
		timeSinceLastSpawn -= Time.deltaTime;

		if (timeSinceLastSpawn <= 0.0f) {
			timeSinceLastSpawn = spawnInterval;
			Vector2 spawnPosition = new Vector2(transform.position.x, transform.position.y + yOffset);
			GameObject spawnedObject = (GameObject)Instantiate (spawnObject, spawnPosition, Quaternion.identity);
			ObjectSpawned(spawnedObject);
		}
	}

	/***
	 * 	Callback method for child classes. This gets called once an object has been spawned. 
	 */
	protected virtual void ObjectSpawned(GameObject spawnedObject) {

	}
}
