using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CommonData;
using Cysharp.Threading.Tasks;
using Game.LevelComponents.Environment;
using GoogleMobileAds.Api;
using Interfaces;
using Services;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;
using Zenject;

public class LevelDataModel : MonoBehaviour
{
    private enum GameState
    {
        Play,
        Draw
    }

    public event Action OnRestart;
    public event Action OnPlay;
    public event Action<Vector3> OnShaking;

    [Inject] public readonly MainCircle Cricle;
    public IReadOnlyReactiveProperty<int> StarCount => _starCount;

    private List<Iinput> InputSubscribers = new List<Iinput>();
    private List<RaycastResult> raycastResult = new List<RaycastResult>();
    private PointerEventData pointData;
    private Acceleration acceleration;
    private float maxTime = 0.25f;
    private bool isExecute = false;
    private GameState state = GameState.Draw;
    private IReactiveProperty<int> _starCount = new ReactiveProperty<int>();

    [Inject] private readonly LevelsService _data;
    [Inject] private readonly AdService _adService;
    [Inject] private readonly LevelService _levelService;
    [Inject] private readonly Level _level;
    [Inject] private readonly DrawLine _lineDrawer;
    [Inject] private readonly EndOfLevel _endOfLevel;
    [Inject] private readonly IAdData _adData;

    void Start()
    {
        Subscribe(_lineDrawer);

        _adService.ShowBanner(AdPosition.Top).Forget();
        _endOfLevel.gameObject.SetActive(false);
        pointData = new PointerEventData(EventSystem.current);

        Observable
            .FromEvent<Action<Vector3>, Vector3>
            (
                handler => handler.Invoke,
                add => OnShaking += add,
                remove => OnShaking += remove
            )
            .Subscribe(direction =>
            {
                Analytics.CustomEvent(
                    Constants.ShakeDetection,
                    new Dictionary<string, object>()
                    {
                        { "magnitude", direction.magnitude }
                    }
                );
            })
            .AddTo(this);

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        acceleration = new Acceleration();
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (InputSubscribers.Count == 0)
        {
            return;
        }

        Iinput lastSubscriber = InputSubscribers[InputSubscribers.Count - 1];

#if UNITY_STANDALONE || UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            lastSubscriber.OnStart(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            lastSubscriber.OnMove(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            lastSubscriber.OnEnd(Input.mousePosition);
        }
#endif

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
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

    void FixedUpdate()
    {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
		if (!isExecute) {
			Vector3 result = acceleration.GetlinearAcceleration() / Time.fixedDeltaTime;
			if (result.magnitude > 1) {
				StartCoroutine(CollectInput(result));
			}
		}
#endif
    }

