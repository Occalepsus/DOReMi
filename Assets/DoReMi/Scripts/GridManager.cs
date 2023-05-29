using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.DoReMi.Scripts
{
    struct Grid
    {
        /**
         * Grid organisation:
         *     x              x
         *      \            / offset
         *       [1]      [2]
         *     
         *      
         *     
         *  gridZDir  
         *     v    
         *       [0]      [3]
         *     |/            \
         *     x- gridXDir    x
         * gridOrigin    
         */

        /// <summary>
        /// The size of the grid for the tiling
        /// </summary>
        private Vector2Int _gridSize;
        /// <summary>
        /// The number of points in the grid
        /// </summary>
        private readonly int _pointsCount;

        Matrix4x4 rotationToGridMatrix;
        Matrix4x4 rotationToWorldMatrix;

        public Grid(Vector3[] playAreaBounds, float tileSize, int gridOffset)
        {
            if (playAreaBounds == null || playAreaBounds.Length != 4) throw new ArgumentException("Invalid argument");

            Vector3 gridOrigin = playAreaBounds[0] - gridOffset * tileSize * ((playAreaBounds[1] - playAreaBounds[0]).normalized + (playAreaBounds[3] - playAreaBounds[0]).normalized);

            _gridSize = new(Mathf.RoundToInt((playAreaBounds[1] - playAreaBounds[0]).magnitude / tileSize) + 2 * gridOffset + 1,
                Mathf.RoundToInt((playAreaBounds[3] - playAreaBounds[0]).magnitude / tileSize) + 2 * gridOffset + 1);
            _pointsCount = _gridSize.x * _gridSize.y;

            rotationToWorldMatrix = Matrix4x4.TRS(gridOrigin, Quaternion.FromToRotation(Vector3.right, playAreaBounds[1] - playAreaBounds[0]), Vector3.one * tileSize);
            rotationToGridMatrix = rotationToWorldMatrix.inverse;
        }

        /// <summary>
        /// Gets the position of the point in the world
        /// </summary>
        /// <param name="x">The x coordiante</param>
        /// <param name="y">The y coordinate</param>
        /// <returns>inGridPos projected on the world</returns>
        public Vector3 ToWorldPos(int x, int y)
        {
            return rotationToWorldMatrix.MultiplyPoint3x4(new(x, 0, y));
        }

        /// <summary>
        /// Gets the position of the point in the world
        /// </summary>
        /// <param name="inGridCoords">The coordiantes to get</param>
        /// <returns>inGridPos projected on the world</returns>
        public Vector3 ToWorldPos(Vector2Int inGridCoords)
        {
            return ToWorldPos(inGridCoords.x, inGridCoords.y);
        }

        /// <summary>
        /// Gets the position of the point in the grid rounded to the nearest tile
        /// </summary>
        /// <param name="inWorldPos">The position to get</param>
        /// <param name="inGridCoords">The coordinates in the grid</param>
        /// <returns>true if the inGridPos is in the bounds of the grid, false otherwise</returns>
        public bool ToGridCoords(Vector3 inWorldPos, out Vector2Int inGridCoords)
        {
            Vector3 vec = rotationToGridMatrix.MultiplyPoint3x4(inWorldPos);
            inGridCoords = new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.z));
            return inGridCoords.x >= 0 && inGridCoords.y >= 0 && inGridCoords.x < _gridSize.x && inGridCoords.y < _gridSize.y;
        }

        /// <summary>
        /// Gets the position of the point in the grid rounded to the nearest tile and returns the distance of the position and the rounded one
        /// </summary>
        /// <param name="inWorldPos">The position to get</param>
        /// <param name="inGridCoords">The coordinates in the grid</param>
        /// <returns>The distance between the normal projection and in the grid</returns>
        public float ToGridCoordsDist(Vector3 inWorldPos, out Vector2Int inGridCoords)
        {
            Vector3 vec = rotationToGridMatrix.MultiplyPoint3x4(inWorldPos);
            inGridCoords = new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.z));
            return Vector3.Distance(new(inWorldPos.x, 0, inWorldPos.z) , ToWorldPos(inGridCoords));
        }

        public readonly Vector2Int GetSize()
        {
            return _gridSize;
        }
        public readonly int GetPointsCount()
        {
            return _pointsCount;
        }
    }



    public class GridManager : MonoBehaviour
    {
        /// <summary>
        /// The size of the size of a tile
        /// </summary>
        public float tileSize;
        /// <summary>
        /// The tiles that are offsetted from the playArea
        /// </summary>
        public int gridOffset;

        /// <summary>
        /// The grid to use
        /// </summary>
        private Grid _grid;

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
        private float[,] _computedLevels;

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
            try
            {
                _grid = new Grid(OVRManager.boundary.GetGeometry(OVRBoundary.BoundaryType.PlayArea), tileSize, gridOffset);
            }
            // In case the Guardian is not set
            catch (Exception e)
            {
                Debug.LogWarning(e);
                Vector3[] v = { new Vector3(0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 1) };
                _grid = new Grid(v, tileSize, gridOffset);
            }

            // Initializing levels array
            _scannedLevels = new Dictionary<int, int>[_grid.GetSize().x, _grid.GetSize().y];
            for (int x = 0; x < _grid.GetSize().x; x++)
            {
                for (int y = 0; y < _grid.GetSize().y; y++)
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
            _notScannedSphereTransforms = new(_grid.GetPointsCount());
            _emptyScanSphereTransforms = new(_grid.GetPointsCount());
            _measuredSphereTransforms = new(_grid.GetPointsCount());
            _measuredLineTransforms = new(_grid.GetPointsCount());

            for (int x = 0; x < _grid.GetSize().x; x++)
            {
                for (int y = 0; y < _grid.GetSize().y; y++)
                {
                    // Not scanned case
                    if (_scannedLevels[x, y].Count == 0)
                    {
                        // Creating the transform
                        Matrix4x4 mat = Matrix4x4.TRS(_grid.ToWorldPos(x, y), Quaternion.identity, Vector3.one * sphereSize);
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
                        Matrix4x4 sMat = Matrix4x4.TRS(_grid.ToWorldPos(x, y) + height * Vector3.up, Quaternion.identity, Vector3.one * sphereSize);
                        Matrix4x4 lMat = Matrix4x4.TRS(_grid.ToWorldPos(x, y) + height / 2 * Vector3.up, Quaternion.identity, new Vector3(lineSize, height / 2f, lineSize));

                        // Adding them to the List of scanned points
                        _measuredSphereTransforms.Add(sMat);
                        _measuredLineTransforms.Add(lMat);
                    }
                    // Scanned but not found case
                    else
                    {
                        // Creating the transform
                        Matrix4x4 mat = Matrix4x4.TRS(_grid.ToWorldPos(x, y), Quaternion.identity, Vector3.one * sphereSize);
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
            _computedSphereTransforms = new(_grid.GetPointsCount());
            for (int x = 0; x < _grid.GetSize().x; x++)
            {
                for (int y = 0; y < _grid.GetSize().y; y++)
                {
                    // Unlerping val between lowLevel and highLevel
                    float t = Mathf.InverseLerp(lowLevel, highLevel, _computedLevels[x, y]);
                    // Lerping the value of the height
                    float height = Mathf.Lerp(lowHeight, highHeight, t);

                    Matrix4x4 mat = Matrix4x4.TRS(_grid.ToWorldPos(x, y) + height * Vector3.up, Quaternion.identity, Vector3.one * sphereSize);
                    _computedSphereTransforms.Add(mat);
                }
            }
        }

        /// <summary>
        /// Gets the distance between the world and the nearest tile
        /// </summary>
        /// <param name="position">The position in the world</param>
        /// <param name="inGridPos">The coordinates in the tile</param>
        /// <returns>The distance</returns>
        public float GetDistanceFromNearestTile(Vector3 position, out Vector2Int inGridPos)
        {
            return _grid.ToGridCoordsDist(position, out inGridPos);
        }

        /// <summary>
        /// Gets if a scan has already been performed here
        /// </summary>
        /// <param name="worldPos">The world position of the scan</param>
        /// <param name="inGridPos">Gets the coordinates on the grid</param>
        /// <returns>true if scan is in the bounds and has never been performed here, false otherwise</returns>
        public bool CanScanAtPos(Vector3 worldPos, out Vector2Int inGridPos)
        {
            return _grid.ToGridCoords(worldPos, out inGridPos) &&
                _scannedLevels[inGridPos.x, inGridPos.y].Count == 0;
        }

        /// <summary>
        /// Registers the scan into the grid
        /// </summary>
        /// <param name="position">The position of the scan</param>
        /// <param name="APInfo">The information of the scan</param>
        public void ScanAtPos(Vector3 position, WifiAPInfo[] APInfo)
        {
            if (!_grid.ToGridCoords(position, out Vector2Int coordinates)) return;

            foreach (var AP in APInfo)
            {
                _scannedLevels[coordinates.x, coordinates.y].Add(AP.BSSID.GetHashCode(), AP.level);
            }

            UpdateScanDisplay();
        }

        /// <summary>
        /// Gets the value of the scan at the coordinates for the selected AP
        /// </summary>
        /// <param name="position">The position of the scan</param>
        /// <returns>int.MinValue if no scan found, the scanned value otherwise</returns>
        public int GetValueAt(Vector3 position)
        {
            if (_grid.ToGridCoords(position, out Vector2Int coordinates) && _scannedLevels[coordinates.x, coordinates.y].TryGetValue(_selectedAPHashcode, out int value))
            {
                return value;
            }

            return int.MinValue;
        }

        /// <summary>
        /// Makes the compute levels grid to be calculated with computeAtPos
        /// </summary>
        /// <param name="computeAtPos">The function that computes the model</param>
        public void ComputeGrid(Func<Vector3, float> computeAtPos)
        {
            _computedLevels = new float[_grid.GetSize().x, _grid.GetSize().y];
            for (int x = 0; x < _grid.GetSize().x; x++)
            {
                for (int y = 0; y < _grid.GetSize().y; y++)
                {
                    _computedLevels[x, y] = computeAtPos(_grid.ToWorldPos(x, y));
                }
            }
            UpdateComputeDisplay();
        }
    }
}