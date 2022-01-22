using System;
using Game.Goods.Abstract;
using UniRx;
using UnityEngine;

namespace Game.LevelComponents.UI
{
    [RequireComponent(typeof(StarControll))]
    public class LevelStarController : AbstractLevelComponent
    {
        [SerializeField]
        private StarControll starControll;

        private void Awake()
        {
            levelDataModel
                .StarCount
                .Subscribe(count => starControll.SetStar(count))
                .AddTo(this);
        }
    }
}