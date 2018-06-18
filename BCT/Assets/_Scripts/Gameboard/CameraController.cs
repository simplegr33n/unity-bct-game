using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameBoard gameBoard;

    // Cameras
    public GameObject mainCamera;
    public GameObject minimapCamera;
    public GameObject unitCamera;
    public GameObject unitCameraContainer;
    public LayerMask mapLayerMask;

    // Camera position
    private int CAMERA_POSITION = 0;
    public Vector3[,] cameraPositions;

    public Camera ACTIVE_CAMERA;

    private EntityClass SELECTED_UNIT;

    // Unit camera offset
    Vector3 unitCameraOffset = Maps.unitCameraPositions[0,0];

    private void Start()
    {
        ACTIVE_CAMERA = mainCamera.GetComponent<Camera>();
    }

    private void Update()
    {
        // if no unit selected, set unitCameraContainer to inactive, set ACTIVE_CAMERA to mainCamera
        if (gameBoard.SELECTED_GAME_ENTITY == null && unitCameraContainer.activeSelf)
        {
            unitCameraContainer.SetActive(false);
            ACTIVE_CAMERA = mainCamera.GetComponent<Camera>();
        }

        // If unitCameraContainer is active, set ACTIVE_CAMERA to unitCamera and set unitCamera position
        if (unitCameraContainer.activeSelf && ACTIVE_CAMERA != unitCamera.GetComponent<Camera>())
        {
            ACTIVE_CAMERA = unitCamera.GetComponent<Camera>();
            SetCameraPositions(CAMERA_POSITION);
        }

        if (SELECTED_UNIT != gameBoard.SELECTED_GAME_ENTITY)
        {
            SELECTED_UNIT = gameBoard.SELECTED_GAME_ENTITY;
            // Reset unitCamera within unitCameraContainer for new unit
            ResetUnitCamera();
            unitCamera.transform.LookAt(gameBoard.SELECTED_GAME_ENTITY.transform.position);
            unitCameraContainer.SetActive(true);
        }
    }

    // Using LateUpdate() for camera following to remove jittery effect that comes from overly frequent camera updates
    private void LateUpdate()
    {
        // Unit Camera follow logic
        if (SELECTED_UNIT != null)
        {
            // Follow object (at offset)
            // TODO: consider MoveToward() so as to maybe more smoothly move from unit to unit
            unitCameraContainer.transform.position = Vector3.MoveTowards(unitCameraContainer.transform.position, gameBoard.SELECTED_GAME_ENTITY.transform.position + unitCameraOffset, 1);
            unitCamera.transform.LookAt(gameBoard.SELECTED_GAME_ENTITY.transform.position);

            // If Raycast from unitCamera to unit is blocked by terrain, adjust position forward and rotation down to get clear sight
            if (Physics.Raycast(unitCamera.transform.position
                , (gameBoard.SELECTED_GAME_ENTITY.transform.position - unitCamera.transform.position)
                , (Vector3.Distance(gameBoard.SELECTED_GAME_ENTITY.transform.position, unitCamera.transform.position)) - .2f
                , mapLayerMask))
            {

                unitCamera.transform.position = Vector3.MoveTowards(unitCamera.transform.position,
                    new Vector3(SELECTED_UNIT.transform.position.x, unitCamera.transform.position.y, SELECTED_UNIT.transform.position.z),
                    Time.deltaTime * 8);

                unitCamera.transform.LookAt(gameBoard.SELECTED_GAME_ENTITY.transform.position);

            }

        }
    }

    // Set intial camera positions
    public void InitialSetCameras()
    {
        // Set main camera
        Camera.main.transform.position = cameraPositions[0, 0];
        Camera.main.transform.rotation = Quaternion.Euler(cameraPositions[0, 1]);
        // Set minimap camera
        minimapCamera.transform.position = new Vector3((gameBoard.xSize / 2) - .5f, 20, (gameBoard.zSize / 2) - .5f);
        minimapCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(gameBoard.xSize, gameBoard.zSize) / 2;
    }

    void SetCameraPositions(int position)
    {

        // Reset unitCamera within unitCameraContainer for new camera positions
        ResetUnitCamera();


        switch (position)
        {
            case (1):

                CAMERA_POSITION = 1;

                mainCamera.transform.position = cameraPositions[1, 0];
                mainCamera.transform.rotation = Quaternion.Euler(cameraPositions[1, 1]);

                unitCameraOffset = Maps.unitCameraPositions[1, 0];
                unitCameraContainer.transform.rotation = Quaternion.Euler(Maps.unitCameraPositions[1, 1]);


                minimapCamera.transform.rotation = Quaternion.Euler(90, 0, 90);


                break;

            case (2):

                CAMERA_POSITION = 2;

                mainCamera.transform.position = cameraPositions[2, 0];
                mainCamera.transform.rotation = Quaternion.Euler(cameraPositions[2, 1]);

                unitCameraOffset = Maps.unitCameraPositions[2, 0];
                unitCameraContainer.transform.rotation = Quaternion.Euler(Maps.unitCameraPositions[2, 1]);

                minimapCamera.transform.rotation = Quaternion.Euler(90, 0, 180);


                break;

            case (3):

                CAMERA_POSITION = 3;

                mainCamera.transform.position = cameraPositions[3, 0];
                mainCamera.transform.rotation = Quaternion.Euler(cameraPositions[3, 1]);

                unitCameraOffset = Maps.unitCameraPositions[3, 0];
                unitCameraContainer.transform.rotation = Quaternion.Euler(Maps.unitCameraPositions[3, 1]);

                minimapCamera.transform.rotation = Quaternion.Euler(90, 0, 270);


                break;

            case (0):

                CAMERA_POSITION = 0;

                mainCamera.transform.position = cameraPositions[0, 0];
                mainCamera.transform.rotation = Quaternion.Euler(cameraPositions[0, 1]);

                unitCameraOffset = Maps.unitCameraPositions[0, 0];
                unitCameraContainer.transform.rotation = Quaternion.Euler(Maps.unitCameraPositions[0, 1]);

                minimapCamera.transform.rotation = Quaternion.Euler(90, 0, 0);


                break;
        }
    }


    public void ShiftCamera()
    {
        switch (CAMERA_POSITION)
        {
            case (0):
                SetCameraPositions(1);

                break;

            case (1):
                SetCameraPositions(2);

                break;

            case (2):
                SetCameraPositions(3);

                break;

            case (3):
                SetCameraPositions(0);

                break;
        }
    }

    private void ResetUnitCamera()
    {
        unitCamera.transform.localPosition = new Vector3(0, 0, 0);
        unitCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }



}
