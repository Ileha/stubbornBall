using UnityEngine;
using Zenject;

namespace Game.Goods.Abstract
{
    public class AbstractLevelComponent : MonoBehaviour
    {
        [Inject] public readonly LevelDataModel levelDataModel;
    }
}