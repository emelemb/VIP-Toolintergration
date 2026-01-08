using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class NpcRandomizerTest : MonoBehaviour
{
    public Queue<GameObject> NpcOrder = new();
    [SerializeField] List<ToSpawn> NpcList;
    [SerializeField] bool RandomEventsOn = false;
    [SerializeField] List<RandomEvent> RandomEvents;

    [SerializeField] GameObject EventObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Randomize();
        SpawnNext();
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
                    //NpcOrder.Enqueue(NpcList[x].Npc);
                    spawnedNpcs[x] -= 1;
                    break;
                }
                else
                    odds -= spawnedNpcs[x];
            }

            remainingNpcs -= 1;
        }
    }

    public void SpawnNext()
    {
        if (NpcOrder.Count > 0)
        {
            GameObject newNPC = Instantiate(NpcOrder.Dequeue());

            if (RandomEventsOn)
            {
                float totalWeights = 0;

                foreach (RandomEvent randomEvent in RandomEvents)
                    totalWeights += randomEvent.Odds;

                foreach (RandomEvent randomEvent in RandomEvents)
                {
                    if (Random.Range(0, totalWeights) < randomEvent.Odds)
                    {
                        //newNPC.GetComponent<TestNPC>().OnEnd.AddListener(() => randomEvent.Event.Invoke());
                        break;
                    }

                    totalWeights -= randomEvent.Odds;
                }

            }

            else
                newNPC.GetComponent<TestNPC>().OnEnd.AddListener(SpawnNext);
        }

        else
            Debug.Log("All NPC's spawned");
    }

    public void TestEvent()
    {
        StartCoroutine(Event1());
    }


    IEnumerator Event1()
    {
        GameObject spawnedObject = Instantiate(EventObject);
        yield return new WaitForSeconds(3);
        Destroy(spawnedObject);
        SpawnNext();
    }
}