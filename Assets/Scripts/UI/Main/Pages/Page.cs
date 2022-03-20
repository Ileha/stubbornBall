using System;
using UniRx;
using UnityEngine;
using Zenject;

public abstract class Page : MonoBehaviour
{
    protected enum PageState
    {
        Open = 1,
        Close = 2,
    }
    
    [SerializeField] protected CanvasGroup canvasGroup;
    
    [Inject] protected readonly MainUI MainUi;

    protected IReadOnlyReactiveProperty<PageState> CurrentPageState
        => MainUi
            .CurrentPage
            .Select(page => page == this ? PageState.Open : PageState.Close)
            .ToReadOnlyReactiveProperty();

    protected virtual void Start()
    {
        CurrentPageState
            .Subscribe(state =>
            {
                if (state == PageState.Open)
                {
                    OnOpen();
                }
                else
                {
                    OnClose();
                }
            })
            .AddTo(this);
    }

    private void OnOpen()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    private void OnClose()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
