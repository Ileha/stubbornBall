using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class AudioPlayerService : IInitializable, IDisposable 
{
	public ReactiveProperty<float> Volume = new ReactiveProperty<float>(1);

	private AudioSource _source;
	private IEnumerator<AudioClip> _clips;
	private CancellationTokenSource _cancellation;
	private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

	public void SetClips(IEnumerable<AudioClip> clips) 
	{
		_cancellation?.Cancel();
		_cancellation?.Dispose();
		_cancellation = new CancellationTokenSource();
		var token = _cancellation.Token;
		
		_clips = clips.GetEnumerator();
		Play(token);
	}

	private async void Play(CancellationToken token)
	{
		while (_clips.MoveNext())
		{
			_source.clip = _clips.Current;
			_source.Play();
			await UniTask.Delay(TimeSpan.FromSeconds(_source.clip.length - _source.time));
		}
		_source.Stop();
	}

	public void Initialize()
	{
		GameObject result = new GameObject("AudioPlayer", typeof(AudioSource));
		UnityEngine.Object.DontDestroyOnLoad(result.gameObject);
		_source = result.GetComponent<AudioSource>();
		_source.playOnAwake = false;
		Volume.Value = _source.volume;
		Volume
			.Subscribe(x => _source.volume = x)
			.AddTo(_compositeDisposable);
	}

	public void Dispose()
	{
		_compositeDisposable?.Dispose();
	}
}