using UnityEngine;

public static class DialogueActionResolver
{
    static bool IsCoach(NpcDefinition npc)
    {
        var label = (npc?.displayName ?? npc?.name ?? string.Empty).ToLowerInvariant();
        return label.Contains("Coach");
    }
    public static bool TryResolve(DialogueChoice choice, NpcDefinition npc, out bool success)
    {
        success = false;
        var gm = GameManager.Instance;
        if (gm == null)
            return false;

        var odds = gm.OddsManager;
        if (odds == null)
            return false;
        var player = gm.Player;

        switch (choice.action)
        {
            case "SEEK_INFO":
                success = Random.value <= choice.successChance;
                if (success)
                {
                    odds.RaiseFighterAOdds(0.2f);
                    Debug.Log("[DialogueAction] SEEK_INFO success, fighter A odds increased.");
                }
                else
                {
                    odds.RaiseFighterBOdds(0.2f);
                    Debug.Log("[DialogueAction] SEEK_INFO fail, fighter B odds increased.");
                }
                return true;

            case "BRIBE":
                if (IsCoach(npc))
                {
                    // Coach requires flour. If you don't have it -> auto-fail and DON'T change odds.
                    if (!player.HasFlour)
                    {
                        success = false;
                        Debug.Log("[DialogueAction] BRIBE (Coach) failed: need flour.");
                        return true;
                    }


                    if (player.TryUseFlour())
                    {
                        success = Random.value <= choice.successChance;
                        if (success)
                        {
                            odds.RaiseFighterAOdds(0.35f);
                            Debug.Log("[DialogueAction] BRIBE success,  fighter A odds increased.");
                        }
                        else
                        {
                            odds.RaiseFighterBOdds(0.35f);
                            Debug.Log("[DialogueAction] BRIBE fail, fighter B odds increased.");
                        }
                    }
                    else
                    {
                        success = false;
                        Debug.Log("[DialogueAction] BRIBE (Coach) failed: could not consume flour.");
                    }
                    return true;
                }
                // success = Random.value <= choice.successChance;
                // if (success)
                // {
                //     odds.RaiseFighterAOdds(0.35f);
                //     Debug.Log("[DialogueAction] BRIBE success,  fighter A odds increased.");
                // }
                // else
                // {
                //     odds.RaiseFighterBOdds(0.35f);
                //     Debug.Log("[DialogueAction] BRIBE fail, fighter B odds increased.");
                // }
                success = false;
                return false;

            case "PICKUP_CHIPS":
                if (gm?.Player != null)
                {
                    int coinAmount = Random.Range(10, 30);
                    gm.Player.AddChips(coinAmount);
                    Debug.Log($"[DialogueAction] Player picked up {coinAmount} coins.");
                }
                success = true;
                return true;

            default:
                return false;
        }
    }
}
