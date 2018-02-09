using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Animator))]
public class PopcornKernelAnimator : MonoBehaviour {

	public SpriteRenderer kickEffect;
	private Animator animator;

	public GameObject leftLeg;		//used to disable the legs for the sawblade.
	public GameObject rightLeg;
	public GameObject leg;								//the leg that will be used to pop off when the player pops
	public GameObject shadow;
	public GameObject jumpDust;
	public GameObject landingDust;
	public Cape cape;
	public LayerMask shadowMask;						//the layer that shadows can be displayed on
	public Transform leftLegPopPoint;
	public Transform rightLegPopPoint;
	public Transform groundPosition;

	public delegate void NotifyEvent ();
	public event NotifyEvent finishedPoppingListeners;

	private AudioSource runAudioSource;		// this audio source is used exclusively for the run sound effect. All other real time sounds need to be played through audio manger

	void Start () {
		animator = GetComponent<Animator> ();

		runAudioSource = GetComponent<AudioSource> ();
		runAudioSource.loop = true;
		runAudioSource.clip = Resources.Load ("SoundEffects/Player/run") as AudioClip;
		runAudioSource.Stop ();
	}

	void Update() {
		UpdateShadow ();
		UpdateKickEffect ();

		// need to see if GetFloat is heavy on performance, if so, cache instead
		if (!runAudioSource.isPlaying && animator.GetFloat ("speed") != 0.0f) {
			runAudioSource.Play ();
		} else if (animator.GetFloat ("speed") == 0.0f) {
			runAudioSource.Stop ();
		}
	}

	public void EnableCape(bool enable) {
		cape.gameObject.SetActive (enable);
	}

	public void SetGliding(bool gliding) {
		cape.SetGliding (gliding);
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

		AudioManager.PlaySound ("jump", 1.3f + Random.Range(-0.2f, 0.2f));	//use the same sfx for both jump and kick. 
	}

	/***
	 * This is called by an event in the players PlayerPopping animation
	 */
	public void PopMiddleSection() {
		string soundToPlay = "pop10";
		AudioManager.PlaySound(soundToPlay);
	}

	/***
	 * This is called by an event in the players PlayerPopping animation
	 */
	public void PopRightLeg() {
		AudioManager.PlaySound("pop3");
		PopLegSprite (rightLegPopPoint.position, 300f, 200f, 
			SelectedPlayerCustomisations.selectedShoes + "Right");
	}

	/***
	 * This is called by an event in the players PlayerPopping animation
	 */
	public void PopLeftLeg() {
		AudioManager.PlaySound("pop8");

		PopLegSprite (leftLegPopPoint.position, -300f, -200f, 
			SelectedPlayerCustomisations.selectedShoes + "Left");

		AudioManager.PlaySound ("player-death");
	}

	/***
	 * This is called by an event in the players PlayerPopping animation
	 */
	public void PopBody() {
		AudioManager.PlaySound("pop8");
	}

	/***
	 * This sound effect gets triggered by the animator
	 */
	public void PlayBlinkSoundEffect() {
		AudioManager.PlaySound ("blink");
	}

	public void PlayTitleScreenAnimation() {
		animator.SetTrigger("titleScreen");
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
	}

	private IEnumerator PopAnimationComplete() {
		yield return new WaitForSeconds(3.8f);
		StartBlinking ();
	}

	void UpdateKickEffect() {
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

	public void Jump() {
		AudioManager.PlaySoundAfterTime ("jump", 0.1f);
		Vector2 spawnPos = new Vector2 (groundPosition.position.x, groundPosition.position.y);
		spawnPos.y += 0.25f;
		GameObject dustObject = (GameObject)Instantiate (jumpDust, spawnPos, Quaternion.identity);
		dustObject.transform.localScale = transform.localScale;
	}

	public void Land() {
		AudioManager.PlaySound ("landing");	//only play sound effect when actually landing instead of when the player can jump through platforms from below
		GameObject dustObject = (GameObject)Instantiate (landingDust, groundPosition.position, Quaternion.identity);
		dustObject.transform.localScale = transform.localScale;
	}

	void UpdateShadow() {
		RaycastHit2D hit = Physics2D.Raycast (groundPosition.position, Vector2.up * -1, 20, shadowMask);
		shadow.transform.position = hit.point;

		if (hit.distance > 10f) {
			shadow.SetActive (false);
		} else {
			shadow.SetActive (true);
			float xScale = (1 - hit.distance/7.5f);
			if (xScale < 0.3f) {
				xScale = 0.3f;
			}
			shadow.transform.localScale = new Vector2 (xScale, 1);
		}
	}

	public void CustomisePlayer(string spriteSheet) {
		Sprite[] vikingSprites = Resources.LoadAll<Sprite> (spriteSheet);
		ReskinKernel (vikingSprites, vikingSprites, vikingSprites);
	}

	/***
	 * Called back by the animator. This signals that the kernel has finished popping and notifies any listeners
	 * that this event took place. 
	 */
	public void FinishedPopping() {
		if (finishedPoppingListeners != null) {
			finishedPoppingListeners ();
		}
	}

	/***
	 * Reskins the player with the required hat, facial hair and shoes.
	 */
	public void ReskinKernel(Sprite[] selectedHatSprites, Sprite[] selectedFacialHairSprites, Sprite[] selectedShoesSprites) {
		Reskin (selectedHatSprites, "Hat");
		Reskin (selectedFacialHairSprites, "FacialHair");
		Reskin (selectedShoesSprites, "Shoe");
	}

	private void Reskin(Sprite[] spriteSheet, string rendererNameToReskin) {
		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();

		foreach (SpriteRenderer renderer in renderers) {
			if (renderer.name == rendererNameToReskin) {
				bool spriteWasSet = false;
				foreach (Sprite sprite in spriteSheet) {
					if (sprite.name == rendererNameToReskin) {
						renderer.sprite = sprite;
						spriteWasSet = true;
					}
				}
				if (!spriteWasSet) {
					renderer.sprite = null;
				}
			}
		}
	}
}
