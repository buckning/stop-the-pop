using UnityEngine;
using System.Collections;

public class OilDropCollector : GameObjectDestructor {
	SpriteRenderer spriteRenderer;

	void Start() {
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}

	public override void OnDestroy() {
		if (spriteRenderer.isVisible) {
			AudioManager.PlaySound ("splash1");	
		}
	}
}
