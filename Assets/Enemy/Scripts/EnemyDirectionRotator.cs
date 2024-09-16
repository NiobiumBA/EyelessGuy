using UnityEngine;

public class EnemyDirectionRotator : MonoBehaviour
{
    [SerializeField] private EnemyAI m_enemyAI;
    [SerializeField] private float m_rotationSpeed;

    private float m_currentEulerY;
    private float m_velocityEulerY = 0;

    private void Start()
    {
        m_currentEulerY = transform.eulerAngles.y;
    }

    private void Update()
    {
        if (GamePause.IsPaused == false)
            Rotate();
    }

    private void Rotate()
    {
        Vector2 velocity2D = m_enemyAI.Velocity;
        Vector3 velocity = new Vector3(velocity2D.x, 0, velocity2D.y);

        //if (velocity != Vector3.zero)
        //    transform.forward = velocity;

        if (velocity != Vector3.zero)
        {
            Vector3 eulerCurrent = CalculateRotation(velocity);
            transform.eulerAngles = eulerCurrent;
        }
    }

    private Vector3 CalculateRotation(Vector3 velocity)
    {
        Quaternion targetRotation = Quaternion.LookRotation(velocity);
        Vector3 eulerTarget = targetRotation.eulerAngles;

        m_currentEulerY = Mathf.SmoothDampAngle(m_currentEulerY, eulerTarget.y, ref m_velocityEulerY, m_rotationSpeed);

        //Vector3 eulerCurrent = new Vector3(eulerTarget.x, m_currentEulerY, eulerTarget.y);
        Vector3 eulerCurrent = m_currentEulerY * Vector3.up;
        return eulerCurrent;
    }
}
