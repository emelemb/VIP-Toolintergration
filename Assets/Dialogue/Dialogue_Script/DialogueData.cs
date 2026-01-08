using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueTree
{
    public List<string> start;
    public List<DialogueNode> nodes;

    public DialogueNode GetNode(string id)
    {
        return nodes.Find(n => n.id == id);
    }

    public static DialogueTree FromJson(string json)
    {
        return JsonUtility.FromJson<DialogueTree>(json);
    }
}

[System.Serializable]
public class DialogueNode
{
    public string id;
    public string speaker;
    public string text;
    public string action;
    public int reducedAmount = 20;
    public int amount = 0;
    public List<DialogueChoice> choices;
}

[System.Serializable]
public class DialogueChoice
{
    public string id;
    public string text;
    public string gotoNode;
    public string action;
    public float successChance;
    public string gotoOnSuccess;
    public string gotoOnFail;
}
