using System.Collections;
using System.Collections.Generic;
using Game.Goods.Abstract;
using UnityEngine;

public class BlackHole : AbstractLevelComponent {

	void Awake() {
		levelDataModel.OnRestart += reset;
	}

	void OnTriggerEnter2D(Collider2D other) {
		Rigidbody2D rigidbody = other.gameObject.GetComponent<Rigidbody2D>();
		if (rigidbody != null) {
			StartCoroutine(Fall(rigidbody));
		}
	}

	private IEnumerator Fall(Rigidbody2D rigidbody) {
		Transform targetTransform = rigidbody.transform;
		Vector3 axis = Vector3.Cross(rigidbody.transform.position - gameObject.transform.position, rigidbody.velocity);

		rigidbody.velocity = Vector2.zero;
		rigidbody.angularVelocity = 0;
		rigidbody.bodyType = RigidbodyType2D.Kinematic;
		rigidbody.transform.position = new Vector3(rigidbody.transform.position.x, rigidbody.transform.position.y, transform.position.z-1);
		rigidbody.GetComponent<Collider2D>().enabled = false;

		while (targetTransform.localScale != Vector3.zero) {
			targetTransform.localScale = Vector3.Lerp(targetTransform.localScale, Vector3.zero, Time.deltaTime);
			targetTransform.position = Vector3.Lerp(targetTransform.position, transform.position, Time.deltaTime);
			targetTransform.RotateAround(transform.position, axis, 270*Time.deltaTime);
			yield return null;
		}
	}

	private void reset() {
		StopAllCoroutines();
	}
}
