using Services;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
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