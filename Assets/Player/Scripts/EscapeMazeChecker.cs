using MazeGeneration;
using UnityEngine;

public class EscapeMazeChecker : MonoBehaviour
{
    [SerializeField] private MazeCreator m_mazeCreator;

    public delegate void EscapeMaze();
    public event EscapeMaze OnEscapeMaze;

    private void OnEnable()
    {
        m_mazeCreator.OnCreate += SetPositionToExit;
    }

    private void OnDisable()
    {
        m_mazeCreator.OnCreate -= SetPositionToExit;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnEscapeMaze?.Invoke();
    }

    private void SetPositionToExit()
    {
        IExitableMaze exitableMaze = m_mazeCreator.Maze as IExitableMaze;

        Vector2Int exitPosition = exitableMaze.ExitPosition;

        transform.localPosition = new Vector3(exitPosition.x,
                                         transform.position.y,
                                         exitPosition.y);
    }
}
