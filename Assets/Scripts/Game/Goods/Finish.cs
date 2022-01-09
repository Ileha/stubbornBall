using UnityEngine;
using System.Collections;
using System;

public class Finish : GameElement {
	public float time = 5;

	void Awake() {
		app.OnRestart += reset;
		//transform.rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(-360, 360), Vector3.back);
	}

	void OnTriggerEnter2D(Collider2D other) {
		//app.Finish(other.gameObject);

		if (!app.IsCircle(other.gameObject)) { return; }

		Rigidbody2D rigidbody = other.gameObject.GetComponent<Rigidbody2D>();
		if (rigidbody != null) {
			StartCoroutine(Fall(rigidbody));
			StartCoroutine(Timer(time, () => app.Finish(other.gameObject)));
		}
	}

	private IEnumerator Timer(float time, Action action) {
		yield return new WaitForSeconds(time);
		action();
	}

	private IEnumerator Fall(Rigidbody2D rigidbody)
	{
		Transform targetTransform = rigidbody.transform;
		Vector3 axis = Vector3.Cross(rigidbody.transform.position - gameObject.transform.position, rigidbody.velocity);

		rigidbody.velocity = Vector2.zero;
		rigidbody.angularVelocity = 0;
		rigidbody.bodyType = RigidbodyType2D.Kinematic;
		rigidbody.transform.position = new Vector3(rigidbody.transform.position.x, rigidbody.transform.position.y, transform.position.z - 1);
		rigidbody.GetComponent<Collider2D>().enabled = false;

		while (targetTransform.localScale != Vector3.zero) {
			targetTransform.localScale = Vector3.Lerp(targetTransform.localScale, Vector3.zero, Time.deltaTime);
			targetTransform.position = Vector3.Lerp(targetTransform.position, transform.position, Time.deltaTime);
			targetTransform.RotateAround(transform.position, axis, 270 * Time.deltaTime);
			yield return null;
		}	
	}

	private void reset() {
		StopAllCoroutines();	
	}
}
