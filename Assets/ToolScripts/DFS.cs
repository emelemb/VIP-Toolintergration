//Author: William Friis
using UnityEngine;

#nullable enable
public class TestNode //Class as to be nullable, as well as default ref instead of copy
{
    public bool Visited;
    public string Dialogue;
    public TestNode[]? Children;
}

public class DFS : MonoBehaviour
{
    public uint Iterations;
    public string Goal;
    
    private TestNode treeRoot;
    private void Start()
    {
        treeRoot = new TestNode { Visited = false, Dialogue = "root" };

        CreateTestTree(treeRoot, 0, Iterations);

        TestNode result = Search(treeRoot, Goal);

        if (result == null)
            Debug.Log("Unable to find goal");
        else
            Debug.Log($"Goal found at {result.Dialogue}");
    }

    public TestNode Search(TestNode root, string goal)
    {
        TestNode result;

        root.Visited = true;

        if (root.Dialogue == goal)
            return root;

        for (int i = 0; i < root.Children.Length; i++)
        {
            if (root.Children[i] == null) return null;

            if (root.Children[i].Dialogue == goal)
                result = root.Children[i];
            else
                result = Search(root.Children[i], goal);

            if (result != null) return result;
        }

        return null;
    }
    public void CreateTestTree(TestNode currNode, int iterationCounter, uint iterationMax)
    {
        currNode.Children = new TestNode[2];
        iterationCounter++;

        if (iterationCounter > iterationMax)
        {
            for (int i = 0; i < currNode.Children.Length; i++)
            {
                currNode.Children[i] = null;
            }
        }
        else
        {
            for (int i = 0; i < currNode.Children.Length; i++)
            {
                currNode.Children[i] = new TestNode { Visited = false, Dialogue = $"Node level: {iterationCounter}" };
                CreateTestTree(currNode.Children[i], iterationCounter, iterationMax);
            }
        }
    }

    public void Reset(TestNode root, bool isTreeRoot)
    {
        if(!isTreeRoot)
            root.Visited = false;

        for(int i = 0; i < root.Children.Length; i++)
        {
            if (root.Children[i] != null)
                Reset(root.Children[i], false);
        }
    }
}


