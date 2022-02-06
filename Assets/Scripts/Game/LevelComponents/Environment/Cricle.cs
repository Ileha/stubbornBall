using System;
using CommonData;
using Cysharp.Threading.Tasks;
using Extensions;
using Game.Goods.Abstract;
using Services;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class Cricle : AbstractLevelComponent
{
	[SerializeField] public AudioClip collisionEffect;
	
	public Rigidbody2D rigidbody { get; private set; }

	private Vector3 StartPosition;
	private Vector3 scale;
	private Quaternion rotation;
	private Collider2D CircleCollider;
	[Inject] private readonly AudioPlayerService _audioPlayerService;
	[Inject(Id = GameAudioMixer.Effect)] private readonly AudioMixerGroup _effectMixer;

	void Awake() 
	{
		levelDataModel.OnPlay += Play;
		levelDataModel.OnRestart += reset;
		levelDataModel.OnShaking += shaking;

		CircleCollider = GetComponent<Collider2D>();
		rigidbody = GetComponent<Rigidbody2D>();
		StartPosition = transform.position;
		scale = transform.localScale;
		rotation = transform.rotation;

		gameObject
			.OnCollisionEnter2DAsObservable()
			.Select(collision => (collision, Time.time))
			.CombineWithPrevious((previous, next) => (previous, next))
			.Select(data =>
			{
				if (data.previous != default && 
				    data.next.collision.collider == data.previous.collision.collider &&
				    data.next.time - data.previous.time < 0.25f)
				{
					return Observable.Empty<Collision2D>();
				}

				return Observable.Return(data.next.collision);
			})
			.Switch()
			.Subscribe(x =>
			{
				if (collisionEffect != null)
				{
					_audioPlayerService.Play(collisionEffect, _effectMixer).Forget();
				}
			})
			.AddTo(this);
	}

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

