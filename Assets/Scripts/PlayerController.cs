using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * Script adapted from https://unity3d.com/learn/tutorials/modules/beginner/2d/2d-controllers
 */
public class PlayerController : MonoBehaviour {

	public float maxJumpHeight = 8f;
	public float minJumpHeight = 0.4f;
	public float timeToJumpApex = 1f;	//time to reach the jump height
	float gravity;					//implement our own gravity so we have better control of our jumps
	float maxJumpVelocity;
	float minJumpVelocity;

	public float moveSpeed = 14f;
	float velocityXSmoothing;

	public Transform leftLegPopPoint;
	public Transform rightLegPopPoint;

	string lastCollisionName = "";

	bool groundedOverride = false;	// the point of this is if the player is running and falls off a platform, we want the player to be able to jump for a split second

	[HideInInspector]
	public Vector2 playerInput;
	Vector2 oldPlayerInput;

	public Transform[] groundCheck;
	public Transform ceilingCheck;
	public Transform attackPosition;

	public LayerMask whatIsGround;						//defines what we consider the ground
	public LayerMask whatIsCeilingMask;					//define what is the ceiling (used to see if the player is getting crushed)
	public LayerMask breakableMask;						//breakable layer mask
	public LayerMask collectableMask;
	public LayerMask shadowMask;						//the layer that shadows can be displayed on

	public GameObject landingDust;
	public GameObject jumpDust;
	public GameObject shadow;
	public SpriteRenderer kickEffect;

	public const float POP_TEMPERATURE = 1.0f;

	public float temperature;							//the temperature of the player
	public bool updateTemperature = true;				//Shall we update our temperature every update?
	public bool jumpEnabled = true;						//Is single jump enabled?
	public float regularTemperatureUpdateRate = 0.02f;	//the regular temperature update of the player
	float temperatureUpdateRate = 0.02f;			//the rate at which the temperature increases
	float MAX_TEMPERATURE = 100.0f;				//the maximum temperature the player can reach. If the temperature is any higher, it is game over
	float SNOWFLAKE_TEMPERATURE_DECREASE = -30f;	//The amount of temperature to decrease by when collided with snowflake 
	float ENEMY_COLLISION_TEMPERATURE_INCREASE = 50f;	//The amount of temperature that increases when colliding with an enemy. 10f recommended
	float HAZARD_COLLISION_TEMPERATURE_INCREASE = 50f;	//The amount of temperature that increases when colliding with a hazardous environment. 10f recommended
	float MAX_INVINCIBILITY_TIME = 1.5f;			//the amount of time in seconds that the player will be invincible for if hit by an enemy/hazardous env
	public GameObject leg;								//the leg that will be used to pop off when the player pops
	private float invincibilityTimeLeft = 0f;			//the amount of time that the player will still be invicible for
	private Animator animator;							//our character animator, used to change between animations
	private bool facingRight = true;					//this is used so we can know how to face our sprite
	private Rigidbody2D rigidbody2d;					//the rigid body 2d for the player
	private bool grounded = false;						//defines if the player is on the ground (used for jumps)
	private float groundRadius = 0.3f;					//defines the size of the collider used to check if we are on the ground
	private float ceilingRadius = 0.1f;					//defines the size of the collider used to check if we are on the ground
	private float attackRadius = 0.8f;					//defines the size of the collider used to check if there is an object to attack
	float magnetRadius = 6.0f;
	float magnetAttractForce = 20f;

	private bool popped = false;						//used to see if the player popping animation has been triggered
	bool gliding = false;								//this is set internally when the player should be gliding
	public HudListener inputManager;					//reference to the HUD, so we can check what buttons are being pressed

	[HideInInspector]
	public bool gameOver = false;						//this is used by other objects to disable player input

	public float wallSlideSpeedMax = 2f;

	public bool glidingEnabled = false;					//if this is set to false, the player cannot glide regardless if the player is trying to
	public const float defaultGlideSpeed = -2f;
	[HideInInspector]
	public float glideSpeed = defaultGlideSpeed;								//the velocity in the y axis when gliding

	public PlayerWallTrigger wallCollider;

	public Cape cape;

	public bool magnetEnabled = false;

	public bool playerMovementEnabled = true;

	List<int> collectedCoins = new List<int>();

