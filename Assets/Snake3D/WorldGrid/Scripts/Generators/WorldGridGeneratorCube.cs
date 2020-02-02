using EntroPi;
using UnityEngine;

namespace Snake3D.WorldGrid
{
    [CreateAssetMenu(fileName = "WorldGridGenerator_Cube", menuName = "Snake 3D/World Grid/Generators/Cube")]
    public class WorldGridGeneratorCube : WorldGridGenerator
    {
        private enum ObstaclePattern { Even, Odd }

        [SerializeField, RequiredReference]
        private GameObject m_WalkableNodePrefab = default;

        [SerializeField, RequiredReference]
        private GameObject m_SurfaceNodePrefab = default;

        [SerializeField]
        [Range(4, 32)]
        private int m_GridSize = 10;

        [Header("Obstacles")]
        [SerializeField]
        [Range(0f, 1f)]
        private float m_ObstacleDensity = 0.25f;

        [SerializeField]
        private ObstaclePattern m_ObstaclePattern = ObstaclePattern.Even;

        [SerializeField]
        [Range(1, 8)]
        private int m_MinimumPlayerStartClearance = 3;

        public override Vector3Int PlayerStartDirection { get { return Vector3Int.up; } }

        public override WorldGridModel GenerateWorldGridModel(Transform gridRoot)
        {
            int nodeCount = m_GridSize * m_GridSize * m_GridSize;
            Node[] gridNodes = new Node[nodeCount];

            for (int i = 0; i < nodeCount; ++i)
            {
                Vector3Int nodeCoordinates = WorldGridUtil.ConvertNodeIndexToCoordinates(i, m_GridSize);
                gridNodes[i] = CreateGridNode(nodeCoordinates, m_GridSize, gridRoot);
            }

            NodeWalkable playerStartNode = GetPlayerStartNode(gridNodes, m_GridSize);

            if (m_ObstacleDensity > 0)
            {
                AddObstacles(gridNodes, playerStartNode, m_ObstacleDensity, m_ObstaclePattern, m_MinimumPlayerStartClearance);
            }

            WorldGridModel gridModel = new WorldGridModel(gridNodes, m_GridSize, playerStartNode);

            return gridModel;
        }

        private Node CreateGridNode(Vector3Int nodeCoordinates, int gridSize, Transform gridRoot, float nodeSpacing = 1f)
        {
            Node node = default;

            Vector3Int walkableBoundsMin = Vector3Int.zero;
            Vector3Int walkableBoundsMax = new Vector3Int(gridSize - 1, gridSize - 1, gridSize - 1);

            Vector3Int surfaceBoundsMin = walkableBoundsMin + Vector3Int.one;
            Vector3Int surfaceBoundsMax = walkableBoundsMax - Vector3Int.one;

            if (CheckIfCoordinatesAreOnCubeBoundsSurface(nodeCoordinates, walkableBoundsMin, walkableBoundsMax) == true)
            {
                Vector3Int[] upVectors = CalculateNodeUpVectors(nodeCoordinates, walkableBoundsMin, walkableBoundsMax);
                GameObject nodeView = CreateNodeView(m_WalkableNodePrefab, gridRoot, nodeCoordinates, gridSize, nodeSpacing);

                node = new NodeWalkable(nodeCoordinates, upVectors, nodeView);
            }
            else if (CheckIfCoordinatesAreOnCubeBoundsSurface(nodeCoordinates, surfaceBoundsMin, surfaceBoundsMax) == true)
            {
                Vector3Int[] upVectors = CalculateNodeUpVectors(nodeCoordinates, surfaceBoundsMin, surfaceBoundsMax);
                GameObject nodeView = CreateNodeView(m_SurfaceNodePrefab, gridRoot, nodeCoordinates, gridSize, nodeSpacing);

                node = new NodeSurface(nodeCoordinates, upVectors, nodeView);
            }
            else
            {
                node = new NodeEmpty(nodeCoordinates);
            }

            return node;
        }

        private static GameObject CreateNodeView(GameObject nodePrefab, Transform gridRoot, Vector3Int nodeCoordinates, int gridSize, float nodeSpacing)
        {
            GameObject nodeView = Instantiate(nodePrefab, gridRoot, false);
            nodeView.transform.localPosition = CalculateNodeViewLocalPosition(nodeCoordinates, gridSize, nodeSpacing);

            return nodeView;
        }

        private static NodeWalkable GetPlayerStartNode(Node[] gridNodes, int gridSize)
        {
            Vector3Int playerStartNodeCoordinates = new Vector3Int(gridSize / 2, gridSize / 2, 0);
            int playerStartNodeIndex = WorldGridUtil.ConvertNodeCoordinatesToIndex(playerStartNodeCoordinates, gridSize);

            NodeWalkable playerStartNode = gridNodes[playerStartNodeIndex] as NodeWalkable;

            Debug.Assert(playerStartNode != null, "Invalid player start node!");

            return playerStartNode;
        }

