using UnityEngine;

namespace Scan
{
    public class ScanHitColor : MonoBehaviour
    {
        [SerializeField] private Color m_color;

        public Color Color
        {
            get => m_color;
            set => m_color = value;
        }
    }
}