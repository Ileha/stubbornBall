using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using CommonData;
using Cysharp.Threading.Tasks;
using Game.Goods.Abstract;
using Services;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class Fan : AbstractLevelComponent {
	public float liftingForse;
	public float sideForse;
	[SerializeField] public AudioClip fanEffect;
	
	[Inject] private readonly AudioPlayerService _audioPlayerService;
	private CancellationTokenSource _cancellationTokenSource;
	[Inject(Id = GameAudioMixer.Effect)] private readonly AudioMixerGroup _effectMixer;

	private void Awake()
	{
		_cancellationTokenSource = new CancellationTokenSource();
		var token = _cancellationTokenSource.Token;
		_audioPlayerService.Play(GetFanPlayList(), _effectMixer, token).Forget();
	}

	private IEnumerable<AudioClip> GetFanPlayList()
	{
		while (true)
		{
			yield return fanEffect;
		}
	}
	
	void OnTriggerStay2D(Collider2D other) {
		Rigidbody2D rigibody = other.GetComponent<Rigidbody2D>();

		if (rigibody != null) {
			Vector3 toFan = gameObject.transform.position - rigibody.transform.position;

			//Debug.DrawRay(rigibody.transform.position,(transform.right * 0.1f / Vector3.Dot(toFan, transform.right)* sideForse));
			//Debug.DrawRay(transform.position, -transform.up * 1f/Vector3.Dot(toFan, transform.up), Color.red);
//(transform.right * Vector3.Dot(toFan, transform.right) * sideForse)
			rigibody.AddForce((-transform.up * 1f / Vector3.Dot(toFan, transform.up) * liftingForse), ForceMode2D.Force);
		}
	}

	private void OnDestroy()
	{
		_cancellationTokenSource?.Cancel();
		_cancellationTokenSource?.Dispose();
		_cancellationTokenSource = null;
	}
}
