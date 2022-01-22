using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LevelIcon : MonoBehaviour
{
	public StarControll Stars;
	public Button Select;
	public TextMeshProUGUI Text;
	public Image Lock;

	[Inject] private readonly MainUI mainUi;
	
	public Level CurrentLevel { get; private set; }

	[Inject]
	private void Init(Level level) 
	{
		CurrentLevel = level;
		Stars.SetStar(level.Stars);
		Text.text = level.SceneNumber.ToString();
		
		Select
			.onClick
			.AsObservable()
			.Subscribe(x => OnSelect())
			.AddTo(this);
		
		SetLevelActive(true);
	}

	public void SetLevelActive(bool active) 
	{
		Select.interactable = active;
		Lock.gameObject.SetActive(!active);
	}

	private void OnSelect() 
	{
		mainUi.LoadLevel(CurrentLevel);
	}
	
	public class Factory : PlaceholderFactory<Level, LevelIcon>
	{
		
	}
}