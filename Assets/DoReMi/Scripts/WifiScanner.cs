using TMPro;
using UnityEngine;
using UnityEngine.Android;

public struct WifiInfo
{
    public string SSID;
    public string BSSID;
    public int level;
}

public class WifiScanner : MonoBehaviour
{
    public TMP_Text WifiNumberText;
    public TMP_Text WifiSSIDText;
    public TMP_Text WifiLevelText;
    public TMP_Text WifiMACText;
    public TMP_Text WifiCountText;

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

    private void Start()
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

    internal void PermissionGrantedCallback(string permissionName)
    {
        if (permissionName.EndsWith(Permission.FineLocation))
        {
            AndroidJavaClass unityPlayer = new("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            wifiManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "wifi");
        }
    }

    public WifiInfo[] GetWifiInfo()
    {
        AndroidJavaObject scanResults = wifiManager.Call<AndroidJavaObject>("getScanResults");

        int size = scanResults.Call<int>("size");
        WifiInfo[] wifiInfoArray = new WifiInfo[size];
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

    private static int GetSDKInt()
    {
        using var version = new AndroidJavaClass("android.os.Build$VERSION");
        return version.GetStatic<int>("SDK_INT");
    }


    // ======= Code à essayer, peut être plus optimal :
    
    //AndroidJavaObject scanResults = wifiManager.Call<AndroidJavaObject>("getScanResults").Call<AndroidJavaObject>("toArray");
    //Debug.LogWarning("=========================== getScanResults in an array instead of a list done");
    //AndroidJavaObject[] list = AndroidJNIHelper.ConvertFromJNIArray<AndroidJavaObject[]>(scanResults.GetRawObject());
    //Debug.LogWarning("=========================== convert to array done");
    //WifiInfo[] wiList = new WifiInfo[list.Length];
    //for (int i = 0; i < list.Length; i++)
    //{
    //    wiList[i] = new WifiInfo();
    //    wiList[i].SSID = list[i].Call<string>("getWifiSsid");
    //    wiList[i].level = list[i].Get<int>("level");
    //}
    //Debug.LogWarning("=========================== convert to WifiInfo array done");

    //WifiLevelText.SetText("WiFi level : " + wiList[0] + "dBm");
}
