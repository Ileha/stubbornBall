using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EndOfLevel : GameElement {
	public StarControll stars;
	public Button menu;
	public Button restart;
	public Button next;

	public void Enable(int starsCount, Action onNext, Action onRestart, Action onMenu) {
		stars.SetStar(starsCount);
		configureKey(menu, onMenu);
		configureKey(restart, onRestart);
		configureKey(next, onNext);

		gameObject.SetActive(true);
	}

	private void configureKey(Button key, Action handler) {
		key.onClick.RemoveAllListeners();
		if (handler == null) {
			key.gameObject.SetActive(false);
		}
		else {
			key.gameObject.SetActive(true);
			key.onClick.AddListener(() => handler());
		}
	}

	public void Disable() {
		gameObject.SetActive(false);
	}
}
