using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleSpawner : MonoBehaviour {

	public GameObject objectToSpawn;
	public bool randomRotation = false;
	public bool useRotation = false;
	public float spawnedRotation = 0.0f;
	List<GameObject> spawnedObjects = new List<GameObject>();

	public void Reset() {
		foreach (GameObject spawnedObject in spawnedObjects) {
			Destroy (spawnedObject);
		}
		spawnedObjects.Clear ();
	}

	public void Spawn() {
		GameObject spawnedObject = (GameObject)Instantiate (objectToSpawn, transform.position, Quaternion.identity);
		if (randomRotation) {
			float rotation = Random.value * 360;
			spawnedObject.transform.localEulerAngles = new Vector3 (0, 0, rotation);
		} else if (useRotation) {
			spawnedObject.transform.localEulerAngles = new Vector3 (0, 0, spawnedRotation);
		}
		spawnedObjects.Add (spawnedObject);
		spawnedObject.transform.parent = gameObject.transform;
	}
}
