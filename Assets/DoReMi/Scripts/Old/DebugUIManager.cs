using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugUIManager : MonoBehaviour
{
    public TMP_Text mainDebug;
    public TMP_Text secondDebug;

    public Toggle scanContinuously;

    public void SetMainDebug(string text)
    {
        mainDebug.text = text;
    }

    public void SetSecondDebug(string text)
    {
        secondDebug.text = text;
    }

    public bool isOn
    {
        get => scanContinuously.isOn;
        set => scanContinuously.isOn = value;
    }
}
