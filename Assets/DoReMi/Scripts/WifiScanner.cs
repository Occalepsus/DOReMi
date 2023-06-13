using TMPro;
using UnityEngine;
using UnityEngine.Android;


namespace Assets.DoReMi.Scripts
{
    public struct WifiAPInfo
    {
        public string SSID;
        public string BSSID;
        public int level;
    }

    public class WifiScanner : MonoBehaviour
    {
        /// <summary>
        /// The Android Java Native Interface object to call to get AP data
        /// </summary>
        private AndroidJavaObject wifiManager;

        private void Awake()
        {
            PermissionCallbacks callbacks = new();
            callbacks.PermissionGranted += PermissionGrantedCallback;
            Permission.RequestUserPermission(Permission.FineLocation, callbacks);
        }

        /// <summary>
        /// Called once the permission has been granted by the user, gets Android wifiManager and sets it to class field
        /// </summary>
        /// <param name="permissionName">The name of the granted permission</param>
        internal void PermissionGrantedCallback(string permissionName)
        {
            // Only if the permission is FineLocation (necessary to get wifiManager)
            if (permissionName.EndsWith(Permission.FineLocation))
            {
                // Calling Java objects from Android JNI
                AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                wifiManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "wifi");
            }
        }

        /// <summary>
        /// Gets the information of every detected wifi access points.
        /// </summary>
        /// <returns>The array of every detected wifi AP</returns>
        public WifiAPInfo[] GetWifiAPInfo()
        {
            // Performs the scan
            AndroidJavaObject scanResults = wifiManager.Call<AndroidJavaObject>("getScanResults");

            // Creating WifiAPInfo[] from Java array object
            int size = scanResults.Call<int>("size");
            WifiAPInfo[] wifiInfoArray = new WifiAPInfo[size];
            for (int i = 0; i < size; i++)
            {
                // Get i-th scan result
                AndroidJavaObject scanResult = scanResults.Call<AndroidJavaObject>("get", i);

                // Initializing and setting WifiInfo
                wifiInfoArray[i] = new()
                {
                    SSID = GetSDKInt() >= 33 ? scanResult.Call<string>("getWifiSsid") : scanResult.Get<string>("SSID"),
                    BSSID = scanResult.Get<string>("BSSID"),
                    level = scanResult.Get<int>("level")
                };
            }
            return wifiInfoArray;
        }

        /// <summary>
        /// Gets the version of the Android SDK
        /// </summary>
        /// <returns>The version of the Android SDK</returns>
        private static int GetSDKInt()
        {
            using var version = new AndroidJavaClass("android.os.Build$VERSION");
            return version.GetStatic<int>("SDK_INT");
        }
    }
}
