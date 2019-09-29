using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarControll : MonoBehaviour {
	public Image[] stars;
	public Sprite StarOn;
	public Sprite StarOff;

	private int current = 0;

	public int GetCurrentState() {
		return current;
	}

	public void AddStar() {
		if (current == stars.Length) { return; }
		stars[current].sprite = StarOn;
		current++;
	}

	public void RemoveStar() {
		if (current == 0) { return; }
		stars[current].sprite = StarOff;
		current--;
	}

	public void Reset() {
		current = 0;
		for (int i = 0; i < stars.Length; i++) {
			stars[i].sprite = StarOff;
		}
	}

	public void SetStar(int starsCount) {
		for (int i = 0; i < stars.Length; i++) {
			if (i < starsCount) {
				stars[i].sprite = StarOn;
			}
			else {
				stars[i].sprite = StarOff;
			}
		}
		current = starsCount;
	}
}
