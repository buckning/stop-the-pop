using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KernelSpawner : MonoBehaviour {

	public KernelBehaviour objectToSpawn;
	public bool randomRotation = false;
	public float timeToPop = 1f;
	List<KernelBehaviour> spawnedObjects = new List<KernelBehaviour>();

	public void Reset() {
		foreach (KernelBehaviour spawnedObject in spawnedObjects) {
			Destroy (spawnedObject);
		}
		spawnedObjects.Clear ();
	}

	public void Spawn() {
		KernelBehaviour spawnedObject = (KernelBehaviour)Instantiate (objectToSpawn, transform.position, Quaternion.identity);
		if (randomRotation) {
			spawnedObject.maxTimeInIncrease = timeToPop;
			float rotation = Random.value * 360;
			spawnedObject.transform.localEulerAngles = new Vector3 (0, 0, rotation);
		}
		spawnedObjects.Add (spawnedObject);
		spawnedObject.transform.parent = gameObject.transform;
	}
}
