using UnityEngine;

namespace Snake3D.WorldGrid
{
    public class NodeSurface : Node
    {
        public GameObject View { get; }
        public Vector3Int[] UpVectors { get; }

        public NodeSurface(Vector3Int coordinates, Vector3Int[] upVectors, GameObject view) : base(coordinates)
        {
            UpVectors = upVectors;
            View = view;
        }
    }
}