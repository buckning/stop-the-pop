using UnityEngine;
using System.Collections;

/***
 * This script changes the flame time to live. 
 * Spawning functionality is implemented in the GameObjectSpawTimerTrigger base class.
 */
public class FlameSpawner : GameObjectSpawnTimerTrigger {
	public float flameTimeToLive = 0.4f;

	protected override void ObjectSpawned(GameObject spawnedObject) {
		spawnedObject.GetComponent<GameObjectDestroyTimer> ().timeToDestruct = flameTimeToLive;	
	}
}
