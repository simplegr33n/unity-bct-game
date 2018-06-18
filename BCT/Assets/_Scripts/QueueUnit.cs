using System;

[Serializable]
public class QueueUnit
{

    public string entityID;
    public string entityName;
    public int unitCooldownMax;
    public int unitCooldown;
    public int playerTeam;
    public string playerName;



    public QueueUnit(string unitID, int playerTeam, string playerName, string unitName, int unitCooldownMax, int unitCooldown)
    {
        this.entityID = unitID;
        this.playerTeam = playerTeam;
        this.playerName = playerName;
        this.entityName = unitName;
        this.unitCooldownMax = unitCooldownMax;
        this.unitCooldown = unitCooldown;

    }
}
