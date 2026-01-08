using UnityEngine;

public class SwingAnimation : MonoBehaviour
{
    [SerializeField] float m_angle;
    [SerializeField] float m_speed;

    private float m_initialAngle;

    #region Unity Methods
    void Start()
    {
        m_initialAngle = transform.eulerAngles.z;
    }

    void Update()
    {
        float m_targetAngle = Mathf.Sin(Time.time * m_speed) * m_angle;
        transform.rotation = Quaternion.Euler(0, 0, m_initialAngle + m_targetAngle);
    }
    #endregion
}
