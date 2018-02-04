using UnityEngine;
using System.Collections;

public class FryingPanBehaviour : MonoBehaviour {
	public float temperatureIncreaseRate = 2f;	//speed up the temperature increase when collided with pan

	private bool updateTemperature;

	/***
	 * If there is a collision with the player, increase the temperature increase rate
	 */
	void OnTriggerEnter2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			//cache if the player should increase temperature
			updateTemperature = player.GetUpdateTemperature();

			//make the player increase the temperature
			player.SetUpdateTemperature(true);
			player.SetTemperatureUpdateRate(gameObject.name, temperatureIncreaseRate);
		}
	}

	void OnTriggerExit2D(Collider2D otherObject) {
		if(otherObject.gameObject.tag == Strings.PLAYER) {
			PlayerController player = otherObject.gameObject.GetComponent<PlayerController> ();
			player.SetUpdateTemperature (updateTemperature);
			player.SetTemperatureUpdateRate("none-" + gameObject.name, player.regularTemperatureUpdateRate);
		}
	}
}
