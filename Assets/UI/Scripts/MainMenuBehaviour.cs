using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuBehaviour : MonoBehaviour
{
    [SerializeField] private Button m_startButton;
    [SerializeField] private Button m_quitButton;
    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private Image m_characterImg;

    private LerpHelper m_lerpHelper;
    private CanvasGroup m_menuGroup;
    private FadeAnimator m_fadeAnimator;
    private Vector3 m_targetstartButtonPosition;
    private Vector3 m_targetQuitButtonPosition;
    private Vector3 m_targetTitlePosition;
    private Vector3 m_targetCharacterPosition;

    #region Unity Methods
    private void Start()
    {
        m_lerpHelper = new LerpHelper();
        m_fadeAnimator = GetComponent<FadeAnimator>();
        m_menuGroup = GetComponentInChildren<CanvasGroup>();
        m_targetstartButtonPosition = m_startButton.GetComponent<RectTransform>().localPosition;
        m_targetQuitButtonPosition = m_quitButton.GetComponent<RectTransform>().localPosition;
        m_targetTitlePosition = m_titleText.GetComponent<RectTransform>().localPosition;
        m_targetCharacterPosition = m_characterImg.GetComponent<RectTransform>().localPosition;

        InitUIComponent(m_startButton.gameObject, 1000);
        InitUIComponent(m_quitButton.gameObject, 1000);
        InitUIComponent(m_titleText.gameObject, 1000);
        InitUIComponent(m_characterImg.gameObject, -1200);
        m_menuGroup.alpha = 0.0f;
        m_fadeAnimator.FadeIn(m_menuGroup, 1f);
        StartCoroutine(AnimateMenuUI(1f));
    }
    #endregion

    public void StartGame()
    {
        if (Random.Range(0, 10) == 0)
        {
            SceneManager.LoadScene("CharacterTest");
        }
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void InitUIComponent(GameObject uiComponent, float value)
    {
        Vector3 newPos = uiComponent.transform.localPosition;
        newPos.x += value;
        uiComponent.transform.localPosition = newPos;
    }

    IEnumerator AnimateMenuUI(float duration)
    {
        float elapsedTime = 0f;

        Vector3 startButtonStartPos = m_startButton.transform.localPosition;
        Vector3 quitButtonStartPos = m_quitButton.transform.localPosition;
        Vector3 titleStartPos = m_titleText.transform.localPosition;
        Vector3 characterStartPos = m_characterImg.transform.localPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            m_lerpHelper.LerpToTargetPosition(m_startButton.gameObject, startButtonStartPos, m_targetstartButtonPosition, t);
            m_lerpHelper.LerpToTargetPosition(m_quitButton.gameObject, quitButtonStartPos, m_targetQuitButtonPosition, t);
            m_lerpHelper.LerpToTargetPosition(m_titleText.gameObject, titleStartPos, m_targetTitlePosition, t);
            m_lerpHelper.LerpToTargetPosition(m_characterImg.gameObject, characterStartPos, m_targetCharacterPosition, t);
            yield return null;
        }

        m_lerpHelper.SetFinalPosition(m_startButton.gameObject, m_targetstartButtonPosition);
        m_lerpHelper.SetFinalPosition(m_quitButton.gameObject, m_targetQuitButtonPosition);
        m_lerpHelper.SetFinalPosition(m_titleText.gameObject, m_targetTitlePosition);
        m_lerpHelper.SetFinalPosition(m_characterImg.gameObject, m_targetCharacterPosition);
    }
}