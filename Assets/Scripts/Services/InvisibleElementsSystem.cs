using UnityEngine;

namespace Services
{
    public class InvisibleElementsSystem : MonoBehaviour
    {
        public void AddElement(GameObject go)
        {
            go.transform.SetParent(transform);
        }
    }
}