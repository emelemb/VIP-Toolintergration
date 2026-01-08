using System.Collections;
using System.Security;
using UnityEngine;

public class CurtainsController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float openClipLenght = 1f;
    [SerializeField] private float closeClipLenght = 1f;

    [SerializeField] private string openTrigger = "Open";
    [SerializeField] private string closeTrigger = "Close";
    // Safety latch: prevent close for a minimal time after opening
    [SerializeField] private float minOpenVisibleSeconds = 0.15f;
    private float lastOpenedAt = -999f;
    [SerializeField] string openIdleState = "Base Layer.OpenIdle";
    [SerializeField] string closedIdleState = "Base Layer.ClosedIdle";

    public IEnumerator Open()
    {
        if (!animator) yield break;
        Debug.Log($"[Curtains#{GetInstanceID()}] Open()");
        animator.ResetTrigger(closeTrigger);
        animator.SetTrigger(openTrigger);
        lastOpenedAt = Time.time;
        yield return new WaitForSeconds(openClipLenght);
        // Force settle open:
        animator.Play(openIdleState, 0, 0f);
    }
    public IEnumerator Close()
    {
        if (!animator) yield break;
        float dt = Time.time - lastOpenedAt;
        if (dt < minOpenVisibleSeconds)
        {
            float wait = minOpenVisibleSeconds - dt;
            Debug.Log($"[Curtains#{GetInstanceID()}] Close() requested too soon; delaying {wait:0.000}s");
            yield return new WaitForSeconds(wait);
        }
        Debug.Log($"[Curtains#{GetInstanceID()}] Close()");
        animator.ResetTrigger(openTrigger);
        animator.SetTrigger(closeTrigger);
        yield return new WaitForSeconds(closeClipLenght);
        // Force settle closed:
        animator.Play(closedIdleState, 0, 0f);
    }
}
