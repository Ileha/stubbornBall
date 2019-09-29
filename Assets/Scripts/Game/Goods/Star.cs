using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : GameElement {
	void Awake() {
		app.OnRestart += reset;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (app.Cricle.gameObject == other.gameObject) {
			ParticleSystem particle = Instantiate(Singleton.Instanse.OnStar, transform.position, Quaternion.identity);
			Destroy(particle.gameObject, 5);
			gameObject.SetActive(false);
			app.IncreaseStar();
		}
	}

	private void reset() {
		gameObject.SetActive(true);
	}
}
