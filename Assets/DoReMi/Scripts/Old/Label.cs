using System;
using TMPro;
using UnityEngine;

namespace Assets.DoReMi.Scripts
{
    public class Label : MonoBehaviour
    {
        /// <summary>
        /// The camera on which the label will be displayed
        /// </summary>
        public Camera mainCamera;

        /// <summary>
        /// The transform of the label
        /// </summary>
        public Transform labelTransform;
        /// <summary>
        /// The scale of the label
        /// </summary>
        public float defaultLabelScale;

        /// <summary>
        /// The text of the minus symbol (to hide it if needed)
        /// </summary>
        public TMP_Text minus;
        /// <summary>
        /// The text component to display the value
        /// </summary>
        public TMP_Text textValue;

        private void Update()
        {
            // Updates the rotation of the label to make it looks the camera
            labelTransform.transform.LookAt(mainCamera.transform);

            // Updates the scale of the label to make it grows or shrink for the camera
            labelTransform.transform.localScale =
                defaultLabelScale
                * Mathf.Sqrt(Vector3.Distance(labelTransform.transform.position, mainCamera.transform.position))
                * Vector3.one;
        }

        /// <summary>
        /// Sets the value to be displayed on the label
        /// </summary>
        /// <param name="newValue">The value to display</param>
        public void SetValue(int newValue)
        {
            minus.gameObject.SetActive(newValue < 0);
            textValue.SetText(Math.Abs(newValue).ToString());
        }
    }
}