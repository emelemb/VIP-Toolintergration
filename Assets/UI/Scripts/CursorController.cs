using UnityEngine;

public enum CursorState
{
    Default,
    ClickableObject,
    NPC
}

public class CursorController : MonoBehaviour
{
    public static CursorController Instance;
    [SerializeField] private Texture2D m_defaultTexture;
    [SerializeField] private Texture2D m_clickableObjectTexture;
    [SerializeField] private Texture2D m_npcObjectTexture;
    private Vector2 m_clickPosition;

    #region Unity Methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        UpdateCursor(CursorState.Default);
    }
    #endregion

    public void UpdateCursor(CursorState state)
    {
        switch (state)
        {
            case CursorState.Default:
                Cursor.SetCursor(m_defaultTexture, m_clickPosition, CursorMode.Auto);
                break;
            case CursorState.ClickableObject:
                Cursor.SetCursor(m_clickableObjectTexture, m_clickPosition, CursorMode.Auto);
                break;
            case CursorState.NPC:
                Cursor.SetCursor(m_npcObjectTexture, m_clickPosition, CursorMode.Auto);
                break;
            default:
                Cursor.SetCursor(m_defaultTexture, m_clickPosition, CursorMode.Auto);
                break;
        }
        Cursor.lockState = CursorLockMode.Confined;
    }
}
