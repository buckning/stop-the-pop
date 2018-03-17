using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour {
	public Transform playerDropPoint;

	public void ResetLevel() {
		SnowflakeBehaviour[] snowflakes = Resources.FindObjectsOfTypeAll (typeof(SnowflakeBehaviour)) as SnowflakeBehaviour[];
		foreach (SnowflakeBehaviour snowflake in snowflakes) {
			snowflake.Reset ();
			snowflake.gameObject.transform.parent.gameObject.SetActive (true);
		}
	}
}
