using UnityEngine;

public class PopcornKernel {

	public delegate void NotifyEvent ();
	public event NotifyEvent jumpListeners;
	public event NotifyEvent fallEventListeners;	// listener that gets triggered when the kernel falls off an object
	public event NotifyEvent landEventListeners;	// listener that gets triggered when the kernel lands on an object
	public event NotifyEvent kickEventListeners;	// listener that gets triggered when the kernel triggers a kick
	public event NotifyEvent crushEventListeners;	// listener that gets triggered when the kernel gets crushed
	public event NotifyEvent popEventListeners;		// Listener that gets triggered when the kernel pops

	const float MAX_TEMPERATURE = 100.0f;
	
	private float temperature = 0.0f;
	private float moveSpeed = 14f;
	private float accelerationRate = 2.8f;
	private float accelerationRateAirbourne = 2.8f;
	private float glideSpeed = -2f;
	private bool grounded = false;
	private bool crushed = false;
	private bool kicking = false;
	private bool gliding = false;	// track the state if the player is gliding
	private CollisionChecker groundCollisionChecker;
	private CollisionChecker wallCollisionChecker;
	private CollisionChecker ceilingCollisionChecker;

	private float gravity;
	private float minJumpVelocity;
	private float maxJumpVelocity;
	private float invincibilityTimeLeft = 0.0f;
	private bool updateTemperature = false;
	private float temperatureUpdateRate = 40f;			//the rate at which the temperature increases

	private InputManager inputManager;

	private bool facingRight = false;

	private Vector2 velocity;
	private bool glidingEnabled = false;

	private float groundedOverrideTimeLeft = 0.0f; // the point of this is to check if the player is running and falls off a platform, we want the player to be able to jump for a split second

	public PopcornKernel(InputManager inputManager, CollisionChecker groundCollisionChecker, 
				CollisionChecker wallCollisionChecker, CollisionChecker ceilingCollisionChecker, 
				float minJumpHeight, float maxJumpHeight, float timeToJumpApex) {

		this.groundCollisionChecker = groundCollisionChecker;
		this.wallCollisionChecker = wallCollisionChecker;
		this.ceilingCollisionChecker = ceilingCollisionChecker;
		this.inputManager = inputManager;
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs(gravity) * minJumpHeight);
		velocity = Vector2.zero;
	}

	public void Update(Vector2 currentVelocity, float deltaTime) {
		if (IsKickTriggered () && kickEventListeners != null) {
			kickEventListeners ();
		}
		this.velocity = currentVelocity;
		CheckForJump ();
		UpdateVelocity (deltaTime);
		UpdateTemperature(deltaTime);
		CheckIfCrushed ();

		groundedOverrideTimeLeft = Mathf.Clamp (groundedOverrideTimeLeft - deltaTime, 0.0f, 0.1f);
	}

	public void EnableGliding(bool enabled) {
		this.glidingEnabled = enabled;
	}

	public Vector2 GetVelocity() {
		return this.velocity;
	}

	public void UpdateVelocity(float deltaTime) {
		float xInput = inputManager.GetXAxis();

		if(gliding && glidingEnabled && velocity.y <= 0f) {
			velocity.y = glideSpeed;
		} else {
			velocity.y += gravity * deltaTime;
		}

		velocity.x = xInput * moveSpeed;
		velocity.x = Mathf.Clamp (velocity.x, -moveSpeed, moveSpeed); 

		if (kicking || xInput == 0.0f || wallCollisionChecker.isColliding ()) {
			velocity.x = 0.0f;
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
		if (temperature >= MAX_TEMPERATURE) {
			return;
		}

		if (inputManager.JumpKeyDown()) {
			if (groundedOverrideTimeLeft > 0.0f) {
				grounded = true;
			}

			if (grounded) {
				velocity.y = maxJumpVelocity;

				if (jumpListeners != null) {
					jumpListeners ();
				}
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
	}

	private void CheckIfCrushed() {
		if (ceilingCollisionChecker.isColliding() && grounded 
			&& !crushed && crushEventListeners != null) {
			crushEventListeners ();
			crushed = true;	
		}
	}

	private bool IsKickTriggered() {
		if (inputManager.AttackKeyPressed () && !kicking) {
			if (grounded) {
				kicking = true;
				return kicking;
			}
		}
		return false;
	}

	public void StopKicking() {
		kicking = false;
	}

	public void FixedUpdate(Vector2 velocity) {
		bool oldGrounded = grounded;
		grounded = groundCollisionChecker.isColliding ();

		if (grounded) {
			groundedOverrideTimeLeft = 0.0f;


			gliding = false;
			// if we were not grounded last frame and are grounded this frame, we have just landed
			if (!oldGrounded) {
				if (landEventListeners != null) {
					landEventListeners ();
				}
			}
		} else {
			// the player has just fallen off a platform, we want to give the player a chance to jump for a short period after falling off the platform
			if (oldGrounded) {
				if(velocity.y < 0.0f) {
					groundedOverrideTimeLeft = 0.1f;

					if (fallEventListeners != null) {
						fallEventListeners ();
					}
				}
			}
		}
	}

	public bool IsAtMaxTemperature() {
		return temperature >= 100;
	}

	public void SetUpdateTemperature(bool status) {
		updateTemperature = status;
	}

	public bool GetUpdateTemperature() {
		return updateTemperature;
	}


	public void SetUpdateTemperatureUpdateRate(float updateRate) {
		temperatureUpdateRate = updateRate;
	}

	public void UpdateTemperature(float deltaTime) {
		invincibilityTimeLeft -= deltaTime;
		if (invincibilityTimeLeft < 0.0f) {
			invincibilityTimeLeft = 0.0f;
		}

		float oldTemperature = temperature;

		if (updateTemperature && invincibilityTimeLeft == 0.0f) {
			temperature += deltaTime * temperatureUpdateRate;
			temperature = Mathf.Clamp (temperature, 0.0f, MAX_TEMPERATURE);
		}

		if (temperature == MAX_TEMPERATURE && oldTemperature != MAX_TEMPERATURE
				&& popEventListeners != null) {
			popEventListeners ();
		}
	}

	/***
	 * Make the player invulnerable for an amount of time
	 */
	public void MakeInvincibleForTime(float invincibilityTime) {
		invincibilityTimeLeft = invincibilityTime;
	}

	public float GetInvincibleTime() {
		return invincibilityTimeLeft;
	}

	public void increaseTemperature(float temperatureDiff) {
		float oldTemperature = temperature;

		temperature += temperatureDiff;
		temperature = Mathf.Clamp (temperature, 0.0f, MAX_TEMPERATURE);

		if (temperature == MAX_TEMPERATURE && oldTemperature != MAX_TEMPERATURE
			&& popEventListeners != null) {
			popEventListeners ();
		}
	}

	public void Die() {
		temperature = MAX_TEMPERATURE;
	}

	public void ResetTemperature() {
		temperature = 0.0f;
	}

	public float GetTemperature() {
		return temperature;
	}

	public bool IsGliding() {
		return gliding;
	}

	public bool IsGrounded() {
		return grounded;
	}
}
