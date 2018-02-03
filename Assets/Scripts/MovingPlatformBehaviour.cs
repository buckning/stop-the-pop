using UnityEngine;
using System.Collections;

public class MovingPlatformBehaviour : MonoBehaviour {
	Rigidbody2D rigidBody2d;
	
	public float speed = 1f;
	
	public float patrolDistance = 5f;

	private Vector2 basePosition;
	private bool movingRight = true;
	
	void Start () {
		rigidBody2d = GetComponent<Rigidbody2D> ();
		basePosition = transform.position;
	}
	
	void Update () {
		if (movingRight) {
			Move (speed);
			if(transform.position.x > (patrolDistance + basePosition.x)) {
				movingRight = false;
			}
		} else {
			Move (speed*-1f);
			if(transform.position.x < (basePosition.x - patrolDistance)){
				movingRight = true;
			}
		}
	}
	
	public void Move(float speed) {
		rigidBody2d.velocity = new Vector2(speed, rigidBody2d.velocity.y);
	}
}
