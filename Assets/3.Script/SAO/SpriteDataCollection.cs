using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Asset 메뉴에 추가하여 유니티 에디터에서 쉽게 생성할 수 있도록 합니다.
[CreateAssetMenu(fileName = "NewSpriteCollection", menuName = "ScriptableObjects/Sprite Data Collection")]
public class SpriteDataCollection : ScriptableObject
{
    // === 옵션 1: Sprite 리스트만 포함 ===
    // 가장 간단한 형태이며, Sprite 자체가 이름 등의 정보를 포함한다고 가정할 때 적합합니다.
    public List<Sprite> sprites = new List<Sprite>();

    // === 옵션 2: Sprite와 추가 정보를 묶는 구조체/클래스 리스트 포함 ===
    // 각 Sprite에 고유 ID, 이름, 설명 등 추가적인 메타데이터를 연결하고 싶을 때 유용합니다.
    // 이 경우, 'sprites' 리스트 대신 'spriteEntries' 리스트를 사용합니다.
    [System.Serializable]
    public class SpriteEntry
    {
        public string id;          // Sprite를 식별할 고유 ID (예: "item_sword", "effect_explosion")
        public string displayName; // 게임 내에서 보여줄 이름
        public Sprite sprite;      // 실제 Sprite 객체
        public string description; // Sprite에 대한 설명 (선택 사항)
    }

    public List<SpriteEntry> spriteEntries = new List<SpriteEntry>();


    // 특정 ID로 Sprite를 찾아 반환하는 도우미 메서드 (옵션 2를 사용할 경우)
    public Sprite GetSpriteById(string id)
    {
        foreach (SpriteEntry entry in spriteEntries)
        {
            if (entry.id == id)
            {
                return entry.sprite;
            }
        }
        Debug.LogWarning($"Sprite with ID '{id}' not found in {name}.");
        return null;
    }

    public Sprite GetSpriteByName(string name)
    {
        return sprites.FirstOrDefault(x => x.name == name);
    }

    // 인덱스로 Sprite를 찾아 반환하는 도우미 메서드 (옵션 1 또는 2 모두 사용 가능)
    public Sprite GetSpriteByIndex(int index)
    {
        if (index >= 0 && index < sprites.Count) // 옵션 1 사용 시
        {
            return sprites[index];
        }
        else if (index >= 0 && index < spriteEntries.Count) // 옵션 2 사용 시
        {
            return spriteEntries[index].sprite;
        }
        Debug.LogWarning($"Sprite at index {index} is out of bounds in {name}.");
        return null;
    }
}