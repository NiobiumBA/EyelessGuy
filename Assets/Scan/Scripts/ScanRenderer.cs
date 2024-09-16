using System;
using UnityEngine;

namespace Scan
{
    public class ScanRenderer : MonoBehaviour
    {
        public const string ProjectionKernelName = "ProjectScan";
        private const string SquareSphereRadiusName = "_SquareSphereRadius";

        [SerializeField] private ScanInitializer m_initializer;
        [SerializeField] private Camera m_renderOnScreenCamera;
        [SerializeField] private Shader m_drawShader;
        [SerializeField] private ComputeShader m_projectionShader;
        [SerializeField] private Material m_blurCustomMaterial;
        [SerializeField] private float m_resolutionMultiplier = 1;
        [SerializeField] private float m_sphereRadius;

        private Material m_renderMaterial;
        private Camera m_camera;
        private Matrix4x4 m_worldToViewport;
        private int m_projectionKernelId;
        private ComputeBuffer m_projectionBuffer;
        private uint m_threadGroupSizeX;
        private int m_drawSpheresCountId;
        private int m_projectionMatrixId;
        private RenderTexture m_temporaryTexture;

        public event Action OnPreInit;
        public event Action OnPostInit;

        public Camera Camera
        {
            get
            {
                if (m_camera == null)
                    m_camera = GetComponent<Camera>();

                return m_camera;
            }
        }
        public int PixelWidth { get => m_renderOnScreenCamera.pixelWidth; }
        public int PixelHeight { get => m_renderOnScreenCamera.pixelHeight; }
        public ScanData Scan { get => m_initializer.ScanData; }
        public float SphereRadius
        {
            get => m_sphereRadius;
            set
            {
                m_sphereRadius = Mathf.Max(0f, value);
                if (m_renderMaterial != null)
                {
                    m_renderMaterial.SetFloat(SquareSphereRadiusName, m_sphereRadius * m_sphereRadius);
                }
            }
        }

        private void OnEnable()
        {
            m_initializer.OnPostInit += Init;
        }

        private void OnDisable()
        {
            m_initializer.OnPostInit -= Init;
        }

        private void OnDestroy()
        {
            m_projectionBuffer?.Dispose();
        }

        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (Scan.Count == 0)
            {
                Graphics.Blit(source, destination);

                return;
            }

            UpdateMatrix();

            UpdateProjectionShader();

            //print($"Scan.Count: {Scan.Count}");
            //print($"ThreadGroupSize: {m_threadGroupSizeX}");

            m_projectionShader.Dispatch(m_projectionKernelId, Mathf.CeilToInt((float)Scan.Count / m_threadGroupSizeX), 1, 1);

            UpdateMaterial();

            Graphics.Blit(source, m_temporaryTexture, m_renderMaterial);

            Graphics.Blit(m_temporaryTexture, destination, m_blurCustomMaterial);
        }

        public void Init()
        {
            OnPreInit?.Invoke();

            m_camera = GetComponent<Camera>();

            m_camera.aspect = m_renderOnScreenCamera.aspect;

            CreateBuffer<ScanData.SphereInfo>(ref m_projectionBuffer, Scan.MaxSpheres);

            InitRenderMaterial();
            InitTemporaryTexture();
            InitProjectionShader();

            OnPostInit?.Invoke();
        }

        private void InitRenderMaterial()
        {
            m_renderMaterial = new Material(m_drawShader);

            m_renderMaterial.SetBuffer("_ProjectedSpheres", m_projectionBuffer);

            m_renderMaterial.SetFloat(SquareSphereRadiusName, m_sphereRadius * m_sphereRadius);
            m_renderMaterial.SetFloat("_AspectRatio", m_camera.aspect);
            m_renderMaterial.SetFloat("_FarClipPlane", m_camera.farClipPlane);

            m_drawSpheresCountId = Shader.PropertyToID("_SpheresCount");
        }

        private void UpdateMatrix()
        {
            m_worldToViewport = m_camera.projectionMatrix * m_camera.worldToCameraMatrix;
        }

        private void UpdateMaterial()
        {
            m_renderMaterial.SetInteger(m_drawSpheresCountId, Scan.Count);
        }

        private void InitProjectionShader()
        {
            m_projectionKernelId = m_projectionShader.FindKernel(ProjectionKernelName);

            m_projectionShader.GetKernelThreadGroupSizes(m_projectionKernelId, out m_threadGroupSizeX, out _, out _);

            m_projectionShader.SetBuffer(m_projectionKernelId, "_Spheres", Scan.ComputeBuffer);
            m_projectionShader.SetBuffer(m_projectionKernelId, "_ProjectedSpheres", m_projectionBuffer);

            m_projectionShader.SetFloat("_FarClipPlane", m_camera.farClipPlane);
            m_projectionShader.SetFloat("_AspectRatio", m_camera.aspect);

            m_projectionMatrixId = Shader.PropertyToID("_WorldToViewport");
        }

        private static void CreateBuffer<T>(ref ComputeBuffer buffer, int length) where T : struct
        {
            if (buffer != null)
                buffer.Dispose();

            int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(T));

            buffer = new ComputeBuffer(length, stride);
        }

        private void UpdateProjectionShader()
        {
            m_projectionShader.SetMatrix(m_projectionMatrixId, m_worldToViewport);
        }

        private void InitTemporaryTexture()
        {
            Vector2 resolution = m_resolutionMultiplier * new Vector2(m_renderOnScreenCamera.pixelWidth, m_renderOnScreenCamera.pixelHeight);
            Vector2Int roundedResolution = new Vector2Int(Mathf.RoundToInt(resolution.x),
                                                          Mathf.RoundToInt(resolution.y));

            m_temporaryTexture = new RenderTexture(roundedResolution.x, roundedResolution.y, 0);
            m_temporaryTexture.filterMode = FilterMode.Bilinear;
        }
    }
}