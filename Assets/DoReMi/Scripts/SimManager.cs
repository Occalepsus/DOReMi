using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.DoReMi.Scripts
{
    public class SimManager : MonoBehaviour
    {
        /// <summary>
        /// Defines the number of wifi access points scannable during a session
        /// </summary>
        public const int MAX_AP = 30;

        /// <summary>
        /// The WifiScanner MonoBehaviour
        /// </summary>
        public WifiScanner WifiScanner;
        /// <summary>
        /// The GridManager MonoBehaviour
        /// </summary>
        public GridManager GridManager;
        /// <summary>
        /// The UIManager MonoBehaviour
        /// </summary>
        public UIManager uiManager;

        /// <summary>
        /// The transform of the position of the head
        /// </summary>
        public Transform HeadTransform;

        /// <summary>
        /// The range within the scan is performed
        /// </summary>
        public float scanRange;

        /// <summary>
        /// The Dict of all detected access points in space
        /// </summary>
        /// <remarks>maximum MAX_AP elements for optimization</remarks>
        private readonly HashSet<string> _APTable = new(MAX_AP);

        private void Update()
        {
            if (GridManager.CanScanAtPos(HeadTransform.position, out _) && GridManager.GetDistanceFromNearestTile(HeadTransform.position, out _) < scanRange)
            {
                try
                {
                    // Can fail if WifiScanner has not been initialized yet
                    GridManager.ScanAtPos(HeadTransform.position, ScanWifi());
                }
                catch (NullReferenceException)
                {
                    Debug.LogWarning("Warning: Scan failed, can retry");
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
                if (!_APTable.Contains(ap.BSSID))
                {
                    _APTable.Add(ap.BSSID);
                    uiManager.AddWifiInfo(ap);

                    if (_APTable.Count >= MAX_AP)
                        Debug.LogWarning($"Warning: APTable has {_APTable.Count} elements which is more MAX_AP = {MAX_AP}, possible performance loss");
                }
            }
            return wifiAPInfos;
        }
    }
}