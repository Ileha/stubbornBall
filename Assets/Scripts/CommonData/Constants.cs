using System;

namespace CommonData
{
    public static class Constants
    {
        #region Analytics

        public const string MainUIPageOpened = "MainUIPageOpened";
        public const string VolumeValueChanged = "VolumeValueChanged";
        public const string LevelPassed = "LevelPassed";
        public const string LevelRestarted = "LevelRestarted";
        public const string LevelSkipped = "LevelSkipped";
        public const string RewardedVideo = "RewardedVideoStarted";
        public const string RegularVideo = "RegularVideoStarted";
        public const string IIinputEvent = "IIinputEvent";
        public const string LastLevelPassed = "LastLevelPassed";
        public const string ShakeDetection = "ShakeDetection";
        
        #endregion

#if UNITY_ANDROID
        public const string RateUsLink = "http://p1dz.2.vu/F195FA670CA7B0767B3F43F5B";
#elif UNITY_IOS        
        public const string RateUsLink = "http://p1dz.2.vu/727677E305DF9F855BEA4556F";
#endif
        


    }
}