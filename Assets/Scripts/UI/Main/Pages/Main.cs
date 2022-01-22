﻿using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Main : Page 
{
	public Button Levels;
	public Button Settings;

	void Start() {
		Levels
			.onClick
			.AsObservable()
			.Subscribe(x => MainUi.ShowLevels())
			.AddTo(this);
		
		Settings
			.onClick
			.AsObservable()
			.Subscribe(x => MainUi.ShowSettings())
			.AddTo(this);
	}
}
