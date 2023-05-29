using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.DoReMi.Scripts
{
    public class ValueDisplay : MonoBehaviour
    {
        /// <summary>
        /// The global GridManager
        /// </summary>
        public GridManager gridManager;

        /// <summary>
        /// The anchor of the label
        /// </summary>
        public Transform labelAnchor;
        /// <summary>
        /// The label itself
        /// </summary>
        public Label label;

        private void Update()
        {
            // TODO : Why is this not in the if below?
            // Moves the label at its anchor
            label.transform.position = labelAnchor.position;

            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                // Updates the value on which the label is
                int val = gridManager.GetValueAt(label.transform.position);

                // If the label is in the scanned bounds and the value exists (has been scanned and found)
                if (val > int.MinValue)
                {
                    if (!label.isActiveAndEnabled)
                        label.gameObject.SetActive(true);

                    label.SetValue(val);
                }
                else
                {
                    label.gameObject.SetActive(false);
                }
            }
            else if (label.isActiveAndEnabled)
                label.gameObject.SetActive(false);
        }
    }
}