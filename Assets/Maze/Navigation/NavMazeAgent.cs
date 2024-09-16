using UnityEngine;
using MazeGeneration;
using System.Collections.Generic;

namespace NavMaze
{
    public class NavMazeAgent : MonoBehaviour
    {
        [SerializeField] private MazeCreator m_mazeCreator;

        private Vector2 m_target = Vector2.positiveInfinity;
        private Path m_path;

        public Maze Maze
        {
            get => m_mazeCreator.Maze;
        }

        public MazeCreator MazeCreator { get => m_mazeCreator; }

        public Path Path { get => m_path; }

        // Automatically recalculate path
        public Vector2 Target
        {
            get => m_target;
            set
            {
                if (m_target != value)
                    m_path = CalculatePath(value);

                m_target = value;
            }
        }

        public Path CalculatePath(Vector2 target)
        {
            Maze maze = m_mazeCreator.Maze;

            Vector2Int roundedCurrentMazePos = m_mazeCreator.RoundWorldToMazeSpace(transform.position);
            Vector2Int roundedTargetMazePos = m_mazeCreator.RoundWorldToMazeSpace(target);
            
            List<Vector2Int> localWayPoints;
            bool foundSuccess = maze.FindPath(roundedCurrentMazePos, roundedTargetMazePos, out localWayPoints);

            if (foundSuccess == false)
                return null;

            Vector2 currentPos = new Vector2(transform.position.x, transform.position.z);

            List<Vector2> wayPoints = new List<Vector2>(localWayPoints.Count)
            {
                currentPos
            };

            foreach (Vector2Int localMazePoint in localWayPoints)
            {
                Vector2 worldPos = m_mazeCreator.MazeToWorldSpace(localMazePoint);

                wayPoints.Add(worldPos);
            }

            if (wayPoints.Count <= 2)
            {
                wayPoints = new List<Vector2>()
                {
                    currentPos, target
                };

                return new Path(wayPoints);
            }

            float fromCurrentToSecondWayPoint = Vector2.Distance(currentPos, wayPoints[2]);
            float fromFirstToSecondWayPoint = Vector2.Distance(wayPoints[1], wayPoints[2]);

            if (fromCurrentToSecondWayPoint <= fromFirstToSecondWayPoint)
                wayPoints.RemoveAt(1);

            float fromLastWayPointToTarget = Vector2.Distance(wayPoints[^1], target);
            float fromSecondLastWayPointToTarget = Vector2.Distance(wayPoints[^2], target);

            if (fromSecondLastWayPointToTarget < fromLastWayPointToTarget)
                //wayPoints[^1] = target;
                wayPoints.RemoveAt(wayPoints.Count - 1);
            //else
            wayPoints.Add(target);

            return new Path(wayPoints);
        }

        public void MoveAlongPath(float moveLength)
        {
            m_path.TraveledDistance += moveLength;

            Vector2 newPosition = m_path.Position;

            transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.y);
        }

        public void MoveTo(Vector2 position)
        {
            transform.position = new Vector3(position.x, transform.position.y, position.y);

            m_path = CalculatePath(m_target);
        }
    }
}
