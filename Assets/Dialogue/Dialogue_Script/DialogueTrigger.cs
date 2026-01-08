using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue File Info")]
    [SerializeField] private string dialogueFile = "npc_gingerbread1.json";
    [SerializeField] private string startNodeId = "";

    [Header("Manager Reference")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private NpcDefinition npcDefinition;

    private bool playerInRange;
    private void Awake()
    {
        if (!dialogueManager)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();
        }
    }
    public void StartConversation(NpcDefinition defOverride = null)
    {
        if (!dialogueManager)
        {
            Debug.LogError("DialogueManager not found in scene!");
            return;
        }

        var def = defOverride ? defOverride : npcDefinition;
        string fileToLoad = def && !string.IsNullOrWhiteSpace(def.dialogueFile) ? def.dialogueFile : dialogueFile;

        if (string.IsNullOrWhiteSpace(fileToLoad))
        {
            Debug.LogError("[DialogueTrigger] No dialogue file specified.");
            return;
        }

        dialogueManager.LoadTreeFromFile(fileToLoad);
        if (def)
        {
            dialogueManager.SetNpc(def);
        }
        dialogueManager.SetOwningNpc(null);
        if (!string.IsNullOrWhiteSpace(startNodeId))
        {
            dialogueManager.StartDialogueAt(startNodeId);
        }
        else
        {
            dialogueManager.StartFromTreeStart();
        }
    }

    // private void Update()
    // {
    //     if (playerInRange && Input.GetKeyDown(KeyCode.E))
    //     {
    //         return;
    //     }
    //     if (dialogueManager == null)
    //     {
    //         Debug.LogError("DialogueManager not found in scene!");
    //         return;
    //     }
    //     string fileToLoad = npcDefinition != null && !string.IsNullOrWhiteSpace(npcDefinition.dialogueFile) ? npcDefinition.dialogueFile : dialogueFile;
    //     if (string.IsNullOrWhiteSpace(fileToLoad))
    //     {
    //         Debug.LogError("[DialogueTrigger] No dialogue file specified on NpcDefinition or fallback field.");
    //         return;
    //     }
    //     dialogueManager.LoadTreeFromFile(fileToLoad);
    //     if (npcDefinition != null)
    //     {
    //         dialogueManager.SetNpc(npcDefinition);
    //     }
    //     var owner = GetComponentInParent<TestNPC>() ?? GetComponent<TestNPC>();
    //     if (owner != null)
    //     {
    //         dialogueManager.SetOwningNpc(owner);
    //     }
    //     if (!string.IsNullOrWhiteSpace(startNodeId))
    //     {
    //         Debug.Log($"Starting dialogue from file: {fileToLoad} at node: {startNodeId}");
    //         dialogueManager.StartDialogueAt(startNodeId);
    //     }
    //     else
    //     {
    //         Debug.Log($"[DialogueTrigger] Starting '{fileToLoad}' from tree.start[0]");
    //         dialogueManager.StartFromTreeStart();
    //     }


    // }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //         playerInRange = true;
    // }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //         playerInRange = false;
    // }
}
