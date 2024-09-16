using System.Collections.Generic;
using UnityEngine;

namespace NavMaze
{
    public class Path
    {
        private List<Vector2> m_wayPoints;
        private float m_traveledDistance;

        public List<Vector2> WayPoints
        {
            get => m_wayPoints;
        }

        public float TraveledDistance
        {
            get => m_traveledDistance;
            set
            {
                m_traveledDistance = Mathf.Clamp(value, 0, Length);
            }
        }

        public float Length
        {
            get
            {
                float length = 0;

                for (int i = 0; i < m_wayPoints.Count - 1; i++)
                {
                    Vector2 currentWayPoint = m_wayPoints[i];
                    Vector2 nextWayPoint = m_wayPoints[i + 1];
                    length += Vector2.Distance(currentWayPoint, nextWayPoint);
                }
                return length;
            }
        }

        public Vector2 Position
        {
            get
            {
                Vector2 position = m_wayPoints[^1];
                float lastPointLength = 0;

                for (int i = 0; i < m_wayPoints.Count - 1; i++)
                {
                    Vector2 currentWayPoint = m_wayPoints[i];
                    Vector2 nextWayPoint = m_wayPoints[i + 1];
                    float distance = Vector2.Distance(currentWayPoint, nextWayPoint);

                    if (m_traveledDistance < lastPointLength + distance)
                    {
                        float lerpCoef = m_traveledDistance - lastPointLength;
                        lerpCoef /= distance;

                        position = Vector2.Lerp(currentWayPoint, nextWayPoint, lerpCoef);

                        break;
                    }

                    lastPointLength += distance;
                }

                return position;
            }
        }

        public Vector2 Direction
        {
            get
            {
                Vector2 direction = Vector2.zero;
                float lastPointLength = 0;

                for (int i = 0; i < m_wayPoints.Count - 1; i++)
                {
                    Vector2 currentWayPoint = m_wayPoints[i];
                    Vector2 nextWayPoint = m_wayPoints[i + 1];
                    float distance = Vector2.Distance(currentWayPoint, nextWayPoint);

                    if (m_traveledDistance < lastPointLength + distance)
                    {
                        direction = nextWayPoint - currentWayPoint;
                        direction.Normalize();

                        break;
                    }

                    lastPointLength += distance;
                }

                return direction;
            }
        }

        public Path()
        {
            m_wayPoints = new List<Vector2>()
            {
                Vector2.zero
            };
        }

        public Path(List<Vector2> wayPoints)
        {
            m_wayPoints = wayPoints;
        }
    }
}