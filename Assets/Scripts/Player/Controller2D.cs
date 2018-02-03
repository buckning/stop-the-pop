using UnityEngine;
using System.Collections;

/***
 * Collider class. This detects collisions by generating a series of rays from the object both vertical and horizontal to the collider.
 */
public class Controller2D : RaycastController {

	public LayerMask collisionMask;

	float maxClimbAngle = 80f;			//the maximum angle we can climb up
	float maxDescendAngle = 75f;		//the max angle we can descend

	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;

	public override void Start() {
		base.Start ();
		collisions.faceDir = 1;
	}

	public void Move(Vector3 velocity, bool standingOnPlatform = false) {
		Move (velocity, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform = false) {
		UpdateRaycastOrigins ();
		collisions.Reset ();
		collisions.velocityOld = velocity;
		playerInput = input;

		if (velocity.x != 0) {
			collisions.faceDir = (int)Mathf.Sign(velocity.x);
		}

		if (velocity.y < 0) {
			DescendSlope(ref velocity);
		}


		HorizontalCollisions (ref velocity);

		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}

		transform.Translate (velocity);		//actually move us here

		if (standingOnPlatform) {
			collisions.below = true;
		}
	}

	/****
	 * If we are moving right, we project rays to the right of us.
	 * If we are moving left, we project rays to the left of us
	 */
	void HorizontalCollisions(ref Vector3 velocity) {
		float directionX = collisions.faceDir;
		
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;


		if (Mathf.Abs(velocity.x) < skinWidth) {
			rayLength = 2 * skinWidth;
		}
		//draw the our rays 
		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
			Debug.DrawRay (rayOrigin, Vector2.right * directionX, Color.red);
			
			if (hit) {
				if(hit.distance == 0) {
					continue;
				}
				//calculate the angle of the slope. 
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				//only use the bottom most ray to check the slope angle. Only check where are feet are 
				if(i == 0 && slopeAngle <= maxClimbAngle) {
					if(collisions.descendingSlope) {
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}

					float distanceToSlopeStart = 0.0f;
					if(slopeAngle != collisions.slopeAngleOld) {
						//this is to fix the scenario where the collision rays are hitting the slope and the collider moves up the slope
						//even though they have not colliding with the slope
						distanceToSlopeStart = hit.distance -skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if(!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					//this is if there are two rays hitting two objects at different distances away, we always pick the closet object to collide with
					rayLength = hit.distance;	

					if(collisions.climbingSlope) {
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
					}

					//update if we are colliding with objects either to our left or right
					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				
				}
			}
		}
	}



	/****
	 * If we are moving up, we project rays above us.
	 * If we are moving down, we project rays below us.
	 */
	void VerticalCollisions(ref Vector3 velocity) {
		//get the direction of the y velocity. If we are moving down, this is -1, if we are moving up, this will be 1
		float directionY = Mathf.Sign (velocity.y);

		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		//draw the our rays 
		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

			if(hit) {
				//jump through a platform (like platform collider
				if(hit.collider.tag == "OneWayPlatform") {
					if(directionY == 1 || hit.distance == 0) {
						continue;
					}

					if(collisions.fallingThroughPlatform) {
						continue;
					}
					if(playerInput.y == -1) {
						collisions.fallingThroughPlatform = true;
						Invoke("ResetFallingThroughPlatform", 0.5f);
						continue;
					}
				}

				velocity.y = (hit.distance - skinWidth)* directionY;
				//this is if there are two rays hitting two objects at different distances away, we always pick the closet object to collide with
				rayLength = hit.distance;

				if(collisions.climbingSlope) {
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
			}
		}
		
		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign (velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;

			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;

			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if(hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if(slopeAngle != collisions.slopeAngle) {
					//we have collided with new slope
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void ClimbSlope(ref Vector3 velocity, float slopeAngle) {
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
		//do some trig
		if (velocity.y <= climbVelocityY) {
			velocity.y = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
		} 
	}

	void DescendSlope(ref Vector3 velocity) {
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if(slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				if(Mathf.Sign(hit.normal.x) == directionX) {
					if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad * Mathf.Abs (velocity.x))) {
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	void ResetFallingThroughPlatform() {
		collisions.fallingThroughPlatform = false;
	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;
		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;
		public int faceDir;
		public bool fallingThroughPlatform;

		public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			slopeAngleOld = slopeAngle;
			slopeAngle = 0.0f;
			descendingSlope = false;
		}
	}
}
