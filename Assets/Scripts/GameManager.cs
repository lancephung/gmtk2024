using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public static GameManager Instance { get; private set; }
    public static int level;
    public static int highest_level;
    public static bool hasSave;
    public static float volume;

    public static SoundBehavior music;

    public void ChangeVolume(float avolume)
    {
        Debug.Log(avolume);
        volume = avolume;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (!hasSave)
        {
            highest_level = 1;
        }
        volume = 0.5f;
        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += (old, current) => 
        {
            if (music != null) music.FadeOut(3);
            if (current.name == "Menu")
            {
                music = AudioManager.PlaySound("antares-9996");
            }
        };
    }
}
