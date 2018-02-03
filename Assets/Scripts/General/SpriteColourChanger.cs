using UnityEngine;
using System.Collections;

/***
 * Class to change the colour of a sprite to either red, green or blue. 
 * The sprite changes colour over a range of time. 
 */
public class SpriteColourChanger : MonoBehaviour {

	public bool triggerIncreaseWhenVisible = false;		//when set to true, the colour only increases when the sprite is initially seen
	public float maxTimeInIncrease = 3f;				//the amount of time that the colour change will take

	private SpriteRenderer spriteRenderer;
	private bool wasVisible = false;					//used to trigger if this sprite has been seen
	private Renderer myRenderer;

	public float timeOffset = 0.0f;			//the amount of time from when the player sees the object until the timer starts
	float countdownTime;

	private float MAX_COLOUR = 1.0f;					//the maximum colour that a colour component can be
	private float currentColour = 0.0f;					//the current colour that increases as time increases
	private bool endTriggered = false;					//this variable is used to only trigger MaxColourReached once
	private float r = 1f, g = 1f, b = 1f;				//the RGB components of the overall colour

	public enum Colour {
		BLUE,
		RED,
		GREEN
	}

	public Colour colour = Colour.RED;					//the end result that the sprite changes colour to

	void Start () {
		countdownTime = timeOffset;
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		myRenderer = GetComponent<Renderer> ();
	}
	
	void Update () {
		//this is a trigger so the behaviour won't trigger until the 
		//kernel was seen
		if (myRenderer.isVisible) {
			wasVisible = true;
		}

		if (triggerIncreaseWhenVisible) {
			if(wasVisible) {
				countdownTime -= Time.deltaTime;
				if (countdownTime <= 0.0f) {
					UpdateColour ();
				}
			}
		} else {
			UpdateColour();
		}

		if (currentColour >= MAX_COLOUR) {
			if(!endTriggered){
				MaxColourReached();
			}
			endTriggered = true;
		}
	}

	/***
	 * Update the colour of the sprite.
	 * Only update the colour when the max colour has not been reached. 
	 */
	private void UpdateColour () {
		if (!endTriggered) {
			currentColour += Time.deltaTime / maxTimeInIncrease;

			if(colour == Colour.RED) {
				r = 1f;
				g = 1f - currentColour;
				b = 1f - currentColour;
			}
			else if(colour == Colour.GREEN) {
				r = 1f - currentColour;
				g = 1f;
				b = 1f - currentColour;
			}
			else if(colour == Colour.BLUE) {
				r = 1f - currentColour;
				g = 1f - currentColour;
				b = 1f;
			}
			spriteRenderer.color = new Color (r, g, b, 1.0f); 
		}
	}

	/***
	 * This method should be implemented by a child class.
	 * It gets triggered when the colour has reached it's maximum value. 
	 */
	protected virtual void MaxColourReached() {

	}
}
