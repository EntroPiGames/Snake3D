using System.Collections.Generic;
using UnityEngine;

namespace Snake3D.WorldGrid
{
    public class WorldGridModel
    {
        private Node[] m_GridNodes;
        private List<NodeWalkable> m_UnobstructedWalkableGridNodes;

        public NodeWalkable PlayerStartNode { get; }

        public int GridSize { get; }
        public int UnobsctructedWalkableNodeCount { get { return m_UnobstructedWalkableGridNodes != null ? m_UnobstructedWalkableGridNodes.Count : 0; } }

        public WorldGridModel(Node[] gridNodes, int gridSize, NodeWalkable playerStartNode)
        {
            m_GridNodes = gridNodes;
            GridSize = gridSize;

            PlayerStartNode = playerStartNode;

            CreateGridNodeSubSets();
        }

        public NodeWalkable TryGetWalkableNode(Vector3Int nodeCoordinates)
        {
            NodeWalkable walkableNode = null;

            Node node = TryGetNodeAtCoordinates(nodeCoordinates);
            if (node != null)
            {
                walkableNode = node as NodeWalkable;
            }

            return walkableNode;
        }

        public NodeWalkable GetRandomWalkableNode()
        {
            int randomIndex = Random.Range(0, m_UnobstructedWalkableGridNodes.Count);
            NodeWalkable randomWalkableNode = m_UnobstructedWalkableGridNodes[randomIndex];

            return randomWalkableNode;
        }

        private void CreateGridNodeSubSets()
        {
            m_UnobstructedWalkableGridNodes = new List<NodeWalkable>();

            for (int i = 0; i < m_GridNodes.Length; ++i)
            {
                NodeWalkable walkableNode = m_GridNodes[i] as NodeWalkable;

                if (walkableNode != null && walkableNode.IsObstacle == false)
                {
                    m_UnobstructedWalkableGridNodes.Add(walkableNode);
                }
            }
        }

        private Node TryGetNodeAtCoordinates(Vector3Int nodeCoordinates)
        {
            Node node = default;

            if (CheckIfNodeCoordinatesAreValid(nodeCoordinates) == true)
            {
                int nodeIndex = WorldGridUtil.ConvertNodeCoordinatesToIndex(nodeCoordinates, GridSize);
                node = m_GridNodes[nodeIndex];
            }

            return node;
        }

        private bool CheckIfNodeCoordinatesAreValid(Vector3Int nodeCoordinates)
        {
            bool areCoordinatesValid = true;

            areCoordinatesValid &= nodeCoordinates.x >= 0 && nodeCoordinates.x < GridSize;
            areCoordinatesValid &= nodeCoordinates.y >= 0 && nodeCoordinates.y < GridSize;
            areCoordinatesValid &= nodeCoordinates.z >= 0 && nodeCoordinates.z < GridSize;

            return areCoordinatesValid;
        }
    }
}