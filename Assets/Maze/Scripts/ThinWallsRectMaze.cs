using System.Collections.Generic;
using UnityEngine;

namespace MazeGeneration
{
    [CreateAssetMenu(fileName = "Maze", menuName = "ScriptableObjects/Maze/ThinWallsRectMaze")]
    public class ThinWallsRectMaze : Maze, IRectMaze, IExitableMaze
    {
        public class Cell
        {
            public List<Vector2Int> ConnectedCellsDirection;

            public Cell()
            {
                ConnectedCellsDirection = new List<Vector2Int>();
            }

            public void AddDirection(Vector2Int direction)
            {
                ConnectedCellsDirection.Add(direction);
            }
        }

        [SerializeField] protected int m_width;
        [SerializeField] protected int m_height;

        protected Vector2Int m_exitPosition;
        protected List<RotatedWall> m_walls;
        protected Cell[,] m_cells;

        public override IReadOnlyCollection<IWall> Walls => m_walls;
        public IReadOnlyCollection<RotatedWall> ThinWalls => m_walls;
        public int Width => m_width;
        public int Height => m_height;
        public Vector2Int ExitPosition { get => m_exitPosition; }

        public override void Generate()
        {
            GeneratePath();

            GenerateWalls();
        }

        public override bool FindPath(Vector2Int from, Vector2Int to, out List<Vector2Int> path)
        {
            if (IsPositionInMaze(from) == false || IsPositionInMaze(to) == false)
            {
                path = new List<Vector2Int>();
                return false;
            }

            Vector2Int currentPos = from;

            List<int> indices = new List<int>()
            {
                0
            };
            path = new List<Vector2Int>()
            {
                currentPos
            };

            //int iter = 0;

            while (currentPos != to)
            {
                //Debug.Log(currentPos);

                //if (iter++ > 10000) break;

                Cell currentCell = m_cells[currentPos.x, currentPos.y];
                List<Vector2Int> nextDirections = new List<Vector2Int>(currentCell.ConnectedCellsDirection);
                if (path.Count > 1)
                {
                    //Debug.Log(path[^2] - path[^1]);
                    nextDirections.Remove(path[^2] - path[^1]);
                }

                int currentId = indices[^1];

                if (currentId >= nextDirections.Count)
                {
                    indices.RemoveAt(indices.Count - 1);
                    path.RemoveAt(path.Count - 1);

                    currentPos = path[^1];

                    //indices[^1] = indices[^1] + 1;
                    //currentId = indices[^1];
                    continue;
                }

                //Vector2Int direction = currentCell.ConnectedCellsDirection[currentId];

                currentPos += nextDirections[currentId];

                indices[^1] = currentId + 1;
                indices.Add(0);
                path.Add(currentPos);
            }

            return true;
        }

        public override bool IsPositionInMaze(Vector2Int position)
        {
            return 0 <= position.x && position.x < Width && 0 <= position.y && position.y < Height;
        }

        protected Vector2Int GetRndPosWithValidMoves()
        {
            for (int x = Width - 1; x >= 0; x--)
            {
                for (int y = Height - 1; y >= 0; y--)
                {
                    if (m_cells[x, y] == null) continue;

                    Vector2Int currentPos = new Vector2Int(x, y);

                    if (GetValidMoves(currentPos).Count != 0)
                        return currentPos;
                }
            }

            return Vector2Int.zero;
        }

        protected List<Vector2Int> GetValidMoves(Vector2Int position)
        {
            Vector2Int[] moves =
            {
                Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down
            };

            List<Vector2Int> resultMoves = new List<Vector2Int>(4);

            foreach (Vector2Int move in moves)
            {
                Vector2Int movedPos = position + move;

                if (IsPositionInMaze(movedPos) && m_cells[movedPos.x, movedPos.y] == null)
                    resultMoves.Add(move);
            }

            return resultMoves;
        }

