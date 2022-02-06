using System;
using System.Collections.Generic;
using CommonData;
using Cysharp.Threading.Tasks;
using Services;
using UniRx;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Audio;
using UnityEngine.UI;
using Zenject;

public class SettingsPage : Page 
{
	public Button Back;
	public Slider volume;
	
	[Inject] private readonly AudioPlayerService audioPlayerService;
	[Inject(Id = GameAudioMixer.Master)] private readonly AudioMixerGroup _masterMixer;

	protected override void Start()
	{
		base.Start();
		Back.onClick.AddListener(() => MainUi.ShowMain());

		volume.maxValue = Constants.MixerMax;
		volume.minValue = Constants.MixerMin;

		if (_masterMixer.audioMixer.GetFloat(Constants.MainMixerVolume, out var mainMixerVolume))
		{
			volume.value = mainMixerVolume;
		}

		volume
			.onValueChanged
			.AsObservable()
			.DistinctUntilChanged()
			.Subscribe(val =>
			{
				_masterMixer.audioMixer.SetFloat(Constants.MainMixerVolume, val);
			})
			.AddTo(this);
		
		volume
			.onValueChanged
			.AsObservable()
			.Throttle(TimeSpan.FromSeconds(10))
			.Subscribe(val =>
			{
				Analytics.CustomEvent(
					Constants.VolumeValueChanged,
					new Dictionary<string, object>()
					{
						{ "value", val }
					}
				);
			})
			.AddTo(this);
	}
}