using UnityEngine;

public class EffectDisplayer : MonoBehaviour {

    public GameBoard gameBoard;

    // Text Effects
    public PopupText popupText;

    // Attack ... (explosion...)
    public GameObject explosion;

    // Ability Effects
    public GameObject solarFlare;
    public GameObject acidRain;
    public GameObject ornithophobia;




    // For instantiating from static classes (like abilities)
    // TODO: different system for Destroy timing - maybe a script for each ability animation?
    public void CreateEffect(GameObject effect, Vector3 position, Quaternion rotation)
    {
        GameObject newEffect = Instantiate(effect, position, rotation);
        Destroy(newEffect, 2.0f);

    }


    public void CreatePopupText(string text, Vector3 location, Color textColor)
    {

        Vector2 screenPosition = gameBoard.cameraController.ACTIVE_CAMERA.WorldToScreenPoint(location);

        Debug.Log("CreatePopupText @ " + screenPosition);

        if (screenPosition != null)
        {
            PopupText instance = Instantiate(popupText);

            instance.transform.SetParent(gameBoard.canvas.transform, false);
            instance.transform.position = screenPosition;
            instance.SetPopupText(text, textColor);
        }
    }
}
