using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class LevelsPage : Page
{
	public Transform IconRoot;
	public Button Back;

	private LevelIcon[] icons;

	void Start() {
		Back.onClick.AddListener(app.ShowMain);
		Level[] levels = app.GetAvailableLevels();
		icons = new LevelIcon[levels.Length];

		//Debug.Log(string.Format("last pass level: {0}\nlast open level: {1}", lastPassLevel, lastOpenLevel));

		for (int i = 0; i < levels.Length; i++) {
			icons[i] = app.GetLevelIcon();
			icons[i].transform.SetParent(IconRoot);
			icons[i].transform.localScale = new Vector3(1,1,1);
			icons[i].Init(levels[i]);
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

		//float StarsAverrage = 0;
		//if ((lastPassLevel + 1) == 0) {
		//	StarsAverrage = 1;
		//}
		//else {
		//	for (int i = 0; i <= lastPassLevel; i++) {
		//		StarsAverrage += icons[i].CurrentLevel.Stars;
		//	}
		//	StarsAverrage = Mathf.Max(1, Mathf.Floor(StarsAverrage / (lastPassLevel + 1)));
		//}

		int lastOpenLevel = lastPassLevel + 1;//(int)(lastPassLevel + StarsAverrage);

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

