using UnityEngine;

public class ScanPointsRadius : MonoBehaviour
{
    public static ScanPointsRadius Main => m_main;

    [SerializeField] private float m_radius;

    private static ScanPointsRadius m_main;

    public static event System.Action OnMainChanged;

    public float Radius { get => m_radius; }

    private void Start()
    {
        if (m_main == null)
        {
            m_main = this;

            OnMainChanged?.Invoke();
        }
        else Debug.LogError($"Too many {nameof(ScanPointsRadius)} scripts in Scene!");
    }
}
