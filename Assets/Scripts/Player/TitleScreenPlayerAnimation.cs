using UnityEngine;
using System.Collections;

public class TitleScreenPlayerAnimation : MonoBehaviour {
	private Animator animator;
	private PopcornKernelAnimator kernel;

	private Color DISABLED_COLOUR = new Color (0.25f, 0.25f, 0.25f, 1.0f);
	private Color ENABLED_COLOUR = new Color (1.0f, 1.0f, 1.0f, 1.0f);

	void Start () {
		kernel = GetComponent<PopcornKernelAnimator> ();
		animator = GetComponent<Animator> ();
		animator.SetTrigger("titleScreen");
	}

	public void CustomisePlayer() {
		CustomisePlayer (SelectedPlayerCustomisations.selectedHat,
			SelectedPlayerCustomisations.selectedFacialHair,
			SelectedPlayerCustomisations.selectedShoes);
	}

	public void CustomisePlayer(string hat, string facialHair, string shoes) {
		kernel.CustomisePlayer (hat, facialHair, shoes);
		GreyOutDisabledSprites (hat, facialHair, shoes);
	}

	private void GreyOutDisabledSprites(string hat, string facialHair, string shoes) {
		SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer> ();

		foreach (SpriteRenderer renderer in renderers) {
			if (renderer.name == "Hat") {
				EnableOrDisableSprite (hat, renderer);
			} else if (renderer.name == "FacialHair") {
				EnableOrDisableSprite (facialHair, renderer);
			} else if (renderer.name == "Shoe") {
				EnableOrDisableSprite (shoes, renderer);
			}
		}
	}

	private void EnableOrDisableSprite(string name, SpriteRenderer renderer) {
		StoreItem storeItem = Store.GetStoreItem (name);
		if (storeItem == null) {
			return;
		}

		if (Store.GetStoreItem (name).locked) {
			renderer.color = DISABLED_COLOUR;
		} else {
			renderer.color = ENABLED_COLOUR;
		}
	}
}
