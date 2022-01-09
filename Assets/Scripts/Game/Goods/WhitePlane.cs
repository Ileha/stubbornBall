using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhitePlane : GameElement {
	void Awake() {
		app.OnRestart += reset;
	}

	void OnCollisionExit2D(Collision2D other) {
		gameObject.SetActive(false);
	}

	private void reset() {
		gameObject.SetActive(true);
	}
}
