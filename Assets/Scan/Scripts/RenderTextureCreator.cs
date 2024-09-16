using Scan;
using UnityEngine;
using UnityEngine.UI;

public class RenderTextureCreator : MonoBehaviour
{
    [SerializeField] private Camera m_onScreenCamera;
    [SerializeField] private ScanRenderer m_scanRenderer;
    [SerializeField] private RawImage m_image;
    [SerializeField] private float m_resolutionMultiplier = 1;

    private RenderTexture m_texture;

    private void OnEnable()
    {
        m_scanRenderer.OnPostInit += CreateTexture;
    }

    private void OnDisable()
    {
        m_scanRenderer.OnPostInit -= CreateTexture;

        if (m_texture != null)
            m_texture.Release();
    }

    private void CreateTexture()
    {
        Vector2 resolution = m_resolutionMultiplier * new Vector2(m_onScreenCamera.pixelWidth, m_onScreenCamera.pixelHeight);
        Vector2Int roundedResolution = new Vector2Int(Mathf.RoundToInt(resolution.x),
                                                      Mathf.RoundToInt(resolution.y));

        m_texture = new RenderTexture(roundedResolution.x, roundedResolution.y, 0)
        {
            filterMode = FilterMode.Bilinear
        };

        m_scanRenderer.Camera.targetTexture = m_texture;

        m_image.texture = m_texture;
    }
}
