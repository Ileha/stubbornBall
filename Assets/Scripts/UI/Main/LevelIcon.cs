using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelIcon : Page {
	public StarControll Stars;
	public Button Select;
	public Text Text;
	public Image Lock;

	public Level CurrentLevel { get; private set; }

	void Awake() {
		Select.onClick.AddListener(() => OnSelect());
		SetLevelActive(true);
	}

	public void Init(Level level) {
		CurrentLevel = level;
		Stars.SetStar(level.Stars);
		Text.text = level.SceneNumber.ToString();
	}

	public void SetLevelActive(bool active) {
		Select.interactable = active;
		Lock.gameObject.SetActive(!active);
	}

	private void OnSelect() {
		app.LoadLevel(CurrentLevel);
	}
}