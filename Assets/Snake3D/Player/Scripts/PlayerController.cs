using EntroPi;
using Snake3D.WorldGrid;
using System.Collections.Generic;
using UnityEngine;

namespace Snake3D.Player
{
    [RequireComponent(typeof(PlayerInput))]
    [RequireComponent(typeof(PlayerView))]
    public class PlayerController : EntroPiBehaviour
    {
        [SerializeField]
        private int m_StartLength = 4;

        [SerializeField]
        private int m_GrowLength = 3;

        [RequiredComponent]
        private PlayerInput m_Input = default;

        [RequiredComponent]
        private PlayerView m_View = default;

        private Queue<NodeWalkable> m_PathNodes;
        private Vector3Int m_MovementDirection;
        private Vector3Int m_UpVector;
        private int m_Length;

        public IReadOnlyCollection<NodeWalkable> PathNodes { get { return m_PathNodes; } }
        public NodeWalkable ActiveNode { get; private set; }

        public bool IsAlive { get; private set; } = true;

        public void Initialize(NodeWalkable playerStartNode, Vector3Int playerStartDirection)
        {
            m_Length = m_StartLength;

            if (AreReferencesAssigned == true && playerStartNode != null)
            {
                ActiveNode = playerStartNode;
                m_MovementDirection = playerStartDirection;

                if (ActiveNode != null)
                {
                    m_PathNodes = new Queue<NodeWalkable>();

                    Debug.Assert(ActiveNode.UpVectors.Length == 1, "Player start node has more than one up vector");
                    m_UpVector = ActiveNode.UpVectors[0];

                    m_PathNodes.Enqueue(ActiveNode);
                    m_View.UpdatePath(ActiveNode, m_PathNodes);
                }
            }
        }

        public void MoveToNextPosition(WorldGridController worldGridController)
        {
            UpdateInput();
            UpdatePosition(worldGridController);
            UpdatePath();

            m_View.UpdatePath(ActiveNode, m_PathNodes);
        }

        public void IncreaseLength()
        {
            m_Length += m_GrowLength;
        }

        private void UpdateInput()
        {
            PlayerInput.InputAction inputAction = m_Input.GetAndResetLastInputAction();
            if (inputAction != PlayerInput.InputAction.None)
            {
                const float rightAngle = 90;

                if (inputAction == PlayerInput.InputAction.TurnLeft)
                {
                    UpdateDirection(-rightAngle);
                }
                else if (inputAction == PlayerInput.InputAction.TurnRight)
                {
                    UpdateDirection(rightAngle);
                }
            }
        }

        private void UpdateDirection(float angleInDegrees)
        {
            if (angleInDegrees != 0)
            {
                Quaternion rotation = Quaternion.AngleAxis(angleInDegrees, m_UpVector);
                m_MovementDirection = Vector3IntUtil.Rotate(m_MovementDirection, rotation);
            }
        }

        private void UpdatePosition(WorldGridController worldGridController)
        {
            NodeWalkable nextNode = worldGridController.GetNextWalkableNodeInDirection(ActiveNode, m_MovementDirection, m_UpVector);
            if (nextNode != null && nextNode != ActiveNode)
            {
                NodeWalkable previousNode = ActiveNode;
                ActiveNode = nextNode;

                Vector3Int movementDirection = ActiveNode.Coordinates - previousNode.Coordinates;
                if (movementDirection != m_MovementDirection)
                {
                    Quaternion rotation = Quaternion.FromToRotation(m_MovementDirection, movementDirection);
                    m_UpVector = Vector3IntUtil.Rotate(m_UpVector, rotation);

                    m_MovementDirection = movementDirection;
                }

                // Check before adding active node to path!
                bool collidedWithSelf = m_PathNodes.Contains(ActiveNode) == true;
                bool collidedWithObstacle = ActiveNode.IsObstacle == true;

                m_PathNodes.Enqueue(ActiveNode);

                if (collidedWithSelf == true || collidedWithObstacle == true)
                {
                    Die();
                }
            }
        }

        private void UpdatePath()
        {
            while (m_PathNodes.Count > m_Length)
            {
                NodeWalkable removedNode = m_PathNodes.Dequeue();
                m_View.RemoveNode(removedNode);
            }
        }

        private void Die()
        {
            IsAlive = false;

            m_View.PlayDeathAnimation(ActiveNode, m_PathNodes);
        }
    }
}