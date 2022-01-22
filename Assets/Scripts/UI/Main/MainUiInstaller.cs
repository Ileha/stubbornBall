using UnityEngine;
using Zenject;

public class MainUiInstaller : MonoInstaller
{
    [SerializeField]
    private LevelIcon _levelIcon;
    
    public override void InstallBindings()
    {
        Container
            .BindFactory<Level, LevelIcon, LevelIcon.Factory>()
            .FromComponentInNewPrefab(_levelIcon);
    }
}