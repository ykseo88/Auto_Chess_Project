using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

[CreateAssetMenu(fileName = "SAOUnitDatabase", menuName = "ScriptableObjects/Unit Database")]
public class SAOUnitPrefabDatabase : ScriptableObject
{
    // Start is called before the first frame update

    [System.Serializable]
    public class UnitArray
    {
        public string Name;
        public GameObject Unit;
    }

    public UnitArray[] UnitPrefabs;
    
    public GameObject GetUnitPrefabByName(string name)
    {
        foreach (UnitArray unitArray in UnitPrefabs)
        {
            if (unitArray.Name == name)
            {
                return unitArray.Unit;
            }
        }
        Debug.LogError("Unit prefab not found: " + name);
        return null;
    }
}
