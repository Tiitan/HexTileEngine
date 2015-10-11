using UnityEngine;

public static class Vector3Extension
{
    static public Vector3 Mult(this Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
}
