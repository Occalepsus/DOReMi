using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.DoReMi.Scripts
{
    public class APModel : MonoBehaviour
    {
        /// <summary>
        /// The global GridManager
        /// </summary>
        public GridManager gridManager;

        /// <summary>
        /// The anchor of the controller to place the simulated AP
        /// </summary>
        public Transform anchor;

        /// <summary>
        /// The parameters of the formula
        /// </summary>
        public int powerEmitterDBm;
        public float gainEmitterDBi;
        public float gainReceiverDBi;
        public float wifiFrequencyHz;

        /// <summary>
        /// The const part value of the formula
        /// </summary>
        private float _constValue;

        /// <summary>
        /// Gets the computed signal strength at pos
        /// </summary>
        /// <param name="pos">The pos of the strength to get</param>
        /// <returns>The strength of the signal</returns>
        private float GetSignalStrengthAt(Vector3 pos)
        {
            // According to Friis equation
            float dist = Mathf.Sqrt(Mathf.Pow(transform.position.x - pos.x, 2) + Mathf.Pow(transform.position.z - pos.z, 2));
            return _constValue - (20 * Mathf.Log10(dist));
        }

        private void Awake()
        {
            // Computing the const part of the Friis formula in our model
            _constValue = powerEmitterDBm + gainEmitterDBi + gainReceiverDBi + (20 * Mathf.Log10((float)(3e8 / (4 * Mathf.PI * wifiFrequencyHz))));
            // Sets the caller of the method of GridManager that gets the computed signal strength
            gridManager.ComputeGrid(GetSignalStrengthAt);
        }

        private void Update()
        {
            // If the button is pressed, moves the AP model position
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                transform.position = anchor.position;

                gridManager.ComputeGrid(GetSignalStrengthAt);
            }
        }
    }
}