        protected void GeneratePath()
        {
            m_cells = new Cell[Width, Height];

            //int iter = 0;
            int uniquePositionsCount = 0;
            Vector2Int currentPos = Vector2Int.zero;

            while (uniquePositionsCount < Width * Height - 1)
            {
                //Debug.Log(currentPos);

                //if (iter++ > 10000) break;

                List<Vector2Int> moves = GetValidMoves(currentPos);

                if (moves.Count == 0)
                {
                    currentPos = GetRndPosWithValidMoves();

                    continue;
                }

                uniquePositionsCount++;

                int randId = Random.Range(0, moves.Count);
                Vector2Int move = moves[randId];

                Cell currentCell = m_cells[currentPos.x, currentPos.y];

                if (currentCell == null)
                {
                    currentCell = new Cell();
                    m_cells[currentPos.x, currentPos.y] = currentCell;
                }

                currentCell.AddDirection(move);

                currentPos += move;

                Cell nextCell = m_cells[currentPos.x, currentPos.y];

                if (nextCell == null)
                {
                    nextCell = new Cell();
                    m_cells[currentPos.x, currentPos.y] = nextCell;
                }

                nextCell.AddDirection(-move);
            }
        }

        protected void GenerateWalls()
        {
            m_exitPosition = GenerateExitPosition();

            m_walls = new List<RotatedWall>();

            for (int x = 0; x <= Width; x++)
            {
                for (int y = 0; y <= Height; y++)
                {
                    Vector2Int currentPos = new Vector2Int(x, y);

                    if (currentPos == m_exitPosition)
                        continue;

                    if (x == Width)
                    {
                        if (y == Height)
                            continue;

                        m_walls.Add(new RotatedWall(currentPos, RotatedWall.RotationState.Vertical));
                        continue;
                    }

                    if (y == Height)
                    {
                        m_walls.Add(new RotatedWall(currentPos, RotatedWall.RotationState.Horizontal));
                        continue;
                    }

                    List<Vector2Int> connectedCellsPos;

                    if (m_cells[x, y] == null)
                    {
                        connectedCellsPos = new List<Vector2Int>();
                    }
                    else
                    {
                        connectedCellsPos = m_cells[x, y].ConnectedCellsDirection;
                    }

                    if (connectedCellsPos.Contains(Vector2Int.left) == false)
                        m_walls.Add(new RotatedWall(currentPos, RotatedWall.RotationState.Vertical));

                    if (connectedCellsPos.Contains(Vector2Int.down) == false)
                        m_walls.Add(new RotatedWall(currentPos, RotatedWall.RotationState.Horizontal));
                }
            }

            /*bool[,] horizontalWall = new bool[Width + 1, Height + 1];
            bool[,] verticalWall = new bool[Width + 1, Height + 1];

            for (int i = 0; i <= Width; i++)
            {
                for (int j = 0; j <= Height; j++)
                {
                    horizontalWall[i, j] = (i != Width);

                    verticalWall[i, j] = (j != Height);
                }
            }

            for (int i = 0; i < m_fullPath.Count - 1; i++)
            {
                Vector2Int wayPoint = m_fullPath[i];
                Vector2Int nextWayPoint = m_fullPath[i + 1];

                Vector2Int move = nextWayPoint - wayPoint;

                if (move == Vector2Int.left)
                    verticalWall[wayPoint.x, wayPoint.y] = false;
                else if (move == Vector2Int.down)
                    horizontalWall[wayPoint.x, wayPoint.y] = false;
                else if (move == Vector2Int.right)
                    verticalWall[nextWayPoint.x, nextWayPoint.y] = false;
                else
                    horizontalWall[nextWayPoint.x, nextWayPoint.y] = false;
            }

            m_walls = new List<RotatedWall>();

            for (int i = 0; i <= Width; i++)
            {
                for (int j = 0; j <= Height; j++)
                {
                    if (horizontalWall[i, j])
                        m_walls.Add(new RotatedWall(new Vector2Int(i, j), RotatedWall.RotationState.Horizontal));
                    if (verticalWall[i, j])
                        m_walls.Add(new RotatedWall(new Vector2Int(i, j), RotatedWall.RotationState.Vertical));
                }
            }*/
        }

        private Vector2Int GenerateExitPosition()
        {
            int randomNum = Random.Range(0, Width + Height);

            if (randomNum >= Width)
                return new Vector2Int(Width, randomNum - Width);

            return new Vector2Int(randomNum, Height);
        }
    }
}
