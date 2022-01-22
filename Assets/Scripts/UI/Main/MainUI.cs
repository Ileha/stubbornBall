using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using Zenject;
using UniRx;

public class MainUI : MonoBehaviour
{
	public Page Main;
	public Page Levels;
	public Page Settings;

	public TextMeshProUGUI text;
	[SerializeField] private CanvasGroup _canvasGroup;
	
	[Inject] private readonly SavesService _data;
	[Inject] private readonly AdService _adService;
	[Inject] private readonly LevelService levelService;

	void Start() 
	{
		text.text = string.Format("v {0}", Application.version);
		levelService
			.CurrentLevel
			.Subscribe(OnLevelChange)
			.AddTo(this);
		
		_adService.ShowBanner(BannerPosition.TOP_CENTER);
	}

	public void ShowMain() {
		Main.transform.SetAsLastSibling();
	}

	public void ShowLevels() {
		Levels.transform.SetAsLastSibling();
	}

	public void ShowSettings() {
		Settings.transform.SetAsLastSibling();
	}

	public Level[] GetAvailableLevels() 
	{
		return _data.GetAllLevels();
	}

	public void LoadLevel(Level level) 
	{
		levelService.SetLevel(level);
	}

	private void OnLevelChange(Level level)
	{
		_canvasGroup.alpha = level == default ? 1 : 0;
		_canvasGroup.blocksRaycasts = level == default;
	}
}
