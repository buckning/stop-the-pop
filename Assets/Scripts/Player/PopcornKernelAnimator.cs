using System.Collections;
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

		Sprite[] sprites = PlayerCustomisation.hatSprites;

		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();

		foreach (SpriteRenderer renderer in renderers) {
			if (renderer.name == "Hat") {
				foreach (Sprite sprite in sprites) {
					if (sprite.name == SelectedPlayerCustomisations.selectedHat) {
						renderer.sprite = sprite;
					}
				}
			}
		}

		sprites = Resources.LoadAll<Sprite> ("skins/player/glasses");

		foreach (SpriteRenderer renderer in renderers) {
			if (renderer.name == "Glasses") {
				foreach (Sprite sprite in sprites) {
					if (sprite.name == SelectedPlayerCustomisations.selectedGlasses) {
						renderer.sprite = sprite;
					}
				}
			}
		}

		sprites = PlayerCustomisation.facialHairSprites;

		if (SelectedPlayerCustomisations.selectedFacialHair != null) {
			foreach (SpriteRenderer renderer in renderers) {
				if (renderer.name == "FacialHair") {
					foreach (Sprite sprite in sprites) {
						if (sprite.name == SelectedPlayerCustomisations.selectedFacialHair) {
							renderer.sprite = sprite;
						}
					}
				}
			}
		} else {
			foreach (SpriteRenderer renderer in renderers) {
				if (renderer.name == "FacialHair") {
					renderer.sprite = null;
				}
			}
		}


		sprites = PlayerCustomisation.shoesSprites;

		if (SelectedPlayerCustomisations.selectedShoes != null) {
			foreach (SpriteRenderer renderer in renderers) {
				if (renderer.name == "shoe-skin-right") {
					foreach (Sprite sprite in sprites) {
						if (sprite.name == SelectedPlayerCustomisations.selectedShoes + "Right") {
							renderer.sprite = sprite;
						}
					}
				} else if (renderer.name == "shoe-skin-left") {
					foreach (Sprite sprite in sprites) {
						if (sprite.name == SelectedPlayerCustomisations.selectedShoes + "Left") {
							renderer.sprite = sprite;
						}
					}
				}
			}
		}
	}
}
