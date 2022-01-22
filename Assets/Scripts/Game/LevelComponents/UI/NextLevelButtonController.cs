using System;
using Game.Goods.Abstract;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.LevelComponents.UI
{
    [RequireComponent(typeof(Button))]
    public class NextLevelButtonController : AbstractLevelComponent
    {
        [SerializeField]
        private Button button;

        private void Awake()
        {
            button
                .onClick
                .AsObservable()
                .Subscribe(x => levelDataModel.GoToNextLevel())
                .AddTo(this);
        }
    }
}