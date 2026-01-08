using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FightMenuBehaviour : MonoBehaviour
{
    [SerializeField] private Button m_startButton;
    [SerializeField] private GameObject m_overlay;
    [SerializeField] private GameObject m_oddsManagerObj;
    [SerializeField] private GameObject m_messageBoxObj;
    [SerializeField] private GameObject m_parentObj;
    [SerializeField] private Image m_fighterA;
    [SerializeField] private Image m_fighterB;
    [SerializeField] private Button m_returnToGameMenuButton;
    [SerializeField] private Sprite m_butterBusterSprite;
    [SerializeField] private Sprite m_leCroissantSprite;
    [SerializeField] private Sprite m_masterCupcakeSprite;
    [SerializeField] private Sprite m_doughAndNoughtSprite;
    [SerializeField] private float fightDuration;
    [SerializeField] private float durationToFightStart;
    [SerializeField] private float fightInitDuration;
    [SerializeField] private Image m_smoke;
    [SerializeField] private Sprite m_trollSprite;

    public delegate void FightStartHandler(float durationToFightStart, Fighter fighterA, Fighter fighterB);
    public event FightStartHandler OnFightStarted;

    private LerpHelper m_lerpHelper;
    private OddsManager m_oddsManager;
    private FadeAnimator m_fadeAnimator;
    private Vector3 m_fighterAStartPosition;
    private Vector3 m_fighterBStartPosition;
    private Vector3 m_parentTargetPosition;
    private CanvasGroup m_overlayGroup;
    private TextMeshProUGUI m_messageBoxText;

    #region Unity Methods
    void Start()
    {
        m_lerpHelper = new LerpHelper();
        m_oddsManager = m_oddsManagerObj.GetComponent<OddsManager>();
        m_fighterAStartPosition = m_fighterA.GetComponent<RectTransform>().localPosition;
        m_fighterBStartPosition = m_fighterB.GetComponent<RectTransform>().localPosition;
        m_parentTargetPosition = m_parentObj.transform.localPosition;
        m_overlayGroup = m_overlay.GetComponent<CanvasGroup>();
        m_messageBoxText = m_messageBoxObj.GetComponentInChildren<TextMeshProUGUI>();
        m_startButton.onClick.AddListener(Init);
        m_startButton.onClick.AddListener(ResetFightMenu);
        m_returnToGameMenuButton.onClick.AddListener(ResetFightMenu);
        //m_returnToGameMenuButton.onClick.AddListener(m_oddsManager.OnRoundEnd);
        m_fadeAnimator = GetComponent<FadeAnimator>();
        ResetFightMenu();
    }
    #endregion

    public void Init()
    {
        m_returnToGameMenuButton.gameObject.SetActive(false);
        m_messageBoxObj.SetActive(false);
        SetFighterSprites();
        m_fighterA.transform.localPosition = m_fighterAStartPosition;
        m_fighterB.transform.localPosition = m_fighterBStartPosition;
        m_fadeAnimator.FadeIn(m_overlayGroup, 1f);
        StartCoroutine(InitFightScene(fightInitDuration));
        StartCoroutine(StartFight(fightInitDuration, durationToFightStart));
    }

    public void ResetFightMenu()
    {
        m_smoke.gameObject.SetActive(false);
        Vector3 newPos = m_parentObj.transform.localPosition;
        newPos.y += 1100;
        m_parentObj.transform.localPosition = newPos;
        m_overlayGroup.alpha = 0.0f;
        m_messageBoxText.fontSize = 24;
        if (GameManager.Instance.Phase == GameManager.GamePhase.Fight)
        {
            GameManager.Instance.MoveToNextPhase(); //Move gamePhase change from after NPC talking to after returning from fight
        }
    }

    private void SetFighterSprites()
    {
        if (!string.IsNullOrEmpty(m_oddsManager.GetFighterA.Name) && !string.IsNullOrEmpty(m_oddsManager.GetFighterB.Name))
        {
            string fighterAName = m_oddsManager.GetFighterA.Name;
            string fighterBName = m_oddsManager.GetFighterB.Name;

            m_fighterA.sprite = GetFighterSprite(fighterAName);

            m_fighterB.sprite = GetFighterSprite(fighterBName);
        }
        m_fighterA.SetNativeSize();
        m_fighterB.SetNativeSize();

        float scaleFactor = 0.3f;

        RectTransform fighterARect = m_fighterA.GetComponent<RectTransform>();
        RectTransform fighterBRect = m_fighterB.GetComponent<RectTransform>();

        fighterARect.transform.localScale = new Vector3(1, 1, 1);
        fighterBRect.transform.localScale = new Vector3(1, 1, 1);

        fighterARect.transform.localScale *= scaleFactor;
        fighterBRect.transform.localScale *= scaleFactor;
    }

    private Sprite GetFighterSprite(string fighterName)
    {
        switch (fighterName)
        {
            case "Butter Buster":
                return m_butterBusterSprite;
            case "Le Croissant":
                return m_leCroissantSprite;
            case "Master Cupcake":
                return m_masterCupcakeSprite;
            case "Dough & Nought":
                return m_doughAndNoughtSprite;
            default:
                Debug.LogWarning($"Fighter {fighterName} has no assigned sprite!");
                return null;
        }
    }

    IEnumerator InitFightScene(float duration)
    {
        yield return new WaitForSeconds(2f);
        float elapsedTime = 0f;
        Vector3 startPos = m_parentObj.transform.localPosition;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            m_lerpHelper.LerpToTargetPosition(m_parentObj, startPos, m_parentTargetPosition, t);
            yield return null;
        }
        m_lerpHelper.SetFinalPosition(m_parentObj, m_parentTargetPosition);
    }

    IEnumerator StartFight(float startUpDuration, float durationToFightStart)
    {
        OnFightStarted?.Invoke(durationToFightStart, m_oddsManager.GetFighterA, m_oddsManager.GetFighterB);
        yield return new WaitForSeconds(durationToFightStart);
        float elapsedTime = 0f;
        m_smoke.gameObject.SetActive(true);
        Vector3 fighterATargetPosition = m_fighterAStartPosition + new Vector3(250, 0, 0);
        Vector3 fighterBTargetPosition = m_fighterBStartPosition + new Vector3(-250, 0, 0);
        while (elapsedTime < startUpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / startUpDuration);
            m_lerpHelper.LerpToTargetPosition(m_fighterA.gameObject, m_fighterAStartPosition, fighterATargetPosition, t);
            m_lerpHelper.LerpToTargetPosition(m_fighterB.gameObject, m_fighterBStartPosition, fighterBTargetPosition, t);
            yield return null;
        }
        m_lerpHelper.SetFinalPosition(m_fighterA.gameObject, fighterATargetPosition);
        m_lerpHelper.SetFinalPosition(m_fighterB.gameObject, fighterBTargetPosition);
        m_messageBoxObj.SetActive(true);
        yield return new WaitForSeconds(durationToFightStart);
        m_messageBoxText.text = m_oddsManager.FightResult;
        yield return new WaitForSeconds(5f);
        m_returnToGameMenuButton.gameObject.SetActive(true);
    }

    public void TrickButton()
    {
        m_messageBoxObj.GetComponent<Image>().sprite = m_trollSprite;
        m_messageBoxText.fontSize = 50;
        m_messageBoxText.text = "U mad bro?";
    }
}
