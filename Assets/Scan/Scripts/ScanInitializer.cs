using System;
using UnityEngine;

namespace Scan
{
    public class ScanInitializer : MonoBehaviour
    {
        [SerializeField] private int m_maxSpheres;

        private ScanData m_scanData;

        public ScanData ScanData { get => m_scanData; set => m_scanData = value; }
        public int MaxSpheres { get => m_maxSpheres; set => m_maxSpheres = Mathf.Max(value, 0); }

        public event Action OnPreInit;
        public event Action OnPostInit;

        private void Start()
        {
            OnPreInit?.Invoke();

            Init();

            OnPostInit?.Invoke();
        }

        private void OnDestroy()
        {
            ScanData?.Dispose();
        }

        public void Init()
        {
            m_scanData?.Dispose();

            m_scanData = new ScanData(m_maxSpheres);
        }
    }
}