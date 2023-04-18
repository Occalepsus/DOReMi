using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//struct MatCol
//{
//    public MatCol(Matrix4x4 mat, Color col)
//    {
//        this.Mat = mat;
//        this.Col = col;
//    }

//    public Matrix4x4 Mat { get; }
//    public Color Col { get; }
//}

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
    public float tileSize = 0.5f;
    /// <summary>
    /// The side size of a bar
    /// </summary>
    public float barSize = 0.2f;

    /// <summary>
    /// The mesh of the spheres
    /// </summary>
    public Mesh barMesh;

    /// <summary>
    /// The material of the spheres that are not scanned yet
    /// </summary>
    public Material notScannedMat;
    /// <summary>
    /// The material of the spheres that have been scanned but the selected AP was not detected
    /// </summary>
    public Material emptyScanMat;
    /// <summary>
    /// The material of the spheres that have been scanned and the selected AP was detected, the color is to change
    /// </summary>
    public Material measuredMat;

    ///// <summary>
    ///// The Material Property Block used to make scanned points color
    ///// </summary>
    //private MaterialPropertyBlock _propertyBlock;
    ///// <summary>
    ///// The property id of color in _propertyBlock;
    ///// </summary>
    //private int _colorID;

    /// <summary>
    /// The height of the not scanned or not measured bars
    /// </summary>
    public float nullHeight;

    /// <summary>
    /// The lowest level to be detected
    /// </summary>
    public int lowLevel;
    ///// <summary>
    ///// The color of the spheres with a level lower than lowLevel
    ///// </summary>
    //public Color lowColor;
    /// <summary>
    /// The scale of the spheres with a level lower than lowLevel, also the scale of not scanned shperes
    /// </summary>
    public float lowHeight;

    /// <summary>
    /// The highest level to be detected
    /// </summary>
    public int highLevel;
    ///// <summary>
    ///// The color of the spheres with a level higher than highLevel
    ///// </summary>
    //public Color highColor;
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
    private Dictionary<int, int>[,] _levels;

    /// <summary>
    /// The list of Transforms of the points that have not been scanned yet
    /// </summary>
    private List<Matrix4x4> _notScannedTransforms;
    /// <summary>
    /// The list of Transforms of the points that have been scanned but where the selected AP has not been detected
    /// </summary>
    private List<Matrix4x4> _emptyScanTransforms;
    /// <summary>
    /// The list of Transforms and Colors of the points that have been scanned and where the selected AP has been detected
    /// </summary>
    private List<Matrix4x4> _measuredTransforms;

    private void Awake()
    {
        //_propertyBlock = new();
        //_colorID = Shader.PropertyToID("_Color");

        // Logging an error if there are more than 1023 points to draw in the grid
        if ((_pointsCount = gridSize.x * gridSize.y) > 1023)
        {
            Debug.LogError($"Warning: there are {_pointsCount} points in the grid, which is higher than 1023. Possible errors of crashs");
        }

        // Initializing levels array
        _levels = new Dictionary<int, int>[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                _levels[x, y] = new Dictionary<int, int>(SimManager.MAX_AP);
            }
        }

        UpdateScanDisplay();
    }

    private void Update()
    {
        // Drawing not scanned points
        Graphics.DrawMeshInstanced(barMesh, 0, notScannedMat, _notScannedTransforms);

        // Drawing scanned but not found points
        Graphics.DrawMeshInstanced(barMesh, 0, emptyScanMat, _emptyScanTransforms);

        // Drawing measured meshes
        Graphics.DrawMeshInstanced(barMesh, 0, measuredMat, _measuredTransforms);

        //// Drawing each scanned and found points with the right color
        //foreach (var el in _measuredTransforms)
        //{
        //    // Setting the color
        //    _propertyBlock.SetColor(_colorID, el.Col);
        //    // Drawing the actual sphere
        //    Graphics.DrawMesh(barMesh, el.Mat, measuredMat, 0, null, 0, _propertyBlock);
        //}
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
    public void UpdateScanDisplay()
    {
        _notScannedTransforms = new(_pointsCount);
        _emptyScanTransforms = new(_pointsCount);
        _measuredTransforms = new(_pointsCount);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                // Not scanned case
                if (_levels[x, y].Count == 0)
                {
                    // Creating the transform
                    Matrix4x4 mat = Matrix4x4.TRS(new Vector3(x * tileSize, 0, y * tileSize) + gridOrigin, Quaternion.identity, new Vector3(barSize, nullHeight, barSize));
                    // Adding it to the List of not scanned points
                    _notScannedTransforms.Add(mat);
                }
                // Scanned and found case
                else if (_levels[x, y].TryGetValue(_selectedAPHashcode, out int val))
                {
                    // Unlerping val between lowLevel and highLevel
                    float t = Mathf.InverseLerp(lowLevel, highLevel, val);
                    // Lerping the value of the height
                    float height = Mathf.Lerp(lowHeight, highHeight, t);

                    // Creating the transform
                    Matrix4x4 mat = Matrix4x4.TRS(new Vector3(x * tileSize, 0, y * tileSize) + gridOrigin, Quaternion.identity, new Vector3(barSize, height, barSize));
                    // Adding it to the List of not scanned points
                    //_measuredTransforms.Add(new MatCol(mat, Color.Lerp(lowColor, highColor, t)));
                    _measuredTransforms.Add(mat);
                }
                // Scanned but not found case
                else
                {
                    // Creating the transform
                    Matrix4x4 mat = Matrix4x4.TRS(new Vector3(x * tileSize, 0, y * tileSize) + gridOrigin, Quaternion.identity, new Vector3(barSize, nullHeight, barSize));
                    // Adding it to the List of not scanned points
                    _emptyScanTransforms.Add(mat);
                }
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
            _levels[coordinates.x, coordinates.y].Count == 0;
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
            _levels[coordinates.x, coordinates.y].Add(AP.BSSID.GetHashCode(), AP.level);
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
        Debug.Log(_levels[coordinates.x, coordinates.y].Count);
        if (IsInTheBoundsAtPos(coordinates) && _levels[coordinates.x, coordinates.y].TryGetValue(_selectedAPHashcode, out int value))
        {
            return value;
        }

        return int.MinValue;
    }
}
