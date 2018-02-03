using UnityEngine;
using System.Collections;

public class TitleScreenPlayerAnimation : MonoBehaviour {
	Animator animator;

	private Color DISABLED_COLOUR = new Color (0.25f, 0.25f, 0.25f, 1.0f);
	private Color ENABLED_COLOUR = new Color (1.0f, 1.0f, 1.0f, 1.0f);

	void Start () {
		animator = GetComponent<Animator> ();
		animator.SetTrigger("titleScreen");

	}

	public void CustomisePlayer() {
		CustomisePlayer (SelectedPlayerCustomisations.selectedHat,
			SelectedPlayerCustomisations.selectedFacialHair,
			SelectedPlayerCustomisations.selectedShoes);
	}


	public void CustomisePlayer(string hat, string facialHair, string shoes) {
		Sprite[] sprites = Resources.LoadAll<Sprite> ("skins/player/hats");

		if(hat != null && hat.Trim().Length == 0) {
			hat = null;
		}
		if (facialHair != null && facialHair.Trim ().Length == 0) {
			facialHair = null;
		}
		if (shoes != null && shoes.Trim ().Length == 0) {
			shoes = null;
		}

		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();

		if (hat != null) {
			foreach (SpriteRenderer renderer in renderers) {
				if (renderer.name == "Hat") {
					foreach (Sprite sprite in sprites) {
						if (sprite.name == hat) {
							renderer.sprite = sprite;

							if (Store.GetStoreItem (hat).locked) {
								renderer.color = DISABLED_COLOUR;
							} else {
								renderer.color = ENABLED_COLOUR;
							}
						}
					}
				}
			}
		} else {
			foreach (SpriteRenderer renderer in renderers) {
				if (renderer.name == "Hat") {
					renderer.sprite = null;
				}
			}
		}

		sprites = Resources.LoadAll<Sprite> ("skins/player/facialHair");

		if (facialHair != null) {
			foreach (SpriteRenderer renderer in renderers) {
				if (renderer.name == "FacialHair") {
					foreach (Sprite sprite in sprites) {
						if (sprite.name == facialHair) {
							renderer.sprite = sprite;

							if (Store.GetStoreItem (facialHair).locked) {
								renderer.color = DISABLED_COLOUR;
							} else {
								renderer.color = ENABLED_COLOUR;
							}
						}
					}
				}
			}
		} else {
			foreach (SpriteRenderer renderer in renderers) {
				if (renderer.name == "FacialHair") {
					renderer.sprite = null;
				}
			}
		}

		sprites = Resources.LoadAll<Sprite> ("skins/player/shoes");

		if (shoes != null) {
			foreach (SpriteRenderer renderer in renderers) {
				if (renderer.name == "shoe-skin-right") {
					foreach (Sprite sprite in sprites) {
						if (sprite.name == shoes + "Right") {
							renderer.sprite = sprite;

							if (Store.GetStoreItem (shoes).locked) {
								renderer.color = DISABLED_COLOUR;
							} else {
								renderer.color = ENABLED_COLOUR;
							}
						}
					}
				} else if (renderer.name == "shoe-skin-left") {
					foreach (Sprite sprite in sprites) {
						if (sprite.name == shoes + "Left") {
							renderer.sprite = sprite;

							if (Store.GetStoreItem (shoes).locked) {
								renderer.color = DISABLED_COLOUR;
							} else {
								renderer.color = ENABLED_COLOUR;
							}
						}
					}
				}
			}
		} 
		else {
			sprites = Resources.LoadAll<Sprite> ("skins/player/shoes");
			foreach (SpriteRenderer renderer in renderers) {
				if (renderer.name.StartsWith("shoe-skin")) {
					foreach (Sprite sprite in sprites) {
						if (sprite.name == "defaultShoe") {
							renderer.sprite = sprite;
							renderer.color = new Color (1.0f, 1.0f, 1.0f, 1.0f);
						}
					}
				}
			}
		}
	}
}
