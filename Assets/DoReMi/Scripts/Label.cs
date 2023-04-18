using System;
using TMPro;
using UnityEngine;

public class Label : MonoBehaviour
{
    public Camera mainCamera;

    public Transform labelTransform;
    public float defaultLabelScale = 0.1f;
    
    public TMP_Text minus;
    public TMP_Text textValue;

    private int _value;

    private void Update()
    {
        labelTransform.transform.LookAt(mainCamera.transform);

        labelTransform.transform.localScale =
            defaultLabelScale
            * Mathf.Sqrt(Vector3.Distance(labelTransform.transform.position, mainCamera.transform.position))
            * Vector3.one;
    }

    public void SetValue(int newValue)
    {
        _value = newValue;
        minus.gameObject.SetActive(newValue < 0);
        textValue.SetText(Math.Abs(_value).ToString());
    }
}
