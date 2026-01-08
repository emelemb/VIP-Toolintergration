using System.IO;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using BTAI;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public struct ChoiceSlot
    {
        public GameObject root;
        public Button button;
        public TextMeshProUGUI label;
    }
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI npcText;
    //event
    public event System.Action DialogueEnded;
    // [SerializeField] private TextMeshProUGUI speakerText;
    // [SerializeField] private TextMeshProUGUI dialogueText;
    // [SerializeField] private GameObject choiceButtonPrefab;
    // [SerializeField] private Transform choiceContainer;
    //BOMBA NEW DON'T TOUCH PRETTY PLEASE
    [SerializeField]
    private ChoiceSlot[] choiceSlots = new ChoiceSlot[4];
    [Header("Context")]
    [SerializeField] private NpcDefinition npc;
    private TestNPC owningNpc;
    private bool isDSwapFirstAndThird = true;
    private DialogueTree currentTree;
    private DialogueNode currentNode;

    private Dictionary<string, DialogueNode> byId;

    public void LoadTreeFromFile(string filename)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "Dialogues", filename);

        if (!File.Exists(path))
        {
            Debug.LogError("Dialogue file not found at: " + path);
            return;
        }

        string json = File.ReadAllText(path);
        currentTree = DialogueTree.FromJson(json);
        BuildIndex(currentTree);
        Debug.Log($"Dialogue tree loaded from: {filename}");
    }

    public void StartDialogueAt(string nodeId)
    {
        if (currentTree == null)
        {
            Debug.LogError("No dialogue tree loaded!");
            return;
        }
        // var gm = GameManager.Instance;
        // if (gm != null)
        // {
        //     gm.Phase = GameManager.GamePhase.SpeakingToNPC;
        // }
        dialoguePanel.SetActive(true);
        ShowNode(currentTree.GetNode(nodeId));
    }

    private void ShowNode(DialogueNode node)
    {
        if (node == null)
        {
            EndDialogue();
            return;
        }
        currentNode = node;
        //
        if (npcText)
        {
            string textToShow = node.text ?? string.Empty;

            // Inject fighter names
            var odds = GameManager.Instance?.OddsManager;
            if (odds != null)
            {
                textToShow = textToShow
                    .Replace("{fighterA}", odds.GetFighterA?.Name ?? "Fighter A")
                    .Replace("{fighterB}", odds.GetFighterB?.Name ?? "Fighter B");
            }

            npcText.text = textToShow;
        }

        if (!string.IsNullOrEmpty(node.action))
        {
            switch (node.action)
            {
                case "remove_chips":
                    GameManager.Instance.Player.RemoveChips(node.reducedAmount);
                    Debug.Log($"[Dialogue] Removed {node.reducedAmount} chips from player.");
                    break;

                case "buy_flour":
                    {
                        var player = GameManager.Instance.Player;
                        // Prefer node.amount if present; else fall back to reducedAmount (back-compat)
                        int price = (node.amount > 0) ? node.amount : Mathf.Max(0, node.reducedAmount);

                        bool bought = player.TryBuyFlour(price); // sets hasFlour + updates visual
                        if (bought)
                            Debug.Log($"[Dialogue] Bought flour for {price} chips.");
                        else
                            Debug.Log("[Dialogue] Could not buy flour (already have it or insufficient chips).");
                        break;
                    }

                default:
                    Debug.LogWarning($"[Dialogue] Unknown node action: {node.action}");
                    break;
            }
        }

        //
        for (int i = 0; i < choiceSlots.Length; i++)
        {
            var s = choiceSlots[i];
            if (s.root)
            {
                s.root.SetActive(false);
            }
            if (s.button)
            {
                s.button.onClick.RemoveAllListeners();
            }
        }




        var choices = node.choices ?? new List<DialogueChoice>();
        int count = Mathf.Min(choices.Count, choiceSlots.Length);

        for (int i = 0; i < count; i++)
        {
            var slot = choiceSlots[i];
            var displayedChoice = choices[i]; // what the player SEES

            // Show label as usual
            if (slot.label) slot.label.text = displayedChoice.text ?? string.Empty;
            if (slot.root) slot.root.SetActive(true);

            if (slot.button)
            {
                slot.button.onClick.RemoveAllListeners();

                // But wire to the (maybe swapped) underlying choice
                int actualIndex = MapIndexWithDefect(i, count);
                var actualChoice = choices[actualIndex];

                // capture locals to avoid closure gotchas
                var localChoice = actualChoice;

                slot.button.onClick.AddListener(() =>
                {
                    // Actioned choices
                    if (!string.IsNullOrEmpty(localChoice.action))
                    {
                        bool handled = DialogueActionResolver.TryResolve(localChoice, npc, out bool success);
                        if (handled)
                        {
                            var next = GetNode(success ? localChoice.gotoOnSuccess : localChoice.gotoOnFail);
                            if (next != null) { ShowNode(next); return; }
                            EndDialogue(); return;
                        }

                        // Fallback random
                        bool fallback = Random.value <= localChoice.successChance;
                        var nextFallback = GetNode(fallback ? localChoice.gotoOnSuccess : localChoice.gotoOnFail);
                        if (nextFallback != null) { ShowNode(nextFallback); return; }
                        EndDialogue(); return;
                    }

                    // Plain goto
                    if (!string.IsNullOrEmpty(localChoice.gotoNode))
                    {
                        var next = GetNode(localChoice.gotoNode);
                        if (next != null) { ShowNode(next); return; }
                    }

                    EndDialogue();
                });
            }
        }

        // hide extra slots
        for (int i = count; i < choiceSlots.Length; i++)
        {
            var s = choiceSlots[i];
            if (s.root) s.root.SetActive(false);
            if (s.button) s.button.onClick.RemoveAllListeners();
        }
        if (count == 0) EndDialogue();
    }

    private void OnChoiceSelected(DialogueChoice choice)
    {
        //Pref resolver for actioned choices (SEEK_INFO / BRIBE_ / BUY)
        if (!string.IsNullOrEmpty(choice.action))
        {
            bool handled = DialogueActionResolver.TryResolve(choice, npc, out bool success);
            if (handled)
            {
                // string nextId = success ? choice.gotoOnSuccess : choice.gotoOnFail;
                var next = GetNode(success ? choice.gotoOnSuccess : choice.gotoOnFail);
                if (next != null) { ShowNode(next); return; }
                EndDialogue(); return;
            }

            // Unknown action? fall back to your old simple roll:
            Debug.Log($"[Dialogue] Unhandled action '{choice.action}' — using fallback roll.");
            bool fallback = Random.value <= choice.successChance;
            var nextFallback = GetNode(fallback ? choice.gotoOnSuccess : choice.gotoOnFail);
            if (nextFallback != null) { ShowNode(nextFallback); return; }
            EndDialogue(); return;
        }
        //Plain goto
        if (!string.IsNullOrEmpty(choice.gotoNode))
        {
            var next = GetNode(choice.gotoNode);
            if (next != null)
            {
                ShowNode(next); return;
            }
        }
        // nav => end
        EndDialogue();


        // //mostly for testing, but works with the game as well
        // if (!string.IsNullOrEmpty(choice.action))
        // {
        //     Debug.Log("Action triggered: " + choice.action);
        //     bool success = Random.value <= choice.successChance;

        //     string nextNode = success ? choice.gotoOnSuccess : choice.gotoOnFail;
        //     DialogueNode next = currentTree.GetNode(nextNode);

        //     if (next != null)
        //         ShowNode(next);
        //     else
        //         EndDialogue();
        // }
        // else if (!string.IsNullOrEmpty(choice.gotoNode))
        // {
        //     DialogueNode next = currentTree.GetNode(choice.gotoNode);
        //     if (next != null)
        //         ShowNode(next);
        //     else
        //         EndDialogue();
        // }
        // else
        // {
        //     EndDialogue();
        // }
    }

    public void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentNode = null;

        // //back to loop
        // var gm = GameManager.Instance;
        // if (gm != null)
        // {
        //     gm.MoveToNextPhase();
        // }
        // owningNpc?.OnEnd?.Invoke();
        owningNpc?.OnEnd?.Invoke();
        DialogueEnded?.Invoke();
    }

    //NEW added by el Gustaf
    private void BuildIndex(DialogueTree t)
    {
        byId = new Dictionary<string, DialogueNode>();
        if (t?.nodes == null)
        {
            return;
        }
        foreach (var n in t.nodes)
        {
            if (!string.IsNullOrEmpty(n.id))
            {
                byId[n.id] = n;
            }
        }
    }
    public void SetNpc(NpcDefinition def) => npc = def;
    public void SetOwningNpc(TestNPC testNpc) => owningNpc = testNpc;
    private DialogueNode GetNode(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        if (byId != null && byId.TryGetValue(id, out var n)) return n;
        // Fallback to the tree's own lookup if you have one
        return currentTree.GetNode(id);
    }
    private int MapIndexWithDefect(int displayedIndex, int totalChoices)
    {
        if (!isDSwapFirstAndThird) return displayedIndex;
        if (totalChoices >= 3)
        {
            if (displayedIndex == 0) return 2; // button 1 runs choice #3
            if (displayedIndex == 2) return 0; // button 3 runs choice #1
        }
        return displayedIndex;
    }
    public void StartFromTreeStart()
    {
        if (currentTree?.start != null && currentTree.start.Count > 0)
        {
            StartDialogueAt(currentTree.start[0]);
        }
        else
        {
            Debug.LogError("[DialogueManager] No start node defined in dialogue tree.");
        }
    }
}
