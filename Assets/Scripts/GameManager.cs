using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public static GameManager Instance { get; private set; }
    public static int level;
    public static int highest_level;
    public static bool hasSave;
    public static float volume;

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
    }
}
