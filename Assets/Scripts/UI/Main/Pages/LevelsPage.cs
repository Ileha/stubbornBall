using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extensions;
using UniRx;
using UnityEngine.UI;
using Zenject;

public class LevelsPage : Page
{
    public ScrollRect scrollRect;
    public Button back;

    [Inject] private readonly LevelIcon.Factory _levelIconFactory;

    protected override void Start()
    {
        base.Start();
        back.onClick.AddListener(MainUi.ShowMain);

        CurrentPageState
            .Where(state => state == PageState.Open)
            .Select(state => CreateViews())
            .CombineWithPrevious((previous, next) => (previous, next))
            .Select(combination => OnChangeLevelViews(combination.previous, combination.next))
            .Select(icon => RxExtensions.FromAsync(async token =>
            {
                if (icon != default)
                {
                    await scrollRect
                        .GetSnapToPositionToBringChildIntoView(icon.RectTransform, 0.6f)
                        .ToUniTask(cancellationToken: token);
                }
            }))
            .Switch()
            .Subscribe()
            .AddTo(this);
    }

    private IEnumerable<LevelIcon> CreateViews()
    {
        Level[] levels = MainUi.GetAvailableLevels();
        var icons = new LevelIcon[levels.Length];
        for (int i = 0; i < levels.Length; i++)
        {
            icons[i] = _levelIconFactory.Create(levels[i]);
        }
        return icons;
    }

    private LevelIcon OnChangeLevelViews(IEnumerable<LevelIcon> previous, IEnumerable<LevelIcon> next)
    {
        if (previous != null)
        {
            foreach (var levelIcon in previous)
            {
                levelIcon.Dispose();
            }
        }

        if (next != null)
        {
            foreach (var levelIcon in next)
            {
                levelIcon.transform.SetParent(scrollRect.content);
            }

            return CalculateLastOpenLevel(next);
        }

        return default;
    }

    private LevelIcon CalculateLastOpenLevel(IEnumerable<LevelIcon> icons)
    {
        LevelIcon levelIcon = default;
        
        foreach (var icon in icons)
        {
            if (levelIcon == default)
            {
                if (icon.CurrentLevel.passed)
                {
                    icon.SetLevelActive(true);
                }
                else
                {
                    icon.SetLevelActive(true);
                    levelIcon = icon;
                }
            }
            else
            {
                icon.SetLevelActive(false);
            }
        }

        return levelIcon;
    }
}