using UnityEngine;

namespace Scan
{
    public interface IScannable
    {
        public IScanPoint AddPoint(Vector3 position);
    }
}