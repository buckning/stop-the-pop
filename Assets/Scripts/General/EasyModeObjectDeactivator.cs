using UnityEngine;
using System.Collections;

/***
 * Class that reads in the current difficulty level, if it is easy, it deactivates all the objects that are assigned to it
 */
public class EasyModeObjectDeactivator : MonoBehaviour {
	public GameObject[] objectsToDeactivate;

	bool initialized = false;

	void Update() {
		if (!initialized) {
			if (CurrentLevel.GetLevelDifficulty () == CurrentLevel.LevelDifficulty.EASY) {
				foreach (GameObject objectToDeactivate in objectsToDeactivate) {
					Destroy (objectToDeactivate);
				}
				initialized = true;
			}
		}

	}
}
