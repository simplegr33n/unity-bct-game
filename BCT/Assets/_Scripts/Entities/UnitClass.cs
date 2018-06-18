using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitClass : EntityClass {

    // Player attributes
    public string playerName;
    public int playerTeam;

    // Unit attributes
    public int unitMoveRadius;
    public int unitHP;
    public int unitHPMax;
    public int unitMP;
    public int unitMPMax;
    public int unitAtkDamage;
    public int unitCooldownMax;
    public int unitCooldown;

    public float unitElevationPotential = 1f;

    // Unit data
    public Texture unitProfileTexture;
    public List<string> ABILITY_LIST;

    // Action / Move machine
    public bool STATUS_EFFECTS_PROCESSED;
    public bool ACTION_READY;
    public bool MOVE_REMAINING;
    private List<string> actionQueue;
    private List<string> moveQueue;
    public bool ACTION_PROCESSING;
    private bool MOVEMENT_PROCESSING;
    public bool IS_MOVING;
    private bool TERRAIN_COLLISION;

    // Status Effect ints (# of turns)
    public int STATUS_DEFENDING = 0;
    public int STATUS_POISONED = 0;

    // Position / Movement vectors
    public Vector3 unitPosition;
    private Vector3 oldPosition;
    //
    private Vector3 targetEndPosition;
    private Vector3 targetTilePosition;

    // AI settings / data
    public bool IS_AI;
    public List<int[]> availableMoveTilesList;



    private void Start()
    {

        // Set Entity Type
        entityType = "UNIT";

        // Find GameBoard 
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();


        // Initialize oldPosition offboard and UpdatePosition()
        oldPosition = new Vector3(-999, -999, -999);
        UpdatePosition();

        // Initialize action queue and move queue
        actionQueue = new List<string>();
        moveQueue = new List<string>();

        // Instantiate Minimap icon and Status bar
        InstantiateMinimapIcon();
        InstantiateStatusBar();


    }

    private void Update()
    {
        // PROCESSING ACTIVE UNIT
        // Reset all unit turn processing related variables if unit is not the ACTIVE_UNIT
        if (gameBoard.turnManager.ACTIVE_UNIT != this)
        {
            ACTION_READY = false;
            MOVE_REMAINING = false;
            IS_MOVING = false;
            actionQueue = new List<string>();
            moveQueue = new List<string>();
            ACTION_PROCESSING = false;
            MOVEMENT_PROCESSING = false;
            STATUS_EFFECTS_PROCESSED = false;
        }

        // TURN PROCESSING
        // Status effect processing
        if (gameBoard.turnManager.ACTIVE_UNIT == this && STATUS_EFFECTS_PROCESSED == false)
        {
            STATUS_EFFECTS_PROCESSED = true;

            if (STATUS_POISONED > 0)
            {
                STATUS_POISONED -= 1;

                ApplyPoisonEffect();
            }

            if (STATUS_DEFENDING > 0)
            {
                STATUS_DEFENDING -= 1;
            }
        }
        // Action processing
        if (actionQueue != null && actionQueue.Count > 0 && !ACTION_PROCESSING)
        {
            ProcessActionQueue();
        }
        // Movement processing
        if (IS_MOVING)
        {
            transform.LookAt(new Vector3(targetTilePosition.x, transform.position.y, targetTilePosition.z));

            // Check if colliding with terrain
            if (TERRAIN_COLLISION == false)
            {

                transform.position = Vector3.MoveTowards(transform.position,
                    targetTilePosition,
                    Time.deltaTime * 3f);
            } else
            {

                // Check if trying to move down or up
                if (targetTilePosition.y < transform.position.y)
                {
                    //transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);

                    //transform.position = new Vector3((targetTilePosition.x - transform.position.x) / 8 + transform.position.x, 
                        //transform.position.y + 0.5f, 
                        //(targetTilePosition.z - transform.position.z) / 8 + transform.position.z);

                    transform.position = Vector3.MoveTowards(transform.position,
                        new Vector3(targetTilePosition.x, targetTilePosition.y, targetTilePosition.z),
                        Time.deltaTime * 10f);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
                }

                TERRAIN_COLLISION = false;
            }

        }
        if (transform.position == targetTilePosition)
        {
            IS_MOVING = false;
        }
        if (moveQueue != null && moveQueue.Count > 0 && MOVEMENT_PROCESSING && !IS_MOVING)
        {
            SetMovementTarget();
        }
        if ((transform.position == targetEndPosition) && (transform.position != unitPosition))
        {
            GetComponent<Animator>().Play("idle");

            IS_MOVING = false;
            MOVEMENT_PROCESSING = false;
            ACTION_PROCESSING = false;

            UpdatePosition();
        }


        // AI PROCESSING
        if (IS_AI && ACTION_READY)
        {
            gameBoard.aiController.ProcessTurnAI(this, gameBoard);
        }
    }


    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log(entityName + " Colliding with terrain...!!");
        TERRAIN_COLLISION = true;

    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(entityName + " Stayin with terrain...");
        TERRAIN_COLLISION = true;

        // Sorta jump the unit if colliding... then MoveToward above in Update()
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);

    }



    public void UpdatePosition()
    {

        if (transform.position == targetEndPosition)
        {
            // Pickup item if it's there
            if (gameBoard.GetEntity(targetEndPosition) != null && gameBoard.GetEntity(targetEndPosition).entityType == "ITEM")
            {
                gameBoard.GetEntity(targetEndPosition).GetComponent<ItemClass>().PickUp(playerTeam);
            }

        }

        oldPosition = unitPosition;

        // Update unitPosition
        unitPosition = transform.position;
        gameBoard.SetEntity(this, unitPosition);


        // Null oldPosition if there was one
        if (oldPosition != new Vector3(-999, -999, -999))
        {
            gameBoard.NullEntityArrayPosition(oldPosition);
        }

        gameBoard.RefreshSelectedEntity();

    }

    // Queue and Process Actions
    public void QueueAction(string turnInstruction)
    {
        string[] aData = turnInstruction.Split('|');

        for (int i = 0; i < aData.Length; i++)
        {

            switch (aData[i])
            {
                case "MOV":

                    actionQueue.Add("MOV|" + aData[i + 1]);


                    break;


                case "ATK":

                    actionQueue.Add("ATK|" + aData[i + 1] + "|" + aData[i + 2]);


                    break;

                case "DEF":

                    actionQueue.Add("DEF|");


                    break;

                case "ITM":

                    actionQueue.Add("ITM|" + aData[i + 1]);



                    break;
            }

        }
    }
    public void ProcessActionQueue()
    {

        ACTION_PROCESSING = true;

        string actionInstruction = actionQueue[0];
        actionQueue.RemoveAt(0);

        Debug.Log(entityName + " Processing Action: " + actionInstruction);

        string[] aData = actionInstruction.Split('|');

        // Process Unit Act
        switch (aData[0])
        {

            case ("MOV"):
                gameBoard.HideIndicatorPlanes();
                MOVE_REMAINING = false;
                MOVEMENT_PROCESSING = true;
                gameBoard.entityMenu.SetAttackMenu();

                string[] bData = aData[1].Split('*');

                int endX = Convert.ToInt32(bData[bData.Length - 2].Split(':')[0]);
                int endZ = Convert.ToInt32(bData[bData.Length - 2].Split(':')[1]);

                targetEndPosition = new Vector3(endX, gameBoard.TileArray[endX, endZ].tile_elevation, endZ);

                foreach (string location in bData)
                {
                    if (location != "")
                    {
                        moveQueue.Add(location);
                    }
                }

                // Start movement
                SetMovementTarget();



                break;

            case ("ATK"):
                int atkX = Convert.ToInt32(aData[1]);
                int atkZ = Convert.ToInt32(aData[2]);

                StartCoroutine(UnitAttack(new Vector3(atkX, gameBoard.TileArray[atkX, atkZ].tile_elevation + 0.5f, atkZ)));
                break;

            case ("DEF"):

                UnitDefend();
                break;

            case ("ITM"):

                UnitUseItem(aData[1]);
                break;

        }
    }

    // Unit Show functions
    public void ShowMoves()
    {
        // refresh available move tiles list
        availableMoveTilesList = new List<int[]>();

        gameBoard.HideIndicatorPlanes();

        if ((ACTION_READY) && (MOVE_REMAINING))
        {
            gameBoard.TileArray[(int) transform.position.x, (int) transform.position.z]
            .CheckMove(unitMoveRadius, 999, null, this);
        }

    }

    public void SetMovementTarget()
    {

        GetComponent<Animator>().Play("move");

        string movementInstruction = moveQueue[0];
        moveQueue.RemoveAt(0);

        string[] aData = movementInstruction.Split(':');

        int targetX = Convert.ToInt32(aData[0]);
        int targetZ = Convert.ToInt32(aData[1]);

        targetTilePosition = new Vector3(targetX, gameBoard.TileArray[targetX, targetZ].tile_elevation, targetZ);

        IS_MOVING = true;

        // TODO: not find this every time...
        if (playerTeam == gameBoard.PLAYER_TEAM)
        {
            FindObjectOfType<EntityMenu>().SetAttackMenu();
        }



    }

    public void ShowAttacks()
    {
        gameBoard.HideIndicatorPlanes();

        if ((ACTION_READY) && (playerTeam == gameBoard.PLAYER_TEAM))
        {

            foreach (Tile tile in gameBoard.TileArray)
            {



                tile.currentUnit = this;

                if (((Mathf.Abs(transform.position.x - tile.roundedPosition.x) <= 1) &&
                    (Mathf.Abs(transform.position.z - tile.roundedPosition.z) == 0)) ||
                    ((Mathf.Abs(transform.position.z - tile.roundedPosition.z) <= 1) &&
                    (Mathf.Abs(transform.position.x - tile.roundedPosition.x) == 0)))
                {

                    // Ensure height within 1
                    if (Mathf.Abs(transform.position.y - tile.roundedPosition.y) <= 1)
                    {
                        tile.SetAttackIndicator();
                    }

                }

            }


        }


    }


    public IEnumerator<WaitForSeconds> UnitAttack(Vector3 targetPosition)
    {

        if (ACTION_READY)
        {
            // Disable unit MOVE_REMAINING and IS_ACTIVE
            MOVE_REMAINING = false;
            ACTION_READY = false;

            int intX = Mathf.RoundToInt(targetPosition.x);
            int intZ = Mathf.RoundToInt(targetPosition.z);

            gameBoard.HideIndicatorPlanes();
            transform.LookAt(new Vector3(targetPosition.x, transform.position.y, targetPosition.z));

            GetComponent<Animator>().Play("attack");

            yield return new WaitForSeconds(0.5f);


            gameBoard.effectDisplayer.CreateEffect(gameBoard.effectDisplayer.explosion, new Vector3(intX, gameBoard.TileArray[intX, intZ].tile_elevation + 0.5f, intZ), Quaternion.identity);


            // GOTO next turn (Early here incase unit attacks self...)
            gameBoard.turnManager.CoroutineAdvanceTurn(entityName + " UnitAttack");
            gameBoard.turnManager.SetTurnProcessingFalse(entityName + " UnitAttack");

            ACTION_PROCESSING = false;

            if (gameBoard.EntityArray[intX, intZ] != null)
            {
                int realDamage = unitAtkDamage;

                // If unit defending, reduce damage and remove defend status
                if (gameBoard.EntityArray[intX, intZ].GetComponent<UnitClass>().STATUS_DEFENDING > 0)
                {
                    gameBoard.EntityArray[intX, intZ].GetComponent<UnitClass>().STATUS_DEFENDING = 0;
                    realDamage = unitAtkDamage - (unitAtkDamage / 3);
                }
                gameBoard.EntityArray[intX, intZ].GetComponent<UnitClass>().TakeDamage(realDamage);
                gameBoard.effectDisplayer.CreatePopupText("" + realDamage, new Vector3(intX, gameBoard.TileArray[intX, intZ].tile_elevation + 1f, intZ), Color.red);

            }

            yield return new WaitForSeconds(0.5f);

            GetComponent<Animator>().Play("idle");

            // Refresh selected entity
            gameBoard.RefreshSelectedEntity();
        }

    }

    public void UnitUseItem(string itemName)
    {
        // TODO: add some defence...
        gameBoard.HideIndicatorPlanes();

        if (ACTION_READY)
        {
            // Disable unit MOVE_REMAINING and IS_ACTIVE
            MOVE_REMAINING = false;
            ACTION_READY = false;


            gameBoard.entityMenu.SetAttackMenu();

            // Remove item from player inventory (only here, for own player...)
            gameBoard.PLAYER_INVENTORY.Remove(itemName);

            // Do item stuff
            switch (itemName)
            {
                case "Health Potion":
                    gameBoard.effectDisplayer.CreatePopupText(HealthPotion.UseHealthPotion(this).ToString()
                        , new Vector3(transform.position.x, gameBoard.TileArray[(int)transform.position.x, (int)transform.position.z].tile_elevation + 1f, transform.position.z)
                        , Color.green);

                    break;

                case "Mana Potion":
                    gameBoard.effectDisplayer.CreatePopupText(ManaPotion.UseManaPotion(this).ToString()
                        , new Vector3(transform.position.x, gameBoard.TileArray[(int)transform.position.x, (int)transform.position.z].tile_elevation + 1f, transform.position.z)
                        , Color.blue);

                    break;


            }

            // Refresh selected entity
            gameBoard.RefreshSelectedEntity();

            // GOTO next turn
            gameBoard.turnManager.CoroutineAdvanceTurn(entityName + " UnitUseItem");
            gameBoard.turnManager.SetTurnProcessingFalse(entityName + " UnitUseItem");
            ACTION_PROCESSING = false;
        }

    }

    public void UnitDefend()
    {
        if (ACTION_READY)
        {
            // Disable unit MOVE_REMAINING and IS_ACTIVE
            MOVE_REMAINING = false;
            ACTION_READY = false;

            // TODO: add some defence...
            gameBoard.HideIndicatorPlanes();

            gameBoard.entityMenu.SetAttackMenu();



            gameBoard.effectDisplayer.CreatePopupText("DEF", new Vector3(transform.position.x, gameBoard.TileArray[(int)transform.position.x, (int)transform.position.z].tile_elevation + 1f, transform.position.z), Color.white);

            // Set status to defend for 1 turn
            STATUS_DEFENDING = 1;

            // Refresh selected entity
            gameBoard.RefreshSelectedEntity();

            // GOTO next turn
            gameBoard.turnManager.CoroutineAdvanceTurn(entityName + " UnitDefend");
            gameBoard.turnManager.SetTurnProcessingFalse(entityName + " UnitDefend");
            ACTION_PROCESSING = false;
        }

    }


    // Damage taking functions
    public void TakeDamage(int damageAmount)
    {
        unitHP = unitHP - damageAmount;
        if (unitHP <= 0)
        {
            UnitDeath();
        }
    }
    public void ApplyPoisonEffect()
    {
        int damageAmount = (unitHPMax / 10);



        // Set HP to minimum of 1 from poison damage
        int unitHPpre = unitHP;
        int calcHP = unitHPpre - damageAmount;
        if (calcHP <= 0)
        {
            unitHP = 1;
        } else
        {
            unitHP = unitHP - damageAmount;
        }

        // Show popup text
        gameBoard.effectDisplayer.CreatePopupText((unitHPpre - unitHP).ToString(), transform.position, Color.magenta);

        // Refresh entity menu if unit was Selected
        if (gameBoard.SELECTED_GAME_ENTITY == this)
        {
            gameBoard.entityMenu.SetSelectedEntity(this);
        }

    }

    // TODO: don't just delete the gameobject...
    // // // animate something, leave a gravestone behind, have a few turns of incapacitated, etc. 
    public void UnitDeath()
    {
        gameBoard.RemoveUnit(entityID);
        Destroy(gameObject);
    }




    // For minimap icon / status bar
    private void InstantiateMinimapIcon()
    {


        GameObject minimapIcon = Instantiate(gameBoard.minimapIconPrefab);

        minimapIcon.transform.SetParent(transform, false);
        minimapIcon.transform.position = new Vector3(transform.position.x, 10, transform.position.z);


        if (playerTeam == gameBoard.PLAYER_TEAM)
        {
            minimapIcon.GetComponentInChildren<TextMesh>().text = ":)";
            minimapIcon.GetComponentInChildren<TextMesh>().color = Color.green;

        }
        else
        {
            minimapIcon.GetComponentInChildren<TextMesh>().text = ":(";
            minimapIcon.GetComponentInChildren<TextMesh>().color = Color.red;
        }


    }
    private void InstantiateStatusBar()
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);

        GameObject statusBar = Instantiate(gameBoard.statusBarPrefab);

        statusBar.transform.SetParent(gameBoard.canvas.transform, false);
        statusBar.transform.position = screenPosition;
        statusBar.GetComponent<StatusBar>().unit = this;

    }




    void OnMouseDown()
    {
        gameBoard.SetSelectedEntity(this);
    }


}