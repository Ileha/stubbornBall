using UnityEngine;

public class Finish : GameElement {
	void OnTriggerEnter2D(Collider2D other) {
		app.Finish(other.gameObject);
	}
}
