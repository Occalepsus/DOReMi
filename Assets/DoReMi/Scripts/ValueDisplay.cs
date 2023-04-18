using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValueDisplay : MonoBehaviour
{
    public GridManager gridManager;

    public Transform labelAnchor;
    public Label label;

    private void Update()
    {
        label.transform.position = labelAnchor.position;

        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            Vector2Int gridPos = gridManager.GetNearestCoordinate(label.transform.position);
            int val = gridManager.GetValueAt(gridPos);
            Debug.Log(gridPos);
            Debug.Log(val);
            if (gridManager.IsInTheBoundsAtPos(gridPos) && val > int.MinValue)
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
