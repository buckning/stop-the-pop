﻿using UnityEngine;

public class PopcornKernel {

	public delegate void NotifyEvent ();
	public event NotifyEvent jumpListeners;
	public event NotifyEvent fallEventListeners;	// listener that gets triggered when the kernel falls off an object
	public event NotifyEvent landEventListeners;	// listener that gets triggered when the kernel lands on an object
	public event NotifyEvent kickEventListeners;	// listener that gets triggered when the kernel triggers a kick

	const float MAX_TEMPERATURE = 100.0f;
	
	private float temperature = 0.0f;
	private float moveSpeed = 14f;
	private float accelerationRate = 2.8f;
	private float accelerationRateAirbourne = 2.8f;
	private bool grounded = false;
	private bool kicking = false;
	private bool groundedOverride = false;	// the point of this is to check if the player is running and falls off a platform, we want the player to be able to jump for a split second
	private bool gliding = false;	// track the state if the player is gliding
	private CollisionChecker groundCollisionChecker;
	private CollisionChecker wallCollisionChecker;

	private float gravity;
	private float minJumpVelocity;
	private float maxJumpVelocity;
	private float invincibilityTimeLeft = 0.0f;
	private bool updateTemperature = false;
	private float temperatureUpdateRate = 40f;			//the rate at which the temperature increases

	private InputManager inputManager;

	private bool facingRight = false;

	public PopcornKernel(InputManager inputManager, CollisionChecker groundCollisionChecker, CollisionChecker wallCollisionChecker, float minJumpHeight, float maxJumpHeight, float timeToJumpApex) {
		this.groundCollisionChecker = groundCollisionChecker;
		this.wallCollisionChecker = wallCollisionChecker;
		this.inputManager = inputManager;
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs(gravity) * minJumpHeight);
	}

	public Vector2 Update(Vector2 velocity, float deltaTime) {
		if (IsKickTriggered () && kickEventListeners != null) {
			kickEventListeners ();
		}
		velocity = CheckForJump (velocity);
		velocity = UpdateVelocity (velocity, deltaTime);

		return velocity;
	}

	public Vector2 UpdateVelocity(Vector2 velocity, float deltaTime) {
		float xInput = inputManager.getXAxis();

		velocity.y += gravity * deltaTime;

		velocity.x = xInput * moveSpeed;
		velocity.x = Mathf.Clamp (velocity.x, -moveSpeed, moveSpeed); 

		if (kicking || xInput == 0.0f || wallCollisionChecker.isColliding ()) {
			velocity.x = 0.0f;
		}

		return velocity;
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
	public Vector2 CheckForJump(Vector2 velocity) {	
		if (temperature >= MAX_TEMPERATURE) {
			return velocity;
		}

		if (inputManager.JumpKeyDown()) {
			if (groundedOverride) {
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
		return velocity;
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

	public void DisableGroundedOverride() {
		groundedOverride = false;
	}

	public void FixedUpdate(Vector2 velocity) {
		bool oldGrounded = grounded;
		grounded = groundCollisionChecker.isColliding ();

		if (grounded) {
			groundedOverride = false;
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
					groundedOverride = true;

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

		if (updateTemperature && invincibilityTimeLeft == 0.0f) {
			temperature += deltaTime * temperatureUpdateRate;
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
		temperature += temperatureDiff;
		temperature = Mathf.Clamp (temperature, 0.0f, MAX_TEMPERATURE);
	}

	public void Die() {
		temperature = MAX_TEMPERATURE;
	}

	public void ResetTemperature() {
		temperature = 0.0f;
	}

	public float getTemperature() {
		return temperature;
	}

	public bool isGliding() {
		return gliding;
	}

	public bool IsGrounded() {
		return grounded;
	}
}
