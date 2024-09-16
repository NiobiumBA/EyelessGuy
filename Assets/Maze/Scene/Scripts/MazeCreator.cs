using UnityEngine;
using MazeGeneration;

public class MazeCreator : MonoBehaviour
{
    [SerializeField] private Maze m_maze;
    [SerializeField] private SceneWall m_wallPrefab;
    [SerializeField] private Transform m_parent;

    public event System.Action OnCreate;

    public Maze Maze { get => m_maze; }

    public Vector2 WorldToMazeSpace(Vector3 worldPosition)
    {
        Vector3 localMazePosition = transform.InverseTransformPoint(worldPosition);

        return new Vector2(localMazePosition.x,
                           localMazePosition.z);
    }

    public Vector2 WorldToMazeSpace(Vector2 worldPosition)
    {
        return WorldToMazeSpace(new Vector3(worldPosition.x, 0, worldPosition.y));
    }

    public Vector2Int RoundWorldToMazeSpace(Vector3 worldPosition)
    {
        Vector3 localMazePosition = transform.InverseTransformPoint(worldPosition);

        return new Vector2Int(Mathf.RoundToInt(localMazePosition.x),
                              Mathf.RoundToInt(localMazePosition.z));
    }

    public Vector2Int RoundWorldToMazeSpace(Vector2 worldPosition)
    {
        return RoundWorldToMazeSpace(new Vector3(worldPosition.x, 0, worldPosition.y));
    }

    public Vector2 MazeToWorldSpace(Vector2Int mazePosition)
    {
        Vector3 worldPos3D = transform.TransformPoint(new Vector3(mazePosition.x, 0, mazePosition.y));

        return new Vector2(worldPos3D.x, worldPos3D.z);
    }

    private void Start()
    {
        CreateMaze();
    }

    private void CreateMaze()
    {
        m_maze.Generate();

        CreateWalls();

        OnCreate?.Invoke();
    }

    private void CreateWalls()
    {
        foreach (Maze.IWall wall in m_maze.Walls)
        {
            SceneWall newWall = Instantiate(m_wallPrefab, m_parent);
            newWall.Wall = wall;
        }
    }
}
