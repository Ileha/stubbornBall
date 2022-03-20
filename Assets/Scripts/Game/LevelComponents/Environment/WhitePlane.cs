using System.Collections;
using System.Collections.Generic;
using CommonData;
using Cysharp.Threading.Tasks;
using Game.Goods.Abstract;
using Services;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class WhitePlane : AbstractLevelComponent 
{
	[SerializeField] public AudioClip disappearEffect;
	
	[Inject] private readonly AudioPlayerService _audioPlayerService;
	[Inject(Id = GameAudioMixer.Effect)] private readonly AudioMixerGroup _effectMixer;
	
	void Awake() 
	{
		levelDataModel.OnRestart += Reset;
	}

	void OnCollisionExit2D(Collision2D other) 
	{
		gameObject.SetActive(false);
		_audioPlayerService.Play(disappearEffect, _effectMixer).Forget();
	}

	private void Reset() 
	{
		gameObject.SetActive(true);
	}
}
