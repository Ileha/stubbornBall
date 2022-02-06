using System;
using Cysharp.Threading.Tasks;
using Game.Goods.Abstract;
using Services;
using UnityEngine;
using Zenject;

namespace Game.LevelComponents.Environment
{
    public class BluePlane : AbstractLevelComponent
    {
        [SerializeField] public AudioClip bounceEffect;
	
        [Inject] private readonly AudioPlayerService _audioPlayerService;

        private void OnCollisionEnter2D(Collision2D other)
        {
            _audioPlayerService.Play(bounceEffect).Forget();
        }
    }
}