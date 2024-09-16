using UnityEngine;

namespace MazeGeneration
{
    public interface IExitableMaze
    {
        public Vector2Int ExitPosition { get; }
    }
}