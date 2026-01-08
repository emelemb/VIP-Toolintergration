using UnityEngine;

public class LerpHelper
{
    public void LerpToTargetPosition(GameObject uiComponent, Vector3 startPos, Vector3 targetPos, float t)
    {
        Vector3 currentPos = startPos;
        currentPos.x = Mathf.Lerp(currentPos.x, targetPos.x, t);
        currentPos.y = Mathf.Lerp(currentPos.y, targetPos.y, t);
        uiComponent.transform.localPosition = currentPos;
    }

    public void SetFinalPosition(GameObject uiComponent, Vector3 targetPos)
    {
        Vector3 finalPos = uiComponent.transform.localPosition;
        finalPos.x = targetPos.x;
        finalPos.y = targetPos.y;
        uiComponent.transform.localPosition = finalPos;
    }
}
