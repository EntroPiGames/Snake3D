using EntroPi;
using UnityEngine;

namespace Snake3D.WorldGrid
{
    public class WorldGridView : EntroPiBehaviour
    {
        [SerializeField, RequiredReference]
        private ParticleSystem m_TargetParticles = default;

        [SerializeField, RequiredReference]
        private Light m_TargetLight = default;

        [SerializeField]
        private Color m_TargetColor = Color.red;

        [SerializeField]
        [Range(0, 5)]
        private float m_TargetEmissionIntensity = 3f;

        public void SetNewTargetNode(NodeWalkable targetNode, NodeWalkable previousTargetNode)
        {
            if (AreReferencesAssigned == true)
            {
                if (previousTargetNode != null)
                {
                    m_TargetParticles.transform.position = previousTargetNode.ViewPosition;
                    m_TargetParticles.Play();
                }

                if (targetNode != null && targetNode.View != null)
                {
                    targetNode.View.Show(m_TargetColor, m_TargetEmissionIntensity, true);

                    m_TargetLight.enabled = true;
                    m_TargetLight.color = m_TargetColor;
                    m_TargetLight.transform.position = targetNode.ViewPosition;
                }
            }
        }

        protected override void OnInitialize()
        {
            m_TargetLight.enabled = false;
        }
    }
}