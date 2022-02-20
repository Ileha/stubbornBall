using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonData;
using Cysharp.Threading.Tasks;
using Extensions;
using GoogleMobileAds.Api;
using UniRx;
using UnityEngine;
using Zenject;

namespace Services
{
    public class AdService : IInitializable
    {
	    public IReadOnlyReactiveProperty<bool> AdsLoaded => _adsLoaded;
	    
	    private int addCount = 0;

        [Inject] private readonly AdData _adData;
        private InitializationStatus _initializationStatus;

        private InterstitialAd _interstitialAd;
        private RewardedAd _rewardedAd;
        private BannerView _bannerView;
        private AdRequest _adRequest;
        private IReactiveProperty<bool> _adsLoaded = new ReactiveProperty<bool>(false);
        private TaskCompletionSource<object> _loadingCompletion = new TaskCompletionSource<object>();
        private Task AdsLoadingTask => _loadingCompletion.Task;
        
        public void Initialize()
        {
	        MobileAds.Initialize(status =>
	        {
		        _initializationStatus = status;

		        _adsLoaded.Value = IsLoaded();

		        if (IsLoaded())
		        {
			        _loadingCompletion.TrySetResult(default);
		        }

#if AdsTest

		        RequestConfiguration requestConfiguration = new RequestConfiguration
				        .Builder()
			        .SetTestDeviceIds(Constants.DeviceIds)
			        .build();
		        
		        MobileAds.SetRequestConfiguration(requestConfiguration);

#endif
		        
		        _adRequest = new AdRequest.Builder()
			        .Build();
	        });
        }

        public bool IsLoaded()
        {
	        return _initializationStatus != null;
        }

        public bool HasNextAd()
        {
	        var localMax = (AdData.MaxAdFrequency - _adData.AdFrequency)+1;
	        addCount = (addCount + 1) % localMax;
	        Debug.Log($"Advertisement: {addCount}/{localMax}");
	        return addCount == 0;
        }
    
        public async UniTask ShowBanner(AdPosition position)
        {
	        await AdsLoadingTask;

	        _bannerView?.Destroy();
	        _bannerView = new BannerView(_adData.BannerId, AdSize.Banner, position);
	        await new BannerViewLoading(_bannerView).Load(_adRequest);
	        Debug.Log("Advertisement: show banner");
        }
        
        public async UniTask ShowVideo() 
        {
	        await AdsLoadingTask;
	        
	        var id = AnalyticsExtensions.BeginEvent(Constants.RegularVideo);
	        try
	        {
		        _interstitialAd?.Destroy();
		        _interstitialAd = new InterstitialAd(_adData.Video);
		        await new InterstitialAdLoading(_interstitialAd).Load(_adRequest);
		        await ShowInterstitialAd(_interstitialAd);
	        }
	        finally
	        {
		        AnalyticsExtensions.CompleteEvent(id);
		        Debug.Log("Advertisement: show video");
	        }
        }
    
        public async UniTask<Reward> ShowRewardedVideo()
        {
	        await AdsLoadingTask;
	        
	        var id = AnalyticsExtensions.BeginEvent(Constants.RewardedVideo);
	        try
	        {
		        _rewardedAd?.Destroy();
		        _rewardedAd = new RewardedAd(_adData.RewardedVideo);
		        await new RewardedAdLoading(_rewardedAd).Load(_adRequest);
		        var result = await ShowRewardedAd(_rewardedAd);
		        return result;
	        }
	        finally
	        {
		        AnalyticsExtensions.CompleteEvent(id);
		        Debug.Log("Advertisement: show rewarded video");
	        }
        }
        
