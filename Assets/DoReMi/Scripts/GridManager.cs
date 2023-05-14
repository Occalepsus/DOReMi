using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.DoReMi.Scripts
{
    public class GridManager : MonoBehaviour
    {
        /// <summary>
        /// The origin of the grid in the world
        /// </summary>
        public Vector3 gridOrigin = Vector3.zero;
        /// <summary>
        /// The size of the points in the grid
        /// </summary>
        public Vector2Int gridSize;
        /// <summary>
        /// The number of points in the grid
        /// </summary>
        private int _pointsCount;
        /// <summary>
        /// The size of the size of a tile
        /// </summary>
        public float tileSize;

        /// <summary>
        /// The size of the spheres of the beacons
        /// </summary>
        public float sphereSize;
        /// <summary>
        /// The mesh of the spheres of the beacons
        /// </summary>
        public Mesh sphereMesh;
        /// <summary>
        /// The size of the lines of the beacons
        /// </summary>
        public float lineSize;
        /// <summary>
        /// The mesh of the lines of the beacons
        /// </summary>
        public Mesh lineMesh;

        /// <summary>
        /// The material of the spheres that are not scanned yet
        /// </summary>
        public Material notScannedMat;
        /// <summary>
        /// The material of the spheres that have been scanned but the selected AP was not detected
        /// </summary>
        public Material emptyScanMat;
        /// <summary>
        /// The material of the spheres that have been scanned and the selected AP was detected
        /// </summary>
        public Material measuredMat;
        /// <summary>
        /// The material of the spheres that have been computed
        /// </summary>
        public Material computedMat;

        /// <summary>
        /// The height of the not scanned or not measured bars
        /// </summary>
        public float nullHeight;

        /// <summary>
        /// The lowest level to be detected
        /// </summary>
        public int lowLevel;
        /// <summary>
        /// The scale of the spheres with a level lower than lowLevel, also the scale of not scanned shperes
        /// </summary>
        public float lowHeight;

        /// <summary>
        /// The highest level to be detected
        /// </summary>
        public int highLevel;
        /// <summary>
        /// The color of the spheres with a level higher than highLevel
        /// </summary>
        public float highHeight;

        /// <summary>
        /// The hashcode of the selected AP
        /// </summary>
        private int _selectedAPHashcode;

        /// <summary>
        /// A matrix of every AP level at every position of the grid
        /// </summary>
        private Dictionary<int, int>[,] _scannedLevels;
        /// <summary>
        /// A matrix of every AP level at every position of the grid
        /// </summary>
        private int[,] _computedLevels;

        /// <summary>
        /// The list of Transforms of the points that have not been scanned yet
        /// </summary>
        private List<Matrix4x4> _notScannedSphereTransforms;
        /// <summary>
        /// The list of Transforms of the points that have been scanned but where the selected AP has not been detected
        /// </summary>
        private List<Matrix4x4> _emptyScanSphereTransforms;
        /// <summary>
        /// The list of Transforms of the points that have been scanned and where the selected AP has been detected
        /// </summary>
        private List<Matrix4x4> _measuredSphereTransforms;
        /// <summary>
        /// The list of Transforms of the points that have been scanned and where the selected AP has been detected for the line of the becons
        /// </summary>
        private List<Matrix4x4> _measuredLineTransforms;
        /// <summary>
        /// The list of Transforms of the points that have been computed
        /// </summary>
        private List<Matrix4x4> _computedSphereTransforms;

        private void Awake()
        {
            // TODO: Remove if no use of DrawMeshInstanced
            // Logging an error if there are more than 1023 points to draw in the grid
            if ((_pointsCount = gridSize.x * gridSize.y) > 1023)
            {
                Debug.LogError($"Warning: there are {_pointsCount} points in the grid, which is higher than 1023. Possible errors of crashs");
            }

            // Initializing levels array
            _scannedLevels = new Dictionary<int, int>[gridSize.x, gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    _scannedLevels[x, y] = new Dictionary<int, int>(SimManager.MAX_AP);
                }
            }

            UpdateScanDisplay();
        }

        private void Update()
        {
            // Drawing not scanned points
            foreach (var transform in _notScannedSphereTransforms)
            {
                Graphics.DrawMesh(sphereMesh, transform, notScannedMat, 0);
            }
            // Drawing scanned but not found points
            foreach (var transform in _emptyScanSphereTransforms)
            {
                Graphics.DrawMesh(sphereMesh, transform, emptyScanMat, 0);
            }
            // Drawing point of the beacons
            foreach (var transform in _measuredSphereTransforms)
            {
                Graphics.DrawMesh(sphereMesh, transform, measuredMat, 0);
            }
            // Drawing line of the beacons
            foreach (var transform in _measuredLineTransforms)
            {
                Graphics.DrawMesh(lineMesh, transform, measuredMat, 0);
            }

            // Drawing points for the computed beacons
            foreach (var transform in _computedSphereTransforms)
            {
                Graphics.DrawMesh(sphereMesh, transform, computedMat, 0);
            }
        }

        /// <summary>
        /// Sets the selected shown AP
        /// </summary>
        /// <param name="newAPHashcode">The Hashcode of the AP to show</param>
        public void SetSelectedAP(int newAPHashcode)
        {
            _selectedAPHashcode = newAPHashcode;

            UpdateScanDisplay();
        }

        /// <summary>
        /// Updates the spheres that are drawn with the new state of the scan;
        /// </summary>
        private void UpdateScanDisplay()
        {
            _notScannedSphereTransforms = new(_pointsCount);
            _emptyScanSphereTransforms = new(_pointsCount);
            _measuredSphereTransforms = new(_pointsCount);
            _measuredLineTransforms = new(_pointsCount);

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    // Not scanned case
                    if (_scannedLevels[x, y].Count == 0)
                    {
                        // Creating the transform
                        Matrix4x4 mat = Matrix4x4.TRS(new Vector3(x * tileSize, 0, y * tileSize) + gridOrigin, Quaternion.identity, Vector3.one * sphereSize);
                        // Adding it to the List of not scanned points
                        _notScannedSphereTransforms.Add(mat);
                    }
                    // Scanned and found case
                    else if (_scannedLevels[x, y].TryGetValue(_selectedAPHashcode, out int val))
                    {
                        // Unlerping val between lowLevel and highLevel
                        float t = Mathf.InverseLerp(lowLevel, highLevel, val);
                        // Lerping the value of the height
                        float height = Mathf.Lerp(lowHeight, highHeight, t);

                        // Creating the transform of the sphere and the line
                        Matrix4x4 sMat = Matrix4x4.TRS(new Vector3(x * tileSize, height, y * tileSize) + gridOrigin, Quaternion.identity, Vector3.one * sphereSize);
                        Matrix4x4 lMat = Matrix4x4.TRS(new Vector3(x * tileSize, height / 2f, y * tileSize) + gridOrigin, Quaternion.identity, new Vector3(lineSize, height / 2f, lineSize));

                        // Adding them to the List of scanned points
                        _measuredSphereTransforms.Add(sMat);
                        _measuredLineTransforms.Add(lMat);
                    }
                    // Scanned but not found case
                    else
                    {
                        // Creating the transform
                        Matrix4x4 mat = Matrix4x4.TRS(new Vector3(x * tileSize, nullHeight / 2f, y * tileSize) + gridOrigin, Quaternion.identity, Vector3.one * sphereSize);
                        // Adding it to the List of not scanned points
                        _emptyScanSphereTransforms.Add(mat);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the display of the computed points according to the updated list of points
        /// </summary>
        private void UpdateComputeDisplay()
        {
            _computedSphereTransforms = new(_pointsCount);
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    // Unlerping val between lowLevel and highLevel
                    float t = Mathf.InverseLerp(lowLevel, highLevel, _computedLevels[x, y]);
                    // Lerping the value of the height
                    float height = Mathf.Lerp(lowHeight, highHeight, t);

                    Matrix4x4 mat = Matrix4x4.TRS(new Vector3(x * tileSize, height, y * tileSize) + gridOrigin, Quaternion.identity, Vector3.one * sphereSize);
                    _computedSphereTransforms.Add(mat);
                }
            }
        }

        /// <summary>
        /// Gets the nearest coordinate from pos in the grid
        /// </summary>
        /// <remarks>Warning: Does not look for grid bounds</remarks>
        /// <param name="pos">The position in world</param>
        /// <returns>The 2D coordinates of the nearest point</returns>
        public Vector2Int GetNearestCoordinate(Vector3 pos)
        {
            pos -= gridOrigin;
            pos /= tileSize;

            return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.z));
        }

        /// <summary>
        /// Gets if the coordinates are in the bounds of the grid
        /// </summary>
        /// <param name="coordinates">The 2D coordinates to test</param>
        /// <returns>true if in the bound, false otherwise</returns>
        public bool IsInTheBoundsAtPos(Vector2Int coordinates)
        {
            return 0 <= coordinates.x && coordinates.x < gridSize.x &&
                0 <= coordinates.y && coordinates.y < gridSize.y;
        }

        /// <summary>
        /// Gets if a scan has already been performed here
        /// </summary>
        /// <param name="coordinates">The 2D coordinates of the scan</param>
        /// <returns>true if scan is in the bounds and has never been performed here, false otherwise</returns>
        public bool CanScanAtPos(Vector2Int coordinates)
        {
            return IsInTheBoundsAtPos(coordinates) &&
                _scannedLevels[coordinates.x, coordinates.y].Count == 0;
        }

        /// <summary>
        /// Registers the scan into the grid
        /// </summary>
        /// <param name="coordinates">The 2D coordinates of the scan</param>
        /// <param name="APInfo">The information of the scan</param>
        public void ScanAtPos(Vector2Int coordinates, WifiAPInfo[] APInfo)
        {
            foreach (var AP in APInfo)
            {
                _scannedLevels[coordinates.x, coordinates.y].Add(AP.BSSID.GetHashCode(), AP.level);
            }

            UpdateScanDisplay();
        }

        /// <summary>
        /// Gets the value of the scan at the coordinates for the selected AP
        /// </summary>
        /// <param name="coordinates">The coordinates of the scan</param>
        /// <returns>int.MinValue if no scan found, the scanned value otherwise</returns>
        public int GetValueAt(Vector2Int coordinates)
        {
            Debug.Log(_scannedLevels[coordinates.x, coordinates.y].Count);
            if (IsInTheBoundsAtPos(coordinates) && _scannedLevels[coordinates.x, coordinates.y].TryGetValue(_selectedAPHashcode, out int value))
            {
                return value;
            }

            return int.MinValue;
        }

        /// <summary>
        /// Makes the compute levels grid to be calculated with computeAtPos
        /// </summary>
        /// <param name="computeAtPos">The function that computes the model</param>
        public void ComputeGrid(Func<Vector3, int> computeAtPos)
        {
            _computedLevels = new int[gridSize.x, gridSize.y];
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    _computedLevels[x, y] = computeAtPos(new Vector3(x, 0, y) * tileSize + gridOrigin);
                }
            }
            UpdateComputeDisplay();
        }
    }
}