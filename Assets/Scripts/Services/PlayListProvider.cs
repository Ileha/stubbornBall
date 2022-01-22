using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Services
{
    public class PlayListProvider : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> playList = new List<AudioClip>();
            
        [Inject] private readonly AudioPlayerService _playerService;

        private void Start()
        {
            _playerService.SetClips(GetPlayList());
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