        private async UniTask<Reward> ShowRewardedAd(RewardedAd rewardedAd) 
        {
	        if (!rewardedAd.IsLoaded())
	        {
		        throw new AdsNotLoaded();
	        }

	        CompositeDisposable disposable = new CompositeDisposable();
	        UniTaskCompletionSource<Reward> completionSource = new UniTaskCompletionSource<Reward>();
	        
	        try
	        {
		        Observable.FromEvent<EventHandler<EventArgs>, (object, EventArgs)>(
				        (action => (obj, args) => action.Invoke((obj, args))),
				        (handler => rewardedAd.OnAdClosed += handler),
				        (handler => rewardedAd.OnAdClosed -= handler)
			        )
			        .First()
			        .Subscribe(data => completionSource.TrySetResult(new Reward()))
			        .AddTo(disposable);
		        
		        Observable.FromEvent<EventHandler<AdErrorEventArgs>, (object, AdErrorEventArgs)>(
				        (action => (obj, args) => action.Invoke((obj, args))),
				        (handler => rewardedAd.OnAdFailedToShow += handler),
				        (handler => rewardedAd.OnAdFailedToShow -= handler)
			        )
			        .First()
			        .Select(data => new AdsException(data.Item1, data.Item2))
			        .Subscribe(data => completionSource.TrySetException(data))
			        .AddTo(disposable);
		        
		        Observable.FromEvent<EventHandler<Reward>, (object, Reward)>(
				        (action => (obj, args) => action.Invoke((obj, args))),
				        (handler => rewardedAd.OnUserEarnedReward += handler),
				        (handler => rewardedAd.OnUserEarnedReward -= handler)
			        )
			        .First()
			        .Subscribe(data => completionSource.TrySetResult(data.Item2))
			        .AddTo(disposable);
		        
		        rewardedAd.Show();

		        return await completionSource.Task;
	        }
	        finally
	        {
		        disposable?.Dispose();
	        }
        }

        private async UniTask ShowInterstitialAd(InterstitialAd interstitialAd)
        {
	        if (!interstitialAd.IsLoaded())
	        {
		        throw new AdsNotLoaded();
	        }

	        CompositeDisposable disposable = new CompositeDisposable();
	        UniTaskCompletionSource<(object, EventArgs)> completionSource = new UniTaskCompletionSource<(object, EventArgs)>();

	        try
	        {
		        Observable.FromEvent<EventHandler<EventArgs>, (object, EventArgs)>(
				        (action => (obj, args) => action.Invoke((obj, args))),
				        (handler => interstitialAd.OnAdClosed += handler),
				        (handler => interstitialAd.OnAdClosed -= handler)
			        )
			        .First()
			        .Subscribe(data => completionSource.TrySetResult(data))
			        .AddTo(disposable);
		        
		        Observable.FromEvent<EventHandler<AdErrorEventArgs>, (object, AdErrorEventArgs)>(
				        (action => (obj, args) => action.Invoke((obj, args))),
				        (handler => interstitialAd.OnAdFailedToShow += handler),
				        (handler => interstitialAd.OnAdFailedToShow -= handler)
			        )
			        .First()
			        .Select(data => new AdsException(data.Item1, data.Item2))
			        .Subscribe(data => completionSource.TrySetException(data))
			        .AddTo(disposable);

		        interstitialAd.Show();
		        
		        await completionSource.Task;
	        }
	        finally
	        {
		        disposable?.Dispose();
	        }
        }
        
        #region Exceptions

	    public abstract class AdsExceptionBase : Exception
	    {
		    public AdsExceptionBase() : base()
		    {
		    }

		    public AdsExceptionBase(string message) : base(message)
		    {
		    }
	    }
	    
	    private class AdsException : AdsExceptionBase
	    {
		    public object Sender { get; }
		    public AdErrorEventArgs Args { get; }

		    public AdsException(object sender, AdErrorEventArgs args)
		    {
			    Sender = sender;
			    Args = args;
		    }

		    public override string ToString()
		    {
			    return $"object: {Sender}: {Args.AdError.GetMessage()}";
		    }
	    }

	    private class AdsNotLoaded : AdsExceptionBase
	    {
		    public AdsNotLoaded() : base("NotLoaded")
		    {
			    
		    }
		    
		    public AdsNotLoaded(AdFailedToLoadEventArgs args) : base(args.LoadAdError.ToString())
		    {
			    
		    }
	    }
	    
	    #endregion

	    #region Loading

	    private interface IAdLoading
	    {
		    UniTask Load(AdRequest request);
	    }

	    private class RewardedAdLoading : IAdLoading
	    {
		    public RewardedAd Ad { get; }

		    public RewardedAdLoading(RewardedAd ad)
		    {
			    Ad = ad;
		    }

