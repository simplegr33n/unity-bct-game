using UnityEngine;
using UnityEngine.SceneManagement;


public class LobbyManager : MonoBehaviour {

    private GameManager gameManager;


    private void Awake()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>(); 
    }



    public void PlayCanyon()
    {
        gameManager.GAME_ID = null;
        gameManager.MAP_NAME = "CANYON";
        SceneManager.LoadSceneAsync("gameplay");
    }


    public void PlayMountainBend()
    {
        gameManager.GAME_ID = null;
        gameManager.MAP_NAME = "MOUNTAIN_BEND";
        SceneManager.LoadSceneAsync("gameplay");
    }


    public void PlayZarghidasTrade()
    {
        gameManager.GAME_ID = null;
        gameManager.MAP_NAME = "ZARGHIDAS_TRADE";
        SceneManager.LoadSceneAsync("gameplay");
    }


    public void PlayTestHill()
    {
        gameManager.GAME_ID = null;
        gameManager.MAP_NAME = "TEST_HILL";
        SceneManager.LoadSceneAsync("gameplay");
    }

}



