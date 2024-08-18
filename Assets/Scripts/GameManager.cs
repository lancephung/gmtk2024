using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public static GameManager Instance { get; private set; }
    public static int level;
    public static int highest_level;
    public static bool hasSave;

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

        DontDestroyOnLoad(gameObject);
    }
}
