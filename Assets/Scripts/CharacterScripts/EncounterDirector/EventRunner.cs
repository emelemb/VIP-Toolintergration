using UnityEngine;

public class EventRunner : MonoBehaviour
{
    [SerializeField] private GameManager gm;

    public void Execute(NpcDefinition def)
    {
        if (def == null) return;

        if (gm == null) gm = GameManager.Instance;
        if (gm == null) return;

        // Only proceed if this NpcDefinition is actually an Event adapter
        var adapter = def as EventNpcAdapter;
        if (adapter == null || adapter.eventAsset == null) return;

        var ev = adapter.eventAsset;

        var player = gm.Player;
        // var odds = gm.OddsManager;

        switch (ev.kind)
        {
            case EventKind.MoneyDelta:
                if (player != null && ev.moneyDelta != 0)
                {
                    if (ev.moneyDelta > 0)
                    {
                        Debug.LogWarning($"[Event] Positive MoneyDelta {ev.moneyDelta} ignored (temp rule to avoid free money).");
                        break;
                    }
                    player.AddChips(ev.moneyDelta); // negative values reduce chips
                }
                break;

            case EventKind.GiveFlour:
                if (player != null) player.GiveFlourFree();
                break;

            case EventKind.TakeFlour:
                if (player != null) player.TakeFlourIfAny();
                break;

            case EventKind.InfoPopup:
                if (!string.IsNullOrEmpty(ev.infoMessage))
                    Debug.Log($"[Event Info] {ev.infoMessage}");
                break;

            case EventKind.None:
            default:
                // no event
                break;
        }
    }

    // private void ApplyOddsDelta(OddsManager odds, float delta)
    // {
    //     // Your OddsManager semantics:
    //     // - RaiseFighterAOdds(x): moves odds toward 0 (buff A)
    //     // - RaiseFighterBOdds(x): moves odds toward 1 (buff B)
    //     if (delta > 0f) odds.RaiseFighterAOdds(delta);
    //     else if (delta < 0f) odds.RaiseFighterBOdds(-delta);
    // }
}
