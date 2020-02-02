using EntroPi;
using Snake3D.WorldGrid;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snake3D.Player
{
    public class PlayerView : EntroPiBehaviour
    {
        [SerializeField, RequiredReference]
        private ParticleSystem m_Particles = default;

        [SerializeField, RequiredReference]
        private Light m_Light = default;

        [Header("Head")]
        [SerializeField]
        private Color m_HeadColor = Color.magenta;

        [SerializeField]
        [Range(0, 5)]
        private float m_HeadEmissionIntensity = 2f;

        [Header("Body")]
        [SerializeField]
        private List<Color> m_Colors = new List<Color>();

        [SerializeField]
        [Range(0, 5)]
        private float m_EmissionIntensity = 0.5f;

        [SerializeField]
        private int m_FadeOutDistance = 24;

        [SerializeField]
        [Range(0, 1)]
        private float m_FadeOutIntensity = 0.75f;

        [Header("Death Animation")]
        [SerializeField]
        private Color m_DeathAnimationColor = Color.red;

        [SerializeField]
        [Range(0, 5)]
        private float m_DeathAnimationIntensity = 2f;

        [SerializeField]
        [Range(0, 2)]
        private float m_DeathAnimationFadeOutDuration = 0.5f;

        [SerializeField]
        [Range(0, 1)]
        private float m_DeathAnimationNodeInterval = 0.25f;

        public void UpdatePath(NodeWalkable activeNode, IReadOnlyCollection<NodeWalkable> pathNodes)
        {
            if (AreReferencesAssigned == true)
            {
                if (activeNode != null && activeNode.View != null)
                {
                    activeNode.View.Show(m_HeadColor, m_HeadEmissionIntensity);

                    m_Light.color = m_HeadColor;
                    m_Light.transform.position = activeNode.ViewPosition;
                }

                if (pathNodes != null)
                {
                    Color fadeOutColor = m_Colors[m_Colors.Count - 1];

                    // Iterate over the Queue in the reverse order.
                    // This will update the colors starting from the player's head to its tail.
                    int nodeIndex = pathNodes.Count - 1;

                    foreach (NodeWalkable pathNode in pathNodes)
                    {
                        if (pathNode != activeNode)
                        {
                            // Offset index to account for player's head (active node) at the start of the Queue.
                            int offsetNodeIndex = nodeIndex - 1;

                            float gradient = Mathf.Clamp01((m_FadeOutDistance - offsetNodeIndex) / (float)m_FadeOutDistance);
                            int colorIndex = offsetNodeIndex % m_Colors.Count;

                            Color nodeColor = Color.Lerp(m_Colors[colorIndex], fadeOutColor, (1 - gradient) * m_FadeOutIntensity);
                            float emissionIntensity = m_EmissionIntensity * gradient;

                            pathNode.View.Show(nodeColor, emissionIntensity);
                        }

                        --nodeIndex;
                    }
                }
            }
        }

        public void RemoveNode(NodeWalkable removedNode)
        {
            if (removedNode != null && removedNode.View != null)
            {
                removedNode.View.Hide();
            }
        }

        public void PlayDeathAnimation(NodeWalkable activeNode, IReadOnlyCollection<NodeWalkable> pathNodes)
        {
            if (AreReferencesAssigned == true)
            {
                StopAllCoroutines();
                StartCoroutine(DeathAnimationRoutine(activeNode, pathNodes, m_DeathAnimationNodeInterval));
            }
        }

        private IEnumerator DeathAnimationRoutine(NodeWalkable activeNode, IReadOnlyCollection<NodeWalkable> pathNodes, float nodeInterval)
        {
            const float headFadeOutDurationMultiplier = 5f;

            const float deathIntensity = 0;

            if (activeNode != null && activeNode.View != null)
            {
                m_Light.enabled = false;

                m_Particles.transform.position = activeNode.ViewPosition;
                m_Particles.Play();

                activeNode.View.LerpColor(m_HeadColor, m_HeadEmissionIntensity, m_HeadColor, deathIntensity, m_DeathAnimationFadeOutDuration * headFadeOutDurationMultiplier);
            }

            if (pathNodes != null)
            {
                Color deathColor = m_Colors[m_Colors.Count - 1];

                foreach (NodeWalkable pathNode in pathNodes.Reverse())
                {
                    if (pathNode != activeNode)
                    {
                        pathNode.View.LerpColor(m_DeathAnimationColor, m_DeathAnimationIntensity, deathColor, deathIntensity, m_DeathAnimationFadeOutDuration);
                    }

                    yield return new WaitForSeconds(nodeInterval);
                }
            }
        }
    }
}