        private static void AddObstacles(Node[] gridNodes, NodeWalkable playerStartNode, float density, ObstaclePattern pattern, int minPlayerStartClearance)
        {
            for (int i = 0; i < gridNodes.Length; ++i)
            {
                NodeWalkable walkableNode = gridNodes[i] as NodeWalkable;
                if (walkableNode != null && walkableNode != playerStartNode)
                {
                    // Don't add obstacles on edges of the cube.
                    if (walkableNode.UpVectors != null && walkableNode.UpVectors.Length == 1)
                    {
                        if (Random.value <= density)
                        {
                            float nodeDistanceToPlayer = Vector3Int.Distance(playerStartNode.Coordinates, walkableNode.Coordinates);
                            if (nodeDistanceToPlayer > minPlayerStartClearance)
                            {
                                if (CheckIfNodeMatchesObstaclePattern(walkableNode, pattern) == true)
                                {
                                    walkableNode.SetAsObstacle();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool CheckIfNodeMatchesObstaclePattern(NodeWalkable walkableNode, ObstaclePattern pattern)
        {
            bool nodeMatchesObstaclePattern = false;

            if (walkableNode != null)
            {
                Vector3Int nodeCoordinates = walkableNode.Coordinates;
                Vector3Int nodeUpVector = walkableNode.UpVectors[0];
                int remainder = pattern == ObstaclePattern.Even ? 0 : 1;

                // Add obstacles in a pattern so that the player can always pass through and the player target is always reachable.
                // Pattern:
                // #-#
                // ---
                // #-#
                nodeMatchesObstaclePattern = true;
                nodeMatchesObstaclePattern &= nodeCoordinates.y % 2 == remainder || Mathf.Abs(nodeUpVector.y) > 0;
                nodeMatchesObstaclePattern &= nodeCoordinates.x % 2 == remainder || Mathf.Abs(nodeUpVector.x) > 0;
                nodeMatchesObstaclePattern &= nodeCoordinates.z % 2 == remainder || Mathf.Abs(nodeUpVector.z) > 0;
            }

            return nodeMatchesObstaclePattern;
        }

        private static Vector3Int[] CalculateNodeUpVectors(Vector3Int nodeCoordinates, Vector3Int cubeBoundsMin, Vector3Int cubeBoundsMax)
        {
            Vector3Int[] upVectors = default;
            int faceCount = CalculateCoordinatesOutsideFaceCount(nodeCoordinates, cubeBoundsMin, cubeBoundsMax);

            Debug.Assert(faceCount > 0, "Calculating up vectors for node with no faces");

            if (faceCount > 0)
            {
                upVectors = new Vector3Int[faceCount];
                int index = 0;

                if (nodeCoordinates.x == cubeBoundsMin.x)
                {
                    upVectors[index] = Vector3Int.left;
                    ++index;
                }
                else if (nodeCoordinates.x == cubeBoundsMax.x)
                {
                    upVectors[index] = Vector3Int.right;
                    ++index;
                }

                if (nodeCoordinates.y == cubeBoundsMin.y)
                {
                    upVectors[index] = Vector3Int.down;
                    ++index;
                }
                else if (nodeCoordinates.y == cubeBoundsMax.y)
                {
                    upVectors[index] = Vector3Int.up;
                    ++index;
                }

                if (nodeCoordinates.z == cubeBoundsMin.z)
                {
                    upVectors[index] = new Vector3Int(0, 0, -1);
                }
                else if (nodeCoordinates.z == cubeBoundsMax.z)
                {
                    upVectors[index] = new Vector3Int(0, 0, 1);
                }
            }

            return upVectors;
        }

        private static bool CheckIfCoordinatesAreOnCubeBoundsSurface(Vector3Int coordinates, Vector3Int cubeBoundsMin, Vector3Int cubeBoundsMax)
        {
            int cubeFaceCount = CalculateCoordinatesOutsideFaceCount(coordinates, cubeBoundsMin, cubeBoundsMax);

            return cubeFaceCount > 0;
        }

        private static int CalculateCoordinatesOutsideFaceCount(Vector3Int coordinates, Vector3Int cubeBoundsMin, Vector3Int cubeBoundsMax)
        {
            int cubeFaceCount = 0;

            // Check if the coordinates are on the outer surface of the bounding cube.
            cubeFaceCount += coordinates.x == cubeBoundsMin.x || coordinates.x == cubeBoundsMax.x ? 1 : 0;
            cubeFaceCount += coordinates.y == cubeBoundsMin.y || coordinates.y == cubeBoundsMax.y ? 1 : 0;
            cubeFaceCount += coordinates.z == cubeBoundsMin.z || coordinates.z == cubeBoundsMax.z ? 1 : 0;

            return cubeFaceCount;
        }

        private static Vector3 CalculateNodeViewLocalPosition(Vector3Int nodeCoordinates, int gridSize, float nodeSpacing)
        {
            float centerOffset = (gridSize / 2f) * nodeSpacing;
            centerOffset -= nodeSpacing / 2f;
            Vector3 positionOffset = new Vector3(centerOffset, centerOffset, centerOffset);

            Vector3 nodeLocalPosition = nodeCoordinates;
            nodeLocalPosition *= nodeSpacing;
            nodeLocalPosition -= positionOffset;

            return nodeLocalPosition;
        }
    }
}