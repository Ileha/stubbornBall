using UnityEngine;

public class ICricle : GameElement {
	public Rigidbody2D rigidbody { get; private set; }

	private Vector3 StartPosition;
	private Vector3 scale;
	private Quaternion rotation;
	private Collider2D CircleCollider;

	void Awake() {
		app.OnPlay += Play;
		app.OnRestart += reset;
		app.OnShaking += shaking;

		CircleCollider = GetComponent<Collider2D>();
		rigidbody = GetComponent<Rigidbody2D>();
		StartPosition = transform.position;
		scale = transform.localScale;
		rotation = transform.rotation;
	}

	//void Update() {
	//	Debug.DrawRay(gameObject.transform.position, rigidbody.velocity, Color.red);
	//}

	private void shaking(Vector3 direction) {
		rigidbody.AddForce(direction, ForceMode2D.Impulse);
	}

	private void reset() {
		gameObject.SetActive(true);
		transform.position = StartPosition;
		transform.localScale = scale;
		transform.rotation = rotation;
		CircleCollider.enabled = true;
		if (rigidbody.bodyType != RigidbodyType2D.Static) {
			rigidbody.velocity = Vector2.zero;
			rigidbody.angularVelocity = 0;
			rigidbody.bodyType = RigidbodyType2D.Static;
		}
	}

	private void Play() {
		rigidbody.bodyType = RigidbodyType2D.Dynamic;
	}
}

