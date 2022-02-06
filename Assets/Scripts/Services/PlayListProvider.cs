using System;
using System.Collections.Generic;
using CommonData;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;
using Random = UnityEngine.Random;

namespace Services
{
    public class PlayListProvider : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> playList = new List<AudioClip>();
            
        [Inject] private readonly AudioPlayerService _playerService;
        [Inject(Id = GameAudioMixer.Music)] private readonly AudioMixerGroup _musicMixer;

        private void Start()
        {
            _playerService.Play(GetPlayList(), _musicMixer).Forget();
        }

        private IEnumerable<AudioClip> GetPlayList()
        {
            while (true)
            {
                var result = playList[Random.Range(0, playList.Count)];
                yield return result;
            }
        }
    }
}