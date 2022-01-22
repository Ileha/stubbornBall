using System.Collections;
using System.Collections.Generic;
using Game.Goods.Abstract;
using UnityEngine;

public class Star : AbstractLevelComponent
{
	[SerializeField]
	private ParticleSystem _onCollected;
	
	void Awake() 
	{
		levelDataModel.OnRestart += Reset;
	}

	void OnTriggerEnter2D(Collider2D other) 
	{
		if (levelDataModel.Cricle.gameObject == other.gameObject) 
		{
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
