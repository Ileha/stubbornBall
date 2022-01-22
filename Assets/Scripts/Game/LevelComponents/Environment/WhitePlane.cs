using System.Collections;
using System.Collections.Generic;
using Game.Goods.Abstract;
using UnityEngine;

public class WhitePlane : AbstractLevelComponent {
	void Awake() {
		levelDataModel.OnRestart += reset;
	}

	void OnCollisionExit2D(Collision2D other) {
		gameObject.SetActive(false);
	}

	private void reset() {
		gameObject.SetActive(true);
	}
}
