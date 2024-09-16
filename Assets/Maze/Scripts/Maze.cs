using System.Collections.Generic;
using UnityEngine;

namespace MazeGeneration
{
    public abstract class Maze : ScriptableObject
    {
        public interface IWall
        {
            public Vector2Int Position { get; }
        }

        public abstract IReadOnlyCollection<IWall> Walls { get; }

        public abstract void Generate();

        public abstract bool IsPositionInMaze(Vector2Int position);

        public abstract bool FindPath(Vector2Int from, Vector2Int to, out List<Vector2Int> path);
    }
}