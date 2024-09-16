using UnityEngine;

public class OnDestroyDisposer : MonoBehaviour
{
    private void OnDestroy()
    {
        if (!RenderTexturesPool.IsInitialized) return;

        RenderTexturesPool.DestroyPool();
    }
}
