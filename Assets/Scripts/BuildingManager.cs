using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingManager : SingletonBase<BuildingManager>
{
    private List<BuildingController> _controllers = new List<BuildingController>();

    public void Initialize()
    {
        _controllers = FindObjectsByType<BuildingController>(FindObjectsSortMode.None).ToList();
    }

    public string[] GetAllBuildingIDArray()
    {
        List<string> ids = new List<string>();
        foreach (var controller in _controllers)
        {
            controller.Initialize();
            ids.Add(controller.BuildingID);
        }

        return ids.Distinct().ToArray();
    }

    [ContextMenu("DebugGetAllBuildingIDArray")]
    private void DebugGetAllBuildingsIDArray()
    {
        Initialize();
        var array = GetAllBuildingIDArray();
        string result = "";
        foreach (var id in array)
        {
            result += (id) + " ";
        }

        Debug.Log(result);
    }
}
