using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Services
{
	public class AudioPlayerService : IInitializable, IDisposable 
	{
		public ReactiveProperty<float> Volume = new ReactiveProperty<float>(1);

		[Inject] private readonly AudioInstance.Factory _factory;

		private CancellationTokenSource _cancellation;
		private CancellationToken _cancellationToken;

		public async UniTask Play(AudioClip clip, CancellationToken token = default)
		{
			if (token == default)
			{
				token = CancellationToken.None;
			}

			var finalTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, _cancellationToken);
			var finalToken = finalTokenSource.Token;
			
			using (AudioInstance audioInstance = _factory.Create())
			{
				audioInstance.AudioSource.clip = clip;
				audioInstance.AudioSource.Play();
				await UniTask.Delay(
					TimeSpan.FromSeconds(audioInstance.AudioSource.clip.length - audioInstance.AudioSource.time),
					cancellationToken: finalToken
				);
				finalToken.ThrowIfCancellationRequested();
			}
		}

		public async UniTask Play(IEnumerable<AudioClip> clips, CancellationToken token = default)
		{
			if (token == default)
			{
				token = CancellationToken.None;
			}

			var finalTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token, _cancellationToken);
			var finalToken = finalTokenSource.Token;
			
			using (AudioInstance audioInstance = _factory.Create())
			{
				using (var clipsEnumerator = clips.GetEnumerator())
				{
					while (clipsEnumerator.MoveNext())
					{
						audioInstance.AudioSource.clip = clipsEnumerator.Current;
						audioInstance.AudioSource.Play();
						await UniTask.Delay(
							TimeSpan.FromSeconds(audioInstance.AudioSource.clip.length - audioInstance.AudioSource.time),
							cancellationToken: finalToken
						);
						finalToken.ThrowIfCancellationRequested();
					}
				}
			}
		}

		public void Initialize()
		{
			_cancellation = new CancellationTokenSource();
			_cancellationToken = _cancellation.Token;
		}

		public void Dispose()
		{
			_cancellation?.Cancel();
			_cancellation?.Dispose();
			_cancellation = default;
		}
	}
}