	bool kickTriggered = false;
	bool touchingCeiling = false;
	bool crushed = false;
	bool kicking = false;
	public bool collidingWithAreaEffector = false;
	public float effectorSpeed = 5.0f;
	float glidingTime = 0.0f;

	bool lifeAlreadyLost = false;					//when the player loses a life, this is checked to see if a life has already been lost
													//this variable prevents multiple lives being lost in one level
	AudioSource runAudioSource;		//this audio source is used exclusively for the run sound effect. All other real time sounds need to be played through audio manger

	bool leftLegPopped = false;
	bool rightLegPopped = false;
	public GameObject leftLeg;		//used to disable the legs for the sawblade.
	public GameObject rightLeg;

	public PolygonCollider2D bodyCollider;	//this collider is just used by the saw animations

	void Start () {
		gameOver = false;
		oldPlayerInput = Vector2.zero;
		temperatureUpdateRate = regularTemperatureUpdateRate;
		runAudioSource = GetComponent<AudioSource> ();
		runAudioSource.loop = true;
		runAudioSource.clip = Resources.Load ("SoundEffects/Player/run") as AudioClip;
		runAudioSource.Stop ();
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs(gravity) * minJumpHeight);

		rigidbody2d = GetComponent<Rigidbody2D> ();
		temperature = temperature / MAX_TEMPERATURE;
		
		//start the player in a rest animation
		animator = GetComponent<Animator> ();
		animator.SetFloat("speed", 0f);

		if (glidingEnabled) {
			cape.gameObject.SetActive (true);
		}

		//apply the skin customisations
		CustomisePlayer ();

