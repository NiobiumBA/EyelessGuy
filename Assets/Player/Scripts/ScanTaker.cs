using Scan;
using System.Collections.Generic;
using UnityEngine;

public class ScanTaker : MonoBehaviour
{
    [SerializeField] private ScanInitializer m_initializer;
    [SerializeField] private LayerMask m_scanLayer;
    [SerializeField] private int m_maxPoints;
    [SerializeField] private float m_maxScanDistance;
    
    public event System.Action OnTakeScan;

    private List<IScanPoint> m_scanPoints;

    private void OnEnable()
    {
        m_initializer.OnPreInit += SetMaxPoints;
    }

    private void OnDisable()
    {
        m_initializer.OnPreInit -= SetMaxPoints;
    }

    private void SetMaxPoints()
    {
        m_scanPoints = new List<IScanPoint>(m_maxPoints);

        m_initializer.MaxSpheres = m_maxPoints;
    }

    public void ScanOnePoint(Vector3 origin, Vector3 direction)
    {
        if (Physics.Raycast(origin, direction, out RaycastHit hit, m_maxScanDistance, m_scanLayer))
        {
            if (hit.collider.TryGetComponent<IScannable>(out var scannable))
                AddPoint(hit.point, scannable);
        }

        OnTakeScan?.Invoke();
    }

    private void AddPoint(Vector3 position, IScannable scannable)
    {
        if (m_scanPoints.Count == m_maxPoints)
        {
            //int randomId = 1;//Random.Range(0, m_maxPoints);

            float rand01 = Random.value;
            float rand01Square = rand01 * rand01;

            int randomId = Mathf.FloorToInt(rand01Square * m_maxPoints);

            m_scanPoints[randomId].RemoveFromScene();

            m_scanPoints.RemoveAt(randomId);
        }

        IScanPoint point = scannable.AddPoint(position);

        m_scanPoints.Add(point);
    }
}
