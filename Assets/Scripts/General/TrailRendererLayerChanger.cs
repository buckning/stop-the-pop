using UnityEngine;
using System.Collections;

public class TrailRendererLayerChanger : MonoBehaviour {

	void Start () {
		TrailRenderer trailRenderer = GetComponent<TrailRenderer> ();
		trailRenderer.sortingLayerName = "Foreground";
	}
}
