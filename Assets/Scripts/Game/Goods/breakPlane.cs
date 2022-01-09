using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class breakPlane : GameElement {
	public Sprite breakPlate;
	public float Seconds = 5;

	private Coroutine wait;
	private GameObject BrokenPlate;

	void Awake() {
		app.OnRestart += reset;
		BrokenPlate = CreateBrokenPlate();
		BrokenPlate.SetActive(false);
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (wait != null) { return; }

		wait = StartCoroutine(Wait());
	}

	private IEnumerator Wait() {
		yield return new WaitForSeconds(Seconds);

		BrokenPlate.transform.position = transform.position;
		BrokenPlate.SetActive(true);
		gameObject.SetActive(false);
	}

	private void reset() {
		if (wait != null) {
			StopCoroutine(wait);
			wait = null;
		}
		BrokenPlate.SetActive(false);
		gameObject.SetActive(true);
	}

	private GameObject CreateBrokenPlate() {
		GameObject result = new GameObject("BrokenPlate");
		SpriteRenderer render = result.AddComponent<SpriteRenderer>();
		render.sprite = breakPlate;
		result.AddComponent<Rigidbody2D>();
		result.transform.position = transform.position;
		result.transform.localScale = transform.localScale;

		return result;
	}
}
