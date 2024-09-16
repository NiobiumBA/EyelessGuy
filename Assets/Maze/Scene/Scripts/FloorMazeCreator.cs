using MazeGeneration;
using UnityEngine;

public class FloorMazeCreator : MonoBehaviour
{
    [SerializeField] private GameObject m_floorPrefab;
    [SerializeField] private Transform m_parent;
    [SerializeField] private MazeCreator m_mazeCreator;

    private void OnEnable()
    {
        m_mazeCreator.OnCreate += CreateFloor;
    }

    private void OnDisable()
    {
        m_mazeCreator.OnCreate -= CreateFloor;
    }

    private void CreateFloor()
    {
        IRectMaze maze = m_mazeCreator.Maze as IRectMaze;

        for (int x = 0; x < maze.Width; x++)
        {
            for (int y = 0; y < maze.Height; y++)
            {
                Vector2 worldSpawnPos = m_mazeCreator.MazeToWorldSpace(new Vector2Int(x, y));
                Vector3 world3D = new Vector3(worldSpawnPos.x, 0, worldSpawnPos.y);

                Instantiate(m_floorPrefab, world3D, Quaternion.identity, m_parent);
            }
        }
    }
}
