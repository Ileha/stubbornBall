using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.EventSystems;
using UnityEngine.Advertisements;

public delegate void OnGameEvent();
public delegate void OnInputEvent(Vector3 direction);

public class SceneSingleton : MonoBehaviour
{
	private enum GameState { 
		Play,
		Draw
	}

	public DrawLine lineDrawer;

	public ICricle Cricle;
	public StarControll StarView;
	public EndOfLevel endOfLevel;

	public event OnGameEvent OnRestart;
	public event OnGameEvent OnPlay;
	public event OnInputEvent OnShaking;

	public Level select { get; private set; }

	private List<Iinput> InputSubscribers = new List<Iinput>();

	private List<RaycastResult> raycastResult = new List<RaycastResult>();
	private PointerEventData pointData;

	private Acceleration acceleration;
	private float maxTime = 0.25f;
	private bool isExecute = false;
	private GameState state = GameState.Draw;

	void Start() {
		Subscribe(lineDrawer);

		Singleton.Instanse.AdSettings.ShowBanner(BannerPosition.TOP_CENTER);
		endOfLevel.Disable();
		if (Singleton.Instanse.SceneInformation != null && Singleton.Instanse.SceneInformation is LevelInformation) {
			select = (Singleton.Instanse.SceneInformation as LevelInformation).select;
		}
		pointData = new PointerEventData(EventSystem.current);

#if UNITY_ANDROID || UNITY_IOS
		acceleration = new Acceleration();
#endif
	}

	// Update is called once per frame
	void Update() {
		if (InputSubscribers.Count == 0) { return; }
		Iinput lastSubscriber = InputSubscribers[InputSubscribers.Count - 1];

#if UNITY_STANDALONE
		if (Input.GetMouseButtonDown(0)) {
			lastSubscriber.OnStart(Input.mousePosition);
		}
		if (Input.GetMouseButton(0)) {
			lastSubscriber.OnMove(Input.mousePosition);
		}
		if (Input.GetMouseButtonUp(0)) {
			lastSubscriber.OnEnd(Input.mousePosition);
		}
#endif

#if UNITY_ANDROID || UNITY_IOS
		if (Input.touchCount > 0) {
			if (Input.GetTouch(0).phase == TouchPhase.Began) {
				lastSubscriber.OnStart(Input.GetTouch(0).position);
			}
			if (Input.GetTouch(0).phase == TouchPhase.Moved) {
				lastSubscriber.OnMove(Input.GetTouch(0).position);
			}
			if (Input.GetTouch(0).phase == TouchPhase.Ended) {
				lastSubscriber.OnEnd(Input.GetTouch(0).position);
			}
		}
#endif
	}

	void FixedUpdate() {
#if UNITY_ANDROID || UNITY_IOS
		if (!isExecute) {
			Vector3 result = acceleration.GetlinearAcceleration() / Time.fixedDeltaTime;
			if (result.magnitude > 1) {
				StartCoroutine(CollectInput(result));
			}
		}
#endif
	}

	public IEnumerator CollectInput(Vector3 start) {
		isExecute = true;
		List<Vector3> InputData = new List<Vector3>();
		float time = 0;

		while (time < maxTime) {
			InputData.Add(acceleration.GetlinearAcceleration() / Time.deltaTime);
			time += Time.deltaTime;
			yield return null;
		}

		Vector3 second = InputData[0];
		float angle = Vector3.Angle(start, second);
		for (int i = 1; i < InputData.Count; i++) {
			float currentAngle = Vector3.Angle(start, InputData[i]);
			if (currentAngle > angle) {
				second = InputData[i];
				angle = currentAngle;
			}
		}
		float magnitude = start.magnitude + second.magnitude;
		//outAcceleration.text = string.Format("x: {0:0.0000};\ny: {1:0.0000}\nmagnitude: {2:0.000}\n angle: {3}",
		//											 -second.x, -second.y, magnitude, angle);
		
		if (OnShaking != null) {
			OnShaking(-second.normalized * magnitude);
		}
		isExecute = false;
	}

