using Game.Goods.Abstract;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.LevelComponents.UI
{
    public class SkipLevelButtonController : AbstractLevelComponent
    {
        [SerializeField] private Button button;

        private void Awake()
        {
            button
                .onClick
                .AsObservable()
                .Subscribe(x => levelDataModel.SkipLevel())
                .AddTo(this);
        }
    }
}