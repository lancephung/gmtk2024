using System.Collections;
using UnityEngine;

public class SoundBehavior : MonoBehaviour
{
    AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        IEnumerator DestroyEmitter()
        {
            float time = Time.time + 10.0f;
            yield return new WaitUntil(() => sound.isPlaying || Time.time > time);
            yield return new WaitUntil(() => !sound.isPlaying && Application.isFocused);

            if (this != null)
            Destroy(gameObject);
        }

        sound = GetComponent<AudioSource>();

        StartCoroutine(DestroyEmitter());
    }


    IEnumerator Fade(float duration)
    {
        float start = sound.volume;
        while (sound.volume > 0)
        {
            yield return new WaitForEndOfFrame();
            sound.volume -= start / duration * Time.deltaTime;
        }
        Destroy(gameObject);
    }

    public void FadeOut(float duration)
    {
        sound = GetComponent<AudioSource>();
        Debug.Log(sound);
        StartCoroutine(Fade(duration));
    }
}
