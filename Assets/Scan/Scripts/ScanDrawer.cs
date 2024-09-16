using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Scan
{
    [RequireComponent(typeof(MeshRenderer))]
    public class ScanDrawer : MonoBehaviour, IScannable
    {
        public class ScanPoint : IScanPoint
        {
            private readonly ScanDrawer m_drawer;

            private Vector3 m_position;

            public ScanPoint(ScanDrawer drawer, Vector3 position)
            {
                m_drawer = drawer;
                m_position = position;
            }

            public Vector3 Position => m_position;

            public void RemoveFromScene()
            {
                m_drawer.ClearPoint(m_position);
            }
        }

        private static RenderTexture s_emptyTexture;

        [SerializeField] private Material m_drawMaterial;
        [SerializeField] private Color m_color = Color.white;
        [SerializeField] private int m_textureWidth;
        [SerializeField] private int m_textureHeight;
        [SerializeField] private float m_radius;

        private RenderTexture m_renderTexture1;
        private RenderTexture m_renderTexture2;
        private MeshRenderer m_renderer;
        private Mesh m_mesh;
        private CommandBuffer m_commandBuffer;
        private bool m_isFirstTextureFront;
        private int m_pointsCount;

        private RenderTexture FrontRenderTexture => m_isFirstTextureFront ? m_renderTexture1 : m_renderTexture2;
        
        private RenderTexture BackRenderTexture => m_isFirstTextureFront ? m_renderTexture2 : m_renderTexture1;

        public float Radius { get => m_radius; set => m_radius = Mathf.Max(value, 0); }

        public Texture MainTexture
        {
            set => m_renderer.material.mainTexture = value;
            get => m_renderer.material.mainTexture;
        }
        public Color ScanColor
        {
            get => m_color;
            set
            {
                m_color = value;
                m_renderer.material.color = m_color;
            }
        }

        public IScanPoint AddPoint(Vector3 position)
        {
            if (m_pointsCount == 0)
            {
                CreateAllTextures();

                MainTexture = FrontRenderTexture;
            }

            DrawPoint(position);

            m_pointsCount++;

            return new ScanPoint(this, position);
        }

        private static void CreateEmptyTexture()
        {
            if (s_emptyTexture != null) return;
            
            s_emptyTexture = new RenderTexture(16, 16, 0);
            s_emptyTexture.name = "Empty";
        }

        private void Start()
        {
            CreateEmptyTexture();

            m_renderer = GetComponent<MeshRenderer>();
            m_mesh = GetComponent<MeshFilter>().mesh;

            //CreateAllTextures();

            m_commandBuffer = new CommandBuffer();

            MainTexture = s_emptyTexture;

            ScanColor = m_color;

            m_pointsCount = 0;
        }

        private RenderTexture CreateTexture(string name)
        {
            var descriptor = new RenderTextureDescriptor(m_textureWidth,
                                                         m_textureHeight,
                                                         GraphicsFormat.R8_UNorm,
                                                         0);
            RenderTexture texture = RenderTexturesPool.GetTexture(descriptor);
            texture.name = name;

            return texture;
        }

        private void FlipTextures()
        {
            m_isFirstTextureFront = !m_isFirstTextureFront;
        }

        private void DrawPoint(Vector3 worldPosition)
        {
            DrawPoint(worldPosition, 1);
        }

        private void DrawPoint(Vector3 worldPosition, float brightness)
        {
            m_drawMaterial.mainTexture = BackRenderTexture;

            m_drawMaterial.SetFloat("_ScanSquareRadius", m_radius * m_radius);
            m_drawMaterial.SetVector("_ScanPosition", worldPosition);
            //m_drawMaterial.SetColor("_ScanColor", color);
            m_drawMaterial.SetFloat("_ScanBrightness", brightness);

            m_commandBuffer.Clear();
            m_commandBuffer.SetRenderTarget(FrontRenderTexture);

            m_commandBuffer.DrawMesh(m_mesh, transform.localToWorldMatrix, m_drawMaterial, 0);

            Graphics.ExecuteCommandBuffer(m_commandBuffer);

            m_drawMaterial.mainTexture = FrontRenderTexture;

            FlipTextures();
        }

        private void ClearPoint(Vector3 worldPosition)
        {
            m_pointsCount--;

            if (m_pointsCount == 0)
            {
                MainTexture = s_emptyTexture;

                DisposeAllTextures();

                return;
            }

            DrawPoint(worldPosition, 0);
        }

        private void CreateAllTextures()
        {
            m_renderTexture1 = CreateTexture("1");
            m_renderTexture2 = CreateTexture("2");
        }

        private void DisposeAllTextures()
        {
            RenderTexturesPool.Dispose(m_renderTexture1);
            RenderTexturesPool.Dispose(m_renderTexture2);
        }
    }
}