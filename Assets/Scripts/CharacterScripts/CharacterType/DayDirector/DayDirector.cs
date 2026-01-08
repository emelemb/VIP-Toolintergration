using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DayDirector : MonoBehaviour
{
    [SerializeField] private Randomizer randomizer;
    public void SetupDay(GameManager.GameDay day)
    {
        if (!randomizer) { Debug.LogWarning("DayDirector: Randomizer missing"); return; }

        // Build the queue from the Inspector-filled NpcList
        randomizer.Randomize(); // uses Randomizer.NpcList internally

        // Preview the exact order (safe: ToArray() does not dequeue)
        var names = randomizer.NpcOrder.ToArray()
            .Select(n => n ?
                (!string.IsNullOrWhiteSpace(n.displayName) ? n.displayName :
                 !string.IsNullOrWhiteSpace(n.npcId) ? n.npcId : n.name)
                : "<null>");
        Debug.Log("[Randomizer] Today's NPC queue:\n - " + string.Join("\n - ", names));
    }
}
