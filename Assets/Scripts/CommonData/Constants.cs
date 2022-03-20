using System;
using System.Collections.Generic;

namespace CommonData
{
    public static class Constants
    {
        #if AdsTest
        
        public static readonly List<string> DeviceIds = new List<string>()
        {
            "33023DB84D98175E"
        };

        #endif
        
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
        public const string RateUsLink = "https://stubbornball.page.link/RateUsAndroid";
#elif UNITY_IOS        
        public const string RateUsLink = "https://stubbornball.page.link/RateUsIos";
#else
        public const string RateUsLink = "";
#endif
        
        #region SoundMixers

        public const string MainMixerVolume = "MasterVolume";

        public const int MixerMax = 20;
        public const int MixerMin = -80;
        

        #endregion
    }
}