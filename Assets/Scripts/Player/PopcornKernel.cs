using UnityEngine;

public class PopcornKernel {

	public delegate void Jump ();
	public event Jump jumpListeners;

	const int MAX_TEMPERATURE = 100;
	
	private int temperature = 0;
	private bool grounded = false;
	private bool groundedOverride = false;	// the point of this is to check if the player is running and falls off a platform, we want the player to be able to jump for a split second
	private bool gliding = false;	// track the state if the player is gliding
	private CollisionChecker groundCollisionChecker;

	private float gravity;
	private float minJumpVelocity;
	private float maxJumpVelocity;

	private InputManager inputManager;

	public PopcornKernel(InputManager inputManager, CollisionChecker groundCollisionChecker, float minJumpHeight, float maxJumpHeight, float timeToJumpApex) {
		this.groundCollisionChecker = groundCollisionChecker;
		this.inputManager = inputManager;
		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs(gravity) * minJumpHeight);
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
			//allow the player to jump for a short 
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

	public void Update() {
		grounded = groundCollisionChecker.isColliding ();
	}

	public void increaseTemperature(int temperatureDiff) {
		temperature += temperatureDiff;
		if (temperature > MAX_TEMPERATURE) {
			temperature = MAX_TEMPERATURE;
		} else if (temperature < 0) {
			temperature = 0;
		}
	}

	public int getTemperature() {
		return temperature;
	}

	public bool isGliding() {
		return gliding;
	}

	public bool IsGrounded() {
		return grounded;
	}
}
