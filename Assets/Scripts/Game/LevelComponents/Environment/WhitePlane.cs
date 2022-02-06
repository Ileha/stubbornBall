using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Game.Goods.Abstract;
using Services;
using UnityEngine;
using Zenject;

public class WhitePlane : AbstractLevelComponent 
{
	[SerializeField] public AudioClip disappearEffect;
	
	[Inject] private readonly AudioPlayerService _audioPlayerService;
	
	void Awake() 
	{
		levelDataModel.OnRestart += Reset;
	}

	void OnCollisionExit2D(Collision2D other) 
	{
		gameObject.SetActive(false);
		_audioPlayerService.Play(disappearEffect).Forget();
	}

	private void Reset() 
	{
		gameObject.SetActive(true);
	}
}
