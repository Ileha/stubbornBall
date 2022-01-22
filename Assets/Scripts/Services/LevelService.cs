using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Extensions;
using UniRx;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Zenject;

namespace Services
{
    public class LevelService : IInitializable, IDisposable
    {
        public IReadOnlyReactiveProperty<Level> CurrentLevel => _currentLevel;

        [Inject] private readonly ZenjectSceneLoader _sceneLoader;
        private readonly IReactiveProperty<Level> _currentLevel = new ReactiveProperty<Level>(default);
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private Scene _mainScene;
        
        public void Initialize()
        {
            _mainScene = SceneManager.GetActiveScene();

            _currentLevel
                .CombineWithPrevious((previous, next) => (previous, next))
                .Select(combination =>
                    RxExtensions.FromAsync(token => ChangeLevel(combination.previous, combination.next)))
                .Concat()
                .Subscribe()
                .AddTo(_compositeDisposable);
        }

        public void SetLevel(Level level)
        {
            _currentLevel.Value = level;
        }

        public void MoveToMainMenu()
        {
            _currentLevel.Value = default;
        }

        private async UniTask ChangeLevel(Level previous, Level next)
        {
            UniTask unloadTask = UniTask.CompletedTask;
            if (previous != null)
            {
                unloadTask = SceneManager.UnloadSceneAsync(previous.Scene).ToUniTask();
            }

            UniTask loadTask = UniTask.CompletedTask;
            if (next != null)
            {
                loadTask = _sceneLoader
                    .LoadSceneAsync(next.SceneNumber, LoadSceneMode.Additive, 
                        (container) =>
                        {
                            container
                                .BindInstance(next)
                                .AsSingle();
                        })
                    .ToUniTask();
            }

            await UniTask.WhenAll(unloadTask, loadTask);
            if (next != null)
            {
                SceneManager.SetActiveScene(next.Scene);
            }
            else
            {
                SceneManager.SetActiveScene(_mainScene);
            }
        }

        // public async UniTask LoadLevel(Level level)
        // {
        //     IDisposable disposable = null;
        //     try
        //     {
        //         UniTaskCompletionSource<Scene> sceneLoading = new UniTaskCompletionSource<Scene>();
        //
        //         disposable = Observable
        //             .FromEvent<UnityAction<Scene, LoadSceneMode>, (Scene, LoadSceneMode)>
        //             (
        //                 handler => (scene, mode) => handler.Invoke((scene, mode)),
        //                 action => SceneManager.sceneLoaded += action,
        //                 action => SceneManager.sceneLoaded -= action
        //             )
        //             .Subscribe(data => sceneLoading.TrySetResult(data.Item1));
        //
        //         _sceneLoader
        //             .LoadSceneAsync(level.SceneNumber, LoadSceneMode.Additive, 
        //                 (container) =>
        //                 {
        //                     container
        //                         .BindInstance(level)
        //                         .AsSingle();
        //                 });
        //
        //         var scene = await sceneLoading.Task;
        //         _stack.Push(scene);
        //         SceneManager.SetActiveScene(scene);
        //         _currentLevel.Value = level;
        //     }
        //     finally
        //     {
        //         disposable?.Dispose();
        //     }
        // }
        //
        // public async UniTask MoveToMainMenu()
        // {
        //     var allScenes = _stack.ToArray();
        //     var mainScene = allScenes.Last();
        //     _stack.Clear();
        //     _stack.Push(mainScene);
        //
        //     await UniTask.WhenAll(
        //         allScenes
        //             .Except(new Scene[] {mainScene})
        //             .Select(scene => SceneManager.UnloadSceneAsync(scene).ToUniTask())
        //     );
        //     
        //     SceneManager.SetActiveScene(_stack.Peek());
        //     
        //     _currentLevel.Value = default;
        // }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}