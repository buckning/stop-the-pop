﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Animator))]
public class PopcornKernelAnimator : MonoBehaviour {

	public SpriteRenderer kickEffect;
	private Animator animator;

	bool leftLegPopped = false;
	bool rightLegPopped = false;
	public GameObject leftLeg;		//used to disable the legs for the sawblade.
	public GameObject rightLeg;
	public GameObject leg;								//the leg that will be used to pop off when the player pops

	public Transform leftLegPopPoint;
	public Transform rightLegPopPoint;

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
		StartCoroutine(PopAnimationComplete());
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

	public void PopRightLegSprite() {
		if(!rightLegPopped) {
			PopLegSprite (rightLegPopPoint.position, 300f, 200f, 
				SelectedPlayerCustomisations.selectedShoes + "Right");
			rightLegPopped = true;
		}
	}

	public void PopLeftLegSprite() {
		if (!leftLegPopped) {
			PopLegSprite (leftLegPopPoint.position, -300f, -200f, 
				SelectedPlayerCustomisations.selectedShoes + "Left");
			leftLegPopped = true;
		}
	}

	/***
	 * Pop off a players leg and reskin the shoe it with the current skin
	 */
	private void PopLegSprite(Vector2 position, float maxTorque, float forceX, string spriteName) {
		GameObject poppedObject = (GameObject)Instantiate (leg, position, Quaternion.identity);
		//reskin the popped leg
		SpriteRenderer[] legRenderers = poppedObject.GetComponentsInChildren<SpriteRenderer> ();
		Sprite[] sprites = Resources.LoadAll<Sprite> ("skins/player/shoes");
		if (SelectedPlayerCustomisations.selectedShoes != null) {
			foreach (SpriteRenderer renderer in legRenderers) {
				if (renderer.name == "shoe-skin") {
					foreach (Sprite sprite in sprites) {
						if (sprite.name == spriteName) {
							renderer.sprite = sprite;
						}
					}
				}
			}
		}

		poppedObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (forceX, 100f));
		poppedObject.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (0f, maxTorque));
		leftLegPopped = true;
	}

	private IEnumerator PopAnimationComplete() {
		yield return new WaitForSeconds(3.8f);
		StartBlinking ();
	}

	public void CustomisePlayer() {
		if (PlayerCustomisation.facialHairSprites == null) {
			PlayerCustomisation.facialHairSprites = Resources.LoadAll<Sprite> ("skins/player/facialHair");
		} 
		if (PlayerCustomisation.shoesSprites == null) {
			PlayerCustomisation.shoesSprites = Resources.LoadAll<Sprite> ("skins/player/shoes");
		}
		if (PlayerCustomisation.hatSprites == null) {
			PlayerCustomisation.hatSprites = Resources.LoadAll<Sprite> ("skins/player/hats");
		}

		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();

		Sprite[] sprites = PlayerCustomisation.hatSprites;
		reskinSprites (renderers, sprites, "Hat", SelectedPlayerCustomisations.selectedHat);

		sprites = PlayerCustomisation.facialHairSprites;
		reskinSprites (renderers, sprites, "FacialHair", SelectedPlayerCustomisations.selectedFacialHair);

		sprites = PlayerCustomisation.shoesSprites;
		reskinSprites (renderers, sprites, "shoe-skin-right", SelectedPlayerCustomisations.selectedShoes + "Right");
		reskinSprites (renderers, sprites, "shoe-skin-left", SelectedPlayerCustomisations.selectedShoes + "Left");
	}

	private void reskinSprites(SpriteRenderer[] renderers, Sprite[] sprites, string rendererName, string spriteName) {
		foreach (SpriteRenderer renderer in renderers) {
			if (renderer.name == rendererName) {
				foreach (Sprite sprite in sprites) {
					if (sprite.name == spriteName) {
						renderer.sprite = sprite;
					}
				}
			}
		}
	}
}
