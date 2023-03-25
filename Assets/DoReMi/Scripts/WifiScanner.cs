using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Android;

public struct WifiInfo
{
    public string SSID;
    public int level;
}

public class WifiScanner : MonoBehaviour
{
    public TMP_Text WifiNumberText;
    public TMP_Text WifiSSIDText;
    public TMP_Text WifiLevelText;
    public TMP_Text WifiCountText;

    private AndroidJavaObject wifiManager;

    private WifiInfo[] wifiInfoArray;

    private bool canChange = true;

    private int _wifiIdx = 0;
    public int WifiIdx
    {
        get => _wifiIdx;
        set
        {
            _wifiIdx = wifiInfoArray.Length == 0 ? 0 : value % wifiInfoArray.Length;
            WifiNumberText.SetText("WiFi number : " + _wifiIdx);
        }
    }

    private void Start()
    {
        PermissionCallbacks callbacks = new();
        callbacks.PermissionGranted += PermissionGrantedCallback;
        Permission.RequestUserPermission(Permission.FineLocation, callbacks);
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            UpdateWifiInfoList();
            WifiIdx = 0;
            UpdateDisp();
        }

        float YThumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick).y;

        if (YThumbstick < -0.01f && canChange)
        {
            WifiIdx--;
            UpdateDisp();
            canChange = false;
        }
        else if (YThumbstick > 0.01f && canChange)
        {
            WifiIdx++;
            UpdateDisp();
            canChange = false;
        }
        else if (YThumbstick == 0 && !canChange)
        {
            canChange = true;
        }
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

    private void UpdateWifiInfoList()
    {
        AndroidJavaObject scanResults = wifiManager.Call<AndroidJavaObject>("getScanResults");

        int size = scanResults.Call<int>("size");
        wifiInfoArray = new WifiInfo[size];
        for (int i = 0; i < size; i++)
        {
            wifiInfoArray[i] = new WifiInfo();
            AndroidJavaObject javaWifiInfo = scanResults.Call<AndroidJavaObject>("get", i);
            wifiInfoArray[i].SSID = getSDKInt() >= 33 ? javaWifiInfo.Call<string>("getWifiSsid") : javaWifiInfo.Get<string>("SSID");
            wifiInfoArray[i].level = javaWifiInfo.Get<int>("level");
        }
    }

    private void UpdateDisp()
    {
        WifiNumberText.SetText("WiFi number : " + WifiIdx);
        WifiCountText.SetText("WiFi count : " + wifiInfoArray.Length);

        if (wifiInfoArray.Length > 0)
        {
            WifiSSIDText.SetText("WiFi SSID : " + wifiInfoArray[WifiIdx].SSID);
            WifiLevelText.SetText("WiFi level : " + wifiInfoArray[WifiIdx].level + "dBm");
        }
        else
        {
            WifiSSIDText.SetText("WiFi SSID : NO WIFI FOUND");
            WifiLevelText.SetText("WiFi level : NO WIFI FOUND");
        }
    }


    static int getSDKInt()
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
