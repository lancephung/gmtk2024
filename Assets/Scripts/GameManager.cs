using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public static GameManager Instance { get; private set; }
    public static int level;
    public static int highest_level;
    public static bool hasSave;

    static float _volume = 1.0f;
    public static float volume { get { return _volume; } set { _volume = value; music.Sound.volume = value; } }

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
        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += (old, current) => 
        {
            string musicName = current.name == "Menu" ? "antares" : "assembled with chair";

            if (current.name == "Menu")
            {
                Time.timeScale = 1.0f;
            }

            GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
            GetComponentInChildren<Animator>().SetTrigger("Entry");


            if (this == null || old == current) return;
            Debug.Log("changing music to " + musicName);
            if (music != null)
            {
                if (musicName == music.Sound.clip.name) return;
                else
                {
                    music.FadeOut(3);
                    IEnumerator Delay()
                    {
                        yield return new WaitForSeconds(2);
                        music = AudioManager.PlaySound(musicName);
                        music.Sound.loop = true;
                        music.FadeIn(3);
                    }
                    StartCoroutine(Delay());
                    return;
                }
            }

            music = AudioManager.PlaySound(musicName);
            music.Sound.loop = true;
        };
    }
}
