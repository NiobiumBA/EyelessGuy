using Scan;
using UnityEngine;

[RequireComponent(typeof(ScanDrawer))]
public class ScanDrawerRadiusSetter : MonoBehaviour
{
    private void OnEnable()
    {
        ScanPointsRadius.OnMainChanged += SetRadius;
    }

    private void OnDisable()
    {
        ScanPointsRadius.OnMainChanged -= SetRadius;
    }

    private void Start()
    {
        SetRadius();
    }

    private void SetRadius()
    {
        ScanDrawer drawer = GetComponent<ScanDrawer>();

        drawer.Radius = ScanPointsRadius.Main.Radius;
    }
}
