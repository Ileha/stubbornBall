using UnityEngine.EventSystems;
using Zenject;

namespace Services
{
    public class EventSystemService : IInitializable
    {
        [Inject] private readonly EventSystem _eventSystem;
        
        public void Initialize()
        {
            _eventSystem.transform.SetParent(null);
        }
    }
}