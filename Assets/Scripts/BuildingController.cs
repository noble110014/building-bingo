using PLATEAU.CityGML;
using PLATEAU.CityInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PLATEAUCityObjectGroup))]
public class BuildingController : MonoBehaviour
{
    private string buildingID;
    public string BuildingID => buildingID;

    private PLATEAUCityObjectGroup info;

    public void Initialize()
    {
        info = GetComponent<PLATEAUCityObjectGroup>();
        SetBuildingID();
    }

    private void SetBuildingID()
    {
        var cityObjects = info.CityObjects;
        // rootCityObjects には複数の CityObject が入る可能性があります
        foreach (var cityObj in cityObjects.rootCityObjects)
        {
            // 1. トップレベルの属性マップを取得
            var attributes = cityObj.AttributesMap;

            // --- 階層 1: uro:buildingIDAttribute を取得 ---
            if (attributes.TryGetValue("uro:buildingIDAttribute", out var attrLevel1))
            {
                // 型が AttributeSet か確認
                if (attrLevel1.Type == AttributeType.AttributeSet)
                {
                    // 子の属性マップを取得
                    var childMap1 = attrLevel1.AttributesMapValue;

                    // --- 階層 2: uro:BuildingIDAttribute を取得 ---
                    if (childMap1.TryGetValue("uro:BuildingIDAttribute", out var attrLevel2))
                    {
                        if (attrLevel2.Type == AttributeType.AttributeSet)
                        {
                            var childMap2 = attrLevel2.AttributesMapValue;

                            // --- 階層 3: uro:buildingID (目的の値) を取得 ---
                            if (childMap2.TryGetValue("uro:buildingID", out var targetAttr))
                            {
                                // 文字列として値を取得
                                string id = targetAttr.StringValue;

                                buildingID = id.Substring(id.Length - 2);
                                
                                Debug.Log($"Building ID Found: {buildingID}");
                            }
                        }
                    }
                }
            }
        }
    }

    [ContextMenu("DebugSetBuildingID")]
    public void DebugSetBuildingID()
    {
        Initialize();
        SetBuildingID();
    }
}
