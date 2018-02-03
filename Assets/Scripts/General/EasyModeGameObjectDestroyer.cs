using UnityEngine;
using System.Collections;

/***
 * The purpose of this class is to destroy some objects when the game has just been switched to easy mode
 */
public class EasyModeGameObjectDestroyer : MonoBehaviour {
	public string destroyTag;
	public string destroyName;
	bool wasTriggered = false;

	void Update () {
		if ((CurrentLevel.GetLevelDifficulty() == CurrentLevel.LevelDifficulty.EASY) && !wasTriggered) {

			GameObject[] gameObjects = GameObject.FindGameObjectsWithTag (destroyTag);
			foreach (GameObject go in gameObjects) {
				if (go.name.StartsWith (destroyName)) {
					Destroy (go);
				}
			}
			wasTriggered = true;
		}
	}
}
