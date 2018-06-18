using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticRandom
    {
    private static System.Random rnd = new System.Random();
    public static int GetRandom(int min, int max)
    {

        return rnd.Next(min, max);

      
    }
}


