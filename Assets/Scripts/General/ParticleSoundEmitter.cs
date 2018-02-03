using UnityEngine;
using System.Collections;

public class ParticleSoundEmitter : MonoBehaviour {

	ParticleSystem myParticleSystem;

	int oldParticleCount = 0;

	void Start () {
		myParticleSystem = GetComponent<ParticleSystem> ();
	}

	void Update () {
		if ((myParticleSystem.particleCount - oldParticleCount) > 10) {
			AudioManager.PlaySound ("platform-bang", Random.Range(0.8f, 1.2f));
		}
		oldParticleCount = myParticleSystem.particleCount;
	}
}
