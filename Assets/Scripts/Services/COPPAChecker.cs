using System;
using Common;
using CommonData;
using Cysharp.Threading.Tasks;
using Extensions;
using GoogleMobileAds.Api;
using UnityEngine;
using Zenject;

namespace Services
{
    public class COPPAChecker : IInitializable, IDisposable
    {
        public UniTask IsCompleted
            => _completionSource.Task;

        [Inject] private readonly AdService _adService;
        private AbstractSaver<UserData> _userData;
        private UniTaskCompletionSource _completionSource = new UniTaskCompletionSource();

        public void Initialize()
        {
            _userData = new AbstractSaver<UserData>("user.dat");
            _userData.Load().Wait();

            CheckCOPPACompliance().Forget();
        }

        private async UniTask CheckCOPPACompliance()
        {
            try
            {
                if (!_userData.Data.Birthday.HasValue)
                {
                    try
                    {
#if UNITY_EDITOR
                        var birthday = DateTime.Now;
                        birthday -= TimeSpan.FromDays(365 * 10);
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
                    catch (OperationCanceledException cancel)
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

                    //Set variable to admob
#if DEBUG
                    Debug.Log($"set tag {childTag}");
#endif
                    try
                    {
                        if (childTag == TagForChildDirectedTreatment.True)
                        {
                            await _adService.SetTagForChildDirectedTreatment(childTag);
                        }
                    }
                    catch (NotImplementedException e)
                    {
                        Debug.LogWarning($"{nameof(COPPAChecker)} usage of unsupported platform detected\n" +
                                         $"{e.Message}");
                    }
                }
                
                _completionSource.TrySetResult();
            }
            catch (Exception e)
            {
                _completionSource.TrySetException(e);
            }
        }

        public void Dispose()
        {
            _userData.Dispose();
        }
    }
}