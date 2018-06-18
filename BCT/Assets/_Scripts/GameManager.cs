using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    // UI Menus
    public static GameObject baseMenu;
    public static GameObject loginMenu;
    public static GameObject connectDialog;
    public static GameObject hostJoinButtons;

    private Transform availableGamesContainer;

    public GameObject availableGamePrefab;

    public string PLAYER_NAME;
    public string PLAYER_ID;

    public string MAP_NAME;

    public string GAME_ID;
    public int PLAYER_TEAM;

    public DatabaseReference firebaseDatabaseReference;


    // TODO: one or other...?
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        // Set up the Editor before calling into the realtime database.
        FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://cbtalpha.firebaseio.com/");

        // Get the root reference location of the database.
        firebaseDatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        // Find menus
        GameObject canvasObject = GameObject.Find("Canvas");
        loginMenu = canvasObject.transform.Find("LoginMenu").gameObject;  

        // Watch for sceneload
        SceneManager.sceneLoaded += OnSceneLoaded;



    }



    public void Login()
    {

        string username = GameObject.Find("UsernameInput").GetComponent<InputField>().text;
        firebaseDatabaseReference.Child("users").Child("userslist").GetValueAsync()
               .ContinueWith(task =>
               {

                   if (task.IsFaulted)
                   {
                       // Handle the error...

                   }
                   else if (task.IsCompleted)
                   {
                       
                       DataSnapshot snapshot = task.Result;

                       // Check if user exists
                       string existingPlayerID = "";

                       IEnumerable<DataSnapshot> snapChildren = snapshot.Children;
                       foreach (DataSnapshot child in snapChildren)
                       {
                           if (child.Value.ToString() == username)
                           {
                               existingPlayerID = child.Key.ToString();

                               break;
           
                           }

                       }

                       if (existingPlayerID != "")
                       {
                           // Login
                           PLAYER_NAME = username;
                           PLAYER_ID = existingPlayerID;

                           

                       }
                       else
                       {
                           // Create user and login
                           // Userlist entry
                           DatabaseReference pendingGameReference = firebaseDatabaseReference.Child("users").Child("userslist").Push();
                           pendingGameReference.SetValueAsync(username);

                           string newPlayerID = pendingGameReference.Key;

                           // Actual user entry
                           firebaseDatabaseReference.Child("users").Child(newPlayerID).Child("username").SetValueAsync(username);

                           PLAYER_NAME = username;
                           PLAYER_ID = newPlayerID;

                       }

                       // Load Lobby scene
                       SceneManager.LoadSceneAsync("lobby");

                   }

               });


        

    }

    // Changes scene to gameplay
    public void GoToGame()
    {
        // Load gameplay scene
        SceneManager.LoadSceneAsync("gameplay");
    }




    // Host / Join functions
    public void HostGame()
    {
        connectDialog.SetActive(true);
        hostJoinButtons.SetActive(false);

        // Create MultiplayerInstance for firebase, set GameManager vars
        DatabaseReference pendingGameReference = firebaseDatabaseReference.Child("games").Child("pending").Push();
        string referenceKey = pendingGameReference.Key;
        GAME_ID = referenceKey;
        PLAYER_TEAM = 0;

        List<string> newPlayerList = new List<string>();
        newPlayerList.Add(PLAYER_ID);
        MultiplayerInstance newGame = new MultiplayerInstance(false, referenceKey, newPlayerList);
        string newGameJson = JsonUtility.ToJson(newGame);

        pendingGameReference.SetRawJsonValueAsync(newGameJson);

        // Listen for a Joiner
        pendingGameReference.ValueChanged += FirebaseJoinListener;

    }
    public void CancelHostGame()
    {
        foreach (Transform child in availableGamesContainer)
        {
            Destroy(child.gameObject);
        }

        connectDialog.SetActive(false);
        hostJoinButtons.SetActive(true);

        // Remove game from Firebase and id from local GameID var
        if (GAME_ID != null) { 
        firebaseDatabaseReference.Child("games").Child("pending").Child(GAME_ID).RemoveValueAsync();
        GAME_ID = null;
        }

    }
    public void ShowAvailableGames()
    {

        connectDialog.SetActive(true);
        hostJoinButtons.SetActive(false);

        firebaseDatabaseReference.Child("games").Child("pending").GetValueAsync()
            .ContinueWith(task => {

                if (task.IsFaulted)
                {
                    // Handle the error...

                }
                else if (task.IsCompleted)
                {
                    // Get availableGames from snapshot
                    DataSnapshot snapshot = task.Result;

                    IEnumerable<DataSnapshot> snapChildren = snapshot.Children;
                    foreach (DataSnapshot child in snapChildren)
                    {

                        MultiplayerInstance availableGame = JsonUtility.FromJson<MultiplayerInstance>(child.GetRawJsonValue());

                        // Display availableGame item
                        GameObject availableGameItem = Instantiate(availableGamePrefab) as GameObject;
                        availableGameItem.transform.SetParent(availableGamesContainer);
                        availableGameItem.GetComponentInChildren<Text>().text = availableGame.gameKey;
                    }

             
                }

            });

    }
    public void JoinGame(string joinGameID)
    {



        // Join MultiplayerInstance for firebase, set GameManager vars
        DatabaseReference newGameReference = firebaseDatabaseReference.Child("games").Child("pending").Child(joinGameID);
        newGameReference.GetValueAsync()
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


                    MultiplayerInstance joinGameGame = JsonUtility.FromJson<MultiplayerInstance>(snapshot.GetRawJsonValue());

                    joinGameGame.playersList.Add(PLAYER_ID);

                    string joinGameJson = JsonUtility.ToJson(joinGameGame);

                    newGameReference.SetRawJsonValueAsync(joinGameJson);

                    // Listen for a Joiner
                    newGameReference.ValueChanged += FirebaseReadyGameListener;

                    GAME_ID = joinGameID;
                    PLAYER_TEAM = 1;
                }




            });





    }

    // Firebase listeners
    void FirebaseJoinListener(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Set isStarted to true and start game instance in host client
        MultiplayerInstance joinedGame = JsonUtility.FromJson<MultiplayerInstance>(args.Snapshot.GetRawJsonValue());
        if (joinedGame.playersList.Count >= 2)
        {
            // Set isStarted to true in pending game
            firebaseDatabaseReference.Child("games").Child("pending").Child(GAME_ID).Child("isStarted").SetValueAsync(true);
            // Remove Joiner listener
            firebaseDatabaseReference.Child("games").Child("pending").Child(GAME_ID).ValueChanged -= FirebaseJoinListener;
            // Create game in active node
            string joinedGameJson = JsonUtility.ToJson(joinedGame);
            firebaseDatabaseReference.Child("games").Child("active").Child(GAME_ID).SetRawJsonValueAsync(joinedGameJson);

            // GOTO game
            GoToGame();
        }

    }
    void FirebaseReadyGameListener(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Delete pending game and start game in client
        MultiplayerInstance joinedGame = JsonUtility.FromJson<MultiplayerInstance>(args.Snapshot.GetRawJsonValue());
        if (joinedGame.isStarted == true)
        {
            // Set isStarted to true in pending game
            firebaseDatabaseReference.Child("games").Child("pending").Child(GAME_ID).RemoveValueAsync();
            // Remove gameReady listener
            firebaseDatabaseReference.Child("games").Child("pending").Child(GAME_ID).ValueChanged -= FirebaseReadyGameListener;

            // GOTO game
            GoToGame();
        }

    }



    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        //do stuff
        if (scene.name == "lobby")
        {

            Debug.Log("SetUpLobby :: PLAYER_ID = " + PLAYER_ID + ", NAME = " + PLAYER_NAME);

            GAME_ID = null;
            PLAYER_TEAM = 0;

            // TODO: Wire this shit up in the interface...
            // Find ui items
            GameObject canvasObject = GameObject.Find("Canvas");
            baseMenu = canvasObject.transform.Find("BaseMenu").gameObject;
            hostJoinButtons = baseMenu.transform.Find("HostJoinButtons").gameObject;
            GameObject hostButton = hostJoinButtons.transform.Find("HostButton").gameObject;
            GameObject joinButton = hostJoinButtons.transform.Find("JoinButton").gameObject;

            connectDialog = baseMenu.transform.Find("ConnectDialog").gameObject;
            GameObject backButton = connectDialog.transform.Find("BackButton").gameObject;

            availableGamesContainer = connectDialog.transform.Find("Games").transform.Find("AvailableGamesContainer");

            // Set button onClick functions
            hostButton.GetComponent<Button>().onClick.AddListener(delegate { HostGame(); });
            joinButton.GetComponent<Button>().onClick.AddListener(delegate { ShowAvailableGames(); });
            backButton.GetComponent<Button>().onClick.AddListener(delegate { CancelHostGame(); });

            // Set userinfo panel
            GameObject.Find("UsernameText").GetComponent<Text>().text = PLAYER_NAME;
            DatabaseReference winsReference = firebaseDatabaseReference.Child("users").Child(PLAYER_ID).Child("wins");
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
                        int winsInt;

                        if (snapshotString == "")
                        {
                            winsInt = 0;
                        }
                        else
                        {
                            winsInt = Convert.ToInt32(snapshotString);
                        }

                        GameObject.Find("WinsText").GetComponent<Text>().text = "" + winsInt;

                    }


                });
            DatabaseReference lossesReference = firebaseDatabaseReference.Child("users").Child(PLAYER_ID).Child("losses");
            lossesReference.GetValueAsync()
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
                        int lossesInt;

                        if (snapshotString == "")
                        {
                            lossesInt = 0;
                        }
                        else
                        {
                            lossesInt = Convert.ToInt32(snapshotString);
                        }

                        GameObject.Find("LossesText").GetComponent<Text>().text = "" + lossesInt;
                    }


                });

            connectDialog.SetActive(false);
            hostJoinButtons.SetActive(true);
            baseMenu.SetActive(true);

        }

        
    }

    public void LoginOffline()
    {
        // Load Lobby scene
        SceneManager.LoadSceneAsync("lobby");
    }

}



