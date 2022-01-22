using System.Collections;
using System.Collections.Generic;
using Game.Goods.Abstract;
using UnityEngine;

public class Spring : AbstractLevelComponent {
	public float forse = 1f;

	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}

	void OnTriggerStay2D(Collider2D other) {
		Rigidbody2D rigibody = other.GetComponent<Rigidbody2D>();

		if (rigibody != null) {
			animator.SetTrigger("play");
			rigibody.AddForce(transform.up * forse, ForceMode2D.Impulse);
		}
	}
}
