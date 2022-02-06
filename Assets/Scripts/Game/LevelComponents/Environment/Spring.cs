using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Goods.Abstract;
using Services;
using UnityEngine;
using Zenject;

public class Spring : AbstractLevelComponent {
	public float forse = 1f;
	[SerializeField] public AudioClip springEffect;
	
	[Inject] private readonly AudioPlayerService _audioPlayerService;
	private Animator animator;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}

	void OnTriggerStay2D(Collider2D other) 
	{
		Rigidbody2D rigibody = other.GetComponent<Rigidbody2D>();

		if (rigibody != null) 
		{
			_audioPlayerService.Play(springEffect).Forget();
			animator.SetTrigger("play");
			rigibody.AddForce(transform.up * forse, ForceMode2D.Impulse);
		}
	}
}
