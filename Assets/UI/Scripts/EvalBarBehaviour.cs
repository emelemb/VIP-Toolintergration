using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class EvalBarBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject m_evalIconObj;
    [SerializeField] private TextMeshProUGUI m_fighterAText;
    [SerializeField] private TextMeshProUGUI m_fighterBText;

    private LerpHelper m_lerpHelper;
    private FightMenuBehaviour m_fightMenuBehaviour;
    private Vector3 m_evalIconOrigin;
    private float m_distanceFromOrigin = 150;

    #region Unity Methods
    private void Start()
    {
        m_lerpHelper = new LerpHelper();
        m_evalIconOrigin = m_evalIconObj.transform.localPosition;
    }

    private void OnEnable()
    {
        m_fightMenuBehaviour = GetComponent<FightMenuBehaviour>();
        if (m_fightMenuBehaviour != null)
            m_fightMenuBehaviour.OnFightStarted += UpdateEvalBar;
    }
    private void OnDisable()
    {
        m_fightMenuBehaviour = GetComponent<FightMenuBehaviour>();
        if (m_fightMenuBehaviour != null)
            m_fightMenuBehaviour.OnFightStarted -= UpdateEvalBar;
    }
    #endregion

    private void UpdateEvalBar(float fightDuration, Fighter fighterA, Fighter fighterB)
    {
        m_evalIconObj.transform.localPosition = m_evalIconOrigin;
        m_fighterAText.text = fighterA.Name;
        m_fighterBText.text = fighterB.Name;

        Vector3 leftLimit = m_evalIconOrigin + new Vector3(m_distanceFromOrigin, 0, 0);
        Vector3 rightLimit = m_evalIconOrigin + new Vector3(-m_distanceFromOrigin, 0, 0);
        Vector3 finalEvalIconPosition = fighterA.IsWinner() ? rightLimit : leftLimit;
        StartCoroutine(LerpEvalIconPosition(fightDuration, finalEvalIconPosition));
    }

    private IEnumerator LerpEvalIconPosition(float fightDuration, Vector3 finalEvalIconPosition)
    {
        yield return new WaitForSeconds(fightDuration);
        float elapsedTime = 0f;
        Vector3 startPosition = m_evalIconObj.transform.localPosition;
        while (elapsedTime < fightDuration)
        {
           elapsedTime += Time.deltaTime;
           float t = Mathf.Clamp01(elapsedTime / fightDuration);
           m_lerpHelper.LerpToTargetPosition(m_evalIconObj, startPosition, finalEvalIconPosition, t);
           yield return null;
        }
        m_lerpHelper.SetFinalPosition(m_evalIconObj, finalEvalIconPosition);
    }
}
