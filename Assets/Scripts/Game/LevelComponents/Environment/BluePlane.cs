using System;
using CommonData;
using Cysharp.Threading.Tasks;
using Game.Goods.Abstract;
using Services;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace Game.LevelComponents.Environment
{
    public class BluePlane : AbstractLevelComponent
    {
        [SerializeField] public AudioClip bounceEffect;
	
        [Inject] private readonly AudioPlayerService _audioPlayerService;
        [Inject(Id = GameAudioMixer.Effect)] private readonly AudioMixerGroup _effectMixer;

        private void OnCollisionEnter2D(Collision2D other)
        {
            _audioPlayerService.Play(bounceEffect, _effectMixer).Forget();
        }
    }
}