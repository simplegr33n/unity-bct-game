using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectID
    {

    private static int unitInt = 0;

    public static string CreateID(string playerName)
    {
        unitInt += 1;
        //TODO: better ID system
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int currentTime = (int)(DateTime.UtcNow - epochStart).TotalSeconds;

        return unitInt + playerName + currentTime;
    }
}


