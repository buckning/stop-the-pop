using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ScreenFader : MonoBehaviour {

	public float fadeSpeed = 4.0f;

	public delegate void NotifyEvent ();
	public event NotifyEvent fadeOutCompleteListeners;
	public event NotifyEvent fadeInCompleteListeners;

	private Image screenFader;
	private bool fadingOut = false, fadingIn = false;

	void Update () {
		if (fadingIn) {
			FadeIn ();
			return;
		}
		if (fadingOut) {
			FadeOut();
			return;
		}
	}

	public void StartFadingIn() {
		fadingIn = true;
		gameObject.SetActive (true);

		if (screenFader == null) {
			screenFader = GetComponent<Image> ();
		}

		screenFader.color = new Color(screenFader.color.r, screenFader.color.g, screenFader.color.b, 1.0f);
	}

	public void StartFadingOut() {
		fadingOut = true;
		gameObject.SetActive (true);

		if (screenFader == null) {
			screenFader = GetComponent<Image> ();
		}

		screenFader.color = new Color(screenFader.color.r, screenFader.color.g, screenFader.color.b, 0.0f);
	}

	private void FadeIn() {
		Color color = new Color(screenFader.color.r, screenFader.color.g, screenFader.color.b, screenFader.color.a);
		color.a = Mathf.MoveTowards (screenFader.color.a, 0.0f, Time.deltaTime * fadeSpeed);
		screenFader.color = color;
		screenFader.gameObject.SetActive (true);

		if (color.a <= 0.05f) {
			fadingIn = false;
			screenFader.gameObject.SetActive(false);

			if (fadeInCompleteListeners != null) {
				fadeInCompleteListeners ();
			}
		}
	}

	private void FadeOut() {
		Color color = new Color(screenFader.color.r, screenFader.color.g, screenFader.color.b, screenFader.color.a);
		color.a = Mathf.MoveTowards (screenFader.color.a, 1.0f, Time.deltaTime * fadeSpeed);
		screenFader.color = color;
		screenFader.gameObject.SetActive (true);

		if (color.a >= 0.95f) {
			fadingOut = false;

			if (fadeOutCompleteListeners != null) {
				fadeOutCompleteListeners ();
			}
		}
	}
}
