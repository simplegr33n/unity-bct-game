using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueueMenu : MonoBehaviour {

    private List<QueueUnit> cloneSecondaryUnitQueue;

    private List<QueueUnit> finalQueueList;

    public RawImage profImage0;
    public Text unitName0;
    public Image teamIndicator0;

    public RawImage profImage1;
    public Text unitName1;
    public Image teamIndicator1;

    public RawImage profImage2;
    public Text unitName2;
    public Image teamIndicator2;

    public RawImage profImage3;
    public Text unitName3;
    public Image teamIndicator3;

    public RawImage profImage4;
    public Text unitName4;
    public Image teamIndicator4;

    public RawImage profImage5;
    public Text unitName5;
    public Image teamIndicator5;

    // Player avatars
    public Texture cloudAvatar;
    public Texture blackRookAvatar;
    public Texture insektobotAvatar;
    public Texture medicAvatar;
    public Texture badRobutAvatar;
    public Texture sunManAvatar;


    private void Awake()
    {
        gameObject.SetActive(false);

    }

    public void SetQueue(List<QueueUnit> tertiaryQueueList)
    {

        cloneSecondaryUnitQueue = new List<QueueUnit>();

        finalQueueList = new List<QueueUnit>();



        // Create list allQueueUnitsPriorities of allQueueUnits objects from unitQueueList
        foreach (QueueUnit unit in tertiaryQueueList)
        {

            // TODO: system for checking if not destroyed...
            cloneSecondaryUnitQueue.Add(new QueueUnit(unit.entityID, unit.playerTeam, unit.playerName, unit.entityName, unit.unitCooldownMax, unit.unitCooldown));

        }




        // Sort into Final Queue List
        while (finalQueueList.Count < 6)
        {

            List<QueueUnit> minCooldownList = new List<QueueUnit>();

            foreach (QueueUnit unit in cloneSecondaryUnitQueue)
            {
                if ((minCooldownList.Count == 0) || (unit.unitCooldown < minCooldownList[0].unitCooldown))
                {
                    minCooldownList.Clear();
                    minCooldownList.Add(unit);
                }
                else if (unit.unitCooldown == minCooldownList[0].unitCooldown)
                {
                    minCooldownList.Add(unit);
                }

            }

            int subtractCooldown = minCooldownList[0].unitCooldown;

            // Subtract the lowest cooldown from all of the units cooldowns
            foreach (QueueUnit unit in cloneSecondaryUnitQueue)
            {

                unit.unitCooldown = unit.unitCooldown - subtractCooldown;

            }



            if (minCooldownList.Count > 1)
            {
                foreach (QueueUnit unit in cloneSecondaryUnitQueue)
                {
                    if (minCooldownList.Contains(unit))
                    {
                        finalQueueList.Add(unit);

                        // Move unit to back of Secondary Queue
                        cloneSecondaryUnitQueue.Remove(unit);
                        cloneSecondaryUnitQueue.Add(unit);


                        // Top up unit cooldown
                        unit.unitCooldown = unit.unitCooldownMax;

                        break;
                    }
                }
            } else
            {

                finalQueueList.Add(minCooldownList[0]);

                // Top up unit cooldown
                minCooldownList[0].unitCooldown = minCooldownList[0].unitCooldownMax;


            }



        }

        GameBoard gameBoard = FindObjectOfType<GameBoard>();

        // Update Menu 0
        profImage0.texture = GetAvatar(finalQueueList[0].entityName);
        unitName0.text = finalQueueList[0].entityName;
        if (finalQueueList[0].playerTeam == gameBoard.PLAYER_TEAM)
        {
            teamIndicator0.color = Color.green;
        } else
        {
            teamIndicator0.color = Color.red;
        }

        // Update Menu 1
        profImage1.texture = GetAvatar(finalQueueList[1].entityName);
        unitName1.text = finalQueueList[1].entityName;
        if (finalQueueList[1].playerTeam == gameBoard.PLAYER_TEAM)
        {
            teamIndicator1.color = Color.green; 
        }
        else
        {
            teamIndicator1.color = Color.red;
        }

        // Update Menu 2
        profImage2.texture = GetAvatar(finalQueueList[2].entityName);
        unitName2.text = finalQueueList[2].entityName;
        if (finalQueueList[2].playerTeam == gameBoard.PLAYER_TEAM)
        {
            teamIndicator2.color = Color.green;
        }
        else
        {
            teamIndicator2.color = Color.red;
        }

        // Update Menu 3
        profImage3.texture = GetAvatar(finalQueueList[3].entityName);
        unitName3.text = finalQueueList[3].entityName;
        if (finalQueueList[3].playerTeam == gameBoard.PLAYER_TEAM)
        {
            teamIndicator3.color = Color.green;
        }
        else
        {
            teamIndicator3.color = Color.red;
        }

        // Update Menu 4
        profImage4.texture = GetAvatar(finalQueueList[4].entityName);
        unitName4.text = finalQueueList[4].entityName;
        if (finalQueueList[4].playerTeam == gameBoard.PLAYER_TEAM)
        {
            teamIndicator4.color = Color.green;
        }
        else
        {
            teamIndicator4.color = Color.red;
        }

        // Update Menu 5
        profImage5.texture = GetAvatar(finalQueueList[5].entityName);
        unitName5.text = finalQueueList[5].entityName;
        if (finalQueueList[5].playerTeam == gameBoard.PLAYER_TEAM)
        {
            teamIndicator5.color = Color.green;
        }
        else
        {
            teamIndicator5.color = Color.red;
        }

        gameObject.SetActive(true);

    }

    private Texture GetAvatar(string unitName)
    {
        Texture avatarTexture = null;

        switch (unitName)
        {
            case "Cloud":
                avatarTexture = cloudAvatar;
            

                break;

            case "Medic":
                avatarTexture = medicAvatar;

                break;

            case "Insektobot":
                avatarTexture = insektobotAvatar;

                break;

            case "Black Rook":
                avatarTexture = blackRookAvatar;

                break;

            case "Bad Robut":
                avatarTexture = badRobutAvatar;

                break;

            case "Sun Man":
                avatarTexture = sunManAvatar;

                break;



        }

        return avatarTexture;
    }
}
