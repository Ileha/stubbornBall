using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class MainUI : MonoBehaviour
{
	public Page Main;
	public Page Levels;
	public Page Settings;

	public Text text;

	//public GraphicRaycaster gr;

	void Start() {
		text.text = string.Format("v {0}", Application.version);
		if (Singleton.Instanse.SceneInformation != null && Singleton.Instanse.SceneInformation is IMainUIInformation) {
			(Singleton.Instanse.SceneInformation as IMainUIInformation).Handle(this);
		}
		else {
			ShowMain();
		}
		Singleton.Instanse.AdSettings.ShowBanner(BannerPosition.TOP_CENTER);
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

	public Level[] GetAvailableLevels() {
		return Singleton.Instanse.Data.GetAllLevels();
	}

	public LevelIcon GetLevelIcon() {
		return Instantiate(Singleton.Instanse.LevelIcon);
	}

	public void LoadLevel(Level level) {
		Singleton.Instanse.SceneInformation = new LevelInformation(level);
		SceneManager.LoadScene(level.SceneNumber, LoadSceneMode.Single);
	}
}
