using UnityEngine;
using UnityEngine.UI;

public class StatusBar : MonoBehaviour {

    public static Vector3 offsetBar = new Vector3(0, 1, 0);

    public UnitClass unit;

    // display bools
    private bool DEFEND_DISPLAY;
    private bool POISON_DISPLAY;

    // Status icons
    public Texture defendIconTexture;
    public Texture poisonIconTexture;

    // Status effect GameObjects
    private GameObject defendIconObject;
    private GameObject poisonIconObject;

    private GameBoard gameBoard;


    private void Start()
    {
        gameBoard = FindObjectOfType<GameBoard>();
    }

    // Update is called once per frame
    void Update () {

        if (unit != null)
        {
            // Update statusBar position (try, as sometimes unit will be off screen)
            try
            {
                Vector2 screenPosition = gameBoard.cameraController.ACTIVE_CAMERA.WorldToScreenPoint(unit.transform.position + offsetBar);
                transform.position = screenPosition;
            } catch
            {
                // do nothing, unit is off screen
            }

        } else
        {
            Destroy(gameObject);
        }


        // Update individual status icons
        // DEFEND
        if (defendIconObject == null && unit.STATUS_DEFENDING > 0)
        {
            defendIconObject = Instantiate(unit.gameBoard.statusEffectPrefab) as GameObject;
            defendIconObject.transform.SetParent(transform);

            defendIconObject.GetComponent<RawImage>().texture = defendIconTexture;
        }
        if (defendIconObject != null && unit.STATUS_DEFENDING < 1)
        {
            Destroy(defendIconObject);
        }

        // POISON
        if (poisonIconObject == null && unit.STATUS_POISONED > 0)
        {
            poisonIconObject = Instantiate(unit.gameBoard.statusEffectPrefab) as GameObject;
            poisonIconObject.transform.SetParent(transform);

            poisonIconObject.GetComponent<RawImage>().texture = poisonIconTexture;
        }
        if (poisonIconObject != null && unit.STATUS_POISONED < 1)
        {
            Destroy(poisonIconObject);
        }

    }
}
