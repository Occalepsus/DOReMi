using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField, FormerlySerializedAs("saveButton_")]
    private GameObject _saveButton;

    [SerializeField, FormerlySerializedAs("measureModeButton_")]
    private GameObject _measureModeButton;

    [SerializeField, FormerlySerializedAs("displayModeButton_")]
    private GameObject _displayModeButton;

    [SerializeField, FormerlySerializedAs("configModeButton_")]
    private GameObject _configModeButton;

    [SerializeField, FormerlySerializedAs("trackedDevice_")]
    private Transform _trackedDevice;

    /// <summary>
    /// App Mode switches between measure and config
    /// </summary>
    public enum AppMode { Measure, Config, Display };

    private AppMode _mode = AppMode.Measure;


    [SerializeField, FormerlySerializedAs("buttonList_")]
    private List<Button> _buttonList;

    private int _menuIndex = 0;

    private Button _selectedButton;

    // Start is called before the first frame update
    void Start()
    {
        _selectedButton = _buttonList[0];
        _buttonList[0].OnSelect(null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
