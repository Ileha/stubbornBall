using System.Collections;
using System.Collections.Generic;
using CommonData;
using Cysharp.Threading.Tasks;
using Game.Goods.Abstract;
using Services;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class BreakPlane : AbstractLevelComponent {
	public Sprite breakPlate;
	public float Seconds = 5;
	[SerializeField] public AudioClip crackEffect;

	private Coroutine wait;
	private GameObject BrokenPlate;
	[Inject] private readonly AudioPlayerService _audioPlayerService;
	[Inject(Id = GameAudioMixer.Effect)] private readonly AudioMixerGroup _effectMixer;

	void Awake() {
		levelDataModel.OnRestart += reset;
		BrokenPlate = CreateBrokenPlate();
		BrokenPlate.SetActive(false);
	}

	void OnCollisionEnter2D(Collision2D col) {
		if (wait != null) { return; }

		wait = StartCoroutine(Wait());
	}

	private IEnumerator Wait() {
		yield return new WaitForSeconds(Seconds);

		_audioPlayerService.Play(crackEffect, _effectMixer).Forget();
		BrokenPlate.transform.position = transform.position;
		BrokenPlate.SetActive(true);
		gameObject.SetActive(false);
	}

	private void reset() {
		if (wait != null) {
			StopCoroutine(wait);
			wait = null;
		}
		BrokenPlate.SetActive(false);
		gameObject.SetActive(true);
	}

	private GameObject CreateBrokenPlate() {
		GameObject result = new GameObject("BrokenPlate");
		SpriteRenderer render = result.AddComponent<SpriteRenderer>();
		render.sprite = breakPlate;
		result.AddComponent<Rigidbody2D>();
		result.transform.position = transform.position;
		result.transform.localScale = transform.localScale;

		return result;
	}
}
