using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Advertisements;
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
	        addCount = (addCount + 1) % (AdData.MaxAdFrequency-_adData.AdFrequency);
        	Debug.Log(addCount);
        	return addCount == ((AdData.MaxAdFrequency - _adData.AdFrequency)-1);
        }
    
        public async UniTask ShowBanner(BannerPosition position) 
        {
        	Advertisement.Banner.SetPosition(position);
        	await ShowBannerWhenReady(_adData.BannerId);
        	Debug.Log("show banner");
        }
    
        private async UniTask ShowBannerWhenReady(string placementId)
        {
        	await UniTask.WaitWhile(() => !Advertisement.IsReady(placementId));
        	Advertisement.Banner.Show(placementId);
        }
    
        public async UniTask ShowVideo() 
        {
        	await ShowAdWhenReady(_adData.Video);
        	Debug.Log("show video");
        }
    
        public async UniTask<ShowResult> ShowRewardedVideo() 
        {
        	var result = await ShowAdWhenReady(_adData.RewardedVideo);
        	Debug.Log("show rewarded video");
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