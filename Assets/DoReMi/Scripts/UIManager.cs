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
    /// The global RestoreManager
    /// </summary>
    public SaveRestoreManager restoreManager;

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
    private bool xJoystickReady = true; // true if the joystick is ready to be used on axis x
    private bool yJoystickReady = true; // true if the joystick is ready to be used on axis y

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
        xJoystickReady = true;
        yJoystickReady = true;
    }

    // Update is called once per frame
    void Update()
    {
        // change mode
        if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x > 0.85 && xJoystickReady)
        {
            NextMode();
            xJoystickReady = false;
        }
        if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x < -0.85 && xJoystickReady)
        {
            PreviousMode();
            xJoystickReady = false;
        }
        // updates wifi infos if the wifiInfo display is active
        if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y > 0.85 && wifiInfo.activeSelf && yJoystickReady && wifiAPInfos.Any())
        {
            displayIndex = (displayIndex + 1 + wifiAPInfos.Count) % wifiAPInfos.Count;
            gridManager.SetSelectedAP(wifiAPInfos[displayIndex].BSSID.GetHashCode());
            yJoystickReady = false;
            
        }
        if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y < -0.85 && wifiInfo.activeSelf && yJoystickReady && wifiAPInfos.Any())
        {
            displayIndex = (displayIndex - 1 + wifiAPInfos.Count) % wifiAPInfos.Count;
            gridManager.SetSelectedAP(wifiAPInfos[displayIndex].BSSID.GetHashCode());
            yJoystickReady = false;
        }
        // updated joystick state
        if (Math.Abs(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).x) < 0.4)
        {
            xJoystickReady = true;
        }
        if (Math.Abs(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y) < 0.4)
        {
            yJoystickReady = true;
        }

        // executes operation of button A depending on the mode
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

        // Reset grid when pressing B on modes Measure and Config
        if (OVRInput.GetDown(OVRInput.RawButton.B) && _mode != AppMode.Display)
        {
            restoreManager.ResetGrid();
        }

        // Updates the value on which the label is
        gridManager.GetValuesAt(_trackedDevice.transform.position, out int actualVal, out float expectedVal, out int savedVal);
        SetValue(actualVal, actualMinus, actualValue);
        SetValue((int)expectedVal, expectedMinus, expectedValue);
        SetValue(savedVal, savedMinus, savedValue);

        // Updates the wifi info values
        if (wifiAPInfos.Any())
        {
            wifiNumber.SetText((displayIndex + 1).ToString() + "/" + wifiAPInfos.Count);
            wifiSSID.SetText(wifiAPInfos[displayIndex].SSID);
            wifiBSSID.SetText(wifiAPInfos[displayIndex].BSSID);
        }        
    }

    /// <summary>
    /// Sets the value to be displayed on the label
    /// </summary>
    /// <param name="newValue">The value to display</param>
    private void SetValue(int val, TMP_Text minus, TMP_Text value)
    {
        if (val > int.MinValue)
        {
            minus.gameObject.SetActive(val < 0);
            value.SetText(Math.Abs(val).ToString());
        }
        else
        {
            minus.gameObject.SetActive(false);
            value.SetText("N/A");
        }
    }

    private void NextMode()
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

    private void PreviousMode()
    {
        switch (_mode)
        {
            case AppMode.Config:
                {
                    _mode = AppMode.Display;
                    wifiInfo.SetActive(true);
                    measureMenu.SetActive(false);
                    displayMenu.SetActive(true);
                    configMenu.SetActive(false);
                    return;
                }
            case AppMode.Measure:
                {
                    _mode = AppMode.Config;
                    wifiInfo.SetActive(false);
                    measureMenu.SetActive(false);
                    displayMenu.SetActive(false);
                    configMenu.SetActive(true);
                    return;
                }
            case AppMode.Display:
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
            restoreManager.SaveGrid();
            saveButton.SetActive(false);
        }
        else
        {
            restoreManager.DiscardSavedGrid();
            saveButton.SetActive(true);
        }
    }

    private void MatchMeasures()
    {
        if (matchCheck.activeSelf)
        {
            apModel.MatchWithUserValues(0, 0, 0, 2.4e9f);
            matchCheck.SetActive(false);
        }
        else
        {
            gridManager.GetValuesAt(_trackedDevice.transform.position, out int val, out _, out _);
            // If no value at this position, keep the check inactive
            if (val == int.MinValue)
            {
                Debug.LogWarning("No value at this position, keeping unchecked.");
                matchCheck.SetActive(false);
                return;
            }

            apModel.MatchWithLevel(val);
            matchCheck.SetActive(true);
        }
    }

    public void AddWifiInfo(WifiAPInfo newAPInfo)
    {
        Debug.Log("adding a network");
        wifiAPInfos.Add(newAPInfo);
    }
}
