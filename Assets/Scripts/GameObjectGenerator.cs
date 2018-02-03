using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/***
 * Programatically generate a list of GameObjects at random positions 
 * between xOffset to xLength, yOffset to yLength and zOffset to zLength.
 */
public class GameObjectGenerator : MonoBehaviour {
	
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

	void Start () {
		gameObjects = new List<GameObject>();
		GenerateGameObjects ();
	}
	
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

			GameObject gameObject = (GameObject)Instantiate (generatedObjectType, position, Quaternion.identity);
			gameObjects.Add(gameObject);
		}
	}
}
