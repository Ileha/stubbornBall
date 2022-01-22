using System;
using UnityEngine;
using Zenject;

public abstract class Page : MonoBehaviour
{
    [Inject] protected readonly MainUI MainUi;

    public virtual void OnOpen()
    {
    }

    public virtual void OnClose()
    {
    }
}
