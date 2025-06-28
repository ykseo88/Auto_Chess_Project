using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SAOSpriteDatabase", menuName = "ScriptableObjects/Sprite Database")]
public class SAOSpriteDatabase : ScriptableObject
{
    [System.Serializable]
    public class SpriteArray
    {
        public string Name;
        public int SpriteId;
        public Sprite sprite;
    }
    
    public SpriteArray[] Sprites;
    
    public Sprite GetSpriteByName(string name)
    {
        foreach (SpriteArray unitArray in Sprites)
        {
            if (unitArray.Name == name)
            {
                return unitArray.sprite;
            }
        }
        Debug.LogError("Sprite not found: " + name);
        return null;
    }

    public Sprite GetSpriteById(int id)
    {
        foreach (SpriteArray unitArray in Sprites)
        {
            if (unitArray.SpriteId == id)
            {
                return unitArray.sprite;
            }
        }
        Debug.LogError("Sprite not found: " + id);
        return null;
    }
    
    public int GetIdBySprite(Sprite sprite)
    {
        foreach (SpriteArray spriteArray in Sprites)
        {
            if (spriteArray.sprite == sprite)
            {
                return spriteArray.SpriteId;
            }
        }
        Debug.LogError("Sprite not found: " + sprite);
        return -1;
    }

    public int GetIdByName(string name)
    {
        foreach (SpriteArray spriteArray in Sprites)
        {
            if (spriteArray.Name == name)
            {
                return spriteArray.SpriteId;
            }
        }
        
        Debug.LogError("Sprite not found: " + name);
        return -1;
    }
    
}
