using UnityEngine;
using UnityEngine.UI;

public class GameListItem : MonoBehaviour {

    public Text gameCodeField;

    public void ClickJoinPlay()
    {

        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.JoinGame(gameCodeField.text);

    }

}
