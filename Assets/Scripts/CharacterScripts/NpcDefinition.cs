using UnityEngine;

public enum NpcArchetype { Coach, Referee, Dealer, Gingerbread, Event }

[CreateAssetMenu(menuName = "NPC/NpcDefinition")]
public class NpcDefinition : ScriptableObject
{
    public string npcId;
    public NpcArchetype archetype;

    [Header("Dialogue (StreamingAssets)")]
    public string dialogueFile;
    public string displayName;
    public bool acceptBribes = false;
    public string prefferedBribeItemId;
    public int minBribeAmount = 1;
    public float bribeBonus = 0f;
    public float infoBonus = 0f;
    [Header("Visuals")]
    public Sprite characterSprite;

    [Header("Vendor options")]
    public bool isVendor;
    public string commodityId;
    public int commodityPrice = 1;
}
