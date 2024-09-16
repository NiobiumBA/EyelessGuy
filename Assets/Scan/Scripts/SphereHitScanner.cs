using UnityEngine;

namespace Scan
{
    public class SphereHitScanner : MonoBehaviour, IScannable
    {
        public class ScanPoint : IScanPoint
        {
            private readonly ScanData m_data;

            private Vector3 m_position;

            public ScanPoint(ScanData data, Vector3 position)
            {
                m_position = position;
                m_data = data;
            }

            public Vector3 Position => m_position;

            public void RemoveFromScene()
            {
                for (int i = 0; i < m_data.Count; i++)
                {
                    ScanData.SphereInfo sphereInfo = m_data[i];
                    
                    if (sphereInfo.position == m_position)
                    {
                        m_data.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        [SerializeField] private ScanInitializer m_initializer;
        [SerializeField] private Color m_color;

        public ScanData Data => m_initializer.ScanData;

        public IScanPoint AddPoint(Vector3 position)
        {
            ScanData.SphereInfo sphere = new ScanData.SphereInfo(position, m_color);

            Data.Add(sphere);
            Data.Apply();

            return new ScanPoint(Data, position);
        }
    }
}