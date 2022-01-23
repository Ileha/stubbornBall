using Cysharp.Threading.Tasks;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using Zenject;
using UniRx;

public class MainUI : MonoBehaviour
{
    public IReadOnlyReactiveProperty<Page> CurrentPage
        => Observable.CombineLatest(
                _currentPage,
                levelService.CurrentLevel,
                (page, level) => (page, level))
            .Select(combination => combination.level != default ? default : combination.page)
            .ToReadOnlyReactiveProperty();

    [SerializeField] private Page Main;
    [SerializeField] private Page Levels;
    [SerializeField] private Page Settings;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup _canvasGroup;

    [Inject] private readonly SavesService _data;
    [Inject] private readonly AdService _adService;
    [Inject] private readonly LevelService levelService;
    private readonly IReactiveProperty<Page> _currentPage = new ReactiveProperty<Page>();

    private void Awake()
    {
        _currentPage.Value = Main;
    }

    void Start()
    {
        text.text = string.Format("v {0}", Application.version);
        levelService
            .CurrentLevel
            .Subscribe(OnLevelChange)
            .AddTo(this);

        CurrentPage
            .Where(page => page != default)
            .Subscribe(page => _adService.ShowBanner(BannerPosition.TOP_CENTER).Forget())
            .AddTo(this);
    }
    
    public void ShowMain()
    {
        _currentPage.Value = Main;
    }

    public void ShowLevels()
    {
        _currentPage.Value = Levels;
    }

    public void ShowSettings()
    {
        _currentPage.Value = Settings;
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