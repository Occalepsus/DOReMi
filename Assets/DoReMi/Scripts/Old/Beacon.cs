using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    public Camera mainCamera;

    public Transform cylTransform;

    public Transform labelTransform;
    public float defaultLabelScale = 0.1f;
    public TMP_Text minus;
    public TMP_Text textValue;

    public AnimationCurve curve;

    public int minValue;
    public int maxValue;

    public float maxSize;

    public float animationTimeInSeconds;

    private int _value;

    private void OnEnable()
    {
        SetValue(10);
    }

    private void Update()
    {
        labelTransform.transform.LookAt(mainCamera.transform);

        labelTransform.transform.localScale =
            defaultLabelScale
            * Mathf.Sqrt(Vector3.Distance(labelTransform.transform.position, mainCamera.transform.position))
            * Vector3.one;

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            SetValue(10);
        }
    }

    public void SetValue(int newValue)
    {
        this._value = newValue;
        minus.gameObject.SetActive(newValue < 0);
        
        StartCoroutine(SetGauge());
    }

    private IEnumerator SetGauge()
    {
        float finalHeight = (_value - minValue) / (float)(maxValue - minValue) * maxSize;

        float t = 0;
        Vector3 pos = cylTransform.localPosition;
        Vector3 scale = cylTransform.localScale;

        do
        {
            t += Time.deltaTime / animationTimeInSeconds;

            textValue.SetText(((int)(t * (_value - minValue) + minValue)).ToString());

            float d = finalHeight * curve.Evaluate(t);

            pos.y = d;
            scale.y = d;

            cylTransform.localPosition = pos;
            cylTransform.localScale = scale;

            pos.y *= 2;
            labelTransform.transform.position = pos;

            yield return null;
        } while (t < 1);

        pos.y = finalHeight;
        scale.y = finalHeight;

        textValue.SetText(_value.ToString());

        yield return null;
    }
}
