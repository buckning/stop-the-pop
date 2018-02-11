using UnityEngine;

public class WallCollisionCheck: CollisionChecker {
	private PlayerWallTrigger wallCheck;

	public WallCollisionCheck(PlayerWallTrigger wallCheck) {
		this.wallCheck = wallCheck;
	}

	public bool isColliding() {
		return wallCheck.isCollidingWithWall ();
	}
}
