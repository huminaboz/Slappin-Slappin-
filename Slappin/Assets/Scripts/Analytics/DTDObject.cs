using DevToDev.Analytics;
using UnityEngine;

public class DTDObject : MonoBehaviour
{
    void Awake()
    {
        var config = new DTDAnalyticsConfiguration
        {
            ApplicationVersion = "1.2.3",
            LogLevel = DTDLogLevel.No,
            TrackingAvailability = DTDTrackingStatus.Enable,
            CurrentLevel = 1,
            UserId = "unique_userId"
        };

#if UNITY_ANDROID
        DTDAnalytics.Initialize("androidAppID", config);
#elif UNITY_IOS
        DTDAnalytics.Initialize("iOSAppID", config);
#elif UNITY_WEBGL
        DTDAnalytics.Initialize("WebAppID", config);
#elif UNITY_STANDALONE_WIN
        DTDAnalytics.Initialize("winAppID", config);
#elif UNITY_STANDALONE_OSX
        DTDAnalytics.Initialize("OSXAppID", config);
#elif UNITY_WSA
        DTDAnalytics.Initialize("UwpAppID", config);
#endif
    }
}