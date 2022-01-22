using Game.LevelComponents.Environment;
using Zenject;

namespace Game
{
    public class LevelInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<DrawLine>()
                .FromComponentsInHierarchy()
                .AsSingle();
            
            Container
                .Bind<StarControll>()
                .FromComponentsInHierarchy()
                .AsSingle();
            
            Container
                .Bind<MainCircle>()
                .FromComponentsInHierarchy()
                .AsSingle();
            
            Container
                .Bind<LevelDataModel>()
                .FromComponentsInHierarchy()
                .AsSingle();
            
            Container
                .Bind<EndOfLevel>()
                .FromComponentsInHierarchy()
                .AsSingle();
        }
    }
}