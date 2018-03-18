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
	private float attackRadius = 0.8f;

	public Transform ceilingCheck;
	public Transform attackPosition;
	public LayerMask whatIsCeilingMask;

	public PlayerWallTrigger wallCollider;

	public PopcornKernelAnimator popcornKernelAnimator;

	public float regularTemperatureUpdateRate = 0.02f;	//the regular temperature update of the player

	public Magnet magnet;

	public delegate void NotifyEvent ();
	public event NotifyEvent popcornKernelHurtListeners;
	public event NotifyEvent popcornKernelHealListeners;
	public event NotifyEvent popcornKernelRestartListeners;
	public event NotifyEvent popcornKernelStartPoppingListeners;

	private PopcornKernel popcornKernel;
	private Rigidbody2D rigidbody2d;

	private LayerMask breakableMask;

	private bool facingRight = true;
	private bool grounded = true;
	private bool glidingEnabled = false;
	private float MAX_INVINCIBILITY_TIME = 1.5f;

	public void Start() {
		breakableMask = 1 << LayerMask.NameToLayer ("Breakable");
	}

	public void Init() {
		if(popcornKernel == null) {
			rigidbody2d = GetComponent<Rigidbody2D> ();
			GroundCheck groundCollisionChecker = new GroundCheck (groundCheck, groundRadius * transform.localScale.x, whatIsGround);
			WallCollisionCheck wallCollisionChecker = new WallCollisionCheck (wallCollider);
			CeilingCollisionCheck ceilingCollisionCheck = new CeilingCollisionCheck (ceilingCheck, whatIsCeilingMask);

			popcornKernel = new PopcornKernel (inputManager, groundCollisionChecker, wallCollisionChecker, ceilingCollisionCheck, minJumpHeight, maxJumpHeight, timeToJumpApex);
			popcornKernel.jumpListeners += popcornKernelAnimator.Jump;
			popcornKernel.popEventListeners += popcornKernelAnimator.StartPopping;
			popcornKernel.popEventListeners += StartPopping;
			popcornKernel.kickEventListeners += popcornKernelAnimator.Kick;
			popcornKernelAnimator.kickListeners += popcornKernel.StopKicking;
			popcornKernelAnimator.kickListeners += Kick;
			popcornKernel.landEventListeners += popcornKernelAnimator.Land;
//			popcornKernel.crushEventListeners += Crush;
//			popcornKernelAnimator.popEventListeners += ShakeScreen;
			popcornKernelAnimator.popLeftLegEventListeners += PopLeftLeg;
			popcornKernelAnimator.popRightLegEventListeners += PopRightLeg;
			popcornKernelAnimator.popCompleteListeners += DisableCollider;
			popcornKernelAnimator.finishedPoppingListeners += Restart;
		}
	}

	public void EnableGliding(bool enabled) {
		glidingEnabled = enabled;
		popcornKernelAnimator.EnableCape (enabled);
		popcornKernel.EnableGliding (enabled);
	}

	private void StartPopping() {
		if (popcornKernelStartPoppingListeners != null) {
			popcornKernelStartPoppingListeners ();
		}
	}

	private void Restart() {
		if (popcornKernelRestartListeners != null) {
			popcornKernelRestartListeners ();
		}
	}

	private void PopLeftLeg() {
		PopLeg (new Vector2 (300f, 300f), 100f);
	}

	private void PopRightLeg() {
		PopLeg (new Vector2 (-300f, 300f), -100f);
	}

	private void PopLeg(Vector2 force, float maxTorque) {
		float dir = (Random.Range (0f, 1f) < 0.5f) ? -1f : 1f;
		force.x = force.x * dir;
		rigidbody2d.freezeRotation = false;
		rigidbody2d.AddForce (force);
		rigidbody2d.AddTorque (Random.Range (0f, maxTorque));
	}

	public void ResetTemperature() {
		popcornKernel.ResetTemperature ();
		if (popcornKernelHealListeners != null) {
			popcornKernelHealListeners ();
		}
	}

	public bool IsGrounded() {
		return popcornKernel.IsGrounded ();
	}

	/***
	 * This is for the platform movement. When the platform moves, this is called by the platform.
	 */
	public void Translate(Vector2 translation) {
		Vector3 translation3 = new Vector3 (translation.x * (facingRight ? 1f : -1f),
			translation.y);
		transform.Translate (translation3);
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

	public void EnableMagnet(bool enabled) {
		magnet.gameObject.SetActive (enabled);
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

	/***
	 * Performs a physics detection around the attack position for objects with Breakable layer.
	 * The break method is then called on the object
	 */
	private void Kick() {
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

	private void UpdateAnimator() {
		popcornKernelAnimator.SetVelocityX (Mathf.Abs(popcornKernel.GetVelocity().x));
		popcornKernelAnimator.SetVelocityY (rigidbody2d.velocity.y);
		popcornKernelAnimator.SetGrounded (grounded);
		popcornKernelAnimator.SetGliding (popcornKernel.IsGliding ());
	}

	/***
	 * Flip the popcorn kernel. The flip is performed by a y-axis rotation
	 */
	private void Flip() {
		facingRight = !facingRight;
		if(facingRight)transform.rotation = Quaternion.Euler(transform.rotation.x, 0, transform.rotation.z);  
		else transform.rotation = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);  
	}

	private void DisableCollider() {
		GetComponent<BoxCollider2D> ().enabled = false;
	}
}
