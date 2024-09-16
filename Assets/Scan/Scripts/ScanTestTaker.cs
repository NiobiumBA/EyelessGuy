using Scan;
using UnityEngine;

public class ScanTestTaker : MonoBehaviour
{
    [SerializeField] private LayerMask m_scanLayer;
    [SerializeField] private float m_radius;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 origin = new Vector3(0, 10, 0);
            Vector3 direction = Vector3.down;

            if (Physics.Raycast(origin, direction, out RaycastHit hitInfo, 100, m_scanLayer))
            {
                if (hitInfo.collider.TryGetComponent<ScanDrawer>(out ScanDrawer scan))
                {
                    scan.AddPoint(hitInfo.point);
                }
            }
        }
    }
}
