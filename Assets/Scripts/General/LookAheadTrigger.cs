using UnityEngine;
using System.Collections;

public class LookAheadTrigger : MonoBehaviour {
	CameraFollow myCamera;
	public GameObject target;
	public float cameraMoveSpeed = 50f;

	public float timeLookingAtObject = 2.5f;

	private Vector3 previousTransform;

	void Start() {
		myCamera = GameObject.Find ("Main Camera").GetComponent<CameraFollow>();
	}

	void OnTriggerEnter2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.hud.EnableControlPanel (false);
			player.hud.DirectionalButtonUp ();
			player.playerMovementEnabled = false;
			target.SetActive (true);

			if (myCamera.temporaryTarget == Vector3.zero) {
				previousTransform = new Vector3(myCamera.transform.position.x, myCamera.transform.position.y, myCamera.transform.position.z);
				myCamera.temporaryTarget = target.transform.position;
				myCamera.temporyTargetMoveSpeed = cameraMoveSpeed;
				StartCoroutine(ResetAndDestroy());
			}
		}
	}

	IEnumerator ResetAndDestroy() {
		yield return new WaitForSeconds (timeLookingAtObject);
		myCamera.temporaryTarget = previousTransform;
		yield return new WaitForSeconds (1.5f);
		PlayerController player = GameObject.Find ("LevelHUD").GetComponent<HudListener>().player;
		player.hud.DirectionalButtonUp ();
		player.hud.EnableControlPanel (true);
		player.playerMovementEnabled = true;
		myCamera.temporaryTarget = Vector3.zero;
		Destroy (gameObject);
	}
}
