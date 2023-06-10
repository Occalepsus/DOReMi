using System.Collections;
using UnityEngine;
using UnityEngine.Android;

namespace Assets.DoReMi.Scripts
{
    public class SaveRestoreManager : MonoBehaviour
    {
        /// <summary>
        /// Reference to the GridManager
        /// </summary>
        public GridManager gridManager;

        private int[] _savedValues;

        /// <summary>
        /// Calls SaveDisplayedGrid() on the GridManager and stores the returned values
        /// </summary>
        public void SaveGrid()
        {
            _savedValues = gridManager.SaveDisplayedGrid();
        }

        /// <summary>
        /// Calls ResetGrid() on the GridManager
        /// </summary>
        public void ResetGrid()
        {
            gridManager.ResetGrid();
        }

        /// <summary>
        /// Calls RestoreGrid() on the GridManager with saved values
        /// </summary>
        public void RestoreValues()
        {
            gridManager.RestoreSavedGrid(_savedValues);
        }

        // TODO: Remove this, it's just for testing
        private void Update()
        {
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                SaveGrid();
            }
            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
            {
                RestoreValues();
            }
            if (OVRInput.GetDown(OVRInput.RawButton.B))
            {
                ResetGrid();
            }
        }
    }
}