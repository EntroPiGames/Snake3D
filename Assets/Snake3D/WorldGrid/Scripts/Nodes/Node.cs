using UnityEngine;

namespace Snake3D.WorldGrid
{
    public abstract class Node
    {
        public Vector3Int Coordinates { get; }

        public Node(Vector3Int coordinates)
        {
            Coordinates = coordinates;
        }
    }
}