using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextFieldNumberAnimator : MonoBehaviour {

	public float currentNumber;
	public float desiredNumber;
	public float animationTime = 1.0f;
	public float initialNumber;

	private Text textField;

	private bool animationComplete = false;

	public delegate void AnimationComplete ();

	public delegate void ValueIncremented (float value);

	public delegate void ValueDecremented (float value);

	public AnimationComplete animationCompleteListeners;

	public ValueIncremented valueIncrementedListeners;

	public ValueDecremented valueDecrementedListeners;

	void Start() {
		textField = GetComponent<Text> ();
	}

	public void SetNumber(float value) {
		initialNumber = currentNumber;
		desiredNumber = value;
	}

	public void AddToNumber(float value) {
		initialNumber = currentNumber;
		desiredNumber += value;
	}

	void Update () {
		float oldNumber = currentNumber;
		if (currentNumber != desiredNumber) {
			if (initialNumber < desiredNumber) {
				currentNumber += (animationTime * Time.unscaledDeltaTime) * (desiredNumber - initialNumber);

				if (valueIncrementedListeners != null) {
					valueIncrementedListeners (currentNumber - oldNumber);
				}

				if (currentNumber >= desiredNumber) {
					currentNumber = desiredNumber;
					if (animationCompleteListeners != null && !animationComplete) {
						animationCompleteListeners ();
						animationComplete = true;
					}
				}
			} else {
				currentNumber -= (animationTime * Time.unscaledDeltaTime) * (initialNumber - desiredNumber);

				if (valueDecrementedListeners != null) {
					valueDecrementedListeners (oldNumber - currentNumber);
				}

				if (currentNumber <= desiredNumber) {
					currentNumber = desiredNumber;
					if (animationCompleteListeners != null && !animationComplete) {
						animationCompleteListeners ();
						animationComplete = true;
					}
				}
			}
		} 
		textField.text = currentNumber.ToString ("0");
	}

	public void Reset() {
		animationComplete = false;
	}
}
