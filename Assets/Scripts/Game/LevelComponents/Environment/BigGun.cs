using System;
using System.Collections;
using System.Collections.Generic;
using Game.Goods.Abstract;
using UnityEngine;

public class BigGun : AbstractLevelComponent, Iinput 
{
	public Vector2 ShootDirection;
	public Animator animator;
	public float rotationSpeed = 1;
	public float forse;
	public float MaxAngle;
	public float MinAngle;

	private Rigidbody2D rigidbody;
	private float rigidbodyZ;
	private Coroutine currentRotation;

	void Awake() {
		levelDataModel.OnRestart += reset;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (rigidbody != null) { return; }

		rigidbody = other.gameObject.GetComponent<Rigidbody2D>();
		if (rigidbody != null) {
			rigidbodyZ = rigidbody.transform.position.z;
			rigidbody.velocity = Vector2.zero;
			rigidbody.angularVelocity = 0;
			rigidbody.bodyType = RigidbodyType2D.Kinematic;
			rigidbody.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z+1);
			levelDataModel.Subscribe(this);

			currentRotation = StartCoroutine(Rotate());
		}
	}

	private void reset() {
		levelDataModel.Unsubscribe(this);
		if (currentRotation != null) {
			StopCoroutine(currentRotation);
			currentRotation = null;
		}
		rigidbody = null;
	}

	private Vector3 GetShootDirection() {
		return (ShootDirection.x * transform.right + ShootDirection.y * transform.up).normalized;
	}

	void OnDrawGizmosSelected() {
		Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(MaxAngle, Vector3.forward)*Vector3.right);
		Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(MinAngle, Vector3.forward)*Vector3.right);

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, GetShootDirection());
	}

	public IEnumerator Rotate() {
		Quaternion[] states = new Quaternion[] {
			transform.rotation* Quaternion.FromToRotation(GetShootDirection(),
														  Quaternion.AngleAxis(MaxAngle, Vector3.forward)*Vector3.right
														 ),
			transform.rotation* Quaternion.FromToRotation(GetShootDirection(),
														  Quaternion.AngleAxis(MinAngle, Vector3.forward) * Vector3.right)
		};

		int current = 0;

		while (true) {
			Quaternion next = states[current];
			current = (current + 1) % states.Length;

			while (Quaternion.Angle(transform.rotation, next) > 10) {
				transform.rotation = Quaternion.RotateTowards(transform.rotation, next, Time.deltaTime * rotationSpeed);
				yield return null;
			}	
		}
	}

	public void OnShoot() {//invoked from animation
		rigidbody.bodyType = RigidbodyType2D.Dynamic;
		rigidbody.AddForce(GetShootDirection()* forse, ForceMode2D.Impulse);
		rigidbody.transform.position = new Vector3(rigidbody.transform.position.x, rigidbody.transform.position.y, rigidbodyZ);
		rigidbody = null;
	}

	public void OnStart(Vector3 ScreenPosition) {
		if (!levelDataModel.CanDraw(ScreenPosition)) { return; }
		animator.SetTrigger("shoot");
		levelDataModel.Unsubscribe(this);
		StopCoroutine(currentRotation);
		currentRotation = null;
	}

	public void OnMove(Vector3 ScreenPosition) {
		
	}

	public void OnEnd(Vector3 ScreenPosition) {
		
	}
}
