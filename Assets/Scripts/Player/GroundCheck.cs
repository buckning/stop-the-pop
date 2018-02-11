using UnityEngine;

public class GroundCheck : CollisionChecker {

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