		    public async UniTask Load(AdRequest request)
		    {
			    CompositeDisposable disposable = new CompositeDisposable();
			    UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
	        
			    try
			    {
				    Observable.FromEvent<EventHandler<EventArgs>, (object, EventArgs)>(
						    (action => (obj, args) => action.Invoke((obj, args))),
						    (handler => Ad.OnAdLoaded += handler),
						    (handler => Ad.OnAdLoaded -= handler)
					    )
					    .First()
					    .Subscribe(data => completionSource.TrySetResult())
					    .AddTo(disposable);
				    
				    Observable.FromEvent<EventHandler<AdFailedToLoadEventArgs>, (object, AdFailedToLoadEventArgs)>(
						    (action => (obj, args) => action.Invoke((obj, args))),
						    (handler => Ad.OnAdFailedToLoad += handler),
						    (handler => Ad.OnAdFailedToLoad -= handler)
					    )
					    .First()
					    .Subscribe(data => completionSource.TrySetException(new AdsNotLoaded(data.Item2)))
					    .AddTo(disposable);

				    Ad.LoadAd(request);
		        
				    await completionSource.Task;
			    }
			    finally
			    {
				    disposable?.Dispose();
			    }
		    }
	    }

	    private class BannerViewLoading : IAdLoading
	    {
		    public BannerView Ad { get; }

		    public BannerViewLoading(BannerView ad)
		    {
			    Ad = ad;
		    }

		    public async UniTask Load(AdRequest request)
		    {
			    CompositeDisposable disposable = new CompositeDisposable();
			    UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
	        
			    try
			    {
				    Observable.FromEvent<EventHandler<EventArgs>, (object, EventArgs)>(
						    (action => (obj, args) => action.Invoke((obj, args))),
						    (handler => Ad.OnAdLoaded += handler),
						    (handler => Ad.OnAdLoaded -= handler)
					    )
					    .First()
					    .Subscribe(data => completionSource.TrySetResult())
					    .AddTo(disposable);
				    
				    Observable.FromEvent<EventHandler<AdFailedToLoadEventArgs>, (object, AdFailedToLoadEventArgs)>(
						    (action => (obj, args) => action.Invoke((obj, args))),
						    (handler => Ad.OnAdFailedToLoad += handler),
						    (handler => Ad.OnAdFailedToLoad -= handler)
					    )
					    .First()
					    .Subscribe(data => completionSource.TrySetException(new AdsNotLoaded(data.Item2)))
					    .AddTo(disposable);

				    Ad.LoadAd(request);
		        
				    await completionSource.Task;
			    }
			    finally
			    {
				    disposable?.Dispose();
			    }
		    }
	    }
	    
	    private class InterstitialAdLoading : IAdLoading
	    {
		    public InterstitialAd Ad { get; }

		    public InterstitialAdLoading(InterstitialAd ad)
		    {
			    Ad = ad;
		    }

		    public async UniTask Load(AdRequest request)
		    {
			    CompositeDisposable disposable = new CompositeDisposable();
			    UniTaskCompletionSource completionSource = new UniTaskCompletionSource();
	        
			    try
			    {
				    Observable.FromEvent<EventHandler<EventArgs>, (object, EventArgs)>(
						    (action => (obj, args) => action.Invoke((obj, args))),
						    (handler => Ad.OnAdLoaded += handler),
						    (handler => Ad.OnAdLoaded -= handler)
					    )
					    .First()
					    .Subscribe(data => completionSource.TrySetResult())
					    .AddTo(disposable);
				    
				    Observable.FromEvent<EventHandler<AdFailedToLoadEventArgs>, (object, AdFailedToLoadEventArgs)>(
						    (action => (obj, args) => action.Invoke((obj, args))),
						    (handler => Ad.OnAdFailedToLoad += handler),
						    (handler => Ad.OnAdFailedToLoad -= handler)
					    )
					    .First()
					    .Subscribe(data => completionSource.TrySetException(new AdsNotLoaded(data.Item2)))
					    .AddTo(disposable);

				    Ad.LoadAd(request);
		        
				    await completionSource.Task;
			    }
			    finally
			    {
				    disposable?.Dispose();
			    }
		    }
	    }
	    
	    #endregion
    }
}