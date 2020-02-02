using EntroPi;
using System.Collections;
using UnityEngine;

namespace Snake3D.WorldGrid
{
    public class NodeWalkableView : EntroPiBehaviour
    {
        [SerializeField, RequiredReference]
        private Renderer m_Renderer = default;

        [SerializeField, RequiredReference]
        private MeshOutline m_Outline = default;

        [SerializeField]
        private Color m_ObstacleColor = Color.white;

        private Material m_Material;

        public Color ObstacleColor { get { return m_ObstacleColor; } }
        public NodeWalkable Node { private get; set; }

        public void Show(Color color, float emissionIntensity = 0, bool showOutline = false)
        {
            if (AreReferencesAssigned == true)
            {
                m_Renderer.enabled = true;
                SetMaterialColor(color, emissionIntensity);

                m_Outline.MaterialColor = color * emissionIntensity;
                m_Outline.enabled = showOutline;
            }
        }

        public void Hide()
        {
            if (AreReferencesAssigned == true)
            {
                m_Renderer.enabled = false;
                m_Outline.enabled = false;
            }
        }

        public void LerpColor(Color fromColor, float fromIntensity, Color toColor, float toIntensity, float flashDuration)
        {
            if (AreReferencesAssigned == true)
            {
                StopAllCoroutines();
                StartCoroutine(LerpColorRoutine(fromColor, fromIntensity, toColor, toIntensity, flashDuration));
            }
        }

        protected override bool OnAssignAdditionalReferences()
        {
            m_Material = m_Renderer.material;

            return m_Material != null;
        }

        protected override void OnInitialize()
        {
            Hide();
        }

        private void OnDrawGizmosSelected()
        {
            if (Node != null && Node.UpVectors != null)
            {
                for (int i = 0; i < Node.UpVectors.Length; ++i)
                {
                    Ray gizmoRay = new Ray(transform.position, Node.UpVectors[i]);

                    Gizmos.color = Color.cyan;
                    Gizmos.DrawRay(gizmoRay);
                }
            }
        }

        private IEnumerator LerpColorRoutine(Color fromColor, float fromIntensity, Color toColor, float toIntensity, float flashDuration)
        {
            float timer = 0;

            while (timer < flashDuration)
            {
                float lerpValue = timer / flashDuration;

                Color color = Color.Lerp(fromColor, toColor, lerpValue);
                float intensity = Mathf.Lerp(fromIntensity, toIntensity, lerpValue);

                SetMaterialColor(color, intensity);

                timer += Time.deltaTime;

                yield return null;
            }

            SetMaterialColor(toColor, toIntensity);
        }

        private void SetMaterialColor(Color color, float intensity)
        {
            m_Material.color = color;
            m_Material.SetColor("_EmissionColor", color * intensity);
        }
    }
}