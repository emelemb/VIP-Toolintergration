using System.Collections;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

public class TestNPC : MonoBehaviour
{
    [SerializeField] float LifeTime;
    public UnityEvent OnEnd;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        yield return new WaitForSeconds(0.1f);
        GetComponent<SpriteRenderer>().enabled = true;
        yield return new WaitForSeconds(LifeTime);
        OnEnd.Invoke();
        Destroy(gameObject);
    }
}
