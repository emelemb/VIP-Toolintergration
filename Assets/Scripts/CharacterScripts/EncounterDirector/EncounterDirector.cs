using System.Collections;
using System.Linq;
using UnityEngine;

public class EncounterDirector : MonoBehaviour
{
    [SerializeField] private CharacterSlot slot;
    [SerializeField] private Randomizer randomizer;
    [SerializeField] private EventRunner eventRunner;
    [Header("Phase cap")]
    [SerializeField] private int maxNpcsPerSpeaking = 3;
    private int shownThisPhase = 0;
    private bool endingSpeaking = false; // prevent double-end
    // [ContextMenu("Print the NPC queue (preview)")]
    // public void PrintQueuePreview()
    // {
    //     if (randomizer == null)
    //     {
    //         Debug.LogWarning("EncounterDirector: Randomizer ref is missing.");
    //         return;
    //     }

    //     var arr = randomizer.NpcOrder.ToArray(); // safe: does NOT dequeue
    //     if (arr.Length == 0)
    //     {
    //         Debug.Log("[Randomizer] Queue is EMPTY. Did you call SetNpcs() yet?");
    //         return;
    //     }

    //     string Label(NpcDefinition n) =>
    //         n == null ? "<null>" :
    //         !string.IsNullOrWhiteSpace(n.displayName) ? n.displayName :
    //         !string.IsNullOrWhiteSpace(n.npcId) ? n.npcId : n.name;

    //     var names = arr.Select(Label);
    //     Debug.Log("[Randomizer] Today's NPC queue:\n - " + string.Join("\n - ", names));
    // }
    public void BeginSpeakingPhase()
    {
        shownThisPhase = 0;
        endingSpeaking = false;
        Debug.Log("[ED] BeginSpeakingPhase()");
    }
    public void StartNpcEncounter()
    {
        if (!slot || randomizer == null)
        {
            Debug.LogWarning("[ED] missing refs");
            return;
        }
        if (endingSpeaking) { Debug.Log("[ED] already ending"); return; }

        if (shownThisPhase >= maxNpcsPerSpeaking)
        {
            Debug.Log("[ED] cap reached -> EndSpeakingPhase()");
            StartCoroutine(EndSpeakingPhase());
            return;
        }

        var def = randomizer.GetNextNpc();
        if (def == null)
        {
            Debug.Log("[ED] no more NPCs -> EndSpeakingPhase()");
            StartCoroutine(EndSpeakingPhase());
            return;
        }

        shownThisPhase++;
        Debug.Log($"[ED] Show #{shownThisPhase}: {def.displayName ?? def.name}");
        slot.Present(def);
    }
    public void FinishNpcAndEvent()
    {
        if (endingSpeaking) return;

        var ev = randomizer.GetRandomEvent();
        if (ev is EventNpcAdapter && eventRunner != null)
        {
            eventRunner.Execute(ev);
        }

        // Try to fetch the next NPC of THIS phase (cap respected in StartNpcEncounter)
        StartNpcEncounter();


        // var ev = randomizer.GetRandomEvent();
        // if (ev is EventNpcAdapter && eventRunner != null)
        // {
        //     eventRunner.Execute(ev);
        // }
        // if (randomizer != null && randomizer.NpcOrder.Count > 0)
        // {
        //     StartNpcEncounter();
        // }
        // else
        // {
        //     GameManager.Instance.MoveToNextPhase();
        // }
    }
    // public bool HasMoreNpcsToday() => randomizer && randomizer.NpcOrder.Count > 0;
    private System.Collections.IEnumerator EndSpeakingPhase()
    {
        if (endingSpeaking) yield break;
        endingSpeaking = true;
        Debug.Log("[ED] EndSpeakingPhase() -> Close curtains, then MoveToNextPhase()");

        if (slot != null)
            yield return slot.CloseCurtainsAndWait();

        GameManager.Instance.MoveToNextPhase(); // SpeakingToNPC -> RoundEnd
    }
}
