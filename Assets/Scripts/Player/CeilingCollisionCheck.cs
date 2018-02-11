using UnityEngine;

public class CeilingCollisionCheck: CollisionChecker {
	private PlayerWallTrigger wallCheck;
	private float ceilingRadius = 0.1f;
	private LayerMask whatIsCeilingMask;
	private Transform ceilingCheck;

	public CeilingCollisionCheck(Transform ceilingCheck, LayerMask whatIsCeilingMask) {
		this.wallCheck = wallCheck;
		this.whatIsCeilingMask = whatIsCeilingMask;
		this.ceilingCheck = ceilingCheck;
	}

	public bool isColliding() {
		return Physics2D.OverlapCircle (this.ceilingCheck.position, this.ceilingRadius, this.whatIsCeilingMask);
	}
}
