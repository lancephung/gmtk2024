using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public static GameManager Instance { get; private set; }
    public static int level;
    public static int highest_level;
    public static bool hasSave;

    static float _volume = 1.0f;
    public static float volume { get { return _volume; } set { _volume = value; if (music == null) return; music.Sound.volume = value; } }

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


            if (this == null || old == current) return;
            // Debug.Log("changing music to " + musicName);
            if (music != null)
            {
                if (musicName == music.Sound.clip.name) return;
                else
                {
                    music.FadeOut(3);
                    music = AudioManager.PlaySound(musicName);
                    music.Sound.playOnAwake = false;
                    music.Sound.Stop();
                    IEnumerator Delay()
                    {
                        yield return new WaitForSeconds(2);
                        music.FadeIn(3);
                        music.Sound.Play();
                    }
                    StartCoroutine(Delay());
                }
            }
            else
            {
                music = AudioManager.PlaySound(musicName);
            }
            music.Sound.loop = true;
        };
    }
}
