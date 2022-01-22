using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using Zenject;

public class LevelsPage : Page
{
	public Transform IconRoot;
	public Button Back;

	private LevelIcon[] icons;

	[Inject] private readonly LevelIcon.Factory _levelIconFactory;
	
	void Start() 
	{
		Back.onClick.AddListener(MainUi.ShowMain);
		Level[] levels = MainUi.GetAvailableLevels();
		icons = new LevelIcon[levels.Length];

		for (int i = 0; i < levels.Length; i++)
		{
			icons[i] = _levelIconFactory.Create(levels[i]);
			icons[i].transform.SetParent(IconRoot);
		}

		CalkLastOpenLevel();
	}

	private void CalkLastOpenLevel() {
		int lastPassLevel = icons.Length - 1;

		for (; lastPassLevel >= 0; lastPassLevel--) {
			if (icons[lastPassLevel].CurrentLevel.passed) {
				break;
			}
		}

		int lastOpenLevel = lastPassLevel + 1;

		for (int i = 0; i < icons.Length; i++) {
			if (i <= lastOpenLevel) {
				icons[i].SetLevelActive(true);
			}
			else {
				icons[i].SetLevelActive(false);
			}
		}
	}
}

