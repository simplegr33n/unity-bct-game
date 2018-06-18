using UnityEngine;

public static class Maps {

    public static Vector3[,] unitCameraPositions = new Vector3[4, 2]
    {
        {new Vector3 (-4f, 5f, -4f), new Vector3 (40, 45, 0) },
        {new Vector3 (4f, 5f, -4f), new Vector3 (40, 315, 0) },
        {new Vector3 (4f, 5f, 4f), new Vector3 (40, 225, 0) },
        {new Vector3 (-4f, 5f, 4f), new Vector3 (40, 135, 0) }

    };


    public static Vector3[,] camerasCanyon = new Vector3[4, 2]
    {
        {new Vector3 (-10f, 23, -10f), new Vector3 (45, 45, 0) },
        {new Vector3 (21f, 23, -11.5f), new Vector3 (45, 315, 0) },
        {new Vector3 (22f, 23, 20f), new Vector3 (45, 225, 0) },
        {new Vector3 (-10f, 23, 21f), new Vector3 (45, 135, 0) }

    };

    public static Vector3[,] camerasMountainBend = new Vector3[4, 2]
    {
        {new Vector3 (-10f, 22.5f, -10f), new Vector3 (45, 45, 0) },
        {new Vector3 (22.5f, 22.5f, -10f), new Vector3 (45, 315, 0) },
        {new Vector3 (23f, 22.5f, 22f), new Vector3 (45, 225, 0) },
        {new Vector3 (-9f, 22.5f, 21f), new Vector3 (45, 135, 0) }

    };

    public static Vector3[,] camerasZarghidas = new Vector3[4, 2]
    {
        {new Vector3 (-11f, 23f, -12f), new Vector3 (45, 45, 0) },
        {new Vector3 (24.5f, 23f, -12f), new Vector3 (45, 315, 0) },
        {new Vector3 (24.5f, 23f, 21f), new Vector3 (45, 225, 0) },
        {new Vector3 (-9.6f, 23f, 21f), new Vector3 (45, 135, 0) }

    };

    public static Vector3[,] camerasTestHill = new Vector3[4, 2]
{
        {new Vector3 (-25, 60, -25), new Vector3 (45, 45, 0) },
        {new Vector3 (-25, 60, -25), new Vector3 (45, 315, 0) },
        {new Vector3 (-25, 60, -25), new Vector3 (45, 225, 0) },
        {new Vector3 (-25, 60, -25), new Vector3 (45, 135, 0) }

};


}
