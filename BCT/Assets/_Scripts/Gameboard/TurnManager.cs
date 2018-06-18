using Firebase.Database;
using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {

    private GameBoard gameBoard;

    // Active unit for current turn
    public UnitClass ACTIVE_UNIT;

    public int TURN = 0;
    public string TURN_INSTRUCTION = "";
    bool TURN_IS_PROCESSING;

    public bool GAME_STARTED;

    // Queue order of units for same CD conflict resolution
    public List<QueueUnit> queueList;
    // List of queued turn instruction strings
    private List<string> turnQueue;




    // Use this for initialization
    void Start () {

        gameBoard = FindObjectOfType<GameBoard>();

        // Turn queue list
        turnQueue = new List<string>();
    }
	
	// Update is called once per frame
	void Update () {

        // Singleplayer StartGame listener
        if (gameBoard.GAME_MODE == "singleplayer" && !GAME_STARTED && gameBoard.unitList.Count == 6)
        {
            GAME_STARTED = true;

            StartGame();
        }

        // Turn watcher
        if (turnQueue.Count > 0 && TURN_IS_PROCESSING == false)
        {
            TURN_IS_PROCESSING = true;

            StartCoroutine(ProcessTurn());
        }

    }

    private IEnumerator<WaitForSeconds> ProcessTurn()
    {

        yield return new WaitForSeconds(1f);

        string turnInstruction = turnQueue[0];
        turnQueue.RemoveAt(0);

        string[] aData = turnInstruction.Split('|');


        // Process Unit Act
        if (aData[1] == "CACT")
        {

            foreach (UnitClass unit in gameBoard.unitList)
            {
                if (unit.entityID == aData[2])
                {
                    unit.QueueAction(turnInstruction);
                }

            }

        }

        // Process Item Add
        if (aData[1] == "IADD")
        {


            gameBoard.AddItem(null, new Vector3(999, 999, 999), turnInstruction);


        }



    }

    private IEnumerator<WaitForSeconds> AdvanceTurn()
    {

        Debug.Log("========ADVANCE TURN=========");

        // TryItemAdd() if AI Acting
        if (gameBoard.aiController.RUNNING_AI)
        {
            // Try Item Add
            gameBoard.TryItemAdd();

            // reset AI controller
            gameBoard.aiController.RUNNING_AI = false;
        }


        // Send move and TryItemAdd() if acting client
        if (TURN_INSTRUCTION != "")
        {
            if (gameBoard.GAME_MODE == "multiplayer")
            {
                SendMoveToFirebase(ACTIVE_UNIT.playerTeam + "|CACT|" + ACTIVE_UNIT.entityID + "|" + TURN_INSTRUCTION);
            }

            // Try Item Add
            gameBoard.TryItemAdd();
        }
        TURN_INSTRUCTION = "";

        yield return new WaitForSeconds(1f);

        // Check for win, if no win, cycle queueList and Advance turn
        if (gameBoard.CheckWin() != true)
        {

            if ((queueList != null) && (queueList.Count != 0))
            {
                List<QueueUnit> minCooldownList = new List<QueueUnit>();


                // Get a list of units with minimal cooldown
                foreach (QueueUnit unit in queueList)
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
                foreach (QueueUnit unit in queueList)
                {
                    if (unit != null)
                    {
                        unit.unitCooldown = unit.unitCooldown - subtractCooldown;
                    }

                }

                // Update Queue menu
                gameBoard.queueMenu.GetComponent<QueueMenu>().SetQueue(queueList);


                if (minCooldownList.Count > 1)
                {
                    foreach (QueueUnit unit in queueList)
                    {
                        if (minCooldownList.Contains(unit))
                        {
                            minCooldownList[0] = unit;
                            queueList.Remove(unit);
                            queueList.Add(unit);

                            break;
                        }
                    }
                }


                // Set the low CD unit to the Active Unit (if not null)
                foreach (QueueUnit queuedUnit in minCooldownList)
                {
                    if (queuedUnit != null)
                    {
                        foreach (UnitClass unit in gameBoard.unitList)
                        {
                            if (unit.entityID == queuedUnit.entityID)
                            {
                                // Set unit to active unit
                                ACTIVE_UNIT = unit;
                                ACTIVE_UNIT.ACTION_READY = true;
                                ACTIVE_UNIT.MOVE_REMAINING = true;

                                // Top up unit cooldown
                                minCooldownList[0].unitCooldown = minCooldownList[0].unitCooldownMax;

                                // Set selection to active unit / refresh if already selected
                                if (gameBoard.SELECTED_GAME_ENTITY != null && gameBoard.SELECTED_GAME_ENTITY.entityID == ACTIVE_UNIT.entityID)
                                {
                                    gameBoard.RefreshSelectedEntity();
                                }
                                else
                                {
                                    gameBoard.SetSelectedEntity(ACTIVE_UNIT);
                                }

                                break;
                            }
                        }

                    }
                }

            }
        }


    }

    void StartGame()
    {

        queueList = new List<QueueUnit>();

        foreach (UnitClass unit in gameBoard.unitList)
        {

            QueueUnit addQueueUnit = new QueueUnit(unit.entityID, unit.playerTeam, unit.playerName, unit.entityName, unit.unitCooldownMax, unit.unitCooldown);
            queueList.Add(addQueueUnit);

        }

        StartCoroutine(AdvanceTurn());
    }


    // Firebase
    public void SendMoveToFirebase(string move)
    {
        Debug.Log("FIREBASE send: " + move);
        gameBoard.gameDatabaseReference.Child("turns").Push().SetValueAsync(move);
    }

    public void FirebaseTurnListener(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Debug.Log("FIREBASE receive: " + args.Snapshot.Value);

        // Get Move
        string data = args.Snapshot.Value.ToString();
        string[] aData = data.Split('|');

        switch (aData[1])
        {

            // TODO: separate chat listener in separate ChatManager...?
            case "CMSG":

                gameBoard.ReceiveChatMessage(aData[2]);

                break;

            case "CADD":

                gameBoard.AddUnit(null, new Vector3(999, 999, 999), 999, false, data);

                break;

            case "CACT":

                if (Convert.ToInt32(aData[0]) != gameBoard.PLAYER_TEAM)
                {
                    turnQueue.Add(data);
                }

                break;

            case "IADD":

                if (Convert.ToInt32(aData[0]) != gameBoard.PLAYER_TEAM)
                {
                    turnQueue.Add(data);

                }

                break;

        }

    }
    public void StartListenerFirebase(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        // When game has all units, build queue list and advance turn
        if (args.Snapshot.ChildrenCount == 6)
        {

            gameBoard.gameDatabaseReference.Child("units").ValueChanged -= StartListenerFirebase;

            queueList = new List<QueueUnit>();

            foreach (DataSnapshot child in args.Snapshot.Children)
            {

                QueueUnit addQueueUnit = JsonUtility.FromJson<QueueUnit>(child.GetRawJsonValue());
                queueList.Add(addQueueUnit);

            }

            StartCoroutine(AdvanceTurn());

            GAME_STARTED = true;
        }

    }

    public void CoroutineAdvanceTurn(string source)
    {
        Debug.Log(source + " calling turnManager.CoroutineAdvanceTurn()");

        StartCoroutine(AdvanceTurn());
    }

    public void SetTurnProcessingFalse(string source)
    {
        Debug.Log(source + " setting TURN_IS_PROCESSING = false");

        TURN_IS_PROCESSING = false;
    }
}