	public void Subscribe(Iinput input) {
		if (InputSubscribers.Contains(input)) { return; }
		InputSubscribers.Add(input);
	}

	public void Unsubscribe(Iinput input) {
		InputSubscribers.Remove(input);
	}

	public bool CanDraw(Vector3 ScreenPosition) {
		pointData.position = ScreenPosition;
		raycastResult.Clear();
		EventSystem.current.RaycastAll(pointData, raycastResult);
		if (raycastResult.Count == 0) {
			return true;
		}

		return false;
	}

	public void Play() {
		if (state != GameState.Draw) { return; }

		state = GameState.Play;

		Unsubscribe(lineDrawer);
		if (OnPlay != null) { OnPlay(); }
	}

	public void Restart() {
		if (state != GameState.Play) { return; }

		state = GameState.Draw;

		Subscribe(lineDrawer);
		Singleton.Instanse.AdSettings.ShowBanner(BannerPosition.TOP_CENTER);

		endOfLevel.Disable();
		StarView.Reset();
		if (OnRestart != null) { OnRestart(); }

		if (Singleton.Instanse.AdSettings.HasNextAd()) {
			Singleton.Instanse.AdSettings.ShowVideo(null);
		}
	}

	public void ToMeny() {
		Singleton.Instanse.SceneInformation = new ShowLevels();
		SceneManager.LoadScene(Singleton.Instanse.Data.MenyLevel, LoadSceneMode.Single);
	}

	public void IncreaseStar() {
		StarView.AddStar();
	}

	public bool IsCircle(GameObject player) {
		return player == Cricle.gameObject;
	}

	public void Finish(GameObject player) {
		if (player == Cricle.gameObject) {
			ParticleSystem particle = Instantiate(Singleton.Instanse.OnEnd, player.transform.position, Quaternion.identity);
			Destroy(particle, 5);

			Cricle.gameObject.SetActive(false);

			Singleton.Instanse.Data.SetLevelStar(select, StarView.GetCurrentState());

			Level result = null;
			if (HasNextLevel(out result)) {
				endOfLevel.Enable(StarView.GetCurrentState(), GoToNextLevel, Restart, ToMeny);
			}
			else {
				endOfLevel.Enable(StarView.GetCurrentState(), null, Restart, ToMeny);
			}

			Singleton.Instanse.AdSettings.ShowBanner(BannerPosition.BOTTOM_CENTER);
		}
	}

	public bool HasNextLevel(out Level next) {
		try {
			next = Singleton.Instanse.Data.GetLevelInfo(select.SceneNumber+1);
			return true;
		}
		catch (Exception err) {
			next = null;
			return false;
		}
	}

	public void GoToNextLevel() {
		Level nextLevel = null;
		if (HasNextLevel(out nextLevel)) {
			if (Singleton.Instanse.AdSettings.IsInitialized() && Singleton.Instanse.AdSettings.HasNextAd()) {
				Singleton.Instanse.AdSettings.ShowVideo((obj) => {
					Singleton.Instanse.SceneInformation = new LevelInformation(nextLevel);
					SceneManager.LoadScene(nextLevel.SceneNumber, LoadSceneMode.Single);
				});
			}
			else {
				Singleton.Instanse.SceneInformation = new LevelInformation(nextLevel);
				SceneManager.LoadScene(nextLevel.SceneNumber, LoadSceneMode.Single);
			}
		}
	}

	public void skipLevel() {
		Singleton.Instanse.AdSettings.ShowRewardedVideo((obj) => {
			if (obj == ShowResult.Finished) {
				Singleton.Instanse.Data.SetLevelStar(select, 0);
				Level nextLevel = null;
				if (HasNextLevel(out nextLevel)) {
					Singleton.Instanse.SceneInformation = new LevelInformation(nextLevel);
					SceneManager.LoadScene(nextLevel.SceneNumber, LoadSceneMode.Single);
				}
			}
		});
	}
}
