using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

	[HideInInspector]
	public bool isLocked = false;	//this is a flag to show that this camera is locked and should not be updated by another script

	public PlayerController target = null;
	public Vector3 temporaryTarget;
	public float temporyTargetMoveSpeed = 10f;

	private Vector2 focusAreaSize = new Vector2(5,5);

	FocusArea focusArea;

	private float verticalOffset = 1f;
	private float lookAheadDstX = 1f;
	private float lookSmoothTimeX = 0.5f;
	private float verticalSmoothTime = 0.2f;

	private float zoomSpeed = 2f, zoomAmount = -3f;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelocityY;
	bool lookAheadStopped;

	public HudListener hud;

	public bool gameOver = false;

	void Start() {
		if (hud == null) {
			hud = GameObject.Find ("LevelHUD").GetComponent<HudListener>();
		}
		target = hud.GetPlayer ();
		if (target != null) {
			focusArea = new FocusArea (target.GetComponent<BoxCollider2D> ().bounds, focusAreaSize);
		}
	}

	//all update methods have been called at this stage.
	//this method is typically for cameras
	void LateUpdate() {
		if (temporaryTarget != Vector3.zero) {
			Vector2 targetPosition = new Vector3 (temporaryTarget.x, temporaryTarget.y, transform.position.z);

			Vector3 newPos = Vector3.MoveTowards (transform.position, targetPosition, Time.deltaTime * temporyTargetMoveSpeed);
			transform.position = new Vector3 (newPos.x, newPos.y, transform.position.z);
			return;
		}

		if (target == null) {
			target = hud.GetPlayer ();
			focusArea = new FocusArea (target.GetComponent<BoxCollider2D> ().bounds, focusAreaSize);
		}
		focusArea.Update (target.GetComponent<BoxCollider2D> ().bounds);

		Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;

		if (focusArea.velocity.x != 0) {
			lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
			if(Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) &&  target.playerInput.x != 0) {
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDstX;
			}
			else {
				if(!lookAheadStopped) {
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX)/4;
				}
			}
		}

		currentLookAheadX = Mathf.SmoothDamp (currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

		//fixes an intermittent bug when smoothVelocityY would be NaN and the camera follow would break
		if (float.IsNaN (smoothVelocityY)) {
			smoothVelocityY = 0.0f;
		}

		focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);

		focusPosition += Vector2.right * currentLookAheadX;

		Vector3 pos = (Vector3)focusPosition + Vector3.forward * -10;

		float zoom = transform.position.z;
		if (target.temperature >= 1.0f || gameOver) {
			isLocked = true;
			//zoom in on player
			float playerX = Mathf.Lerp (transform.position.x, target.transform.position.x, zoomSpeed * Time.deltaTime);
			float playerY = Mathf.Lerp (transform.position.y, target.transform.position.y, zoomSpeed * Time.deltaTime);
			zoom = Mathf.Lerp (transform.position.z, zoomAmount, zoomSpeed * Time.deltaTime);
			transform.position = new Vector3 (playerX, playerY, zoom);
		} else {
			transform.position = new Vector3 (pos.x, pos.y, zoom);
		}
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color (1, 0, 0, 0.5f);

		Gizmos.DrawCube (focusArea.centre, focusAreaSize);
	}

	struct FocusArea {
		public Vector2 centre;
		public float left, right;
		public float top, bottom;
		public Vector2 velocity;
		public FocusArea(Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x/2;
			right = targetBounds.center.x + size.x/2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;
			velocity = Vector2.zero;
			centre = new Vector2((left +right)/2, (top + bottom)/2);
		}
		
		public void Update(Bounds targetBounds) {
			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;

			centre = new Vector2((left +right)/2, (top + bottom)/2);

			velocity = new Vector2 (shiftX, shiftY);
		}
	}
}
