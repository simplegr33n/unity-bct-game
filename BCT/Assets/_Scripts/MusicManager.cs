using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance = null;

    private AudioSource audioSource;

    public AudioClip song1;
    public AudioClip song2;
    public AudioClip song3;

    public bool IS_MUTED;

    // Use this for initialization
    void Awake () {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);

        // Watch for sceneload
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Get audiosource
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        switch (scene.name)
        {
            case ("lobby"):
                //audioSource.clip = song2;

                audioSource.Stop();

                break;

            case ("gameplay"):
                audioSource.clip = song3;
                audioSource.Play();

                break;

        }


    }

    public void MuteOrUnmute()
    {

        if (IS_MUTED)
        {
            IS_MUTED = false;
            audioSource.mute = false;
        } else
        {
            IS_MUTED = true;
            audioSource.mute = true;
        }

    }

}
