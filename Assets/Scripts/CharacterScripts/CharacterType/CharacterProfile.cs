using UnityEngine;

public enum CharacterType { NPC, COACH, REFEREE }
public enum SpecialActionKind { None, Bribe, PlantDoping, BuyFlour }
[CreateAssetMenu(menuName = "Dialogue/Character Profile")]
public class CharacterProfile : ScriptableObject
{
    [Header("Identiy/start")]
    public string displayName = "Someone";
    public Sprite img;
    public TextAsset dialogueJson; //Assign the TextAsset
    public string startNodeId;

    [Header("Role/Special action (The 4th button choice)")]
    public CharacterType type = CharacterType.NPC;
    public SpecialActionKind special = SpecialActionKind.None;
    public int cost = 100; //For bribe/buyflour (the cost will be adjusted)
    public string successFlag = Flags.BribedReferee;
}