		//add all the collected coins from the last checkpoint to this internal list. This is just so we can show an accurate coin count when a level is restarted
		List<int> coins = LastCheckpoint.GetCollectedCoins ();
		foreach (int coinId in coins) {
			collectedCoins.Add (coinId);
		}
	}

	// Update is called at a fixed rate - this is better when interacting with physics objects (time.delta time is not needed here)
	public void FixedUpdate () {
		bool oldGrounded = grounded;

		grounded = false;

		foreach(Transform check in groundCheck) {
			if (Physics2D.OverlapCircle (check.position, groundRadius * transform.localScale.x, whatIsGround)) {
				grounded = true;
			}
		}

		if (grounded) {
			groundedOverride = false;
			gliding = false;
			//if we were not grounded last frame and are grounded this frame, we have just landed
			if (!oldGrounded) {
				
				if (rigidbody2d.velocity.y <= 0) {
					AudioManager.PlaySound ("landing");	//only play sound effect when actually landing instead of when the player can jump through platforms from below
				}
				GameObject dustObject = (GameObject)Instantiate (landingDust, groundCheck [0].position, Quaternion.identity);
				dustObject.transform.localScale = transform.localScale;
			}
		} else {
			//the player has just fallen off a platform, we want to give the player a chance to jump for a short period after falling off the platform
			if (oldGrounded) {
				if(rigidbody2d.velocity.y < 0.0f) {
					groundedOverride = true;
					//kick off timer to reset the grounded override
					StartCoroutine(DisableGroundedOverride());
				}
			}
		}

		touchingCeiling = Physics2D.OverlapCircle (ceilingCheck.position, ceilingRadius  * transform.localScale.x, whatIsCeilingMask);

		UpdateMagnetBehaviour ();
	}

	public void PlaySawBladeDeathAnimation() {
		animator.SetTrigger ("SawbladeDeathAnimationStatic");
	}

	public void PlayMovingSawBladeDeathAnimation() {
		animator.SetTrigger ("SawbladeDeathAnimationMoving");
	}

	public void EnableBodyCollider() {
		bodyCollider.enabled = true;
		rigidbody2d.freezeRotation = false;
		rigidbody2d.AddTorque (Random.Range (-100f, 100f));
	}

	public IEnumerator DisableGroundedOverride() {
		yield return new WaitForSeconds(0.1f);
		groundedOverride = false;
	}
	
	public void setPosition(Vector2 position) {
		gameObject.transform.position = position;
	}

	public void SetVelocity(Vector2 velocity) {
		rigidbody2d.velocity = velocity;
	}
	
	public void AddForce(Vector2 force) {
		rigidbody2d.AddForce(force);
	}
	
	public void AddForce(Vector2 force, ForceMode2D forceMode) {
		rigidbody2d.AddForce(force, forceMode);
	}

	void UpdateShadow() {
		if (temperature >= 1.0f) {
			shadow.SetActive (false);
			return;
		}

		RaycastHit2D hit = Physics2D.Raycast (groundCheck[0].position, Vector2.up * -1, 20, shadowMask);
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

	void Update() {
		UpdateShadow();

		kickTriggered = false;

		if (temperature >= 1.0f || !playerMovementEnabled || gameOver) {
			playerInput = Vector2.zero;
			cape.SetVelocity (Vector2.zero);
		} else {
			playerInput = new Vector2 (inputManager.getXAxis(), inputManager.getYAxis());
		}

		if (kickEffect.gameObject.activeInHierarchy) {
			float alpha = kickEffect.color.a - Time.deltaTime * 4;
			if (alpha < 0.0f) {
				alpha = 0.0f;
				kickEffect.color = new Color (1, 1, 1, 0.0f);
				kickEffect.gameObject.SetActive (false);
			}
			kickEffect.color = new Color (1, 1, 1, alpha);
		}
		UpdateMagnetBehaviour ();
		CheckForJump ();
		CheckForAttack ();
		UpdateVelocity ();
		CheckIfCrushed ();

		//update the way our sprite is facing.
		if (playerInput.x > 0 && !facingRight) {
			Flip ();
		} else if (playerInput.x < 0 && facingRight) {
			Flip();
		}

		//update our invincibility time
		invincibilityTimeLeft -= Time.deltaTime;
		if (invincibilityTimeLeft < 0.0f) {
			invincibilityTimeLeft = 0.0f;
		}

		UpdateTemperature ();

		if (glidingEnabled) {
			Vector2 velocity = new Vector2(playerInput.x, rigidbody2d.velocity.y);
			if (grounded) {
				velocity.y = 0.0f;
			}

			if (gliding) {
				cape.SetGliding (true);


			} else {
				cape.SetGliding (false);

			}
		} 
		UpdateAnimations ();
	}

	void UpdateTemperature() {
		//update our temperature
		if (updateTemperature && invincibilityTimeLeft <= 0.0f) {
			temperature += Time.deltaTime * temperatureUpdateRate;
		}
	}

	void CheckIfCrushed() {
		//only want to trigger this code once so crushed variable was introduced
		if (touchingCeiling && grounded && !crushed) {
			//we are crushed, restart the level
			AddLifeLost();
			inputManager.RetryLevel();
			crushed = true;	
		}
	}

	/***
	 * Should only be used by animator
	 */
	public void RetryLevelAfterPopAnimation() {
		inputManager.TriggerFadeOut ();
	}

	void UpdateAnimations() {
		float animationSpeed = 0.0f;
		animationSpeed = (rigidbody2d.velocity.x != 0.0f) ? Mathf.Abs (playerInput.x) : 0.0f;
		//set our animations
		animator.SetFloat ("speed", animationSpeed);
		animator.SetFloat ("yVelocity", rigidbody2d.velocity.y);
		animator.SetBool ("grounded", grounded);

		if (kickTriggered) {
			animator.SetTrigger("kick");
			AudioManager.PlaySound ("jump", 1.3f + Random.Range(-0.2f, 0.2f));	//use the same sfx for both jump and kick. 
			kickTriggered = false;
		}

		//check to see if we should start our pop animation 
		if (temperature >= 1.0f && !popped && 
		    grounded) {

			magnetEnabled = false;
			AddLifeLost ();

			//start the popping animation
			//start the animator
			animator.SetTrigger("Popping");
			popped = true;

			AnalyticsManager.SendDeathEvent (inputManager.levelName, transform.position, lastCollisionName);

			lastCollisionName = "";
			
			//maybe slow down the time scale here???
			//apply a force after one second

			StartCoroutine(PopAnimationComplete());
		}

		//don't allow to keep gliding when the player should pop
		if (temperature >= 1.0f && !popped && gliding) {
			gliding = false;
		}
	}

	public void PlayKickEffect() {
		kickEffect.gameObject.SetActive (true);
		kickEffect.color = new Color(1, 1, 1, 1f);
	}

	public void CheckForAttack() {
		if (inputManager.AttackKeyPressed () && !kicking) {
			kickTriggered = true;

			if (grounded) {
				//disable player input for a second for the kick
				kicking = true;
				playerMovementEnabled = false;
				StartCoroutine (EnablePlayerMovement ());
			}
		}
	}

	public void PlayPainSound() {
		AudioManager.PlaySound ("popping");
	}

	/***
	 * Check if we need to do any actions with regards to jumping.
	 * We modify the y-velocity of the rigidbody. 
	 * This is a variable sized jump. 
	 * We have a minJumpVelocity and a maxVelocity.
	 * When we initiate the jump we apply the max velocity. 
	 * We stay using this max velocity unless the player releases
	 * the jump key, in which case we change the jump velocity to minVelocity
	 */
	public void CheckForJump() {	
		if (!jumpEnabled || temperature >= 1.0f || !playerMovementEnabled) {
			return;
		}
		Vector2 velocity = rigidbody2d.velocity;
		
		if (inputManager.JumpKeyDown()) {
			//allow the player to jump for a short 
			if (groundedOverride) {
				grounded = true;
			}

			//do a standard jump
			if (grounded) {
				velocity.y = maxJumpVelocity;
				AudioManager.PlaySoundAfterTime ("jump", 0.1f);

				SpawnJumpDust ();
			} else {
				gliding = true;
			}
		}

		//restrict the velocity of the jump if the player releases the key
		if (inputManager.JumpKeyUp()) {
			if(velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
			gliding = false;
		}
		cape.SetVelocity (velocity);
		rigidbody2d.velocity = velocity;
	}

	private void SpawnJumpDust() {
		Vector2 spawnPos = new Vector2 (groundCheck[0].position.x, groundCheck[0].position.y);
		spawnPos.y += 0.25f;
		GameObject dustObject = (GameObject)Instantiate (jumpDust, spawnPos, Quaternion.identity);
		dustObject.transform.localScale = transform.localScale;
	}

	/***
	 * Update our own x and y velocity. This gives us better control of jumping
	 * and better playability since time to fall to ground after jump takes a while
	 */
	public void UpdateVelocity() {
		Vector2 velocity = rigidbody2d.velocity;
		if(gliding && glidingEnabled && velocity.y <= 0f) {
			velocity.y = glideSpeed;
			glidingTime += Time.deltaTime;

			if (glidingTime >= 10.0f) {
				
				#if UNITY_IOS
				SocialServiceManager.GetInstance ().UnlockAchievement ("lookatmemaimflying");
				#endif
				#if UNITY_ANDROID
				SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_look_at_me_ma_im_flying);
				#endif

				glidingTime = 0.0f;
			}
		} else {
			glidingTime = 0.0f;
			velocity.y += gravity * Time.deltaTime;
		}

		if (!playerMovementEnabled) {
			playerInput.x = 0.0f;
		}

		float targetVelocityX = playerInput.x * moveSpeed;

		if (playerInput.x == 0.0f) {
			velocity.x = 0.0f;
		}

		//this caps the max speed of the player
		if (rigidbody2d.velocity.x > 0 && rigidbody2d.velocity.x > targetVelocityX) {
			velocity.x = targetVelocityX;
		}
		//this caps the min speed of the player
		else if (playerInput.x < 0 && rigidbody2d.velocity.x < targetVelocityX) {
			velocity.x = targetVelocityX;
		}

		if (collidingWithAreaEffector) {
			velocity.x += effectorSpeed;
		}

		float accelerationRate = 2.8f;
		float accelerationRateAirbourne = 2.8f;

		if (wallCollider.isCollidingWithWall ()) {
			velocity.x = 0.0f;
		}

		if (Settings.sfxEnabled) {
			if (velocity.x == 0.0f || playerInput.x == 0.0f|| !grounded) {
				runAudioSource.Stop ();
			} else {
				if (!runAudioSource.isPlaying && playerInput.x != 0.0f) {
					runAudioSource.Play ();
				}
			}
		}

		//player has changed directions so reset velocity
		if (playerInput.x != oldPlayerInput.x) {
			velocity.x = 0.0f;
		}

		rigidbody2d.velocity = velocity;
		Vector2 force = new Vector2 (targetVelocityX * (grounded ? accelerationRate : accelerationRateAirbourne), 0f);
		rigidbody2d.AddForce(force);

		oldPlayerInput = playerInput;
	}

	void UpdateMagnetBehaviour() {
		if (magnetEnabled) {
			Collider2D[] collidingObjects  = Physics2D.OverlapCircleAll (transform.position, magnetRadius  * transform.localScale.x, collectableMask);
			foreach(Collider2D collider in collidingObjects) {
				if (collider.gameObject.name.StartsWith("Coin")) {
					collider.gameObject.transform.position = Vector3.MoveTowards (collider.gameObject.transform.position, transform.position, Time.deltaTime * magnetAttractForce);
				}

			}
		}
	}

	public void Translate(Vector2 translation) {
		Vector3 translation3 = new Vector3 (translation.x * (facingRight ? 1f : -1f),
		                                   translation.y);
		transform.Translate (translation3);
	}

	public bool IsJumping() {
		return !grounded;
	}

	private IEnumerator PopAnimationComplete() {
		yield return new WaitForSeconds(3.8f);
		animator.SetBool("blinking", true);
	}

	/***
	 * This sound effect gets triggered by the animator
	 */
	public void PlayBlinkSoundEffect() {
		AudioManager.PlaySound ("blink");
	}

	/***
	 * This is called by an event in the players PlayerPopping animation
	 */
	public void PopLeftLeg() {
		shadow.SetActive (false);
		inputManager.ShakeForDuration (0.2f);
		string soundToPlay = "pop8";
		AudioManager.PlaySound(soundToPlay);
		//		float yOffset = -1.7f;
		float xForce = 300f;
		float yForce = 300f;

		PopLeftLegSprite ();

		//add force and torque to the rigidbody when left leg pops
		float dir = (Random.Range (0f, 1f) < 0.5f) ? -1f : 1f;
		xForce = xForce * dir;
		rigidbody2d.freezeRotation = false;
		rigidbody2d.AddForce (new Vector2 (xForce, yForce));
		rigidbody2d.AddTorque (Random.Range (0f, 100f));

		AudioManager.PlaySound ("player-death");

		if(glidingEnabled) {
			cape.PopOff ();
		}

		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer sprite in renderers) {
			if (sprite.name == "Hat") {
				float popDir = IsFacingRight () ? 1f : -1f;
				PopItemOff (sprite, new Vector2 (100f * popDir, 250f), 40f);
			}
		}
	}

	public void PopLeftLegSprite() {
		if (!leftLegPopped) {
			//pop the left leg
			GameObject poppedObject = (GameObject)Instantiate (leg, leftLegPopPoint.position, Quaternion.identity);
			//reskin the popped off leg
			SpriteRenderer[] legRenderers = poppedObject.GetComponentsInChildren<SpriteRenderer> ();
			Sprite[] sprites = Resources.LoadAll<Sprite> ("skins/player/shoes");
			if (SelectedPlayerCustomisations.selectedShoes != null) {
				foreach (SpriteRenderer renderer in legRenderers) {
					if (renderer.name == "shoe-skin") {
						foreach (Sprite sprite in sprites) {
							if (sprite.name == SelectedPlayerCustomisations.selectedShoes + "Left") {
								renderer.sprite = sprite;
							}
						}
					}
				}
			}

			poppedObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (-200f, 100f));
			poppedObject.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (0f, -300f));
			leftLegPopped = true;
		}
	}

	/***
	 * This is called by an event in the players PlayerPopping animation
	 */
	public void PopMiddleSection() {
		inputManager.ShakeForDuration (0.2f);
		string soundToPlay = "pop10";
		AudioManager.PlaySound(soundToPlay);
	}

	/***
	 * This is called by an event in the players PlayerPopping animation
	 */
	public void PopBody() {
		inputManager.ShakeForDuration (0.2f);
		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
		foreach(SpriteRenderer sprite in renderers) {
			if (sprite.name == "Glasses") {
				PopItemOff(sprite, new Vector2 (-20f, 250f), 80f);
			}
				
			if (sprite.name == "left-eyebrow-skin") {
				PopItemOff(sprite, new Vector2 (-100f, 200f), 300f);
			}

			if (sprite.name == "right-eyebrow-skin") {
				PopItemOff(sprite, new Vector2 (200f, 200f), -150f);
			}
		}
		string soundToPlay = "pop8";
		AudioManager.PlaySound(soundToPlay);

		#if UNITY_IOS
		SocialServiceManager.GetInstance ().UnlockAchievement ("mmmmpopcorn");
		#endif
		#if UNITY_ANDROID
		SocialServiceManager.GetInstance ().UnlockAchievement (GPGSIds.achievement_mmmm_popcorn);
		#endif
	}

	private void PopItemOff(SpriteRenderer sprite, Vector2 popForce, float angularForce) {
		sprite.transform.parent = null;
		sprite.sortingLayerName = "Foreground";
		Rigidbody2D rigidbody = sprite.gameObject.AddComponent<Rigidbody2D> ();
		rigidbody.AddForce (popForce);
		rigidbody.AddTorque (angularForce);
	}

	/***
	 * This is called by an event in the players PlayerPopping animation
	 */
	public void PopRightLeg() {
		string soundToPlay = "pop3";
		inputManager.ShakeForDuration (0.2f);
		AudioManager.PlaySound(soundToPlay);

		float xForce = -300f;
		float yForce = 300f;
		
		PopRightLegSprite ();
		
		//add force and torque to the rigidbody when left leg pops
		float dir = (Random.Range (0f, 1f) < 0.5f) ? -1f : 1f;
		xForce = xForce * dir;
		rigidbody2d.freezeRotation = false;
		rigidbody2d.AddForce (new Vector2 (xForce, yForce));
		rigidbody2d.AddTorque (Random.Range (0f, -100f));

		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer sprite in renderers) {
			if (sprite.name == "FacialHair") {
				float popDir = IsFacingRight () ? 1f : -1f;
				PopItemOff (sprite, new Vector2 (100f * popDir, 250f), -60f);
			}
		}
	}

	public void PopRightLegSprite() {
		if(!rightLegPopped) {
			//this time is dependent on the animation
			GameObject poppedObject = (GameObject)Instantiate (leg, rightLegPopPoint.position, gameObject.transform.rotation);

			//reskin the popped off leg
			SpriteRenderer[] legRenderers = poppedObject.GetComponentsInChildren<SpriteRenderer> ();
			Sprite[] sprites = Resources.LoadAll<Sprite> ("skins/player/shoes");
			if (SelectedPlayerCustomisations.selectedShoes != null) {
				foreach (SpriteRenderer renderer in legRenderers) {
					if (renderer.name == "shoe-skin") {
						foreach (Sprite sprite in sprites) {
							if (sprite.name == SelectedPlayerCustomisations.selectedShoes + "Right") {
								renderer.sprite = sprite;
							}
						}
					}
				}
			}

			poppedObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (200f, 100f));
			poppedObject.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (0f, 300f));
			rightLegPopped = true;
		}
	}

	//called by the animation
	public void Attack() {
		//do a physics detect for breakable layer
		//run break on the object
		Collider2D collider = Physics2D.OverlapCircle (attackPosition.position, attackRadius  * transform.localScale.x, breakableMask);

		if (collider == null) {
			return;
		}

		Breakable breakableObject = collider.gameObject.GetComponent<Breakable> ();
		if (breakableObject == null) {
			return;
		}

		breakableObject.Break (attackPosition.position);

		//this reset is for the scenario where the player is running into a wall and have stopped.
		//the wall breaks but the physics event to say the player has stopped colliding with the wall doesn't trigger
		//so the player is stuck in a situation where they cannot move after a wall is broken
		wallCollider.Reset ();
	}

	public Vector2 GetVelocity() {
		return rigidbody2d.velocity;
	}

	public bool IsFacingRight() {
		return facingRight;
	}

	/***
	 * Flip the direction of the sprite by flipping the entire object
	 */
	void Flip() {
		facingRight = !facingRight;
		if(facingRight)transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);  
		else transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);  
	}
	
	public void ResetTemperature() {
		if (temperature < 1.0f) {
			temperature = 0.0f;
		}
	}
	
	public void CollisionWithSnowflake() {
		incTemperature (SNOWFLAKE_TEMPERATURE_DECREASE);
	}
	
	public void Die() {
		temperature = MAX_TEMPERATURE;
	}
	
	public void CollisionWithHazardousEnvironment(string hazardousEnvName) {
		lastCollisionName = hazardousEnvName;
		if (invincibilityTimeLeft <= 0.0f) {
			inputManager.ShakeForDuration (0.2f);
			animator.SetTrigger("hurt");
			inputManager.ShowDamageIndicator ();
			AudioManager.PlaySound ("sizzle");
			if (CurrentLevel.GetLevelDifficulty () == CurrentLevel.LevelDifficulty.EASY) {
				incTemperature (HAZARD_COLLISION_TEMPERATURE_INCREASE / 8);
			} else {
				incTemperature (HAZARD_COLLISION_TEMPERATURE_INCREASE);
			}
			invincibilityTimeLeft = MAX_INVINCIBILITY_TIME;
		}
	}
	
	public void CollisionWithEnemy(string enemyName) {
		CollisionWithEnemy (enemyName, ENEMY_COLLISION_TEMPERATURE_INCREASE);
	}

	/***
	 * Make the player invulnerable for an amount of time
	 */
	public void MakeInvincibleForTime(float invincibilityTime) {
		invincibilityTimeLeft = invincibilityTime;
	}

	public void CollisionWithEnemy(string enemyName, float suggestedTemperatureIncrease) {
		CollisionWithEnemy (enemyName, suggestedTemperatureIncrease, true);
	}

	public void CollisionWithEnemy(string enemyName, float suggestedTemperatureIncrease, bool disablePlayerMovement) {
		lastCollisionName = enemyName;
		if (invincibilityTimeLeft <= 0.0f) {
			inputManager.ShowDamageIndicator ();
			inputManager.ShakeForDuration (0.2f);
			AudioManager.PlaySound ("sizzle");
			animator.SetTrigger("hurt");

			if (CurrentLevel.GetLevelDifficulty () == CurrentLevel.LevelDifficulty.EASY) {
				incTemperature (suggestedTemperatureIncrease / 2);
			} else {
				incTemperature (suggestedTemperatureIncrease);
			}

			//make the player invincible for a short period of time
			invincibilityTimeLeft = MAX_INVINCIBILITY_TIME;
		}
		if (disablePlayerMovement) {
			playerMovementEnabled = false;
			StartCoroutine (EnablePlayerMovement ());
		}
	}

	IEnumerator EnablePlayerMovement() {
		yield return new WaitForSeconds(0.5f);
		playerMovementEnabled = true;
		kicking = false;
	}
	
	private void incTemperature(float t) {
		temperature += (t / MAX_TEMPERATURE);
		temperature = Mathf.Clamp (temperature, 0f, MAX_TEMPERATURE);
	}

	public void SetTemperatureUpdateRate(string enemyName, float tempUpdateRate) {
		lastCollisionName = enemyName;
		temperatureUpdateRate = tempUpdateRate;
	}

	public float GetTemperature() {
		return temperature;
	}

	void OnDrawGizmos() {
		Color myColor = new Color (1f, 0f, 0f, 0.1f);
		Gizmos.color = myColor;
		Gizmos.DrawSphere (attackPosition.position, attackRadius * transform.localScale.x);
		foreach (Transform check in groundCheck) {
			Gizmos.DrawSphere (check.position, groundRadius * transform.localScale.x);
		}
		Gizmos.DrawSphere (transform.position, magnetRadius * transform.localScale.x);
	}

	public void IncrementCoinCount(int coinId) {
		collectedCoins.Add (coinId);
		inputManager.StartCoinCollectedAnimation ();
		inputManager.coinCountText.text = collectedCoins.Count.ToString ();
	}

	public void LevelComplete() {
		if (!Settings.sfxEnabled) {
			return;
		}
		runAudioSource.Stop ();
		runAudioSource.volume = 0.0f;
	}

	public bool IsGliding() {
		return gliding;
	}

	public bool IsGrounded() {
		return grounded;
	}

	public List<int> GetCollectedCoins() {
		return collectedCoins;
	}

	/***
	 * Add a life lost for this level. This method prevents multiple lives being lost.
	 */
	public void AddLifeLost() {
		if (!lifeAlreadyLost) {
			CurrentLevel.AddLivesLost(1);
			lifeAlreadyLost = true;
		}
	}

	public void DisableCollider() {
		GetComponent<BoxCollider2D> ().enabled = false;
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
