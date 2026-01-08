using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeightedDef
{
    public NpcDefinition def;
    [Min(0f)] public float weight = 1f;

    [Min(0f)] public int cooldownRounds = 0;
    [NonSerialized] public int nextAllowedRound = 0;
}
public class NpcRandomizer : MonoBehaviour
{
    [SerializeField] private List<WeightedDef> table = new List<WeightedDef>();
    [SerializeField] private bool avoidRepeat = true;
    public int RoundIndex { get; private set; } = 0;
    private NpcDefinition lastPicked;
    public void AdvanceRound() => RoundIndex++; //Calls each time encounter completes, before picking a new one. 
    //Picking a character definition by checking weights
    public NpcDefinition PickDefinition()
    {
        if (table == null || table.Count == 0)
        {
            return null;
        }
        //
        var candits = new List<WeightedDef>();
        foreach (var t in table)
        {
            if (t.def == null)
            {
                continue;
            }
            if (t.weight <= 0f)
            {
                continue;
            }
            //Cooldown
            if (RoundIndex < t.nextAllowedRound)
            {
                continue;
            }
            if (avoidRepeat && lastPicked != null && t.def == lastPicked)
            {
                continue;
            }
            candits.Add(t);
        }
        if (candits.Count == 0)
        {
            foreach (var w in table)
            {
                if (w.def != null && w.weight > 0f)
                {
                    candits.Add(w);
                }
            }
        }
        if (candits.Count == 0)
        {
            return null;
        }
        //rolls
        float total = 0f;
        foreach (var c in candits)
        {
            total += c.weight;
        }
        float r = UnityEngine.Random.value * total;
        foreach (var c in candits)
        {
            r -= c.weight;
            if (r <= 0f)
            {
                if (c.cooldownRounds > 0) //COoldown for rounds, which npc's can approach.
                {
                    c.nextAllowedRound = RoundIndex + c.cooldownRounds;
                }
                lastPicked = c.def;
                return c.def;
            }
        }
        lastPicked = candits[candits.Count - 1].def;
        Debug.Log($"The picked characeter is: {lastPicked}");
        return lastPicked;

    }
}
