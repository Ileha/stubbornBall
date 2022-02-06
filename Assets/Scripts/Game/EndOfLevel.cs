using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;
using Services;
using Zenject;

public class EndOfLevel : MonoBehaviour 
{
    [SerializeField] public AudioClip winEffect;
	
    [Inject] private readonly AudioPlayerService _audioPlayerService;

    private void OnEnable()
    {
        _audioPlayerService.Play(winEffect).Forget();
    }
}
