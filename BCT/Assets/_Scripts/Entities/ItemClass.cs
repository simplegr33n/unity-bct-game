using UnityEngine;

public class ItemClass : EntityClass {


    // Item attributes
    public Texture itemProfileTexture;

    public Vector3 itemPosition;
    private Vector3 oldPosition;




    void Start()
    {

        // Set Entity Type
        entityType = "ITEM";

        // Find GameBoard 
        gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();


        // Initialize oldPosition offboard and update
        oldPosition = new Vector3(-999, -999, -999);
        UpdatePosition();

    }


    public void UpdatePosition()
    {


        if (transform.position != itemPosition)
        {
            oldPosition = itemPosition;

            // Update unitPosition
            itemPosition = new Vector3(transform.position.x, 1, transform.position.z);
            gameBoard.GetComponent<GameBoard>().SetEntity(this, itemPosition);

            // Null oldPosition if there was one
            if (oldPosition != new Vector3(-999, -999, -999))
            {
                gameBoard.GetComponent<GameBoard>().NullEntityArrayPosition(oldPosition);
            }
        }

    }

    public void PickUp(int playerTeam)
    {

        Debug.Log("PRE PickUp Count: " + gameBoard.PLAYER_INVENTORY.Count);

        if (gameBoard.PLAYER_TEAM == playerTeam)
        {
            gameBoard.PLAYER_INVENTORY.Add(entityName);

            Debug.Log("POST PickUp Count: " + gameBoard.PLAYER_INVENTORY.Count);
            string itemsListedInString = "";
            foreach (string item in gameBoard.PLAYER_INVENTORY)
            {
                itemsListedInString += item + ", ";
            }
            Debug.Log("ITEMS listed in string: " + itemsListedInString);

        }

        Destroy(gameObject);
    }


    void OnMouseDown()
    {

        gameBoard.SetSelectedEntity(this);

    }






}