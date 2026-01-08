using UnityEngine;

public class Fighter
{
    public string Name { get; set; }
    bool hasWon;

    public Fighter(string name)
    {
        Name = name;
    }

    public void SetAsWinner()
    {
        hasWon = true;
        Debug.Log("And the winner is: " + Name);
    }

    public bool IsWinner()
    {
        return hasWon;
    }

    public void Reset()
    {
        hasWon = false;
    }

}
