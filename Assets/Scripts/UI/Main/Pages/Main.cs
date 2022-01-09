using UnityEngine;
using UnityEngine.UI;

public class Main : Page {
	public Button Levels;
	public Button Settings;

	void Start() {
		Levels.onClick.AddListener(() => app.ShowLevels());
		Settings.onClick.AddListener(() => app.ShowSettings());
	}
}
