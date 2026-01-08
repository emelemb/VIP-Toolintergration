using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Randomizer : MonoBehaviour
{
    // Potentially replace Game Object with npc object when it is implemented
    public Queue<NpcDefinition> NpcOrder = new();
    [SerializeField] List<ToSpawn> NpcList;
    [SerializeField] List<RandomEvent> RandomEvents;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void Randomize()
    {
        NpcOrder.Clear();

        int remainingNpcs = 0;

        foreach (ToSpawn toSpawn in NpcList)
            remainingNpcs += toSpawn.Amount;

        List<int> spawnedNpcs = NpcList.Select(n => n.Amount).ToList();

        while (remainingNpcs > 0)
        {
            int odds = remainingNpcs;

            for (int x = 0; x < spawnedNpcs.Count; x++)
            {
                if (Random.Range(0, odds) < spawnedNpcs[x])
                {
                    NpcOrder.Enqueue(NpcList[x].Npc);
                    spawnedNpcs[x] -= 1;
                    break;
                }
                else
                    odds -= spawnedNpcs[x];
            }

            remainingNpcs -= 1;
        }
    }

    public NpcDefinition GetNextNpc()
    {
        if (NpcOrder.Count > 0)
            return NpcOrder.Dequeue();

        else
        {
            Debug.Log("All NPC's spawned");
            return null;
        }
    }

    public NpcDefinition GetRandomEvent()
    {
        float totalWeights = 0;

        foreach (RandomEvent randomEvent in RandomEvents)
            totalWeights += randomEvent.Odds;

        foreach (RandomEvent randomEvent in RandomEvents)
        {
            if (Random.Range(0, totalWeights) < randomEvent.Odds)
            {
                return randomEvent.Event;
            }

            totalWeights -= randomEvent.Odds;
        }

        return null;
    }

    public void SetNpcs(List<ToSpawn> newNpcs)
    {
        NpcOrder.Clear();
        NpcList.Clear();
        NpcList = newNpcs;
        Randomize();
    }
}

[Serializable]
public class ToSpawn
{
    public NpcDefinition Npc;
    public int Amount;
}

[Serializable]
public class RandomEvent
{
    public NpcDefinition Event = null;
    public float Odds;
}