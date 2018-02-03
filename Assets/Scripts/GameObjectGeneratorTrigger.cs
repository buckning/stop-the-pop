using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * Class that generates a number of objects when it has been collided with. 
 */
public class GameObjectGeneratorTrigger : MonoBehaviour {
	/***
	 * The maximum number of objects to get generated
	 */
	public int maxObjects = 10;
	
	/***
	 * The type of game object to generate
	 */
	public GameObject generatedObjectType;
	
	/***
	 * The x-axis offset to position the game objects
	 */
	public float xOffset = 0f;
	/***
	 * The y-axis offset to position the game objects
	 */
	public float yOffset = 0f;
	/***
	 * The z-axis offset to position the game objects
	 */
	public float zOffset = 0f;
	
	/***
	 * The length of the level on the x-axis
	 */
	public float xLength = 10f;
	/***
	 * The length of the level on the y-axis
	 */
	public float yLength = 10f;
	/***
	 * The length of the level on the z-axis
	 */
	public float zLength = 10f;
	
	/***
	 * Internal list of all the generated game objects. 
	 */
	private List<GameObject> gameObjects;

	private bool alreadyTriggered = false;

	void Start () {
		gameObjects = new List<GameObject>();
	}

	/***
	 * Only trigger the generation of the game objects once. 
	 * This functionality gets triggered when an object has collided with 
	 * this trigger. 
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if (!alreadyTriggered) {
			GenerateGameObjects ();
			alreadyTriggered = true;
		}
	}

	/***
	 * Generate a number (maxObjects) of gameObjects at random positions and random rotations. 
	 */
	public void GenerateGameObjects() {
		float x = transform.position.x;
		float y = transform.position.y;
		float z = transform.position.z;
		for (int i = 0; i < maxObjects; i++) {
			//get a random value between the offset and the length
			float xPos = Mathf.Clamp(xOffset + Random.value * xLength, xOffset, xLength);
			float yPos = Mathf.Clamp(yOffset + Random.value * yLength, yOffset, yLength);
			float zPos = Mathf.Clamp(zOffset + Random.value * zLength, zOffset, zLength);
			
			Vector3 position = new Vector3 (x + xPos, y + yPos, z + zPos);

			Quaternion angle = Quaternion.Euler(0, 0, Random.Range(0.0f, 360.0f));
			GameObject gameObject = (GameObject)Instantiate (generatedObjectType, position, angle);
			gameObjects.Add(gameObject);
		}
	}
}
