using UnityEngine;
using UnityEngine.EventSystems;

public class CursorChange : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CursorState m_newCursorState;

    #region UnityMethods
    void OnDisable()
    {
        CursorController.Instance.UpdateCursor(CursorState.Default);
    }
    #endregion
  
    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorController.Instance.UpdateCursor(m_newCursorState);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorController.Instance.UpdateCursor(CursorState.Default);
    }
}
