using CommonData;
using Interfaces;
using Services;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class CoreInstaller : MonoInstaller
{
    [SerializeField] private AdData AdSettings;
    [SerializeField] private AudioInstance AudioInstance;
    [SerializeField] private AudioMixerGroup effectMixer;
    [SerializeField] private AudioMixerGroup musicMixer;
    [SerializeField] private AudioMixerGroup masterMixer;
    
    public override void InstallBindings()
    {
        Container
            .BindInterfacesAndSelfTo<COPPAChecker>()
            .AsSingle()
            .NonLazy();
        
        Container
            .BindInstance(effectMixer)
            .WithId(GameAudioMixer.Effect)
            .NonLazy();
        
        Container
            .BindInstance(musicMixer)
            .WithId(GameAudioMixer.Music)
            .NonLazy();
        
        Container
            .BindInstance(masterMixer)
            .WithId(GameAudioMixer.Master)
            .NonLazy();
        
        Container
            .BindFactory<AudioMixerGroup, AudioInstance, AudioInstance.Factory>()
            .FromMonoPoolableMemoryPool(x => x
                .FromComponentInNewPrefab(AudioInstance)
                .UnderTransformGroup("AudioInstances")
            );
        
        Container
            .BindInterfacesAndSelfTo<LevelService>()
            .AsSingle()
            .NonLazy();
        
        Container
            .BindInterfacesAndSelfTo<AdService>()
            .AsSingle()
            .NonLazy();
        
        Container
            .BindInterfacesAndSelfTo<AudioPlayerService>()
            .AsSingle()
            .NonLazy();
        
        Container
            .BindInterfacesAndSelfTo<LevelsService>()
            .AsSingle()
            .NonLazy();

        Container
            .Bind<IAdData>()
            .To<AdData>()
            .FromInstance(AdSettings)
            .AsSingle()
            .NonLazy();
        
        Container
            .Bind<Camera>()
            .FromInstance(Camera.main)
            .AsSingle();
    }
}