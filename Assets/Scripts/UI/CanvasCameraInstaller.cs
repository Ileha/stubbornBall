using System;
using UnityEngine;
using Zenject;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasCameraInstaller : MonoBehaviour
    {
        [Inject] private readonly Camera _camera;

        private void Awake()
        {
            if (gameObject.TryGetComponent(out Canvas canvas))
            {
                canvas.worldCamera = _camera;
            }
        }
    }
}