using UnityEngine;
using MazeGeneration;

public class SceneWall : MonoBehaviour
{
    private Maze.IWall m_wall;

    public Maze.IWall Wall
    {
        get => m_wall;
        set
        {
            m_wall = value;
        }
    }

    private void Start()
    {
        SetPosition();

        if (m_wall is RotatedWall rotatedWall)
        {
            if (rotatedWall.Rotation == RotatedWall.RotationState.Vertical)
                transform.localEulerAngles = new Vector3(0, 90, 0);
            else
                transform.localEulerAngles = Vector3.zero;
        }
    }

    private void SetPosition()
    {
        Vector3 localPos = new Vector3(m_wall.Position.x, 0, m_wall.Position.y);

        if (m_wall is RotatedWall rotatedWall)
        {
            if (rotatedWall.Rotation == RotatedWall.RotationState.Vertical)
                localPos += 0.5f * Vector3.left;
            else
                localPos += 0.5f * Vector3.back;
        }

        transform.localPosition = localPos;
    }
}
