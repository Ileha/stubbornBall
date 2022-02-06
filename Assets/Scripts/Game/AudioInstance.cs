using System;
using Services;
using UniRx;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

[RequireComponent(typeof(AudioSource))]
public class AudioInstance : MonoBehaviour, IPoolable<AudioMixerGroup, IMemoryPool>, IDisposable
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

	public void OnDespawned()
	{
		_pool = null;
		AudioSource.Stop();
		_compositeDisposable?.Dispose();
	}

	public void OnSpawned(AudioMixerGroup audioMixerGroup, IMemoryPool pool)
	{
		_pool = pool;
		_compositeDisposable = new CompositeDisposable();

		AudioSource.outputAudioMixerGroup = audioMixerGroup;
	}

	public void Dispose()
	{
		_pool.Despawn(this);
	}
	
	public class Factory : PlaceholderFactory<AudioMixerGroup, AudioInstance>
	{
	}
}