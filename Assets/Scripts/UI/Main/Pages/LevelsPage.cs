using System.Collections.Generic;
using System.Linq;
using Extensions;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class LevelsPage : Page
{
    public Transform IconRoot;
    public Button Back;

    // private LevelIcon[] icons;

    [Inject] private readonly LevelIcon.Factory _levelIconFactory;

    protected override void Start()
    {
        base.Start();
        Back.onClick.AddListener(MainUi.ShowMain);

        CurrentPageState
            .Where(state => state == PageState.Open)
            .Select(state => CreateViews())
            .CombineWithPrevious((previous, next) => (previous, next))
            .Subscribe(combination => OnChangeLevelViews(combination.previous, combination.next))
            .AddTo(this);

        // Level[] levels = MainUi.GetAvailableLevels();
        // icons = new LevelIcon[levels.Length];
        // for (int i = 0; i < levels.Length; i++)
        // {
        //     icons[i] = _levelIconFactory.Create(levels[i]);
        //     icons[i].transform.SetParent(IconRoot);
        // }
        //
        // CalculateLastOpenLevel();
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

    private void OnChangeLevelViews(IEnumerable<LevelIcon> previous, IEnumerable<LevelIcon> next)
    {
        if (previous != null)
        {
            foreach (var levelIcon in previous)
            {
                Destroy(levelIcon.gameObject);
            }
        }

        if (next != null)
        {
            foreach (var levelIcon in next)
            {
                levelIcon.transform.SetParent(IconRoot);
            }

            CalculateLastOpenLevel(next);
        }
    }

    private void CalculateLastOpenLevel(IEnumerable<LevelIcon> icons)
    {
        foreach (var icon in icons)
        {
            if (icon.CurrentLevel.passed)
            {
                icon.SetLevelActive(true);
            }
            else
            {
                icon.SetLevelActive(true);
                break;
            }
        }
        
        // int lastPassLevel = icons.Count() - 1;
        

        // int lastPassLevel = icons.Count() - 1;
        //
        // for (; lastPassLevel >= 0; lastPassLevel--)
        // {
        //     if (icons[lastPassLevel].CurrentLevel.passed)
        //     {
        //         break;
        //     }
        // }
        //
        // int lastOpenLevel = lastPassLevel + 1;
        //
        // for (int i = 0; i < icons.Length; i++)
        // {
        //     if (i <= lastOpenLevel)
        //     {
        //         icons[i].SetLevelActive(true);
        //     }
        //     else
        //     {
        //         icons[i].SetLevelActive(false);
        //     }
        // }
    }
}