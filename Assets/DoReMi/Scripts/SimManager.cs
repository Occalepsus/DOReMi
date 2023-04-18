using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEditor;
using UnityEngine;

public class SimManager : MonoBehaviour
{
    /// <summary>
    /// Defines the number of wifi access points scannable during a session
    /// </summary>
    public const int MAX_AP = 20;

    /// <summary>
    /// The WifiScanner MonoBehaviour
    /// </summary>
    public WifiScanner WifiScanner;
    /// <summary>
    /// The GridManager MonoBehaviour
    /// </summary>
    public GridManager GridManager;

    /// <summary>
    /// The transform of the position of the head
    /// </summary>
    public Transform HeadTransform;

    /// <summary>
    /// The range within the scan is performed
    /// </summary>
    public float scanRange;

    // TODO Louise : à compléter ;)
    // public <UIManager> uiManager;
    // Temp :
    public int idx;
    public TMP_Text SSID;

    /// <summary>
    /// The Dict of all detected access points in space
    /// </summary>
    /// <remarks>maximum MAX_AP elements for optimization</remarks>
    private Dictionary<int, WifiAPInfo> _APTable = new(MAX_AP); // Maybe change WifiInfo to just String for BSSID, the rest is useless here

    private void Update()
    {
        Vector2Int nearestCoordinate = GridManager.GetNearestCoordinate(HeadTransform.position);
        if (GridManager.CanScanAtPos(nearestCoordinate))
        {
            Vector3 nearestWorldPos = new Vector3(nearestCoordinate.x, 0, nearestCoordinate.y) * GridManager.tileSize + GridManager.gridOrigin;
            if (Vector3.Distance(nearestWorldPos, Vector3.ProjectOnPlane(HeadTransform.position, Vector3.up)) < scanRange)
            {
                try
                {
                    GridManager.ScanAtPos(nearestCoordinate, ScanWifi());
                }
                catch (NullReferenceException)
                {
                    Debug.LogWarning("Warning: Scan failed, putting test fake scan instead");
                    WifiAPInfo[] info = new WifiAPInfo[1];
                    info[0] = new WifiAPInfo()
                    {
                        SSID = "TEST",
                        BSSID = "00:00",
                        level = -30
                    };
                    GridManager.ScanAtPos(nearestCoordinate, info);
                    GridManager.SetSelectedAP("00:00".GetHashCode());
                }
            }
        }
        // Temp:
        if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
        {
            WifiAPInfo[] val = new WifiAPInfo[_APTable.Count];
            _APTable.Values.CopyTo(val, 0);
            idx++;
            idx %= val.Length;
            SSID.SetText(val[idx].SSID);
            GridManager.SetSelectedAP(val[idx].BSSID.GetHashCode());
        }
    }
    
    /// <summary>
    /// Performs a Wifi scan at the headset position
    /// </summary>
    /// <returns>The WifiAPInfo array at the scanned point</returns>
    private WifiAPInfo[] ScanWifi()
    {
        // Get WifiInfos at this position with WifiScanner
        WifiAPInfo[] wifiAPInfos = WifiScanner.GetWifiAPInfo();

        // Loop through every access points to read data
        foreach (WifiAPInfo ap in wifiAPInfos)
        {
            // If this access point was never discovered before, add it to our access point table
            if (!_APTable.ContainsKey(ap.BSSID.GetHashCode()))
            {
                _APTable.Add(ap.BSSID.GetHashCode(), ap);

                if (_APTable.Count >= MAX_AP)
                    Debug.LogWarning($"Warning: APTable has {_APTable.Count} elements which is more MAX_AP = {MAX_AP}, possible performance loss");

                // TODO : ajouter dans l'UI le nouveau réseau trouvé
            }
        }
        return wifiAPInfos;
    }
}