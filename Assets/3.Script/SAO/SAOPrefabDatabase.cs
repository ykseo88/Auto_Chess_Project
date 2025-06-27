using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SAOPrefabDatabase", menuName = "ScriptableObjects/Prefab Database")]

public class SAOPrefabDatabase : ScriptableObject
{
    [System.Serializable]
    public class PrefabArray
    {
        public string name;
        public int prefabId;
        public GameObject prefab;
    }
    
    public PrefabArray[] Prefabs;
    
    public GameObject GetPrefabsByName(string name)
    {
        foreach (PrefabArray prefabArray in Prefabs)
        {
            if (prefabArray.name == name)
            {
                return prefabArray.prefab;
            }
        }
        Debug.LogError("Sprite not found: " + name);
        return null;
    }

    public GameObject GetPrefabsById(int id)
    {
        foreach (PrefabArray prefabArray in Prefabs)
        {
            if (prefabArray.prefabId == id)
            {
                return prefabArray.prefab;
            }
        }
        Debug.LogError("Sprite not found: " + id);
        return null;
    }
    
    public int GetIdByPrefabs(GameObject prefab)
    {
        foreach (PrefabArray prefabArray in Prefabs)
        {
            if (prefabArray.prefab == prefab)
            {
                return prefabArray.prefabId;
            }
        }
        Debug.LogError("Sprite not found: " + prefab);
        return -1;
    }
}
