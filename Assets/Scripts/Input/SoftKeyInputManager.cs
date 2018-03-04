using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoftKeyInputManager : MonoBehaviour, InputManager {
	public Image jumpButton;
	public Image attackButton;
	public Image rightDirectionButton;
	public Image leftDirectionButton;

	private bool jumpKeyPressed;
	private bool jumpKeyReleased;

	private bool attackKeyPressed;
	private bool attackKeyReleased;

	private bool jumpSoftKeyNew;
	private bool jumpSoftKeyOld;

	private bool attackSoftKeyNew;
	private bool attackSoftKeyOld;

	private Vector2 directionalInput;

	void Start () {
		directionalInput = new Vector2 ();
		jumpKeyPressed = false;
		jumpKeyReleased = false;

		SetUpEventListenerOnImage (jumpButton, EventTriggerType.PointerDown, JumpSoftKeyDown);
		SetUpEventListenerOnImage (jumpButton, EventTriggerType.PointerUp, JumpSoftKeyUp);
		SetUpEventListenerOnImage (attackButton, EventTriggerType.PointerDown, AttackSoftKeyDown);
		SetUpEventListenerOnImage (attackButton, EventTriggerType.PointerUp, AttackSoftKeyUp);
		SetUpEventListenerOnImage (rightDirectionButton, EventTriggerType.PointerDown, RightSoftKeyDown);
		SetUpEventListenerOnImage (rightDirectionButton, EventTriggerType.PointerUp, RightSoftKeyUp);
		SetUpEventListenerOnImage (leftDirectionButton, EventTriggerType.PointerDown, LeftSoftKeyDown);
		SetUpEventListenerOnImage (leftDirectionButton, EventTriggerType.PointerUp, LeftSoftKeyUp);
	}

	void SetUpEventListenerOnImage(Image image, EventTriggerType eventTriggerType, System.Action<BaseEventData> callback) {
		EventTrigger jumpButtonEventTrigger = image.gameObject.AddComponent<EventTrigger> ();
		EventTrigger.Entry entry = new EventTrigger.Entry();
		entry.eventID = eventTriggerType;
		entry.callback = new EventTrigger.TriggerEvent();
		entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>(callback));
		jumpButtonEventTrigger.triggers.Add (entry);
	}

	void Update () {
		jumpKeyPressed = false;
		jumpKeyReleased = false;

		attackKeyPressed = false;
		attackKeyReleased = false;

		if (jumpSoftKeyNew && !jumpSoftKeyOld) {
			jumpKeyPressed = true;
		}

		if (!jumpSoftKeyNew && jumpSoftKeyOld) {
			jumpKeyReleased = true;
		}

		if (attackSoftKeyNew && !attackSoftKeyOld) {
			attackKeyPressed = true;
		}

		if (!attackSoftKeyNew && attackSoftKeyOld) {
			attackKeyReleased = true;
		}

		jumpSoftKeyOld = jumpSoftKeyNew;
		attackSoftKeyOld = attackSoftKeyNew;
	}

	public bool JumpKeyDown () {
		return jumpKeyPressed;
	}

	public bool JumpKeyUp () {
		return jumpKeyReleased;
	}

	public bool AttackKeyPressed() {
		return attackKeyPressed;
	}

	public float getXAxis () {
		return directionalInput.x;
	}

	private void LeftSoftKeyDown(BaseEventData eventData) {
		directionalInput.x = -1f;
	}

	private void LeftSoftKeyUp(BaseEventData eventData) {
		directionalInput.x = 0f;
	}

	private void RightSoftKeyDown(BaseEventData eventData) {
		directionalInput.x = 1f;
	}

	private void RightSoftKeyUp(BaseEventData eventData) {
		directionalInput.x = 0f;
	}

	private void JumpSoftKeyDown(BaseEventData eventData) {
		jumpSoftKeyNew = true;
	}

	private void JumpSoftKeyUp(BaseEventData eventData) {
		jumpSoftKeyNew = false;
	}

	private void AttackSoftKeyDown(BaseEventData eventData) {
		attackSoftKeyNew = true;
	}

	private void AttackSoftKeyUp(BaseEventData eventData) {
		attackSoftKeyNew = false;
	}

//	public void DirectionalButtonUp() {
//		leftButtonImage.color = new Color (0.706f, 0.706f, 0.706f, 0.314f);
//		rightButtonImage.color = new Color (0.706f, 0.706f, 0.706f, 0.314f);
//		LeftSoftKeyUp ();
//		RightSoftKeyUp ();
//	}
//
//	public void DirectionalButtonPressed(BaseEventData data) {
//		PointerEventData pointerData = data as PointerEventData;
//
//		if (RectTransformUtility.RectangleContainsScreenPoint (leftButton.GetComponent<RectTransform> (), pointerData.position, Camera.main)) {
//			leftButtonImage.color = new Color (1f, 1f, 1f, 0.549f);
//			rightButtonImage.color = new Color (0.706f, 0.706f, 0.706f, 0.314f);
//			LeftSoftKeyDown ();
//		} else if (RectTransformUtility.RectangleContainsScreenPoint (rightButton.GetComponent<RectTransform> (), pointerData.position, Camera.main)) {
//			rightButtonImage.color = new Color (1f, 1f, 1f, 0.549f);
//			leftButtonImage.color = new Color (0.705f, 0.705f, 0.705f, 0.314f);
//			RightSoftKeyDown ();
//		}
//	}
}
