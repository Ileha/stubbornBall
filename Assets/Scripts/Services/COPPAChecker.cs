﻿using System;
using System.Threading;
using CommonData;
using Cysharp.Threading.Tasks;
using Extensions;
using GoogleMobileAds.Api;
using pingak9;
using UnityEngine;
using Zenject;

namespace Services
{
    public class COPPAChecker : IInitializable, IDisposable
    {
        private readonly AdService _adService;
        private AbstractSaver<UserData> _userData;

        public COPPAChecker(AdService adService)
        {
            _adService = adService;
        }

        public void Initialize()
        {
            _userData = new AbstractSaver<UserData>("user.dat");
            _userData.Load().Wait();
            
            CheckCOPPACompliance().Forget();
        }

        private async UniTask CheckCOPPACompliance()
        {
            if (!_userData.Data.Birthday.HasValue)
            {
                try
                {
#if UNITY_EDITOR
                    var birthday = DateTime.Now;
                    birthday -= TimeSpan.FromDays(365*14);
#elif UNITY_IOS || UNITY_ANDROID
                    var birthday = await NativeDialog
                        .OpenDatePickerAsync(
                            "Please enter your birth date",
                            1992,5,10, false
                        );
#endif
                    
                    
                    _userData.Data.Birthday = birthday;
                    await _userData.Save();
#if DEBUG
                    Debug.Log($"user birthday {birthday}");           
#endif
                }
                catch(OperationCanceledException cancel)
                {
#if DEBUG
                    Debug.Log($"users cancel");           
#endif
                }
            }

            if (_userData.Data.Birthday.HasValue)
            {
                DateTimeHelpers.GetElapsedTime(_userData.Data.Birthday.Value, DateTime.Now,
                    out var years, out var months, 
                    out var days, out var hours, 
                    out var minutes, out var seconds, 
                    out var milliseconds);

                var childTag = years < Constants.COPPAComplianceAge
                    ? TagForChildDirectedTreatment.True
                    : TagForChildDirectedTreatment.Unspecified;
                
#if DEBUG
                Debug.Log($"user full years: {years} => {childTag}");           
#endif

                if (childTag != _userData.Data.CurrentTagForChildDirected)
                {
                    //Set variable to admob
#if DEBUG
                    Debug.Log($"set tag {childTag}");           
#endif
                    await _adService.SetTagForChildDirectedTreatment(childTag);
                    _userData.Data.CurrentTagForChildDirected = childTag;
                    await _userData.Save();
                }
            }
        }

        public void Dispose()
        {
            _userData.Dispose();
        }
    }
}