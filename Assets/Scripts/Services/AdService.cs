using System.Collections.Generic;
using CommonData;
using Cysharp.Threading.Tasks;
using Extensions;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;
using Zenject;

namespace Services
{
    public class AdService : IInitializable
    {
        private int addCount = 0;

        [Inject] private readonly AdData _adData;
        
        public void Initialize()
        {
	        Advertisement.debugMode = true;
	        Advertisement.Initialize(_adData.GameId, _adData.TestMode);
        }
    
        public bool IsInitialized() 
        {
        	return Advertisement.isInitialized;
        }
    
        public bool HasNextAd()
        {
	        var localMax = (AdData.MaxAdFrequency - _adData.AdFrequency)+1;
	        addCount = (addCount + 1) % localMax;
	        Debug.Log($"Advertisement: {addCount}/{localMax}");
	        return addCount == 0;
        }
    
        public async UniTask ShowBanner(BannerPosition position) 
        {
        	Advertisement.Banner.SetPosition(position);
        	await ShowBannerWhenReady(_adData.BannerId);
        	Debug.Log("Advertisement: show banner");
        }
    
        private async UniTask ShowBannerWhenReady(string placementId)
        {
        	await UniTask.WaitWhile(() => !Advertisement.IsReady(placementId));
        	Advertisement.Banner.Show(placementId);
        }
    
        public async UniTask ShowVideo() 
        {
	        var id = AnalyticsExtensions.BeginEvent(Constants.RegularVideo);
	        await ShowAdWhenReady(_adData.Video);
	        AnalyticsExtensions.CompleteEvent(id);
        	Debug.Log("Advertisement: show video");
        }
    
        public async UniTask<ShowResult> ShowRewardedVideo()
        {
	        var id = AnalyticsExtensions.BeginEvent(Constants.RewardedVideo);
        	var result = await ShowAdWhenReady(_adData.RewardedVideo);
            AnalyticsExtensions.CompleteEvent(id);
        	Debug.Log("Advertisement: show rewarded video");
        	return result;
        }
    
        private async UniTask<ShowResult> ShowAdWhenReady(string placementId) 
        {
        	await UniTask.WaitWhile(() => !Advertisement.IsReady(placementId));
        
        	UniTaskCompletionSource<ShowResult> completionSource = new UniTaskCompletionSource<ShowResult>();
    
        	Advertisement.Show(placementId, new ShowOptions() 
        	{
        		resultCallback = result => completionSource.TrySetResult(result)
        	});
    
        	return await completionSource.Task;
        }
    }
}