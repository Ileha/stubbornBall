using UnityEngine;
using UnityEngine.UI;

public class SettingsPage : Page {
	public Button Back;
	public Slider volume;

	void Start() {
		Back.onClick.AddListener(() => app.ShowMain());
		volume.value = AudioPlayer.CurrentAudioPlayer.Volume;
		volume.onValueChanged.AddListener(
			(val) => {
				AudioPlayer.CurrentAudioPlayer.Volume = val;
			}
		);
	}
}