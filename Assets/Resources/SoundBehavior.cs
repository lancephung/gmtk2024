using System.Collections;
using UnityEngine;

public class SoundBehavior : MonoBehaviour
{
    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        IEnumerator DestroyEmitter()
        {
            float time = Time.time + 10.0f;
            yield return new WaitUntil(() => audio.isPlaying || Time.time > time);
            yield return new WaitUntil(() => !audio.isPlaying);

            if (this != null)
            Destroy(gameObject);
        }

        audio = GetComponent<AudioSource>();

        StartCoroutine(DestroyEmitter());
    }

    public void FadeOut(float duration)
    {
        IEnumerator Loop()
        {
            float start = audio.volume;
            while (audio.volume > 0)
            {
                yield return new WaitForEndOfFrame();
                audio.volume -= start / duration * Time.deltaTime;
            }
            Destroy(gameObject);
        }

        StartCoroutine(Loop());
    }
}
