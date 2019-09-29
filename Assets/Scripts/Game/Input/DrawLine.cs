﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DrawLine : GameElement, Iinput {
	public const float THICKNESS = 0.05f;

	public Toggle draw;
	public Toggle clear;

	private Iinput drawSimpleLine;
	private Iinput clearLine;

	private Iinput currentInput;

	void Awake() {
		drawSimpleLine = new DrawSimpleLine(this);
		clearLine = new LineClearer(this);

		draw.onValueChanged.AddListener(OnDrawSelect);
		clear.onValueChanged.AddListener(OnClearSelect);

		OnDrawSelect(true);
	}

	public void OnStart(Vector3 ScreenPosition) {
		currentInput.OnStart(ScreenPosition);
	}

	public void OnMove(Vector3 ScreenPosition) {
		currentInput.OnMove(ScreenPosition);
	}

	public void OnEnd(Vector3 ScreenPosition) {
		currentInput.OnEnd(ScreenPosition);
	}

	private void OnDrawSelect(bool state) {
		if (!state) { return; }
		currentInput = drawSimpleLine;
	}

	private void OnClearSelect(bool state) {
		if (!state) { return; }
		currentInput = clearLine;
	}
}

class DrawSimpleLine : Iinput {
	private LineComposer CurrentComposer;
	private GameElement drawer;

	public DrawSimpleLine(GameElement drawer) {
		this.drawer = drawer;
	}

	public void OnEnd(Vector3 ScreenPosition) {
		if (CurrentComposer == null) { return; }
		CurrentComposer = null;
	}

	public void OnMove(Vector3 ScreenPosition) {
		if (CurrentComposer != null) {
			Vector3 position = Camera.main.ScreenToWorldPoint(ScreenPosition);
			position.z = 0;
			CurrentComposer.AddPointInGlobalSpace(position);
		}
	}

	public void OnStart(Vector3 ScreenPosition) {
		if (!drawer.app.CanDraw(ScreenPosition)) { return; } //TODO may remake
		Vector3 position = Camera.main.ScreenToWorldPoint(ScreenPosition);
		position.z = 0;
		CurrentComposer = LineComposer.GetLine("line",
											   position,
											   DrawLine.THICKNESS,
											   Singleton.Instanse.LineMaterial
											  );
	}
}
class LineClearer : Iinput {
	private GameElement drawer;

	public LineClearer(GameElement drawer) {
		this.drawer = drawer;
	}

	public void OnEnd(Vector3 ScreenPosition) {}
	public void OnMove(Vector3 ScreenPosition) {
		if (!drawer.app.CanDraw(ScreenPosition)) { return; } //TODO may remake
		Collider2D[] colls = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(ScreenPosition),
														DrawLine.THICKNESS
														);
		for (int i = 0; i<colls.Length; i++) {
			LineComposer line = colls[i].GetComponent<LineComposer>();
			if (line != null) {
				GameObject.Destroy(line.gameObject);
				break;
			}
		}
	}
	public void OnStart(Vector3 ScreenPosition) {}
}