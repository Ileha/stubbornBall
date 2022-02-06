using Services;
using UnityEngine;
using Zenject;

public class CoreInstaller : MonoInstaller
{
    [SerializeField] private AdData AdSettings;
    [SerializeField] private AudioInstance AudioInstance;
    
    public override void InstallBindings()
    {
        Container
            .BindFactory<AudioInstance, AudioInstance.Factory>()
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
            .BindInterfacesAndSelfTo<SavesService>()
            .AsSingle()
            .NonLazy();

        Container
            .BindInstance(AdSettings)
            .AsSingle()
            .NonLazy();
        
        Container
            .Bind<Camera>()
            .FromInstance(Camera.main)
            .AsSingle();
    }
}