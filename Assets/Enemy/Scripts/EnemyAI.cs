using System.Collections;
using UnityEngine;
using NavMaze;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private NavMazeAgent m_agent;
    [SerializeField] private MazeCreator m_mazeCreator;
    [SerializeField] private Transform m_player;
    [SerializeField] private ScanTaker m_scanTaker;
    [SerializeField] private float m_speed;
    [SerializeField] private float m_choseTargetRadius;
    [SerializeField] private float m_timeToChangeTarget;
    [SerializeField] private float m_radiusTargetingPlayer;

    private float m_changeTargetTimer;

    public float Speed
    {
        get => m_speed;
        set => m_speed = Mathf.Max(value, 0);
    }

    public Vector2 Velocity
    {
        get
        {
            if (m_agent.Path == null)
                return Vector2.zero;

            return m_agent.Path.Direction * m_speed;
        }
    }

    private void OnEnable()
    {
        m_scanTaker.OnTakeScan += PlayerTakeScan;
    }

    private void OnDisable()
    {
        m_scanTaker.OnTakeScan -= PlayerTakeScan;
    }

    private void Start()
    {
        //StartCoroutine(ChangingTarget());

        m_changeTargetTimer = 0;
    }

    private void Update()
    {
        m_changeTargetTimer += Time.deltaTime * GamePause.TimeScale;

        if (m_changeTargetTimer > m_timeToChangeTarget)
        {
            m_changeTargetTimer = 0;

            ChooseRandomTarget();
        }

        if (m_agent.Path != null)
            m_agent.MoveAlongPath(m_speed * Time.deltaTime * GamePause.TimeScale);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, m_choseTargetRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, m_radiusTargetingPlayer);
    }

    private IEnumerator ChangingTarget()
    {
        WaitForSeconds waiting = new WaitForSeconds(m_timeToChangeTarget);

        while (true)
        {
            yield return waiting;

            ChooseRandomTarget();
        }
    }

    private void ChooseRandomTarget()
    {
        Vector2 randomPoint = Random.insideUnitCircle * m_choseTargetRadius;

        randomPoint += new Vector2(transform.position.x, transform.position.z);

        Vector2Int roundWorldPoint = m_mazeCreator.RoundWorldToMazeSpace(randomPoint);

        if (m_agent.Maze.IsPositionInMaze(roundWorldPoint))
        {
            m_agent.Target = m_mazeCreator.MazeToWorldSpace(roundWorldPoint);
        }
    }

    private void PlayerTakeScan()
    {
        Vector3 offset = transform.position - m_player.position;
        float squareRadiusTargeting = m_radiusTargetingPlayer * m_radiusTargetingPlayer;

        if (offset.sqrMagnitude < squareRadiusTargeting)
        {
            m_agent.Target = new Vector2(m_player.position.x, m_player.position.z);
        }
    }
}
