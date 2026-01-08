using UnityEngine;

public enum EventKind { None, MoneyDelta, GiveFlour, TakeFlour, InfoPopup }
[CreateAssetMenu(menuName = "Events/Event Definition")]
public class EventDefinition : ScriptableObject
{
    [Header("Type")]
    public EventKind kind = EventKind.None;

    [Header("Values")]
    public int moneyDelta = 0;
    [TextArea]
    public string infoMessage = "";
}
