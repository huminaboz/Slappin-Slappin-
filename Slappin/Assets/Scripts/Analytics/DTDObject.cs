using DevToDev.Analytics;
using UnityEngine;

public class DTDObject : MonoBehaviour
{
    void Awake()
    {
        var config = new DTDAnalyticsConfiguration
        {
            ApplicationVersion = "1.2.3",
            LogLevel = DTDLogLevel.Debug,
            TrackingAvailability = DTDTrackingStatus.Enable,
            CurrentLevel = 1,
            UserId = "unique user id"
        };

#if UNITY_ANDROID
        DTDAnalytics.Initialize("androidAppID", config);
#elif UNITY_IOS
        DTDAnalytics.Initialize("iOSAppID", config);
#elif UNITY_WEBGL
        DTDAnalytics.Initialize("143a92b4-6445-07ec-9e5e-6ae4a5aae1a5", config);
#elif UNITY_STANDALONE_WIN
        DTDAnalytics.Initialize("143a92b4-6445-07ec-9e5e-6ae4a5aae1a5", config);
#elif UNITY_STANDALONE_OSX
        DTDAnalytics.Initialize("OSXAppID", config);
#elif UNITY_WSA
        DTDAnalytics.Initialize("UwpAppID", config);
#endif
    }
}