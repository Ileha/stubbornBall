using System.Collections;
using System.Collections.Generic;
using Game.Goods.Abstract;
using UnityEngine;

public class LevelBorders : AbstractLevelComponent
{
	void OnTriggerEnter2D(Collider2D other) 
	{
		if (levelDataModel.Cricle.gameObject == other.gameObject) 
		{
			levelDataModel.Restart();
		}
	}
}
