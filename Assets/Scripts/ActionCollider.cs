using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCollider : MonoBehaviour
{
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {

        if (hit.gameObject.TryGetComponent<BuildingController>(out var building))
        {
            BingoManager.Instance.OpenNumber(building.BuildingID);
        }
    }
}
