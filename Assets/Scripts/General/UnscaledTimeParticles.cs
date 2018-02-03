using UnityEngine;
using System.Collections;

public class UnscaledTimeParticles : MonoBehaviour {
	ParticleSystem myParticleSystem;

	void Start () {
		myParticleSystem = GetComponent<ParticleSystem> ();
	}

	void Update () {
		myParticleSystem.Simulate(Time.unscaledDeltaTime, true, false);
	}
}
