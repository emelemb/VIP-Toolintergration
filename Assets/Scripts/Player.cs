using UnityEngine;

public class Player : MonoBehaviour
{
    Fighter selectedFighter;
    int chips = 100;
    private bool hasFlour = false;
    [SerializeField] private GameObject flourObject;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        GameManager.Instance.Player = this;
        Debug.Log(GameManager.Instance.Player);
    }

    public Fighter GetSelectedFighter() { return selectedFighter; }
    public string GetSelectedFighterName() { return selectedFighter.Name; }
    public void SetSelectedFigher(Fighter fighter)
    {
        selectedFighter = fighter;
        Debug.Log("Selected fighter is: " + selectedFighter.Name);
        GameManager.Instance.MoveToNextPhase();
    }

    public int GetChips() { return chips; }

    public void RemoveChips(int chipsToRemove)
    {
        chips -= chipsToRemove;
    }

    public void AddChips(int chipsToAdd)
    {
        chips += chipsToAdd;
    }
    //single flour slot
    public bool HasFlour => hasFlour;
    public bool TryBuyFlour(int price)
    {
        price = Mathf.Max(0, price);
        if (hasFlour)
        {
            return false;
        }
        if (chips < price)
        {
            return false;
        }
        chips -= price;
        hasFlour = true;
        RefreshFlourVisual();
        Debug.Log($"[Player] Bought flour for {price}. Chips now {chips}.");
        return true;
    }
    public bool TryUseFlour()
    {
        if (!hasFlour)
        {
            return false;
        }
        hasFlour = false;
        RefreshFlourVisual();
        Debug.Log("[Player] Used flour.");
        return true;
    }
    public void GiveFlourFree()
    {
        hasFlour = true;
        RefreshFlourVisual();
        Debug.Log("[Player] Received flour (free).");
    }
    public void TakeFlourIfAny()
    {
        if (!hasFlour)
        {
            return;
        }
        hasFlour = false;
        RefreshFlourVisual();
        Debug.Log("[Player] Flour removed.");
    }
    private void RefreshFlourVisual()
    {
        if (flourObject != null)
        {
            flourObject.SetActive(hasFlour);
        }
    }

}
