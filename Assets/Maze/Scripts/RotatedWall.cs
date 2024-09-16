using UnityEngine;

namespace MazeGeneration
{
    public class RotatedWall : Maze.IWall
    {
        public enum RotationState
        {
            Horizontal, Vertical
        }

        private Vector2Int m_position;
        private RotationState m_rotation;

        public Vector2Int Position => m_position;
        public RotationState Rotation => m_rotation;

        public RotatedWall(Vector2Int position, RotationState rotation)
        {
            m_position = position;
            m_rotation = rotation;
        }
    }
}
