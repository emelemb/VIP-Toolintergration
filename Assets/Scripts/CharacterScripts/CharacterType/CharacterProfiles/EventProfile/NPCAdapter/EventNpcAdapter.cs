using UnityEngine;
[CreateAssetMenu(menuName = "Events/Event Adapter (for Randomizer)")]
public class EventNpcAdapter : NpcDefinition
{
    [Header("Event Link")]
    public EventDefinition eventAsset;

    public override string ToString()
    {
        return $"[EventAdapter] {(eventAsset ? eventAsset.name : "NULL")}";
    }
}
