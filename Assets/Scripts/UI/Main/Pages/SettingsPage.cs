using System;
using System.Collections.Generic;
using CommonData;
using Cysharp.Threading.Tasks;
using Services;
using UniRx;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using Zenject;

public class SettingsPage : Page 
{
	public Button Back;
	public Slider volume;
	
	[Inject] private readonly AudioPlayerService audioPlayerService;

	protected override void Start()
	{
		base.Start();
		Back.onClick.AddListener(() => MainUi.ShowMain());
		volume.value = audioPlayerService.Volume.Value;

		volume
			.onValueChanged
			.AsObservable()
			.Subscribe(val =>
			{
				audioPlayerService.Volume.Value = val;
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