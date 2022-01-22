using Services;
using UnityEngine;
using Zenject;

public class CoreInstaller : MonoInstaller
{
    [SerializeField]
    private AdData AdSettings;
    
    public override void InstallBindings()
    {
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