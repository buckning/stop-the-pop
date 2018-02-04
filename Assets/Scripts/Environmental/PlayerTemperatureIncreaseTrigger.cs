using UnityEngine;
using System.Collections;

/***
 * Class that updates the player's temperature when they collide with this object.
 * When they are stop colliding with the object, their temperature gets reset to its original value. 
 */
public class PlayerTemperatureIncreaseTrigger : MonoBehaviour {
	private bool updateTemperature;					//a cache of the player.updateTemperature flag, so we can reset after we stop colliding with him
	private bool isCollidingWithPlayer = false;		//flag to see if we're currently colliding with the player
	private PlayerController player;				//reference to the player object

	public float speedOfTemperatureIncrease = 10;

	/***
	 * If there is a collision with the player, increase the temperature increase rate
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			player = otherObject.gameObject.GetComponent<PlayerController> ();
			//cache if the player should increase temperature
			updateTemperature = player.GetUpdateTemperature();

			player.inputManager.ShowDamageIndicator ();
			player.inputManager.ShakeForDuration (0.3f);
			AudioManager.PlaySound ("sizzle");
			
			//make the player increase the temperature
			player.SetUpdateTemperature(true);
			player.SetTemperatureUpdateRate (gameObject.name, speedOfTemperatureIncrease);
			isCollidingWithPlayer = true;
		}
	}

	/***
	 * 	When we stop colliding with the player, reset the temperature increase rate
	 */
	void OnTriggerExit2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.SetUpdateTemperature (updateTemperature);
			player.SetTemperatureUpdateRate("none", player.regularTemperatureUpdateRate);
			isCollidingWithPlayer = false;
		}
	}

	/***
	 * If we're still colliding with the player and we are getting destroyed, we must
	 * make sure that we reset the temperature increase rate of the player 
	 */
	void OnDestroy() {
		if (isCollidingWithPlayer) {
			player.SetUpdateTemperature (updateTemperature);
			player.SetTemperatureUpdateRate("none", player.regularTemperatureUpdateRate);
			isCollidingWithPlayer = false;
		}
	}
}
