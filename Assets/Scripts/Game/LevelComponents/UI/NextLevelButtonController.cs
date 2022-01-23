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
            if (!levelDataModel.IsNextLevelAvailable)
            {
                gameObject.SetActive(false);
                return;
            }
            
            button
                .onClick
                .AsObservable()
                .Subscribe(x => levelDataModel.GoToNextLevel())
                .AddTo(this);
        }
    }
}