using Scan;
using UnityEngine;

public class BindReloadScan : MonoBehaviour
{
    [SerializeField] private ScanInitializer m_initializer;
    [SerializeField] private KeyCode m_key = KeyCode.R;

    private void Update()
    {
        if (Input.GetKeyDown(m_key))
            m_initializer.Init();
    }
}
