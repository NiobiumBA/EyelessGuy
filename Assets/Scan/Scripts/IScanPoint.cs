using UnityEngine;

namespace Scan
{
    public interface IScanPoint
    {
        public Vector3 Position { get; }

        public void RemoveFromScene();
    }
}