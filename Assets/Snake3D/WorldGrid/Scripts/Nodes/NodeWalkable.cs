using UnityEngine;

namespace Snake3D.WorldGrid
{
    public class NodeWalkable : Node
    {
        public NodeWalkableView View { get; }
        public Vector3Int[] UpVectors { get; }
        public bool IsObstacle { get; private set; } = false;

        public Vector3 ViewPosition { get { return View != null ? View.transform.position : Vector3.zero; } }

        public NodeWalkable(Vector3Int coordinates, Vector3Int[] upVectors, GameObject viewObject) : base(coordinates)
        {
            UpVectors = upVectors;

            View = viewObject.GetAndAssertComponentInChildren<NodeWalkableView>();
            if (View != null)
            {
                View.Node = this;
            }
        }

        public void SetAsObstacle()
        {
            IsObstacle = true;

            if (View != null)
            {
                View.Show(View.ObstacleColor);
            }
        }
    }
}