using UnityEngine;

namespace Snake3D.WorldGrid
{
    public static class WorldGridUtil
    {
        public static int ConvertNodeCoordinatesToIndex(Vector3Int coordinates, int gridSize)
        {
            int index = coordinates.z * (gridSize * gridSize);
            index += coordinates.y * gridSize;
            index += coordinates.x;

            return index;
        }

        public static Vector3Int ConvertNodeIndexToCoordinates(int index, int gridSize)
        {
            Vector3Int coordinates = new Vector3Int(0, 0, 0);

            coordinates.x = index % gridSize;
            index -= coordinates.x;

            coordinates.y = index % (gridSize * gridSize);
            coordinates.z = index - coordinates.y;

            coordinates.y /= gridSize;
            coordinates.z /= gridSize * gridSize;

            return coordinates;
        }
    }
}