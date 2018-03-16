using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopcornKernelController : MonoBehaviour {

	public float maxJumpHeight = 8f;
	public float minJumpHeight = 0.4f;
	public float timeToJumpApex = 1f;

	public InputManager inputManager;
	public Transform[] groundCheck;
	public LayerMask whatIsGround;
	private float groundRadius = 0.3f;

	public Transform ceilingCheck;
	public LayerMask whatIsCeilingMask;

	public PlayerWallTrigger wallCollider;

	private PopcornKernel popcornKernel;
	public PopcornKernelAnimator popcornKernelAnimator;

	private Rigidbody2D rigidbody2d;

	private bool facingRight = true;
	private bool grounded = true;
	private float MAX_INVINCIBILITY_TIME = 1.5f;

	public float regularTemperatureUpdateRate = 0.02f;	//the regular temperature update of the player

	public delegate void NotifyEvent ();
	public event NotifyEvent popcornKernelHurtListeners;
	public event NotifyEvent popcornKernelHealListeners;

	public void Init() {




		if(popcornKernel == null) {
			rigidbody2d = GetComponent<Rigidbody2D> ();
			GroundCheck groundCollisionChecker = new GroundCheck (groundCheck, groundRadius * transform.localScale.x, whatIsGround);
			WallCollisionCheck wallCollisionChecker = new WallCollisionCheck (wallCollider);
			CeilingCollisionCheck ceilingCollisionCheck = new CeilingCollisionCheck (ceilingCheck, whatIsCeilingMask);

			popcornKernel = new PopcornKernel (inputManager, groundCollisionChecker, wallCollisionChecker, ceilingCollisionCheck, minJumpHeight, maxJumpHeight, timeToJumpApex);
			popcornKernel.jumpListeners += popcornKernelAnimator.Jump;
			popcornKernel.popEventListeners += Pop;
			popcornKernel.kickEventListeners += popcornKernelAnimator.Kick;
			popcornKernelAnimator.kickListeners += popcornKernel.StopKicking;
//			popcornKernel.fallEventListeners += FallOff;
//					popcornKernel.landEventListeners += popcornKernelAnimator.Land;

//					popcornKernel.crushEventListeners += Crush;
					

//					popcornKernelAnimator.popEventListeners += ShakeScreen;
//					popcornKernelAnimator.popLeftLegEventListeners += PopLeftLeg;
//					popcornKernelAnimator.popRightLegEventListeners += PopRightLeg;
//					popcornKernelAnimator.popCompleteListeners += DisableCollider;
//					popcornKernelAnimator.finishedPoppingListeners += RetryLevelAfterPopAnimation;
		}
	}

	void Pop() {
		popcornKernelAnimator.StartPopping();
	}

	public void ResetTemperature() {
		popcornKernel.ResetTemperature ();
		if (popcornKernelHealListeners != null) {
			popcornKernelHealListeners ();
		}
	}

	public void CollisionWithEnemy(float suggestedTemperatureIncrease) {
		if (popcornKernel.GetInvincibleTime() <= 0.0f) {

			if (popcornKernelHurtListeners != null) {
				popcornKernelHurtListeners ();
			}

			AudioManager.PlaySound ("sizzle");

			popcornKernel.increaseTemperature (suggestedTemperatureIncrease);
			popcornKernel.MakeInvincibleForTime (MAX_INVINCIBILITY_TIME);
		}
//		if (disablePlayerMovement) {
//			playerMovementEnabled = false;
//			StartCoroutine (EnablePlayerMovement ());
//		}
	}

	public bool GetUpdateTemperature() {
		return popcornKernel.GetUpdateTemperature ();
	}

	public void SetTemperatureUpdateRate(float updateRate) {
		popcornKernel.SetUpdateTemperatureUpdateRate (updateRate);
	}

	public void SetUpdateTemperature(bool status) {
		popcornKernel.SetUpdateTemperature (status);
	}

	public int GetTemperature() {
		return (int) popcornKernel.GetTemperature ();
	}

	public void FixedUpdate () {
		popcornKernel.FixedUpdate (rigidbody2d.velocity);
		grounded = popcornKernel.IsGrounded ();
	}

	void Update () {
		popcornKernel.Update (rigidbody2d.velocity, Time.deltaTime);
		UpdateAnimator ();
		rigidbody2d.velocity = popcornKernel.GetVelocity ();

		if (inputManager.GetXAxis() > 0 && !facingRight) {
			Flip ();
		} else if (inputManager.GetXAxis() < 0 && facingRight) {
			Flip();
		}
	}

	private void UpdateAnimator() {
		popcornKernelAnimator.SetVelocityX (Mathf.Abs(popcornKernel.GetVelocity().x));
		popcornKernelAnimator.SetVelocityY (rigidbody2d.velocity.y);
		popcornKernelAnimator.SetGrounded (grounded);
	}

	private void Flip() {
		facingRight = !facingRight;
		if(facingRight)transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);  
		else transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);  
	}
}
