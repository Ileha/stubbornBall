using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Analytics;

namespace Extensions
{
    public static class AnalyticsExtensions
    {
        private enum AnalyticsEventState 
        {
            Begin = 0,
            End = 1
        }
        
        private class EventData : IDisposable
        {
            public readonly string Name;
            public readonly Stopwatch Stopwatch;

            public EventData(string name)
            {
                Name = name;
                Stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                Stopwatch.Stop();
            }
        }
        
        private static Dictionary<Guid, EventData> _eventData = new Dictionary<Guid, EventData>();
        
        public static Guid BeginEvent(string eventName)
        {
            Analytics.CustomEvent(
                eventName,
                new Dictionary<string, object>()
                {
                    { "stage", AnalyticsEventState.Begin }
                }
            );

            var guid = Guid.NewGuid();
            _eventData.Add(guid, new EventData(eventName));
            return guid;
        }

        public static void CompleteEvent(Guid guid)
        {
            if (_eventData.TryGetValue(guid, out var data))
            {
                Analytics.CustomEvent(
                    data.Name,
                    new Dictionary<string, object>()
                    {
                        { "stage", AnalyticsEventState.End },
                        { "elapsedTime", data.Stopwatch.ElapsedMilliseconds }
                    }
                );
                _eventData.Remove(guid);
            }
            else
            {
                UnityEngine.Debug.LogWarning($"Analytics: guid {guid:N} not found");
            }
        }
    }
}