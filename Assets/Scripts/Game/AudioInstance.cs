using System;
using Services;
using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class AudioInstance : MonoBehaviour, IPoolable<IMemoryPool>, IDisposable
{
	public AudioSource AudioSource
	{
		get
		{
			if (_audioSource == null)
			{
				_audioSource = GetComponent<AudioSource>();
			}

			return _audioSource;
		}
	}

	private IMemoryPool _pool;
	private AudioSource _audioSource;
	private CompositeDisposable _compositeDisposable;
	[Inject] private readonly AudioPlayerService _audioPlayerService;

	public void OnDespawned()
	{
		_pool = null;
		AudioSource.Stop();
		_compositeDisposable?.Dispose();
	}

	public void OnSpawned(IMemoryPool pool)
	{
		_pool = pool;
		_compositeDisposable = new CompositeDisposable();

		_audioPlayerService
			.Volume
			.Subscribe(volume => AudioSource.volume = volume)
			.AddTo(_compositeDisposable);
	}

	public void Dispose()
	{
		_pool.Despawn(this);
	}
	
	public class Factory : PlaceholderFactory<AudioInstance>
	{
	}
}