using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SettingsPage : Page 
{
	public Button Back;
	public Slider volume;
	
	[Inject] private readonly AudioPlayerService audioPlayerService;

	void Start() 
	{
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
	}
}