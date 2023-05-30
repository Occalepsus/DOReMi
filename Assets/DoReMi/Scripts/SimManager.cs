using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Assets.DoReMi.Scripts
{
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
        public TMP_Text txtIdx;
        public TMP_Text SSID;
        public TMP_Text BSSID;
        public TMP_Text txtCnt;

        /// <summary>
        /// The Dict of all detected access points in space
        /// </summary>
        /// <remarks>maximum MAX_AP elements for optimization</remarks>
        private readonly Dictionary<int, WifiAPInfo> _APTable = new(MAX_AP); // Maybe change WifiInfo to just String for BSSID, the rest is useless here

        private void Update()
        {
            if (GridManager.CanScanAtPos(HeadTransform.position, out _) && GridManager.GetDistanceFromNearestTile(HeadTransform.position, out _) < scanRange)
            {
                try
                {
                    GridManager.ScanAtPos(HeadTransform.position, ScanWifi());
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
                    GridManager.ScanAtPos(HeadTransform.position, info);
                    GridManager.SetSelectedAP("00:00".GetHashCode());
                }
            }
            // Temp:
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick)[0] > 0.9)
            {
                WifiAPInfo[] wifiList = new WifiAPInfo[_APTable.Count];
                if (idx < wifiList.Length - 1)
                {
                    _APTable.Values.CopyTo(wifiList, 0);
                    idx++;
                    if (idx == wifiList.Length - 1)
                    {
                        // TODO : unactivate right arrow
                    }
                    if (idx > 0)
                    {
                        // TODO : activate left arrow
                    }
                    idx %= wifiList.Length;
                    txtIdx.SetText("nb: " + idx);
                    SSID.SetText(wifiList[idx].SSID);
                    BSSID.SetText(wifiList[idx].BSSID);
                    txtCnt.SetText("total: " + wifiList.Length);
                    GridManager.SetSelectedAP(wifiList[idx].BSSID.GetHashCode());
                }
            }
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick)[0] > 0.9)
            {
                WifiAPInfo[] wifiList = new WifiAPInfo[_APTable.Count];
                if (idx > 0)
                {
                    _APTable.Values.CopyTo(wifiList, 0);
                    idx--;
                    if (idx < wifiList.Length - 1)
                    {
                        // TODO : activate right arrow
                    }
                    if (idx == 0)
                    {
                        // TODO : unactivate left arrow
                    }
                    idx %= wifiList.Length;
                    txtIdx.SetText("nb: " + idx);
                    SSID.SetText(wifiList[idx].SSID);
                    BSSID.SetText(wifiList[idx].BSSID);
                    txtCnt.SetText("total: " + wifiList.Length);
                    GridManager.SetSelectedAP(wifiList[idx].BSSID.GetHashCode());
                }
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
}