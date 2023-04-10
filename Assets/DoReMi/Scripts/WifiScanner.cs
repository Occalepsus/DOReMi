using TMPro;
using UnityEngine;
using UnityEngine.Android;

public struct WifiAPInfo
{
    public string SSID;
    public string BSSID;
    public int level;
}

public class WifiScanner : MonoBehaviour
{
/*    public TMP_Text WifiNumberText;
    public TMP_Text WifiSSIDText;
    public TMP_Text WifiLevelText;
    public TMP_Text WifiMACText;
    public TMP_Text WifiCountText;*/

    /// <summary>
    /// The Android Java Native Interface object to call to get AP data
    /// </summary>
    private AndroidJavaObject wifiManager;

    //private WifiInfo[] wifiInfoArray;

    //private bool canChange = true;

    //private int _wifiIdx = 0;
    //public int WifiIdx
    //{
    //    get => _wifiIdx;
    //    set
    //    {
    //        _wifiIdx = wifiInfoArray.Length == 0 ? 0 : value % wifiInfoArray.Length;
    //        WifiNumberText.SetText("WiFi number : " + _wifiIdx);
    //    }
    //}

    private void Awake()
    {
        PermissionCallbacks callbacks = new();
        callbacks.PermissionGranted += PermissionGrantedCallback;
        Permission.RequestUserPermission(Permission.FineLocation, callbacks);
    }

    private void Update()
    {
        //if (OVRInput.GetDown(OVRInput.Button.One))
        //{
        //    UpdateWifiInfoList();
        //    WifiIdx = 0;
        //    UpdateDisp();
        //}

        //float YThumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;

        //if (YThumbstick < -0.01f && canChange)
        //{
        //    WifiIdx--;
        //    UpdateDisp();
        //    canChange = false;
        //}
        //else if (YThumbstick > 0.01f && canChange)
        //{
        //    WifiIdx++;
        //    UpdateDisp();
        //    canChange = false;
        //}
        //else if (YThumbstick == 0 && !canChange)
        //{
        //    canChange = true;
        //}
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

    //private void UpdateDisp()
    //{
    //    WifiNumberText.SetText("WiFi number : " + WifiIdx);
    //    WifiCountText.SetText("WiFi count : " + wifiInfoArray.Length);

    //    if (wifiInfoArray.Length > 0)
    //    {
    //        WifiSSIDText.SetText("WiFi SSID : " + wifiInfoArray[WifiIdx].SSID);
    //        WifiLevelText.SetText("WiFi level : " + wifiInfoArray[WifiIdx].level + "dBm");
    //        WifiMACText.SetText("WiFi MAC : " + wifiInfoArray[WifiIdx].MAC);
    //    }
    //    else
    //    {
    //        WifiSSIDText.SetText("WiFi SSID : NO WIFI FOUND");
    //        WifiLevelText.SetText("WiFi level : NO WIFI FOUND");
    //        WifiMACText.SetText("WiFi MAC : NO WIFI FOUND");
    //    }
    //}

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
