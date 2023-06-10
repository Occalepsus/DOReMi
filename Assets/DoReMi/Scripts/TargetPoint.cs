using Assets.DoReMi.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetPoint : MonoBehaviour
{
    /// <summary>
    /// The global GridManager
    /// </summary>
    public GridManager gridManager;

    /// <summary>
    /// The anchor of the controller to place the simulated AP
    /// </summary>
    public Transform anchor;

    public MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (gridManager.NearestTilePosition(anchor, out Vector3 tilePosition))
        {
            meshRenderer.enabled = true;
            transform.position = tilePosition;
        }
        else
        {
            meshRenderer.enabled = false;
        }
    }
}
