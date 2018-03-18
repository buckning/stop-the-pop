using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Animator))]
public class Cape : MonoBehaviour {

	Vector2 velocity;
	bool gliding = false;

	Animator animator;

	void Start () {
		animator = GetComponent<Animator> ();
	}

	void Update () {
		animator.SetBool ("gliding", gliding);
		animator.SetFloat ("xVelocity", Mathf.Abs(velocity.x));
		animator.SetFloat ("yVelocity", velocity.y);
	}

	public void PopOff() {
		animator.SetTrigger ("popOff");
		AddRigidBody ();
	}

	void AddRigidBody() {
		Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
		rigidbody2D.velocity = new Vector2 (1, 3f);
		rigidbody2D.angularVelocity = 100f;
	}

	public void SetGliding(bool isGliding) {
//		if (isGliding && !gliding) {
//			AudioManager.PlaySound ("cape-new", Random.Range(0.7f, 1.2f));
//		}
		gliding = isGliding;
	}

	public void SetXVelocity(float xVel) {
		this.velocity.x = xVel;
	}

	public void SetYVelocity(float yVel) {
		this.velocity.y = yVel;
	}

	public void SetVelocity(Vector2 vel) {
		this.velocity = vel;
	}
}
