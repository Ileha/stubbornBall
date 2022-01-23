using System;
using Services;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LevelIcon : MonoBehaviour, IPoolable<Level, IMemoryPool>, IDisposable
{
	public StarControll Stars;
	public Button Select;
	public TextMeshProUGUI Text;
	public Image Lock;

	[Inject] private readonly MainUI mainUi;
	[Inject] private readonly InvisibleElementsSystem _invisibleElementsSystem;
	
	public Level CurrentLevel { get; private set; }
	
	private IMemoryPool _pool;
	private CompositeDisposable _disposable;

	public void SetLevelActive(bool active) 
	{
		Select.interactable = active;
		Lock.gameObject.SetActive(!active);
	}

	private void OnSelect() 
	{
		mainUi.LoadLevel(CurrentLevel);
	}
	
	public void OnDespawned()
	{
		_pool = null;
		_disposable?.Dispose();
		_disposable = null;
		_invisibleElementsSystem.AddElement(gameObject);
	}

	public void OnSpawned(Level level, IMemoryPool pool)
	{
		_disposable = new CompositeDisposable();
		_pool = pool;
		CurrentLevel = level;
		Stars.SetStar(level.Stars);
		Text.text = level.SceneNumber.ToString();
		
		Select
			.onClick
			.AsObservable()
			.Subscribe(x => OnSelect())
			.AddTo(_disposable);
	}

	public void Dispose()
	{
		_pool.Despawn(this);
	}
	
	public class Factory : PlaceholderFactory<Level, LevelIcon>
	{
		
	}
}