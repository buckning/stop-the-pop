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

	public bool jumpEnabled = true;						//Is single jump enabled?
	public float regularTemperatureUpdateRate = 0.02f;	//the regular temperature update of the player
	float temperatureUpdateRate = 0.02f;			//the rate at which the temperature increases
	float SNOWFLAKE_TEMPERATURE_DECREASE = -30f;	//The amount of temperature to decrease by when collided with snowflake 
	float ENEMY_COLLISION_TEMPERATURE_INCREASE = 50f;	//The amount of temperature that increases when colliding with an enemy. 10f recommended
	float HAZARD_COLLISION_TEMPERATURE_INCREASE = 50f;	//The amount of temperature that increases when colliding with a hazardous environment. 10f recommended
	float MAX_INVINCIBILITY_TIME = 1.5f;			//the amount of time in seconds that the player will be invincible for if hit by an enemy/hazardous env
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

	public bool glidingEnabled = false;					//if this is set to false, the player cannot glide regardless if the player is trying to
	public const float defaultGlideSpeed = -2f;
	[HideInInspector]
	public float glideSpeed = defaultGlideSpeed;								//the velocity in the y axis when gliding

	public PlayerWallTrigger wallCollider;

	public Cape cape;

	public bool magnetEnabled = false;

	public bool playerMovementEnabled = true;

	List<int> collectedCoins = new List<int>();

	bool touchingCeiling = false;
	bool crushed = false;
	float glidingTime = 0.0f;

	bool lifeAlreadyLost = false;					//when the player loses a life, this is checked to see if a life has already been lost
													//this variable prevents multiple lives being lost in one level

	public PolygonCollider2D bodyCollider;	//this collider is just used by the saw animations

	public PopcornKernelAnimator popcornKernelAnimator;

	private PopcornKernel popcornKernel;

	class GroundCheck: CollisionChecker {
		private Transform[] groundCheck;
		private LayerMask whatIsGround;
		private float groundRadius;

		public GroundCheck(Transform[] groundCheck, float groundRadius, LayerMask whatIsGround) {
			this.groundCheck = groundCheck;
			this.whatIsGround = whatIsGround;
			this.groundRadius = groundRadius;
		}

		public bool isColliding() {
			foreach(Transform check in groundCheck) {
				if (Physics2D.OverlapCircle (check.position, groundRadius, whatIsGround)) {
					return true;
				}
			}
			return false;
		}
	}

	class WallCollisionCheck: CollisionChecker {
		private PlayerWallTrigger wallCheck;

		public WallCollisionCheck(PlayerWallTrigger wallCheck) {
			this.wallCheck = wallCheck;
		}

		public bool isColliding() {
			return wallCheck.isCollidingWithWall ();
		}
	}

	void Start () {
		GroundCheck groundCollisionChecker = new GroundCheck (groundCheck, groundRadius * transform.localScale.x, whatIsGround);
		WallCollisionCheck wallCollisionChecker = new WallCollisionCheck (wallCollider);
		popcornKernel = new PopcornKernel (inputManager, groundCollisionChecker, wallCollisionChecker, minJumpHeight, maxJumpHeight, timeToJumpApex);
		popcornKernel.jumpListeners += popcornKernelAnimator.Jump;
		popcornKernel.fallEventListeners += FallOff;
		popcornKernel.landEventListeners += popcornKernelAnimator.Land;
		popcornKernel.kickEventListeners += popcornKernelAnimator.Kick;
		popcornKernelAnimator.kickListeners += popcornKernel.StopKicking;
		popcornKernelAnimator.popEventListeners += ShakeScreen;

		oldPlayerInput = Vector2.zero;
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs(gravity) * minJumpHeight);

		rigidbody2d = GetComponent<Rigidbody2D> ();

		//add all the collected coins from the last checkpoint to this internal list. This is just so we can show an accurate coin count when a level is restarted
		List<int> coins = LastCheckpoint.GetCollectedCoins ();
		foreach (int coinId in coins) {
			collectedCoins.Add (coinId);
		}
	}

	/***
	 * Called back from the popcornKernel when it falls off something.
	 * The purpose of this is that the popcornKernel allows the player
	 * to jump after falling off something for a short period of time.
	 * This is purely for better user experience. The length of the
	 * period where the player can do this is controlled by this method.
	 */
	private void FallOff() {
		StartCoroutine(DisableGroundedOverride());
	}

	// Update is called at a fixed rate - this is better when interacting with physics objects (time.delta time is not needed here)
	public void FixedUpdate () {
		popcornKernel.FixedUpdate (rigidbody2d.velocity);
		grounded = popcornKernel.IsGrounded ();

		touchingCeiling = Physics2D.OverlapCircle (ceilingCheck.position, ceilingRadius  * transform.localScale.x, whatIsCeilingMask);

		UpdateMagnetBehaviour ();
	}

	public void PlaySawBladeDeathAnimation() {
		popcornKernelAnimator.PlaySawBladeDeathAnimation ();
	}

	public void PlayMovingSawBladeDeathAnimation() {
		popcornKernelAnimator.PlayMovingSawBladeDeathAnimation ();
	}

	public void EnableBodyCollider() {
		bodyCollider.enabled = true;
		rigidbody2d.freezeRotation = false;
		rigidbody2d.AddTorque (Random.Range (-100f, 100f));
	}

	public IEnumerator DisableGroundedOverride() {
		yield return new WaitForSeconds(0.1f);
		popcornKernel.DisableGroundedOverride ();
	}
	
	public void setPosition(Vector2 position) {
		gameObject.transform.position = position;
	}

	public void SetVelocity(Vector2 velocity) {
		rigidbody2d.velocity = velocity;
	}

	void Update() {
		playerInput = new Vector2 (inputManager.getXAxis(), inputManager.getYAxis());
		if (popcornKernel.IsAtMaxTemperature() || !playerMovementEnabled) {
			playerInput = Vector2.zero;
		} 
		rigidbody2d.velocity = popcornKernel.Update (rigidbody2d.velocity, Time.deltaTime);
		UpdateMagnetBehaviour ();

		CheckIfCrushed ();

		//update the way our sprite is facing.
		if (playerInput.x > 0 && !facingRight) {
			Flip ();
		} else if (playerInput.x < 0 && facingRight) {
			Flip();
		}

		popcornKernel.UpdateTemperature (Time.deltaTime);

		if (glidingEnabled) {
			Vector2 velocity = new Vector2(playerInput.x, rigidbody2d.velocity.y);
			if (grounded) {
				velocity.y = 0.0f;
			}

			cape.SetGliding (gliding);
		} 
		UpdateAnimations ();
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
		popcornKernelAnimator.SetVelocityX (animationSpeed);
		popcornKernelAnimator.SetVelocityY (rigidbody2d.velocity.y);
		popcornKernelAnimator.SetGrounded (grounded);

		//check to see if we should start our pop animation 
		if (popcornKernel.IsAtMaxTemperature() && !popped && grounded) {
			magnetEnabled = false;
			AddLifeLost ();

			//start the popping animation
			//start the animator
			popcornKernelAnimator.StartPopping();
			popped = true;

			AnalyticsManager.SendDeathEvent (inputManager.levelName, transform.position, lastCollisionName);

			lastCollisionName = "";
			
			//maybe slow down the time scale here???
			//apply a force after one second
		}

		if (popcornKernel.IsAtMaxTemperature()) {
			gliding = false;
		}
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
		if (!jumpEnabled || !playerMovementEnabled) {
			return;
		}

		Vector2 velocity = popcornKernel.CheckForJump (rigidbody2d.velocity);
//		cape.SetVelocity (velocity);
		rigidbody2d.velocity = velocity;
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

	private void ShakeScreen() {
		inputManager.ShakeForDuration (0.2f);
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
		popcornKernel.ResetTemperature ();
	}
	
	public void CollisionWithSnowflake() {
		popcornKernel.increaseTemperature (SNOWFLAKE_TEMPERATURE_DECREASE);
	}
	
	public void CollisionWithHazardousEnvironment(string hazardousEnvName) {
		lastCollisionName = hazardousEnvName;
		if (popcornKernel.GetInvincibleTime() <= 0.0f) {
			inputManager.ShakeForDuration (0.2f);
			inputManager.ShowDamageIndicator ();
			AudioManager.PlaySound ("sizzle");
			popcornKernel.increaseTemperature (HAZARD_COLLISION_TEMPERATURE_INCREASE);
			popcornKernel.MakeInvincibleForTime (MAX_INVINCIBILITY_TIME);
		}
	}
	
	public void CollisionWithEnemy(string enemyName) {
		CollisionWithEnemy (enemyName, ENEMY_COLLISION_TEMPERATURE_INCREASE);
	}

	public void SetUpdateTemperature(bool status) {
		popcornKernel.SetUpdateTemperature (status);
	}

	public bool GetUpdateTemperature() {
		return popcornKernel.GetUpdateTemperature ();
	}

	/***
	 * Make the player invulnerable for an amount of time
	 */
	public void MakeInvincibleForTime(float invincibilityTime) {
		popcornKernel.MakeInvincibleForTime(invincibilityTime);
	}

	public void CollisionWithEnemy(string enemyName, float suggestedTemperatureIncrease) {
		CollisionWithEnemy (enemyName, suggestedTemperatureIncrease, true);
	}

	public void CollisionWithEnemy(string enemyName, float suggestedTemperatureIncrease, bool disablePlayerMovement) {
		lastCollisionName = enemyName;
		if (popcornKernel.GetInvincibleTime() <= 0.0f) {
			inputManager.ShowDamageIndicator ();
			inputManager.ShakeForDuration (0.2f);
			AudioManager.PlaySound ("sizzle");

			popcornKernel.increaseTemperature (suggestedTemperatureIncrease);
			popcornKernel.MakeInvincibleForTime (MAX_INVINCIBILITY_TIME);
		}
		if (disablePlayerMovement) {
			playerMovementEnabled = false;
			StartCoroutine (EnablePlayerMovement ());
		}
	}

	IEnumerator EnablePlayerMovement() {
		yield return new WaitForSeconds(0.5f);
		playerMovementEnabled = true;
	}

	public void SetTemperatureUpdateRate(string enemyName, float tempUpdateRate) {
		lastCollisionName = enemyName;
		popcornKernel.SetUpdateTemperatureUpdateRate (tempUpdateRate);
	}

	public float GetTemperature() {
		return popcornKernel.getTemperature ();
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
}
