using UnityEngine;

public class CursorHandler : MonoBehaviour {

    private GameBoard gameBoard;

    public LayerMask tileLayerMask;

    public LayerMask mapLayerMask;

    private RaycastHit tileHit;
    private RaycastHit meshHit;

    private Tile currentTile;


    void Start () {
        gameBoard = FindObjectOfType<GameBoard>();
	}

    void Update()
    {

        Ray ray = gameBoard.cameraController.ACTIVE_CAMERA.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(ray, out tileHit, Mathf.Infinity, tileLayerMask))
        {
            Physics.Raycast(ray, out meshHit, Mathf.Infinity, mapLayerMask);

            // If ray hits mesh before it hits the tile, return
            if (Vector3.Distance(meshHit.point, ray.origin) < Vector3.Distance(tileHit.point, ray.origin))
            {
                return;
            }
            
            // If current tile not set to raycast hit, set it and activate indicator
            if (currentTile != tileHit.transform.gameObject.GetComponent<Tile>())
            {
                gameBoard.HideCursorPlanes();
                currentTile = tileHit.transform.gameObject.GetComponent<Tile>();
                currentTile.RaycastToTile();
            } 
           

        } else
        {
            if (currentTile != null)
            {
                currentTile = null;
                gameBoard.HideCursorPlanes();
            }
        }


    }
}
