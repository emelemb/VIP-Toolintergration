using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterSlot : MonoBehaviour
{
    [Header("Curtains")]
    [SerializeField] CurtainsController curtains;
    [Header("Dialogue")]
    [SerializeField] DialogueManager dialogueManager;
    [Header("Stage")]
    [SerializeField] Transform characterRoot;
    [SerializeField] SpriteRenderer characterRenderer;
    [Header("Refs")]
    [SerializeField] private EncounterDirector encounterDirector;

    NpcDefinition activeDef;
    public void Present(NpcDefinition def, string startNodeId = "")
    {
        Debug.Log($"[Slot#{GetInstanceID()}] Present {def?.displayName ?? def?.name}");
        StartCoroutine(DoPresent(def, startNodeId));
    }
    IEnumerator DoPresent(NpcDefinition def, string nodeId)
    {
        activeDef = def;
        if (characterRenderer)
        {
            characterRenderer.sprite = def.characterSprite;
            characterRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }

        if (curtains) yield return curtains.Open();

        if (dialogueManager != null)
        {
            dialogueManager.DialogueEnded -= OnDialogueEnded;
            dialogueManager.DialogueEnded += OnDialogueEnded;

            dialogueManager.LoadTreeFromFile(def.dialogueFile);
            dialogueManager.SetNpc(def);
            dialogueManager.SetOwningNpc(null);

            if (!string.IsNullOrWhiteSpace(nodeId))
                dialogueManager.StartDialogueAt(nodeId);
            else
                dialogueManager.StartFromTreeStart();
        }
    }
    private void OnDialogueEnded()
    {
        Debug.Log($"[Slot#{GetInstanceID()}] DialogueEnded()");
        if (dialogueManager != null)
            dialogueManager.DialogueEnded -= OnDialogueEnded;

        // DO NOT close here; EncounterDirector decides when to end the speaking phase.
        encounterDirector?.FinishNpcAndEvent();
    }
    public void CloseCurtainsNow()
    {
        if (gameObject.activeInHierarchy && curtains)
        {
            StartCoroutine(curtains.Close());
        }
    }
    public IEnumerator CloseCurtainsAndWait()
    {
        if (gameObject.activeInHierarchy && curtains)
        {
            // Allow end-of-speaking to close even while Phase is SpeakingToNPC.
            yield return curtains.Close();
        }
    }
}
