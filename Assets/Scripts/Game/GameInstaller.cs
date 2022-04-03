using IngameDebugConsole;
using Services;
using UnityEngine.EventSystems;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container
            .BindInterfacesAndSelfTo<EventSystemService>()
            .AsSingle()
            .NonLazy();
        
        Container
            .Bind<EventSystem>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
        
        Container
            .Bind<DebugLogManager>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
        
        Container
            .BindInterfacesAndSelfTo<InGameConsoleConfigurator>()
            .AsSingle()
            .NonLazy();
        
        Container
            .Bind<MainUI>()
            .FromComponentInHierarchy()
            .AsSingle();

        Container
            .Bind<InvisibleElementsSystem>()
            .FromComponentInHierarchy()
            .AsSingle();
    }
}