using UnityEngine;
using System.Collections;

public class EasyModeObjectActivator : MonoBehaviour {
	public GameObject[] objectsToActivate;

	bool initialized = false;

	void Update() {
		if (!initialized) {
			if (CurrentLevel.GetLevelDifficulty () == CurrentLevel.LevelDifficulty.EASY) {
				foreach (GameObject objectToDeactivate in objectsToActivate) {
					objectToDeactivate.SetActive (true);
				}
				initialized = true;
			}
		}
	}
}
