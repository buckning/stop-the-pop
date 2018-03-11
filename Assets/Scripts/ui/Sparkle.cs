using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Sparkle : MonoBehaviour {

	public float delayBetweenSparkles = 1.0f;
	public float initialDelay = 0.0f;
	public float fadeSpeed = 10.0f;
	private float timeBetweenSparkles = 0.0f;

	private bool fadingIn = true;
	private Image sparkleImage;
	private Color zeroAlphaColor;

	void Start () {
		sparkleImage = GetComponent<Image> ();
		zeroAlphaColor = Color.white;
		zeroAlphaColor.a = 0.0f;
		timeBetweenSparkles = delayBetweenSparkles - initialDelay;
		sparkleImage.color = zeroAlphaColor;
	}

	void Update () {
		timeBetweenSparkles += Time.unscaledDeltaTime;

		if (timeBetweenSparkles > delayBetweenSparkles) {
			if (fadingIn) {
				FadeIn ();
			} else {
				FadeOut ();
			}
		}
	}

	private void FadeIn() {
		fadingIn = true;
		sparkleImage.color = Color.Lerp(sparkleImage.color, Color.white, Time.fixedUnscaledDeltaTime * fadeSpeed);
		if (sparkleImage.color.a > 0.99f) {
			fadingIn = false;
		}
	}

	private void FadeOut() {
		sparkleImage.color = Color.Lerp (sparkleImage.color, zeroAlphaColor, Time.fixedUnscaledDeltaTime * fadeSpeed);
		if (sparkleImage.color.a < 0.01f) {
			timeBetweenSparkles = 0.0f;
			fadingIn = true;
		}
	}
}
