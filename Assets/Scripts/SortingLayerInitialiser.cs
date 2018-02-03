using UnityEngine;
using System.Collections;

/***
 * Simple script to set the sorting layer of a mesh renderer
 */
public class SortingLayerInitialiser : MonoBehaviour {

	public string sortingLayerName = "Foreground";
	public int orderInLayer = 0;

	void Start () {
		MeshRenderer meshRenderer = GetComponent<MeshRenderer> ();
		if (meshRenderer != null) {
			meshRenderer.sortingLayerName = sortingLayerName;
			meshRenderer.sortingOrder = orderInLayer;
		} else {
			Renderer renderer = GetComponent<Renderer> ();
			renderer.sortingLayerName = sortingLayerName;
			renderer.sortingOrder = orderInLayer;
		}
	}
}
