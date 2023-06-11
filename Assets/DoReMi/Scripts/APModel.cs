using System;
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
        /// The const part value of the formula
        /// </summary>
        private float _constValue = float.MinValue;

        // TODO: Replace this with user values input
        private void Awake()
        {
            MatchWithUserValues(0, 0, 0, 2.4e9f);
        }

        /// <summary>
        /// Gets the computed signal strength at pos
        /// </summary>
        /// <param name="pos">The pos of the strength to get</param>
        /// <returns>The strength of the signal</returns>
        public float GetSignalStrengthAt(Vector3 pos)
        {
            // According to Friis equation
            float dist = Mathf.Sqrt(Mathf.Pow(transform.position.x - pos.x, 2) + Mathf.Pow(transform.position.z - pos.z, 2));
            return _constValue - (20 * Mathf.Log10(dist));
        }

        /// <summary>
        /// Places the APModel at the given position
        /// </summary>
        /// <param name="newTransfrom">The position to give</param>
        /// <exception cref="InvalidOperationException">The component has not been initialized</exception>
        public void PlaceAPModel(Transform newTransfrom)
        {
            if (_constValue == float.MinValue)
            {
                Debug.LogError("Error: APModel not initialized");
                throw new InvalidOperationException("APModel not initialized. Please call MatchWithLevel or MatchWithUserValues before placing the APModel");
            }
            transform.position = new(newTransfrom.position.x, 0, newTransfrom.position.z);
            gridManager.ComputeGrid(GetSignalStrengthAt);
        }

        /// <summary>
        /// Matches the APModel at origin with the given value
        /// </summary>
        /// <param name="valueToMatch"></param>
        public void MatchWithLevel(int valueToMatch)
        {
            // Setting the const part of the Friis formula in our model with the value to match
            _constValue = valueToMatch;
            // Sets the caller of the method of GridManager that gets the computed signal strength
            gridManager.ComputeGrid(GetSignalStrengthAt);
        }

        /// <summary>
        /// Matches the APModel at origin with the given values
        /// </summary>
        /// <param name="powerEmitterDBm">The power of the emitter in dBm</param>
        /// <param name="gainEmitterDBi">The gain of the emitter in dBi</param>
        /// <param name="gainReceiverDBi">The gain of the receiver in dBi</param>
        /// <param name="wifiFrequencyHz">The frequency of the WiFi in Hz (2.4e9 or 5.0e9)</param>
        public void MatchWithUserValues(int powerEmitterDBm, float gainEmitterDBi, float gainReceiverDBi, float wifiFrequencyHz)
        {
            // Computing the const part of the Friis formula in our model
            _constValue = powerEmitterDBm + gainEmitterDBi + gainReceiverDBi + (20 * Mathf.Log10((float)(3e8 / (4 * Mathf.PI * wifiFrequencyHz))));
            // Sets the caller of the method of GridManager that gets the computed signal strength
            gridManager.ComputeGrid(GetSignalStrengthAt);
        }
    }
}