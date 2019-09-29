using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitFromScene : GameElement {

	void OnTriggerEnter2D(Collider2D other) {
		if (app.Cricle.gameObject == other.gameObject) {
			app.Restart();
		}
	}
}
