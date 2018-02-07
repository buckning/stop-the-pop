using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Animator))]
public class PopcornKernelAnimator : MonoBehaviour {

	public SpriteRenderer kickEffect;
	private Animator animator;

	void Start () {
		animator = GetComponent<Animator> ();
	}

	void Update() {
		if (kickEffect.gameObject.activeInHierarchy) {
			float alpha = kickEffect.color.a - Time.deltaTime * 4;
			if (alpha < 0.0f) {
				alpha = 0.0f;
				kickEffect.color = new Color (1, 1, 1, 0.0f);
				kickEffect.gameObject.SetActive (false);
			}
			kickEffect.color = new Color (1, 1, 1, alpha);
		}
	}
	
	public void SetVelocityX(float speed) {
		animator.SetFloat("speed", speed);
	}

	public void SetVelocityY(float yVel) {
		animator.SetFloat ("yVelocity", yVel);
	}

	public void SetGrounded(bool grounded) {
		animator.SetBool ("grounded", grounded);
	}

	public void Kick() {
		animator.SetTrigger("kick");
	}

	public void StartPopping() {
		animator.SetTrigger("Popping");
	}

	public void StartBlinking() {
		animator.SetBool("blinking", true);
	}

	public void PlaySawBladeDeathAnimation() {
		animator.SetTrigger ("SawbladeDeathAnimationStatic");
	}

	public void PlayMovingSawBladeDeathAnimation() {
		animator.SetTrigger ("SawbladeDeathAnimationMoving");
	}

	/***
	 * Called by the animator
	 */
	public void PlayKickEffect() {
		kickEffect.gameObject.SetActive (true);
		kickEffect.color = new Color(1, 1, 1, 1f);
	}
}
