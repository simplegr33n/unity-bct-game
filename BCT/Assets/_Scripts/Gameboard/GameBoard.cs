using Firebase;
using Firebase.Unity.Editor;
using Firebase.Database;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameBoard : MonoBehaviour {

    // GameManager instance
    public GameManager gameManager;
    public MusicManager musicManager;
    public TurnManager turnManager;

    public AiController aiController;

    public CameraController cameraController;

    public string GAME_MODE;

    bool CHAIN_RIGHT_CLICK;


    // From GameManager
    public int PLAYER_TEAM;
    private string GAME_ID;
    private string PLAYER_NAME;
    private string PLAYER_ID;

    // For storing player items (TODO: consider storing items within units // maybe max 6 without costing turn to help balance // always visible in entity menu?)
    public List<string> PLAYER_INVENTORY;

    // Entity selection
    public EntityClass SELECTED_GAME_ENTITY;
    public GameObject selectedEntityArrow;


    // Firebase references
    public DatabaseReference gameDatabaseReference;
    private DatabaseReference usersDatabaseReference;

    // Mapping components
    public Tile multiTile;
    string[,] MapArray;
    public Tile[,] TileArray;
    public EntityClass[,] EntityArray;

    // Map size
    public int xSize;
    public int zSize;
    // Map Prefabs
    public GameObject canyonMap;
    public GameObject mountainBendMap;
    public GameObject zargidasMap;
    public GameObject testHillMap;



    // For explosions/popuptext/ect.
    public EffectDisplayer effectDisplayer;

    // UI objects
    // Canvas and menus
    public GameObject canvas;
    public EntityMenu entityMenu;
    public GameObject queueMenu;
    public GameObject winDisplay;
    // Minimap
    public GameObject minimapIconPrefab;
    // Status bar
    public GameObject statusBarPrefab;
    public GameObject statusEffectPrefab;
    // Chat UI
    public GameObject chatUI;
    public Transform chatMessageContainer;
    public GameObject chatMessagePrefab;
    public GameObject chatScroll;
    // Mute and Unmute button
    public GameObject muteButton;
    public Texture musicPlayingTexture;
    public Texture musicMutedTexture;




    // Lists for in game units
    public List<UnitClass> unitList;


    // ENTITIES  
    // UNIT TYPES (TODO: only access units in game?)
    public UnitClass unitCloud;
    public UnitClass unitBlackRook;
    public UnitClass unitMedic;
    public UnitClass unitInsektobot;
    public UnitClass unitBadRobut;
    public UnitClass unitSunMan;
    // ITEM TYPES
    public ItemClass itemHealthPotion;
    public ItemClass itemManaPotion;

    // Indicator Materials
    // TODO: maybe hold these somewhere else
    public Material moveIndicatorMaterial;
    public Material attackIndicatorMaterial;
    public Material abilityIndicatorMaterial;
    public Material enemyIndicatorMaterial;
    public Material invisibleIndicatorMaterial;
    // Highlight plane Prefab
    public GameObject highlightPlane;

    





    // START FUNCTIONS

    //TODO: REMOVE
    public void DEBUGaddUnits()
    {

        GameObject addButton = GameObject.Find("DebugAddUnitButton");
        Destroy(addButton);

        if (GAME_MODE == "multiplayer")
        {

            if (PLAYER_TEAM == 0)
            {
                AddUnit(unitCloud, new Vector3(1, 999, 1), 0, false, null);
                AddUnit(unitBlackRook, new Vector3(2, 999, 1), 0, false, null);
                AddUnit(unitInsektobot, new Vector3(1, 999, 2), 0, false, null);
            }

            if (PLAYER_TEAM == 1)
            {
                AddUnit(unitSunMan, new Vector3(xSize - 3, 999, zSize - 2), 1, false, null);
                AddUnit(unitBadRobut, new Vector3(xSize - 2, 999, zSize - 3), 1, false, null);
                AddUnit(unitInsektobot, new Vector3(xSize - 4, 999, zSize - 2), 1, false, null);
            }

        }
        else
        {

            AddUnit(unitCloud, new Vector3(1, 999, 1), 0, false, null);
            AddUnit(unitSunMan, new Vector3(2, 999, 1), 0, false, null);
            AddUnit(unitBlackRook, new Vector3(1, 999, 2), 0, false, null);

            AddUnit(unitInsektobot, new Vector3(xSize - 3, 999, zSize - 2), 1, true, null);
            AddUnit(unitBadRobut, new Vector3(xSize - 2, 999, zSize - 3), 1, true, null);
            AddUnit(unitInsektobot, new Vector3(xSize - 4, 999, zSize - 2), 1, true, null);


        }


    }



    void Awake()
    {
        canvas = GameObject.Find("Canvas");
        entityMenu.SetVisible(false);
        winDisplay.SetActive(false);

        gameManager = FindObjectOfType<GameManager>();
        musicManager = FindObjectOfType<MusicManager>();
        turnManager = FindObjectOfType<TurnManager>();
        cameraController = FindObjectOfType<CameraController>();

        GAME_ID = gameManager.GAME_ID;
        PLAYER_TEAM = gameManager.PLAYER_TEAM;
        PLAYER_NAME = gameManager.PLAYER_NAME;
        PLAYER_ID = gameManager.PLAYER_ID;

        if (musicManager.IS_MUTED)
        {
            muteButton.GetComponentInChildren<RawImage>().texture = musicMutedTexture;
        }

        // If GAME_ID is not null, set up firebase for Multiplayer
        // else set up for Single Player
        if (GAME_ID != null)
        {
            GAME_MODE = "multiplayer";

            // Set up the Editor before calling into the realtime database.
            FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://cbtalpha.firebaseio.com/");

            // Get the game reference
            gameDatabaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child("games").Child("active").Child(GAME_ID);
            usersDatabaseReference = FirebaseDatabase.DefaultInstance.RootReference.Child("users");

            // Firebase turn reference + listener
            gameDatabaseReference.Child("turns").ChildAdded += turnManager.FirebaseTurnListener;

            // Firebase chat reference + listener
            gameDatabaseReference.Child("chat").ChildAdded += FirebaseChatListener;

            // Listen for all units added
            gameDatabaseReference.Child("units").ValueChanged += turnManager.StartListenerFirebase;

        } else
        {
            GAME_MODE = "singleplayer";

            chatUI.SetActive(false);
        }


        // Get map, set cameras, tile array, and entity array
        LoadMap();

        // Units List
        unitList = new List<UnitClass>();

        // Player Inventory
        PLAYER_INVENTORY = new List<string>();

    }

    void Update()
    {
        // Ensure chatScroll is bottomed out 
        // TODO: do this outside of Update()...
        chatScroll.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;

        // Break CHAIN_RIGHT_CLICK any time left click is pressed
        if (Input.GetMouseButtonDown(0))
        {
            CHAIN_RIGHT_CLICK = false;
        }

        // Undo current action and set CHAIN_RIGHT_CLICK to true or deselect entity
        if (Input.GetMouseButtonDown(1))
        {
            if (CHAIN_RIGHT_CLICK == false)
            {
                CHAIN_RIGHT_CLICK = true;
                entityMenu.inventoryMenu.SetActive(false);
                entityMenu.abilityMenu.SetActive(false);
                RefreshSelectedEntity();
            } else
            {
                CHAIN_RIGHT_CLICK = false;
                SetSelectedEntity(null);
            }
        }
    }

    // GetMap
    private void LoadMap()
    {

        switch (gameManager.MAP_NAME)
        {
            case ("CANYON"):
                xSize = 12;
                zSize = 10;
                Instantiate(canyonMap, new Vector3((xSize / 2) -.5f, 0, (zSize / 2) -.5f), Quaternion.Euler(0, 0, 0));
                cameraController.cameraPositions = Maps.camerasCanyon;

                break;

            case ("MOUNTAIN_BEND"):
                xSize = 14;
                zSize = 12;
                Instantiate(mountainBendMap, new Vector3((xSize / 2) - .5f, 0, (zSize / 2) - .5f), Quaternion.Euler(-90, 270, 0));
                cameraController.cameraPositions = Maps.camerasMountainBend;

                break;

            case ("ZARGHIDAS_TRADE"):
                xSize = 16;
                zSize = 10;
                Instantiate(zargidasMap, new Vector3((xSize / 2) - .5f, 0, (zSize / 2) - .5f), Quaternion.Euler(0, 180, 0));
                cameraController.cameraPositions = Maps.camerasZarghidas;

                break;

            case ("TEST_HILL"):
                xSize = 30;
                zSize = 30;
                Instantiate(testHillMap, new Vector3((xSize / 2) - .5f, 0, (zSize / 2) - .5f), Quaternion.Euler(0, 0, 0));
                cameraController.cameraPositions = Maps.camerasTestHill;

                break;
        }

        // Set intial camera positions
        cameraController.InitialSetCameras();

        // Build tile and entity arrays
        BuildTileArray();
        BuildEntityArray();

    }

    // Create Tile and Entity arrays
    private void BuildTileArray()
    {
        // TileArray from in game Tiles
        GameObject tileArray = GameObject.Find("TileArray");
        Tile[] tiles = tileArray.GetComponentsInChildren<Tile>();
        TileArray = new Tile[xSize, zSize];


        foreach (Tile tile in tiles)
        {
            // TODO: try without rounding... might be necessary for angled tiles?
            TileArray[Mathf.RoundToInt(tile.transform.position.x), Mathf.RoundToInt(tile.transform.position.z)] = tile;
        }
    }
    private void BuildEntityArray()
    {
        EntityArray = new EntityClass[xSize, zSize];
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {

                EntityArray[x, z] = null;

            }
        }
    }



    // Unit functions
    public void AddUnit(UnitClass unitType, Vector3 addPosition, int unitTeam, bool is_ai, string unitInfoFromServer)
    {

        if (unitInfoFromServer == null)
        {

            // Instantiate unit and instantiate starting cooldown and player
            UnitClass newUnit = Instantiate(unitType, new Vector3(addPosition.x, TileArray[(int) addPosition.x, (int) addPosition.z].tile_elevation, addPosition.z), Quaternion.identity);

            // Point unit at center of map...
            newUnit.transform.LookAt(new Vector3(5, newUnit.transform.position.y, 5));

            newUnit.playerName = PLAYER_NAME;
            newUnit.playerTeam = unitTeam;

            // Create unitID and Cooldown
            newUnit.entityID = GameObjectID.CreateID(PLAYER_NAME);
            int initRandomCooldown = StaticRandom.GetRandom(1, newUnit.unitCooldownMax);
            newUnit.unitCooldown = initRandomCooldown;

            if (is_ai)
            {
                newUnit.IS_AI = true;
            }


            // Add to local unitList
            unitList.Add(newUnit);

            // Add to unit firebase unit list
            if (GAME_MODE == "multiplayer")
            {
                string unitJson = JsonUtility.ToJson(newUnit);
                gameDatabaseReference.Child("units").Push().SetRawJsonValueAsync(unitJson);
                turnManager.SendMoveToFirebase(PLAYER_TEAM + "|CADD|" + newUnit.playerName + "|" + newUnit.entityID + "|" + newUnit.entityName + "|" + addPosition.x + "|" + addPosition.z + "|" + newUnit.unitCooldown);
            }

        }
        else
        {

            string[] aData = unitInfoFromServer.Split('|');

            if (Convert.ToInt32(aData[0]) != PLAYER_TEAM)
            {

                string unitName = aData[4].Replace("(Clone)", "");

                // Determine unit type
                switch (unitName)
                {
                    case "Cloud":
                        unitType = unitCloud;
                        break;
                    case "Black Rook":
                        unitType = unitBlackRook;
                        break;
                    case "Medic":
                        unitType = unitMedic;
                        break;
                    case "Insektobot":
                        unitType = unitInsektobot;
                        break;
                    case "Bad Robut":
                        unitType = unitBadRobut;
                        break;
                    case "Sun Man":
                        unitType = unitSunMan;
                        break;
                }

                int xCoord = Convert.ToInt32(aData[5]);
                int zCoord = Convert.ToInt32(aData[6]);

                addPosition.x = xCoord;
                addPosition.y = TileArray[xCoord, zCoord].tile_elevation;
                addPosition.z = zCoord;

                // Instantiate unit and instantiate starting cooldown and player
                UnitClass newUnit = Instantiate(unitType, addPosition, Quaternion.identity);

                newUnit.playerName = aData[2];
                newUnit.playerTeam = Convert.ToInt32(aData[0]);
                newUnit.entityID = aData[3];
                newUnit.unitCooldown = Convert.ToInt32(aData[7]);

                // Look at center of map...
                newUnit.transform.LookAt(new Vector3(5, transform.position.y, 5));

                // Add to local unitList
                unitList.Add(newUnit);

            }

        }

    }
    public void RemoveUnit(string unitID)
    {
        foreach (QueueUnit unit in turnManager.queueList)
        {
            if (unit.entityID == unitID)
            {
                turnManager.queueList.Remove(unit);
                break;
            }
        }
    }

    // Item Functions
    public void TryItemAdd()
    {

        System.Random rnd = new System.Random();

        int itemLottery = rnd.Next(0, 2);

        if (itemLottery == 0)
        {

            int tryX = rnd.Next(0, xSize);
            int tryZ = rnd.Next(0, zSize);
            Vector3 tryPosition = new Vector3(tryX, TileArray[tryX, tryZ].tile_elevation, tryZ);


            // if there's a space in the entity array, place item
            if (EntityArray[tryX, tryZ] == null && TileArray[tryX, tryZ].isWalkable)
            {
                itemLottery = rnd.Next(0, 2);

                switch (itemLottery)
                {
                    case (0):
                        AddItem(itemHealthPotion, tryPosition, null);
                        break;

                    case (1):
                        AddItem(itemManaPotion, tryPosition, null);
                        break;
                }
         
            }
        }

    }
    public void AddItem(ItemClass itemType, Vector3 addPosition, string itemInfoFromServer)
    {

        if (itemInfoFromServer == null)
        {

            // Instantiate unit and instantiate starting cooldown and player
            ItemClass newItem = Instantiate(itemType, new Vector3(addPosition.x, TileArray[(int) addPosition.x, (int) addPosition.z].tile_elevation, addPosition.z), Quaternion.identity);

            // Create entityID
            newItem.entityID = GameObjectID.CreateID("ITEM");

            // Send item to firebase
            if (GAME_MODE == "multiplayer")
            {
                turnManager.SendMoveToFirebase(PLAYER_TEAM + "|IADD|" + newItem.entityID + "|" + newItem.entityName + "|" + addPosition.x + "|" + addPosition.z);
            }

        }
        else
        {

            string[] aData = itemInfoFromServer.Split('|');

            if (Convert.ToInt32(aData[0]) != PLAYER_TEAM)
            {

                string itemName = aData[3].Replace("(Clone)", "");

                // Determine item type
                switch (itemName)
                {
                    case "Health Potion":
                        itemType = itemHealthPotion;
                        break;
                    case "Mana Potion":
                        itemType = itemManaPotion;
                        break;
                }

                int xCoord = Convert.ToInt32(aData[4]);
                int zCoord = Convert.ToInt32(aData[5]);

                addPosition.x = xCoord;
                addPosition.y = TileArray[xCoord, zCoord].tile_elevation;
                addPosition.z = zCoord;

                // Instantiate item and set entityID (... might be unnecessary step)
                ItemClass newItem = Instantiate(itemType, addPosition, Quaternion.identity);

                newItem.entityID = aData[2];

                // Set TURN_PROCESSING to false since AdvanceTurn() is not called here
                turnManager.SetTurnProcessingFalse("AddItem from Firebase");

            }
        }

    }


    // Gameboard UI functions
    public void HideIndicatorPlanes()
    {

        foreach (Tile tile in TileArray)
        {
            if (tile != null)
            {
                tile.HideIndicator();
            }
        }

    }
    public void HideCursorPlanes()
    {

        foreach (Tile tile in TileArray)
        {

            tile.highlightPlane.SetActive(false);

        }

    }

    // Entity selection functions
    public void SetSelectedEntity(EntityClass selectedEntity)
    {

        // Hide any previously shown available moves
        HideIndicatorPlanes();

        // Remove focus from previously focused unit
        if (selectedEntity == SELECTED_GAME_ENTITY || selectedEntity == null)
        {
            // Remove entity arrow
            GameObject[] oldArrows = GameObject.FindGameObjectsWithTag("EntityArrow");
            foreach (GameObject arrow in oldArrows)
            {
                Destroy(arrow);
            }

            SELECTED_GAME_ENTITY = null;
            entityMenu.SetVisible(false);

        }
        else
        {
            // Set SELECTED_GAME_OBJECT
            SELECTED_GAME_ENTITY = selectedEntity;

            // Remove entity and then reset arrow
            GameObject[] oldArrows = GameObject.FindGameObjectsWithTag("EntityArrow");
            foreach (GameObject arrow in oldArrows)
            {
                Destroy(arrow);
            }

            GameObject newEntityArrow = Instantiate(selectedEntityArrow);
            newEntityArrow.transform.SetParent(selectedEntity.transform, false);
            newEntityArrow.transform.position = new Vector3(selectedEntity.transform.position.x, selectedEntity.transform.position.y, selectedEntity.transform.position.z);



            if (selectedEntity.entityType == "UNIT")
            {
                UnitClass unit = selectedEntity.GetComponent<UnitClass>();

                // Show moves (if own unit)
                if (unit.playerTeam == PLAYER_TEAM && unit.ACTION_READY)
                {
                    unit.ShowMoves();
                }

                // Set UI
                entityMenu.SetSelectedEntity(SELECTED_GAME_ENTITY);

                if (unit.unitHP <= 0)
                {
                    entityMenu.SetVisible(false);
                }
                else
                {
                    entityMenu.SetVisible(true);
                }
            }

        }

    }
    public void RefreshSelectedEntity()
    {
        if (SELECTED_GAME_ENTITY != null)
        {

            if (SELECTED_GAME_ENTITY.entityType == "UNIT")
            {

                UnitClass unit = SELECTED_GAME_ENTITY.GetComponent<UnitClass>();

                // Show new moves
                // Show moves (if own unit)
                if (unit.playerTeam == PLAYER_TEAM && unit.ACTION_READY)
                {
                    unit.ShowMoves();
                }

                // Set UI
                entityMenu.SetSelectedEntity(SELECTED_GAME_ENTITY);

                if (SELECTED_GAME_ENTITY.GetComponent<UnitClass>().unitHP <= 0)
                {
                    entityMenu.SetVisible(false);
                }
                else
                {
                    entityMenu.SetVisible(true);
                }
            }

        } else
        {
            entityMenu.SetVisible(false);
        }
    }
    public void ClearSelectedGameObject()
    {

        // Hide showing indicator planes
        HideIndicatorPlanes();

        // Remove entity arrow
        GameObject[] oldArrows = GameObject.FindGameObjectsWithTag("EntityArrow");
        foreach (GameObject arrow in oldArrows)
        {
            Destroy(arrow);
        }

        SELECTED_GAME_ENTITY = null;

    }


    // For Entity Array
    // TODO: think about whether you really need an entity array... a lot could probably be simplified without
    // // // that is to say, we already have objects with int X,Y and Z locations - an extra dimension and would save having to update a synchronized array...
    // // // --- can easily just contruct a list of tagged GameObjects...
    public void SetEntity(EntityClass entity, Vector3 location)
    {
        int intX = (int)location.x;
        int intZ = (int)location.z;

        EntityArray[intX, intZ] = entity;
    }
    public EntityClass GetEntity(Vector3 location)
    {
        int intX = (int)location.x;
        int intZ = (int)location.z;
        if (EntityArray[intX, intZ] == null)
        {
            return null;
        }
        else
        {
            return EntityArray[intX, intZ];
        }

    }
    public void NullEntityArrayPosition(Vector3 location)
    {
        int intX = (int)location.x;
        int intZ = (int)location.z;
        EntityArray[intX, intZ] = null;
    }
    // TODO: remove, just use GetEntity()
    public bool IsNullEntity(Vector3 location)
    {
        int intX = (int)location.x;
        int intZ = (int)location.z;
        if (EntityArray[intX, intZ] == null)
        {
            return true;
        } else
        {
            return false;
        }

    }

    // Check for walkability of tile...
    // TODO: move to UnitClass?
    public bool IsWalkableTile(Vector3 location)
    {
        int intX = (int)location.x;
        int intZ = (int)location.z;
        if (EntityArray[intX, intZ] == null || EntityArray[intX, intZ].GetComponent<EntityClass>().entityType == "ITEM" )
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    // Chat
    public void SendChatMessage()
    {
        string message = GameObject.Find("MessageInput").GetComponent<InputField>().text;
        gameDatabaseReference.Child("chat").Push().SetValueAsync(PLAYER_NAME + ": " + message);

    }
    public void ReceiveChatMessage(string message)
    {
        GameObject messageObject = Instantiate(chatMessagePrefab) as GameObject;
        messageObject.transform.SetParent(chatMessageContainer);

        messageObject.GetComponentInChildren<Text>().text = message;

    }
    void FirebaseChatListener(object sender, ChildChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        // Get String
        string message = args.Snapshot.Value.ToString();

        ReceiveChatMessage(message);



    }





    // Game functions
    public bool CheckWin()
    {
        bool playerZeroLives = false;
        bool playerOneLives = false;

        foreach (QueueUnit qUnit in turnManager.queueList)
        {
            if (qUnit.playerTeam == 0)
            {
                playerZeroLives = true;
            }

            if (qUnit.playerTeam == 1)
            {
                playerOneLives = true;
            }

        }

        if (!(playerZeroLives && playerOneLives))
        {

            // Hide unnecessary menus
            queueMenu.SetActive(false);
            entityMenu.SetVisible(false);

            if (playerZeroLives)
            {

                if (PLAYER_TEAM == 0)
                {
                    //win
                    StartCoroutine(WinDisplay());

                }
                else
                {
                    //lose
                    StartCoroutine(LoseDisplay());


                }
            }

            if (playerOneLives)
            {
                if (PLAYER_TEAM == 1)
                {
                    //win
                    StartCoroutine(WinDisplay());


                }
                else
                {
                    //lose
                    StartCoroutine(LoseDisplay());


                }
            }

            // Remove status bars from win display
            StatusBar[] statusBars = FindObjectsOfType<StatusBar>();
            foreach (StatusBar sb in statusBars)
            {
                Destroy(sb.gameObject);
            }

            return true;
        }
        else
        {
            return false;
        }
    }
    private IEnumerator<WaitForSeconds> LoseDisplay()
    {
        // post to firebase
        PostLoss();

        yield return new WaitForSeconds(1);

        winDisplay.SetActive(true);
        GameObject.Find("EndMessage").GetComponent<Text>().text = "YOU LOSE!";
        winDisplay.GetComponent<Image>().color = Color.red;

    }
    private IEnumerator<WaitForSeconds> WinDisplay()
    {
        // post to firebase
        PostWin();

        yield return new WaitForSeconds(1);

        winDisplay.SetActive(true);
        GameObject.Find("EndMessage").GetComponent<Text>().text = "YOU WIN!";

    }
    public void PostWin()
    {
        if (GAME_MODE != "multiplayer")
        {
            return;
        }

        int winsInt = 0;

        DatabaseReference winsReference = usersDatabaseReference.Child(PLAYER_ID).Child("wins");
        winsReference.GetValueAsync()
            .ContinueWith(task =>
            {

                if (task.IsFaulted)
                {
                    // Handle the error...

                }
                else if (task.IsCompleted)
                {
                    // Get availableGames from snapshot
                    DataSnapshot snapshot = task.Result;
                    string snapshotString = "" + snapshot.Value;


                    if (snapshotString == "")
                    {
                        winsInt = 1;
                    }
                    else
                    {
                        winsInt = Convert.ToInt32(snapshotString) + 1;
                    }


                }

                usersDatabaseReference.Child(PLAYER_ID).Child("wins").SetValueAsync(winsInt);

            });


    }
    public void PostLoss()
    {
        if (GAME_MODE != "multiplayer")
        {
            return;
        }

        int lossesInt = 0;

        DatabaseReference winsReference = usersDatabaseReference.Child(PLAYER_ID).Child("losses");
        winsReference.GetValueAsync()
            .ContinueWith(task =>
            {

                if (task.IsFaulted)
                {
                    // Handle the error...

                }
                else if (task.IsCompleted)
                {
                    // Get availableGames from snapshot
                    DataSnapshot snapshot = task.Result;
                    string snapshotString = "" + snapshot.Value;


                    if (snapshotString == "")
                    {
                        lossesInt = 1;
                    }
                    else
                    {
                        lossesInt = Convert.ToInt32(snapshotString) + 1;
                    }

                }

                usersDatabaseReference.Child(PLAYER_ID).Child("losses").SetValueAsync(lossesInt);

            });


    }

    public void EndGame()
    {
        // Load gameplay scene
        SceneManager.LoadSceneAsync("lobby");
    }

    // Audio functions
    public void MuteOrUnmute()
    {
        musicManager.MuteOrUnmute();
        if (musicManager.IS_MUTED)
        {
            muteButton.GetComponentInChildren<RawImage>().texture = musicMutedTexture;
        }
        else
        {
            muteButton.GetComponentInChildren<RawImage>().texture = musicPlayingTexture;
        }
    }

}
