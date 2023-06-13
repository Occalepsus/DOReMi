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

        /// <summary>
        /// Saves the values of the selected AP on the grid and displays them
        /// </summary>
        public void SaveGrid()
        {
            gridManager.DisplaySavedGrid(gridManager.SaveDisplayedGrid());
        }

        /// <summary>
        /// Remove all saved values from the grid
        /// </summary>
        public void DiscardSavedGrid()
        {
            gridManager.DiscardSavedValues();
        }

        /// <summary>
        /// Calls ResetGrid() on the GridManager
        /// </summary>
        public void ResetGrid()
        {
            gridManager.ResetGrid();
        }
    }
}