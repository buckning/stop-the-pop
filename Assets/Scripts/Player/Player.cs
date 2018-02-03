using UnityEngine;
using System.Collections;

/***
 * Adapted player code from Sebastian Lague
 * https://www.youtube.com/watch?v=MbWK8bCAU2w
 */
[RequireComponent (typeof(Controller2D))]
public class Player : MonoBehaviour {

	public float maxJumpHeight = 4f;
	public float minJumpHeight = 1f;
	public float timeToJumpApex = 0.4f;	//time to reach the jump height

	public float wallSlideSpeedMax = 3f;

	public Vector2 wallJumpClimb;
	public Vector2 wallJumpOff;
	public Vector2 wallLeap;

	public float moveSpeed = 20f;
	float gravity;
	float maxJumpVelocity;
	float minJumpVelocity;
	float velocityXSmoothing;
	float accelerationTimeAirbourne = 0.2f;	//our acceleration speed when we are in the air
	float accelerationTimeGrounded = 0.1f;
	[HideInInspector]
	public Vector3 velocity;

	//our input manager
	public HudListener inputManager;

	Controller2D controller;

	float wallStickTime  = 0.25f;
	float timeToWallUnstick;
	
	public virtual void Start () {
		controller = GetComponent<Controller2D> ();

		gravity = -(2 * maxJumpHeight) / Mathf.Pow (timeToJumpApex, 2);
		maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
		minJumpVelocity = Mathf.Sqrt (2 * Mathf.Abs(gravity) * minJumpHeight);
	}
	
	public virtual void Update () {
		
		Vector2 input = new Vector2 (inputManager.getXAxis(), inputManager.getYAxis());
		int wallDirX = (controller.collisions.left) ? -1 : 1;

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below ? accelerationTimeGrounded : accelerationTimeAirbourne));


		bool wallSliding = false;

		if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0) {
			wallSliding = true;

			if(velocity.y < -wallSlideSpeedMax) {
				velocity.y = -wallSlideSpeedMax;
			}

			if(timeToWallUnstick > 0) {
				velocity.x = 0f;
				velocityXSmoothing = 0f;
				if(input.x != wallDirX && input.x != 0) {
					timeToWallUnstick -= Time.deltaTime;
				}
				else {
					timeToWallUnstick = wallStickTime;
				}
			}
			else {
				timeToWallUnstick = wallStickTime;
			}
		}

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		if (inputManager.JumpKeyDown()) {
			if(wallSliding) {
				if(wallDirX == input.x) {
					velocity.x = -wallDirX * wallJumpClimb.x;
					velocity.y = wallJumpClimb.y;
				}
				else if(input.x == 0) {
					velocity.x = -wallDirX * wallJumpOff.x;
					velocity.y = wallJumpOff.y;
				}
				else {
					velocity.x = -wallDirX * wallLeap.x;
					velocity.y = wallLeap.y;
				}
			}
			if(controller.collisions.below) {
				velocity.y = maxJumpVelocity;
			}
		}

		if (inputManager.JumpKeyUp()) {
			if(velocity.y > minJumpVelocity) {
				velocity.y = minJumpVelocity;
			}
		}


		velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime, input);

		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}
	}
}
