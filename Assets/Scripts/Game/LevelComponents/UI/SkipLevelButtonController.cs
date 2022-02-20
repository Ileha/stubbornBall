using Game.Goods.Abstract;
using Services;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.LevelComponents.UI
{
    public class SkipLevelButtonController : AbstractLevelComponent
    {
        [SerializeField] private Button button;

        [Inject] private readonly AdService _adService;

        private void Awake()
        {
            _adService
                .AdsLoaded
                .Subscribe(SetButtonAvailability)
                .AddTo(this);

            button
                .onClick
                .AsObservable()
                .Subscribe(x => levelDataModel.SkipLevel())
                .AddTo(this);
        }

        private void SetButtonAvailability(bool adLoaded)
        {
            gameObject.SetActive(adLoaded && levelDataModel.IsNextLevelAvailable);
        }
    }
}