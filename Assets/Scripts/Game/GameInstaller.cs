using IngameDebugConsole;
using Services;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
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