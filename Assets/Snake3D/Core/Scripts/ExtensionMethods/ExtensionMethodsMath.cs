using UnityEngine;

public static class Vector3IntUtil
{
    public static Vector3Int Rotate(Vector3Int value, Quaternion rotation)
    {
        Vector3 vector = value;
        Vector3Int rotatedVector3Int = Vector3Int.RoundToInt(rotation * vector);
        return rotatedVector3Int;
    }
}