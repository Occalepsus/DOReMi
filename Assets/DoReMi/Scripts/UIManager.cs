using Assets.DoReMi.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("trackedDevice_")]
    private Transform _trackedDevice;

    /// <summary>
    /// The global GridManager
    /// </summary>
    public GridManager gridManager;

    /// <summary>
    /// The global APModel
    /// </summary>
    public APModel apModel;

    /// <summary>
    /// Menu to be displayed
    /// </summary>
    [SerializeField] private GameObject wifiInfo;
    [SerializeField] private GameObject measureMenu;
    [SerializeField] private GameObject displayMenu;
    [SerializeField] private GameObject configMenu;

    [SerializeField] private GameObject saveButton;
    [SerializeField] private GameObject discardButton;
    [SerializeField] private GameObject matchCheck;

    /// <summary>
    /// The text displaying the measures and expectation
    /// </summary>
    public TMP_Text actualMinus;
    public TMP_Text actualValue;
    public TMP_Text expectedMinus;
    public TMP_Text expectedValue;
    public TMP_Text savedMinus;
    public TMP_Text savedValue;

    /// <summary>
    /// The text displaying the measures and expectation
    /// </summary>
    public TMP_Text wifiNumber;
    public TMP_Text wifiSSID;
    public TMP_Text wifiBSSID;
    private int displayIndex = 0;

    private List<WifiAPInfo> wifiAPInfos = new List<WifiAPInfo> { };


    /// <summary>
    /// App Mode switches between measure and config
    /// </summary>
    public enum AppMode { Measure, Config, Display };

    private AppMode _mode = AppMode.Measure;

    // Start is called before the first frame update
    void Start()
    {
        _mode = AppMode.Measure;
        wifiInfo.SetActive(true);
        measureMenu.SetActive(true);
        displayMenu.SetActive(false);
        configMenu.SetActive(false);
        saveButton.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
        {
            ChangeMode();
        }
        // updates wifi infos if the wifiInfo display is active
        if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x > 0.5 && wifiInfo.activeSelf)
        {
            // TODO: Grid display next network
            displayIndex = (displayIndex + 1) % wifiAPInfos.Count;
        }
        if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < -0.5 && wifiInfo.activeSelf)
        {
            // TODO: Grid display previous network
            displayIndex = (displayIndex - 1) % wifiAPInfos.Count;
        }
        
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            switch(_mode)
            {
                case AppMode.Measure:
                    {
                        SaveMeasure();
                        break;
                    }
                case AppMode.Config:
                    {
                        apModel.PlaceAPModel(_trackedDevice);
                        break;
                    }
                case AppMode.Display:
                    {
                        MatchMeasures();
                        break;
                    }
            }
        }

        // In config mode: change the AP model position, with OVRInput.Get to grab the model when maintaining Button One pressed
        if (OVRInput.Get(OVRInput.Button.One) && _mode == AppMode.Config)
        {
            apModel.PlaceAPModel(_trackedDevice);
        }

        // Updates the value on which the label is
        int val = gridManager.GetValueAt(_trackedDevice.transform.position);
        SetValue(val, actualMinus, actualValue);
        int expected = (int)apModel.GetSignalStrengthAt(_trackedDevice.transform.position);
        SetValue(expected, expectedMinus, expectedValue);
        //TODO: add saved values if existing

        // Updates the wifi info values
        if (wifiAPInfos.Any())
        {
            wifiNumber.SetText(displayIndex.ToString() + "/" + wifiAPInfos.Count);
            wifiSSID.SetText(wifiAPInfos[displayIndex].SSID);
            wifiBSSID.SetText(wifiAPInfos[displayIndex].BSSID);
        }
    }

    /// <summary>
    /// Sets the value to be displayed on the label
    /// </summary>
    /// <param name="newValue">The value to display</param>
    private void SetValue(int newValue, TMP_Text minus, TMP_Text value)
    {
        minus.gameObject.SetActive(newValue < 0);
        value.SetText(Math.Abs(newValue).ToString());
    }

    private void ChangeMode()
    {
        switch (_mode)
        {
            case AppMode.Measure:
                {
                    _mode = AppMode.Display;
                    wifiInfo.SetActive(true);
                    measureMenu.SetActive(false);
                    displayMenu.SetActive(true);
                    configMenu.SetActive(false);
                    return;
                }
            case AppMode.Display:
                {
                    _mode = AppMode.Config;
                    wifiInfo.SetActive(false);
                    measureMenu.SetActive(false);
                    displayMenu.SetActive(false);
                    configMenu.SetActive(true);
                    return;
                }
            case AppMode.Config:
                {
                    _mode = AppMode.Measure;
                    wifiInfo.SetActive(true);
                    measureMenu.SetActive(true);
                    displayMenu.SetActive(false);
                    configMenu.SetActive(false);
                    return;
                }
        }
    }

    private void SaveMeasure()
    {
        if (saveButton.activeSelf)
        {
            //TODO: create save
            saveButton.SetActive(false);
        }
        else
        {
            //TODO: discard save
            saveButton.SetActive(true);
        }
    }

    private void MatchMeasures()
    {
        if (matchCheck.activeSelf)
        {
            //TODO: unmatch measures
            matchCheck.SetActive(false);
        }
        else
        {
            //TODO: match measures
            matchCheck.SetActive(true);
        }
    }

    public void AddWifiInfo(WifiAPInfo newAPInfo)
    {
        wifiAPInfos.Add(newAPInfo);
    }
}
