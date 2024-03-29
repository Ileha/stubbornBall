﻿using System.Collections;
using System.Collections.Generic;
using CommonData;
using Cysharp.Threading.Tasks;
using Game.Goods.Abstract;
using Services;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class Star : AbstractLevelComponent
{
	[SerializeField] private ParticleSystem _onCollected;
	[SerializeField] public AudioClip powerUpEffect;
	
	[Inject] private readonly AudioPlayerService _audioPlayerService;
	[Inject(Id = GameAudioMixer.Effect)] private readonly AudioMixerGroup _effectMixer;
	
	void Awake() 
	{
		levelDataModel.OnRestart += Reset;
	}

	void OnTriggerEnter2D(Collider2D other) 
	{
		if (levelDataModel.Cricle.gameObject == other.gameObject) 
		{
			_audioPlayerService.Play(powerUpEffect, _effectMixer).Forget();
			ParticleSystem particle = Instantiate(_onCollected, transform.position, Quaternion.identity);
			Destroy(particle.gameObject, 5);
			gameObject.SetActive(false);
			levelDataModel.IncreaseStar();
		}
	}

	private void Reset() 
	{
		gameObject.SetActive(true);
	}
}
