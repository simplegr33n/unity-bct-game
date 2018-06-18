using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour {

    public bool isWalkable;
    public float tile_elevation;
    public Vector3 roundedPosition; // TODO: use in place of xPosition/zPosition/tile_elevantion everywhere.

    // for rounded positions
    int xPosition;
    int zPosition;



    public string INDICATOR_MODE;
    public GameObject highlightPlane;
    public GameBoard gameBoard;

    public UnitClass currentUnit;

    // Movement Info
    public string PATH_STRING;
    private int MOVES_LEFT = 0;

    public string SELECTED_ABILITY;

    // set to true or false in SetActive function below
    private bool IS_CLIENT;



    private void Awake()
    {
        // TODO: remove tile_elevation, xPosition, zPosition
        tile_elevation = transform.position.y;
        xPosition = Mathf.RoundToInt(transform.position.x);
        zPosition = Mathf.RoundToInt(transform.position.z);

        roundedPosition = new Vector3 (xPosition, transform.position.y, zPosition);


        gameBoard = FindObjectOfType<GameBoard>();

        // Make indicator invisible to start
        GetComponent<Renderer>().material = gameBoard.invisibleIndicatorMaterial;

        // Add rigidbody and meshcolider for mouse detection
        gameObject.AddComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<Rigidbody>().freezeRotation = true;
        gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        if (gameObject.GetComponent<MeshCollider>() == null)
        {
            gameObject.AddComponent<MeshCollider>().convex = true;
        }

        // Tile highlight plane
        CreateHighlightPlane();


    }

    public void CheckMove(int movesLeft, float elevation, string pathString, UnitClass unit)
    {

        currentUnit = unit;

        int movesLeftDec = movesLeft;

        // Get Map Size
        int xSize = gameBoard.TileArray.GetLength(0);
        int zSize = gameBoard.TileArray.GetLength(1);

        // Find squared height difference
        float heightDifferenceSquared = (tile_elevation - elevation) * (tile_elevation - elevation);
        float unitPotentialSquared = (unit.unitElevationPotential * unit.unitElevationPotential);

        if ((gameBoard.IsWalkableTile(roundedPosition) || pathString == null)
            && isWalkable && movesLeftDec >= 1 && movesLeftDec > MOVES_LEFT &&
            (unitPotentialSquared - heightDifferenceSquared) > 0  || elevation == 999)
        {

            // Activate Move Indicator
            SetMoveIndicator();

            // Hide Indicator if pathString == null (...ie. if at starting position)
            movesLeftDec -= 1;
            if (pathString == null)
            {
                pathString = "";
                HideIndicator();
            }
            PATH_STRING = pathString + xPosition + ":" + zPosition + "*";

            int[] possibleMove = { xPosition, zPosition };

            // add possible move tile to game unit list of possible moves (for AI)
            // TODO: potentially store this in UnitAI rather than here..
            unit.availableMoveTilesList.Add(possibleMove);

            MOVES_LEFT = movesLeftDec;

            // Check moves in cross around current tile
            if (movesLeftDec > 0)
            {

                if (xPosition + 1 < xSize)
                {
                    gameBoard.TileArray[xPosition + 1, zPosition].CheckMove(movesLeftDec, tile_elevation, PATH_STRING, unit);
                }
                if (xPosition - 1 >= 0)
                {
                    gameBoard.TileArray[xPosition - 1, zPosition].CheckMove(movesLeftDec, tile_elevation, PATH_STRING, unit);
                }
                if (zPosition + 1 < zSize)
                {
                    gameBoard.TileArray[xPosition, zPosition + 1].CheckMove(movesLeftDec, tile_elevation, PATH_STRING, unit);
                }
                if (zPosition - 1 >= 0)
                {
                    gameBoard.TileArray[xPosition, zPosition - 1].CheckMove(movesLeftDec, tile_elevation, PATH_STRING, unit);
                }
            }



        }

    }

    public void SetMoveIndicator()
    {
        if (currentUnit.playerTeam == gameBoard.PLAYER_TEAM)
        {
            GetComponent<Renderer>().material = gameBoard.moveIndicatorMaterial;
            INDICATOR_MODE = "MOVE";
            SetActive(true);
        }
        else
        {
            GetComponent<Renderer>().material = gameBoard.enemyIndicatorMaterial;
            INDICATOR_MODE = "MOVE";
            SetActive(true);
        }

    }

    public void SetAttackIndicator()
    {
        GetComponent<Renderer>().material = gameBoard.attackIndicatorMaterial;
        INDICATOR_MODE = "ATTACK";
        SetActive(true);
    }

    public void SetAbilityIndicator(string abilityName)
    {
        GetComponent<Renderer>().material = gameBoard.abilityIndicatorMaterial;
        SELECTED_ABILITY = abilityName;
        INDICATOR_MODE = "ABILITY";
        SetActive(true);
    }

    public void HideIndicator()
    {
        SetActive(false);
        INDICATOR_MODE = null;
        MOVES_LEFT = 0;

    }

    public void RaycastToTile()
    {

        if (EventSystem.current.IsPointerOverGameObject())
        {
            // we're over a UI element, don't highlight, return
            highlightPlane.SetActive(false);
            return;
        }

        if (IS_CLIENT)
        {
            highlightPlane.SetActive(true);
        }

        if (INDICATOR_MODE == "ABILITY")
        {
            ShowAbilityAOE();
        }
    }




    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            // we're over a UI element, return 
            return;
        }

        if (IS_CLIENT)
        {
            // Ensure unit active, and belongs to client
            // This is a double check here, i need to clean this up
            // trying to squash a double turn bug.
            if (currentUnit == gameBoard.turnManager.ACTIVE_UNIT 
                && currentUnit.playerTeam == gameBoard.PLAYER_TEAM)
            {


                if (INDICATOR_MODE == "MOVE")
                {
                    highlightPlane.SetActive(false);
                    currentUnit.QueueAction("MOV|" + PATH_STRING + "|");
                    gameBoard.turnManager.TURN_INSTRUCTION += "MOV|" + PATH_STRING + "|";



                }
                else if (INDICATOR_MODE == "ATTACK")
                {
                    highlightPlane.SetActive(false);
                    currentUnit.QueueAction("ATK|" + roundedPosition.x + "|" + roundedPosition.z + "|");
                    gameBoard.turnManager.TURN_INSTRUCTION += "ATK|" + roundedPosition.x + "|" + roundedPosition.z + "|";
                    gameBoard.entityMenu.SetAttackMenu();



                }
                else if (INDICATOR_MODE == "ABILITY")
                {

                    Debug.Log("ABILITY TIME! " + SELECTED_ABILITY);

                    switch (SELECTED_ABILITY)
                    {
                        case ("Solar Flare"):

                        SolarFlare.CastAbility(gameBoard, currentUnit, this);

                            break;

                        case ("Acid Rain"):

                        AcidRain.CastAbility(gameBoard, currentUnit, this);

                            break;

                        case ("Ornithophobia"):

                        Ornithophobia.CastAbility(gameBoard, currentUnit, this);

                            break;

                    }


                }
            }


        }

    }

    public void SetActive(bool activeSetting)
    {
        if (activeSetting)
        {
            if (currentUnit != null && currentUnit.playerTeam == gameBoard.PLAYER_TEAM && currentUnit == gameBoard.turnManager.ACTIVE_UNIT)
            {
                IS_CLIENT = true;
            }
            gameObject.SetActive(true);

        } else
        {
            IS_CLIENT = false;
            gameObject.SetActive(false);
            highlightPlane.SetActive(false);
       
        }
    }

    void ShowAbilityAOE()
    {
        switch (SELECTED_ABILITY)
        {
            case ("Solar Flare"):

                SolarFlare.HighlightAOE(gameBoard, currentUnit, this);

                break;

            case ("Acid Rain"):

               AcidRain.HighlightAOE(gameBoard, currentUnit, this);

                break;

        }
    }

    public void CreateHighlightPlane()
    {
        // Instantiate Highlight plane
        highlightPlane = Instantiate(gameBoard.highlightPlane, new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z), transform.rotation);
        highlightPlane.transform.SetParent(transform);
        highlightPlane.transform.localScale = new Vector3 (1,1,1);

        // Give highlight plane same mesh as Tile
        MeshFilter tileMesh = GetComponent<MeshFilter>();
        highlightPlane.GetComponent<MeshFilter>().mesh = tileMesh.mesh;


        highlightPlane.SetActive(false);
    }
}
