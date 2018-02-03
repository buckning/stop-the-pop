using UnityEngine;
using System.Collections;

public class GameObjectRotator : MonoBehaviour {
	public float rotationSpeed = 100f;

	void Update () {
		transform.Rotate(0, 0, Time.deltaTime * rotationSpeed);
	}
}
