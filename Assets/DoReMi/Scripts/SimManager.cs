using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimManager : MonoBehaviour
{
    public WifiScanner WifiScanner;

    // TODO Louise : à compléter ;)
    // public <UIManager> uiManager;

    private Dictionary<int, WifiInfo> _wifiTable = new(20);

    private void Update()
    {
        
    }

    private void ScanWifi()
    {
        WifiInfo[] wifiInfos = WifiScanner.GetWifiInfo();

        foreach (WifiInfo wi in wifiInfos)
        {
            if (!_wifiTable.ContainsKey(wi.BSSID.GetHashCode()))
            {
                _wifiTable.Add(wi.BSSID.GetHashCode(), wi);
                // TODO : ajouter dans l'UI le nouveau réseau trouvé
            }
        }
    }
}