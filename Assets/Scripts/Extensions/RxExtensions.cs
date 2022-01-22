using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

namespace Extensions
{
    public static class RxExtensions
    {
        public static IObservable<TResult> CombineWithPrevious<TSource, TResult>(
            this IObservable<TSource> source,
            Func<TSource, TSource, TResult> resultSelector)
        {
            return source.Scan(
                    Tuple.Create(default(TSource), default(TSource)),
                    (previous, current) => Tuple.Create(previous.Item2, current))
                .Select(t => resultSelector(t.Item1, t.Item2));
        }
        
        public static IObservable<Unit> FromAsync(
            Func<CancellationToken, UniTask> functionAsync
        )
        {
            return FromAsync(async token =>
            {
                await functionAsync(token);
                return Unit.Default;
            });
        }
        
        public static IObservable<TSource> FromAsync<TSource>(
            Func<CancellationToken, UniTask<TSource>> functionAsync
        )
        {
            return Observable.Create<TSource>((observer =>
            {
                var cancellable = new CancellationDisposable();

                var task = default(UniTask<TSource>);
                try
                {
                    task = functionAsync(cancellable.Token);
                }
                catch (Exception exception)
                {
                    var s = Observable.Throw<TSource>(exception).Subscribe(observer);
                    return StableCompositeDisposable.Create(cancellable, s);
                }

                var result = task.ToObservable();
                
                var subscription = result.Subscribe(observer);
                return StableCompositeDisposable.Create(cancellable, subscription);
            }));
        }

        #region Bindings

        public enum BindingTypes
        {
            Default = 0,
            OneWay = 1,
            TwoWay = 2
        }

        public class ProtocolWrapper<T> : ReactiveProperty<T>
        {
            private readonly IObservable<T> _observable;
            private readonly Action<T> _setter;
            private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

            public ProtocolWrapper(IObservable<T> observable, Action<T> setter)
            {
                _observable = observable;
                _setter = setter;

                _observable
                    .Subscribe(x => Value = x)
                    .AddTo(_compositeDisposable);
                
                this
                    .Subscribe(x => _setter?.Invoke(x))
                    .AddTo(_compositeDisposable);
            }

            protected override void Dispose(bool disposing)
            {
                _compositeDisposable.Dispose();
                base.Dispose(disposing);
            }
        }
        
        public static IDisposable Bind<T>(
            IReactiveProperty<T> propertyA , 
            IReactiveProperty<T> propertyB, 
            BindingTypes bindingTypes = BindingTypes.Default)
        {
            var propertyBBinding = propertyB
                .DistinctUntilChanged()
                .Subscribe(x => propertyA.Value = x);

            if (bindingTypes == BindingTypes.OneWay)
            { return propertyBBinding; }

            var propertyABinding = propertyA
                .DistinctUntilChanged()
                .Subscribe(x => propertyB.Value = x);

            return new CompositeDisposable(propertyABinding, propertyBBinding);
        }
        
        public static IDisposable Bind<T>(
            Func<T> propertyAGetter, 
            Action<T> propertyASetter, 
            IReactiveProperty<T> propertyB, 
            BindingTypes bindingTypes = BindingTypes.Default)
        {
            var getterObservable = Observable
                .EveryUpdate()
                .Select(x => propertyAGetter())
                .DistinctUntilChanged();

            var wrapper = new ProtocolWrapper<T>(getterObservable, propertyASetter);

            var binding = Bind(wrapper, propertyB, bindingTypes);
            
            return new CompositeDisposable(wrapper, binding);
        }

        #endregion
    }
}