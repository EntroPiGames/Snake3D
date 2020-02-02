using EntroPi;
using System.Collections.Generic;
using UnityEngine;

namespace Snake3D.WorldGrid
{
    [RequireComponent(typeof(WorldGridView))]
    public class WorldGridController : EntroPiBehaviour
    {
        [RequiredComponent]
        private WorldGridView m_GridView = default;

        private WorldGridModel m_GridModel;

        public int GridSize { get { return m_GridModel != null ? m_GridModel.GridSize : 0; } }
        public NodeWalkable PlayerStartNode { get { return m_GridModel != null ? m_GridModel.PlayerStartNode : null; } }
        public NodeWalkable PlayerTargetNode { get; private set; }

        public bool GenerateWorldGrid(WorldGridGenerator generator)
        {
            Debug.Assert(m_GridModel == null, "World Grid already generated", this);

            if (AreReferencesAssigned == true && generator != null)
            {
                m_GridModel = generator.GenerateWorldGridModel(transform);
            }

            return m_GridModel != null;
        }

        public NodeWalkable GetNextWalkableNodeInDirection(NodeWalkable currentNode, Vector3Int direction, Vector3Int currentUpVector)
        {
            Debug.AssertFormat(direction.magnitude == 1, this, "Direction is invalid: {0}", direction);

            NodeWalkable nextWalkableNode = default;

            if (m_GridModel != null && currentNode != null)
            {
                nextWalkableNode = m_GridModel.TryGetWalkableNode(currentNode.Coordinates + direction);

                if (nextWalkableNode == null && currentNode.UpVectors.Length > 1)
                {
                    for (int i = 0; i < currentNode.UpVectors.Length; ++i)
                    {
                        Vector3Int targetUpVector = currentNode.UpVectors[i];
                        if (currentUpVector != targetUpVector)
                        {
                            Quaternion rotation = Quaternion.FromToRotation(currentUpVector, targetUpVector);
                            Vector3Int rotatedDirection = Vector3IntUtil.Rotate(direction, rotation);

                            nextWalkableNode = m_GridModel.TryGetWalkableNode(currentNode.Coordinates + rotatedDirection);

                            if (nextWalkableNode != null)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            Debug.Assert(nextWalkableNode != null, "Failed to get next walkable node.", this);

            return nextWalkableNode;
        }

        public void UpdatePlayerTarget(IReadOnlyCollection<NodeWalkable> excludedNodes)
        {
            if (AreReferencesAssigned == true)
            {
                NodeWalkable newPlayerTargetNode = GetNewPlayerTargetNode(excludedNodes);

                m_GridView.SetNewTargetNode(newPlayerTargetNode, PlayerTargetNode);

                PlayerTargetNode = newPlayerTargetNode;
            }
        }

        private NodeWalkable GetNewPlayerTargetNode(IReadOnlyCollection<NodeWalkable> excludedNodes)
        {
            NodeWalkable newPlayerTargetNode = default;

            if (m_GridModel != null && excludedNodes.Count < m_GridModel.UnobsctructedWalkableNodeCount)
            {
                do
                {
                    newPlayerTargetNode = m_GridModel.GetRandomWalkableNode();
                }
                while (CheckIfWalkableNodeIsExcluded(newPlayerTargetNode, excludedNodes) == true);
            }

            Debug.Assert(newPlayerTargetNode != null, "Failed to get new player target node.", this);

            return newPlayerTargetNode;
        }

        private bool CheckIfWalkableNodeIsExcluded(NodeWalkable walkableNode, IReadOnlyCollection<NodeWalkable> exludedNodes)
        {
            bool isExcluded = false;

            if (walkableNode != null && exludedNodes != null && exludedNodes.Count > 0)
            {
                foreach (NodeWalkable excludedNode in exludedNodes)
                {
                    if (walkableNode == excludedNode)
                    {
                        isExcluded = true;
                        break;
                    }
                }
            }

            return isExcluded;
        }
    }
}