using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace EntroPi
{
    public class MeshOutline : EntroPiBehaviour
    {
        private enum OutlinePosition { Center, Outside, Inside }

        #region Exposed Data Members

        [SerializeField, RequiredReference]
        private Shader m_OutlineShader = default;

        [SerializeField, RequiredReference]
        private Shader m_StencilMaskShader = default;

        [SerializeField]
        [Range(0, 0.5f)]
        private float m_Size = 0.1f;

        [SerializeField]
        private OutlinePosition m_Position = OutlinePosition.Outside;

        [SerializeField]
        private Mesh m_MeshOveride = default;

        #endregion Exposed Data Members

        #region Private Data Members

        private Dictionary<Camera, CommandBuffer> m_Cameras = new Dictionary<Camera, CommandBuffer>();

        private Mesh m_Mesh;
        private SkinnedMeshRenderer m_SkinnedMeshRenderer;
        private Material m_StencilMaskMaterial;
        private Material m_OutlineMaterial;

        private int m_ColorShaderID;
        private int m_SizeShaderID;

        #endregion Private Data Members

        #region Public Interface

        public Color MaterialColor { set { if (AreReferencesAssigned) { m_OutlineMaterial.SetColor(m_ColorShaderID, value); } } }

        public float MaterialSize
        {
            set
            {
                if (AreReferencesAssigned)
                {
                    float outlineSize = m_Position == OutlinePosition.Center ? value / 2f : value;

                    m_StencilMaskMaterial.SetFloat(m_SizeShaderID, m_Position == OutlinePosition.Outside ? 0 : outlineSize);
                    m_OutlineMaterial.SetFloat(m_SizeShaderID, m_Position == OutlinePosition.Inside ? 0 : outlineSize);
                }
            }
        }

        #endregion Public Interface

        protected override bool OnAssignAdditionalReferences()
        {
            if (m_MeshOveride != null)
            {
                m_Mesh = m_MeshOveride;
            }
            else
            {
                m_SkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
                if (m_SkinnedMeshRenderer == null)
                {
                    MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
                    if (meshFilter != null)
                    {
                        m_Mesh = meshFilter.sharedMesh;
                    }
                }
            }

            bool areAllReferencesAssigned = m_SkinnedMeshRenderer != null || m_Mesh != null;
            Debug.Assert(areAllReferencesAssigned, "Outline buffer failed to find mesh");

            m_StencilMaskMaterial = CreateMaterial(m_StencilMaskShader);
            areAllReferencesAssigned &= m_StencilMaskMaterial != null;

            m_OutlineMaterial = CreateMaterial(m_OutlineShader);
            areAllReferencesAssigned &= m_OutlineMaterial != null;

            return areAllReferencesAssigned;
        }

        protected override void OnInitialize()
        {
            m_ColorShaderID = Shader.PropertyToID("_OutlineColor");
            m_SizeShaderID = Shader.PropertyToID("_OutlineSize");
        }

        private void OnEnable()
        {
            MaterialSize = m_Size;
        }

        private void OnDisable()
        {
            foreach (var cameraBuffer in m_Cameras)
            {
                if (cameraBuffer.Key != null)
                {
                    cameraBuffer.Key.RemoveCommandBuffer(CameraEvent.AfterForwardAlpha, cameraBuffer.Value);
                }
            }

            m_Cameras.Clear();
        }

        protected override void OnTerminate()
        {
            if (m_StencilMaskMaterial != null)
            {
                DestroyImmediate(m_StencilMaskMaterial);
            }

            if (m_OutlineMaterial != null)
            {
                DestroyImmediate(m_OutlineMaterial);
            }
        }

        private void LateUpdate()
        {
            if (m_SkinnedMeshRenderer != null)
            {
                m_Mesh = new Mesh();
                m_SkinnedMeshRenderer.BakeMesh(m_Mesh);
            }
        }

        private void OnWillRenderObject()
        {
            Camera currentCamera = Camera.current;

            if (currentCamera != null)
            {
                CommandBuffer outlineBuffer = GetOutlineCommandBuffer(currentCamera);

                outlineBuffer.Clear();
                outlineBuffer.DrawMesh(m_Mesh, transform.localToWorldMatrix, m_StencilMaskMaterial);
                outlineBuffer.DrawMesh(m_Mesh, transform.localToWorldMatrix, m_OutlineMaterial);
            }
        }

        private CommandBuffer GetOutlineCommandBuffer(Camera currentCamera)
        {
            CommandBuffer outlineBuffer = null;

            if (m_Cameras.ContainsKey(currentCamera))
            {
                outlineBuffer = m_Cameras[currentCamera];
            }
            else
            {
                outlineBuffer = new CommandBuffer();
                outlineBuffer.name = "Outline: " + gameObject.name;

                currentCamera.AddCommandBuffer(CameraEvent.AfterForwardAlpha, outlineBuffer);

                m_Cameras.Add(currentCamera, outlineBuffer);
            }

            return outlineBuffer;
        }

        public static Material CreateMaterial(Shader shader)
        {
            Material material = null;

            if (shader != null && shader.isSupported)
            {
                material = new Material(shader);
                material.hideFlags = HideFlags.HideAndDontSave;
            }

            Debug.Assert(material != null, "Failed to created material from shader");

            return material;
        }
    }
}