    public IEnumerator CollectInput(Vector3 start)
    {
        isExecute = true;
        List<Vector3> InputData = new List<Vector3>();
        float time = 0;

        while (time < maxTime)
        {
            InputData.Add(acceleration.GetlinearAcceleration() / Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }

        Vector3 second = InputData[0];
        float angle = Vector3.Angle(start, second);
        for (int i = 1; i < InputData.Count; i++)
        {
            float currentAngle = Vector3.Angle(start, InputData[i]);
            if (currentAngle > angle)
            {
                second = InputData[i];
                angle = currentAngle;
            }
        }

        float magnitude = start.magnitude + second.magnitude;
        //outAcceleration.text = string.Format("x: {0:0.0000};\ny: {1:0.0000}\nmagnitude: {2:0.000}\n angle: {3}",
        //											 -second.x, -second.y, magnitude, angle);

        if (OnShaking != null)
        {
            OnShaking(-second.normalized * magnitude);
        }

        isExecute = false;
    }

    public void Subscribe(Iinput input)
    {
        if (InputSubscribers.Contains(input))
        {
            return;
        }

        InputSubscribers.Add(input);
    }

    public void Unsubscribe(Iinput input)
    {
        InputSubscribers.Remove(input);
    }

    public bool CanDraw(Vector3 ScreenPosition)
    {
        pointData.position = ScreenPosition;
        raycastResult.Clear();
        EventSystem.current.RaycastAll(pointData, raycastResult);
        if (raycastResult.Count == 0)
        {
            return true;
        }

        return false;
    }

    public void Play()
    {
        if (state != GameState.Draw)
        {
            return;
        }

        state = GameState.Play;

        Unsubscribe(_lineDrawer);
        if (OnPlay != null)
        {
            OnPlay();
        }
    }

    public async void Restart()
    {
        if (state != GameState.Play)
        {
            return;
        }

        state = GameState.Draw;

        Subscribe(_lineDrawer);
        _adService.ShowBanner(AdPosition.Top).Forget();
        
        Analytics.CustomEvent(
            Constants.LevelRestarted,
            new Dictionary<string, object>()
            {
                { "level", _level.SceneNumber },
                { "stars", _starCount.Value },
                { "passed", _level.passed }
            }
        );

        _endOfLevel.gameObject.SetActive(false);
        _starCount.Value = 0;
        OnRestart?.Invoke();

        if (_adService.HasNextAd())
        {
            await _adService.ShowVideo();
        }
    }

    public void ToMeny()
    {
        _levelService.MoveToMainMenu();
    }

    public void IncreaseStar()
    {
        _starCount.Value += 1;
    }

    public bool IsCircle(GameObject player)
    {
        return player == Cricle.gameObject;
    }

    public async void Finish(GameObject player)
    {
        if (player == Cricle.gameObject)
        {
            Cricle.gameObject.SetActive(false);

            _data.SetLevelStar(_level, _starCount.Value);
            
            Analytics.CustomEvent(
                Constants.LevelPassed,
                new Dictionary<string, object>()
                {
                    { "level", _level.SceneNumber },
                    { "stars", _starCount.Value },
                    { "passed", _level.passed }
                }
            );
            
            if (await IsNextLevelAvailable())
            {
                _endOfLevel.gameObject.SetActive(true);
            }
            else
            {
                _endOfLevel.gameObject.SetActive(true);
                Analytics.CustomEvent(
                    Constants.LastLevelPassed,
                    new Dictionary<string, object>()
                    {
                        { "level", _level.SceneNumber },
                        { "stars", _starCount.Value },
                        { "passed", _level.passed }
                    }
                );
            }

            _adService.ShowBanner(AdPosition.Bottom).Forget();
        }
    }

    public async UniTask<bool> IsNextLevelAvailable()
    {
        var nextLevel = await HasNextLevel();
        return nextLevel != default;
    }

    private async UniTask<Level> HasNextLevel()
    {
        try
        {
            return await _data.GetLevelInfo(_level.SceneNumber + 1);
        }
        catch (Exception err)
        {
            return default;
        }
    }

    public async void GoToNextLevel()
    {
        var nextLevel = await HasNextLevel();
        if (nextLevel != default)
        {
            if (_adService.HasNextAd())
            {
                try
                {
                    await _adService.ShowVideo();
                }
                catch (AdService.AdsExceptionBase e)
                {
#if DEBUG
                    Debug.LogWarning(e.Message);
#endif
                }
            }
            
            _levelService.SetLevel(nextLevel);
        }
    }

    public async void SkipLevel()
    {
        Reward result = default;
        try
        {
            result = await _adService.ShowRewardedVideo();
        }
        catch (AdService.AdsExceptionBase e)
        {
#if DEBUG
            Debug.LogWarning(e.Message);
#endif
            return;
        }

        async UniTask SetLevelPassed()
        {
            await _data.SetLevelStar(_level, 0);
            
            Analytics.CustomEvent(
                Constants.LevelSkipped,
                new Dictionary<string, object>()
                {
                    { "level", _level.SceneNumber },
                    { "stars", _starCount.Value },
                    { "passed", _level.passed }
                }
            );

            var nextLevel = await HasNextLevel();
            if (nextLevel != default)
            {
                _levelService.SetLevel(nextLevel);
            }
        }


#if UNITY_EDITOR
        
        await SetLevelPassed();
        
#elif UNITY_IOS || UNITY_ANDROID

        if (_adData.SkipLevelRewardIdentity.Equals(result.Type) && result.Amount > 0)
        {
            await SetLevelPassed();
        }
#endif
    }
}