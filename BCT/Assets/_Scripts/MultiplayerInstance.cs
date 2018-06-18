using System;
using System.Collections.Generic;


[Serializable]
public class MultiplayerInstance {

    public bool isStarted;
    public string gameKey;
    public List<string> playersList;

    // Empty constructor for firebase
    public MultiplayerInstance()
    {
    }

    public MultiplayerInstance(bool isStarted, string gameKey, List<string> playersList)
    {
        this.isStarted = isStarted;
        this.gameKey = gameKey;
        this.playersList = playersList;

    }
}
