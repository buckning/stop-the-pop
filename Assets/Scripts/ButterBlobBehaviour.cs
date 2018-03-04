using UnityEngine;
using System.Collections;

/***
 * This class expects there to be both triggers and colliders on the butter blob. 
 * The collision behaviour for the trigger is that this object gets destroyed.
 * The collision behaviour for the collider is that the players temperature increases.
 */
public class ButterBlobBehaviour : Breakable {

	private Vector2 reboundVector = new Vector2(0f, 12f);	//force applied to the player when he interacts with this object
	public float temperatureIncreaseRate = 3.0f;	//the new temperature increase rate of the player when they collide with this object
	private bool updateTemperature;					//a cache of the player.updateTemperature flag, so we can reset after we stop colliding with him
	private PlayerController player;				//reference to the player object
	public float speed = 3f;						//speed of this object when it is moving
	private Animator animator;
	public Vector3[] localWaypoints;
	Vector3[] globalWaypoints;

	float timeSinceLastInteractionWithPlayer;

	public bool startMovingWhenVisible = true;
	private Renderer myRenderer;
	bool wasVisible = false;

	private bool dying = false;

	private Vector2 collisionVector = new Vector2(30f, 8f);	//force applied to the player when he gets hit with this object

	public float maxDistanceToGoal = 0.2f;
	public float waitTime = 0;		//time that the platform waits at when waypoint has been reached
	float delay;
	public bool cyclic = false;
	int fromWaypointIndex;
	float percentBetweenWaypoints;
	public float easeAmount;

	public GameObject effectPlayedFromKick;

	void Start () {
		float timeSinceLastInteractionWithPlayer = Time.time;
		myRenderer = GetComponent<Renderer> ();
		globalWaypoints = new Vector3[localWaypoints.Length];
		animator = GetComponent<Animator> ();

		for(int i = 0; i < localWaypoints.Length; i++) {
			globalWaypoints[i] = localWaypoints[i] + transform.position;
		}
	}
	
	void Update () {
		if (myRenderer.isVisible) {
			wasVisible = true;
		}

		if (dying) {
			return;
		}

		if (!wasVisible) {
			return;
		}
		Vector2 velocity = CalculateMovement ();
		transform.Translate (velocity);

		if (velocity.x > 0) {
			Vector3 theScale = transform.localScale;
			theScale.x = Mathf.Abs(transform.localScale.x) * -1;
			transform.localScale = theScale;//ransform.localRotation = Quaternion.Euler (transform.rotation.x, 0, transform.rotation.z);   //face right
		} else if (velocity.x < 0) {
			Vector3 theScale = transform.localScale;
			theScale.x = Mathf.Abs(transform.localScale.x);
			transform.localScale = theScale;// = Quaternion.Euler(transform.rotation.x, 180, transform.rotation.z);  //face left
		}
	}

	void OnTriggerEnter2D(Collider2D otherObject) {
		if (dying) {
			return;
		}
		if (otherObject.gameObject.tag == Strings.PLAYER) {
			player = otherObject.gameObject.GetComponent<PlayerController> ();

			if (player.GetTemperature() >= 100.0f) {
				return;
			}

			if (player.GetVelocity ().y > 5.0f) {	//if the player is not falling. The 5.0f is a buffer for better feel
				return;
			}

			//time check here is for the player getting hit by the enemy, don't let the enemy die from the player jump back force
			if (player.GetComponent<Rigidbody2D> ().velocity.y < 0.0f && (Time.time - timeSinceLastInteractionWithPlayer) > 0.05f) {
				player.SetVelocity (new Vector2 (player.GetVelocity ().x, reboundVector.y));
				animator.SetTrigger ("die");
				dying = true;
				AudioManager.PlaySound ("squash", Random.Range (1.25f, 1.5f));
				player.hud.ShakeForDuration (0.2f);
			}
		}
	}

	public void Die() {
		Destroy(gameObject);
	}

	/***
	 * If there is a collision with the player, increase the temperature
	 */
	void OnCollisionEnter2D(Collision2D otherObject) {
		if (dying) {
			return;
		}
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			player = otherObject.gameObject.GetComponent<PlayerController> ();

			int direction = (player.transform.position.x < transform.position.x) ? -1 : 1;
			player.SetVelocity(new Vector2(collisionVector.x * direction, collisionVector.y));

			//cache if the player should increase temperature
			player.CollisionWithEnemy(gameObject.name);

			timeSinceLastInteractionWithPlayer = Time.time;
		}
	}

	public override void Break(Vector3 positionOfOriginator) {
		animator.SetTrigger ("die");
		dying = true;
		AudioManager.PlaySound ("squash", Random.Range (1.25f, 1.5f));
		Instantiate (effectPlayedFromKick, positionOfOriginator, Quaternion.identity);
	}

	/***
	 * Calculate the movement that the platform will move this frame
	 */
	Vector3 CalculateMovement() {
		fromWaypointIndex %= globalWaypoints.Length;
		
		int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;
		float distanceBetweenWaypoints = Vector3.Distance (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex]);
		percentBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
		
		percentBetweenWaypoints = Mathf.Clamp01 (percentBetweenWaypoints);
		float easedPercentBetweenWaypoints = Ease (percentBetweenWaypoints);
		
		//this line calculates the next velocity, the rest this method is to update the next waypoint
		Vector3 newPos = Vector3.Lerp (globalWaypoints [fromWaypointIndex], globalWaypoints [toWaypointIndex], easedPercentBetweenWaypoints);
		
		if (percentBetweenWaypoints >= 1) {
			percentBetweenWaypoints = 0;
			fromWaypointIndex++;
			
			if(!cyclic) {
				if(fromWaypointIndex >= globalWaypoints.Length-1) {
					fromWaypointIndex = 0;
					System.Array.Reverse(globalWaypoints);
				}
			}
		}
		
		return newPos - transform.position;
	}
	
	
	float Ease(float x) {
		float a = easeAmount + 1;
		
		return Mathf.Pow (x, a) / (Mathf.Pow (x, a) + Mathf.Pow (1 - x, a));
	}

	void OnDrawGizmos() {
		if (localWaypoints != null) {
			Gizmos.color = Color.red;
			float size = 0.3f;
			
			for(int i = 0; i < localWaypoints.Length; i++) {
				Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
				Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
				Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
			}
		}
	}
}
