using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CommonData;
using Cysharp.Threading.Tasks;
using Services;
using UnityEngine.Audio;
using Zenject;

public class EndOfLevel : MonoBehaviour 
{
    [SerializeField] public AudioClip winEffect;
	
    [Inject] private readonly AudioPlayerService _audioPlayerService;
    [Inject(Id = GameAudioMixer.Effect)] private readonly AudioMixerGroup _effectMixer;

    private void OnEnable()
    {
        _audioPlayerService.Play(winEffect, _effectMixer).Forget();
    }
}
