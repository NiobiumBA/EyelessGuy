using Scan;
using UnityEngine;

public class ScanRendererRadiusSetter : MonoBehaviour
{
    [SerializeField] private ScanRenderer m_renderer;

    private void OnEnable()
    {
        ScanPointsRadius.OnMainChanged += SetRadius;
    }

    private void OnDisable()
    {
        ScanPointsRadius.OnMainChanged -= SetRadius;
    }

    private void SetRadius()
    {
        m_renderer.SphereRadius = ScanPointsRadius.Main.Radius;
    }
}
