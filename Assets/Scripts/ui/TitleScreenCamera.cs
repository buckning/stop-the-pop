using UnityEngine;
using System.Collections;

public class TitleScreenCamera : MonoBehaviour {
	public GameObject titleScreenCameraPosition;
	public GameObject titleScreenPlayerCustomisationCameraPosition;

	public GameObject target;

	void Start() {
		target = titleScreenCameraPosition;
	}

	void Update() {
		Vector3 newPos = Vector3.MoveTowards (transform.position, target.transform.position, Time.deltaTime * 20);
		transform.position = new Vector3 (newPos.x, newPos.y, transform.position.z);
	}
}
