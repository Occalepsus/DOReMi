using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct MatCol
{
    public MatCol(Matrix4x4 mat, Color col)
    {
        this.Mat = mat;
        this.Col = col;
    }

    public Matrix4x4 Mat { get; }
    public Color Col { get; }
}

public class GridManager : MonoBehaviour
{
    /// <summary>
    /// The origin of the grid in the world
    /// </summary>
    public Vector3 gridOrigin = Vector3.zero;
    /// <summary>
    /// The size of the points in the grid
    /// </summary>
    public Vector3Int gridSize = new(3, 2, 5);
    /// <summary>
    /// The number of points in the grid
    /// </summary>
    private int _pointsCount;
    /// <summary>
    /// The space between points
    /// </summary>
    public float pointDist = 0.5f;

    /// <summary>
    /// The mesh of the spheres
    /// </summary>
    public Mesh sphereMesh;

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

    /// <summary>
    /// The Material Property Block used to make scanned points color
    /// </summary>
    private MaterialPropertyBlock _propertyBlock;
    /// <summary>
    /// The property id of color in _propertyBlock;
    /// </summary>
    private int _colorID;

    /// <summary>
    /// The lowest level to be detected
    /// </summary>
    public int lowLevel;
    /// <summary>
    /// The color of the spheres with a level lower than lowLevel
    /// </summary>
    public Color lowColor;
    /// <summary>
    /// The scale of the spheres with a level lower than lowLevel, also the scale of not scanned shperes
    /// </summary>
    public float lowScale;

    /// <summary>
    /// The highest level to be detected
    /// </summary>
    public int highLevel;
    /// <summary>
    /// The color of the spheres with a level higher than highLevel
    /// </summary>
    public Color highColor;
    /// <summary>
    /// The color of the spheres with a level higher than highLevel
    /// </summary>
    public float highScale;

    /// <summary>
    /// The hashcode of the selected AP
    /// </summary>
    private int _selectedAPHashcode;

    /// <summary>
    /// A matrix of every AP level at every position of the grid
    /// </summary>
    private Dictionary<int, int>[,,] _levels;

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
    private List<MatCol> _measuredTransforms;

    private void Awake()
    {
        _propertyBlock = new();
        _colorID = Shader.PropertyToID("_Color");

        // Logging an error if there are more than 1023 points to draw in the grid
        if ((_pointsCount = gridSize.x * gridSize.y * gridSize.z) > 1023)
        {
            Debug.LogError($"Warning: there are {_pointsCount} points in the grid, which is higher than 1023. Possible errors of crashs");
        }

        // Initializing levels array
        _levels = new Dictionary<int, int>[gridSize.x, gridSize.y, gridSize.z];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    _levels[x, y, z] = new Dictionary<int, int>(SimManager.MAX_AP);
                }
            }
        }

        UpdateScanDisplay();
    }

    private void Update()
    {
        // Drawing not scanned points
        Graphics.DrawMeshInstanced(sphereMesh, 0, notScannedMat, _notScannedTransforms);

        // Drawing scanned but not found points
        Graphics.DrawMeshInstanced(sphereMesh, 0, emptyScanMat, _emptyScanTransforms);

        // Drawing each scanned and found points with the right color
        foreach (var el in _measuredTransforms)
        {
            // Setting the color
            _propertyBlock.SetColor(_colorID, el.Col);
            // Drawing the actual sphere
            Graphics.DrawMesh(sphereMesh, el.Mat, measuredMat, 0, null, 0, _propertyBlock);
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
    public void UpdateScanDisplay()
    {
        _notScannedTransforms = new(gridSize.x * gridSize.y * gridSize.z);
        _emptyScanTransforms = new(gridSize.x * gridSize.y * gridSize.z);
        _measuredTransforms = new(gridSize.x * gridSize.y * gridSize.z);

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    // Not scanned case
                    if (_levels[x, y, z].Count == 0)
                    {
                        // Creating the transform
                        Matrix4x4 mat = Matrix4x4.TRS((new Vector3(x, y, z) * pointDist) + gridOrigin, Quaternion.identity, Vector3.one * lowScale);
                        // Adding it to the List of not scanned points
                        _notScannedTransforms.Add(mat);
                    }
                    // Scanned and found case
                    else if (_levels[x, y, z].TryGetValue(_selectedAPHashcode, out int val))
                    {
                        // Getting the t of the level
                        float t;
                        if (val <= lowLevel) t = 0;
                        else if (val >= highLevel) t = 1;
                        else t = (val - lowLevel) / (float)(highLevel - lowLevel);

                        // Creating the transform
                        Matrix4x4 mat = Matrix4x4.TRS((new Vector3(x, y, z) * pointDist) + gridOrigin, Quaternion.identity, Vector3.one * (t * (highScale - lowScale) + lowScale));
                        // Adding it to the List of not scanned points
                        _measuredTransforms.Add(new MatCol(mat, Color.Lerp(lowColor, highColor, t)));
                    }
                    // Scanned but not found case
                    else
                    {
                        // Creating the transform
                        Matrix4x4 mat = Matrix4x4.TRS((new Vector3(x, y, z) * pointDist) + gridOrigin, Quaternion.identity, Vector3.one * lowScale);
                        // Adding it to the List of not scanned points
                        _emptyScanTransforms.Add(mat);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the nearest coordinate from pos in the grid
    /// </summary>
    /// <remarks>Warning: Does not look for grid bounds</remarks>
    /// <param name="pos">The position in world</param>
    /// <returns>The coordinates of the nearest point</returns>
    public Vector3Int GetNearestCoordinate(Vector3 pos)
    {
        pos -= gridOrigin;
        pos /= pointDist;

        return new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
    }

    /// <summary>
    /// Gets if a scan has already been performed here
    /// </summary>
    /// <param name="coordinates">The coordinates of the scan</param>
    /// <returns>true if scan has never been performed here, false otherwise</returns>
    public bool CanScanAtPos(Vector3Int coordinates)
    {
        return 
            0 <= coordinates.x && coordinates.x < gridSize.x &&
            0 <= coordinates.y && coordinates.y < gridSize.y &&
            0 <= coordinates.z && coordinates.z < gridSize.z &&            
            _levels[coordinates.x, coordinates.y, coordinates.z].Count == 0;
    }

    /// <summary>
    /// Registers the scan into the grid
    /// </summary>
    /// <param name="coordinates">The coordinates of the scan</param>
    /// <param name="APInfo">The information of the scan</param>
    public void ScanAtPos(Vector3Int coordinates, WifiAPInfo[] APInfo)
    {
        foreach (var AP in APInfo)
        {
            _levels[coordinates.x, coordinates.y, coordinates.z].Add(AP.BSSID.GetHashCode(), AP.level);
        }

        UpdateScanDisplay();
    }
}
