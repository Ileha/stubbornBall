using System.Threading;
using Cysharp.Threading.Tasks;
using Extensions;
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
                .Select(adLoaded => RxExtensions.FromAsync(token => SetButtonAvailability(adLoaded, token)))
                .Switch()
                .Subscribe()
                .AddTo(this);

            button
                .onClick
                .AsObservable()
                .Subscribe(x => levelDataModel.SkipLevel())
                .AddTo(this);
        }

        private async UniTask SetButtonAvailability(bool adLoaded, CancellationToken token)
        {
            gameObject.SetActive(adLoaded && await levelDataModel.IsNextLevelAvailable());
        }
    }
}