using System.Collections;
using UnityEngine;

public class SoundBehavior : MonoBehaviour
{
    AudioSource _sound;
    public AudioSource Sound { get
    {
        if (_sound == null) _sound = GetComponent<AudioSource>();
        return _sound;
    }
    }

    Coroutine coroutine;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        IEnumerator DestroyEmitter()
        {
            float time = Time.time + 10.0f;
            yield return new WaitUntil(() => Sound.isPlaying || Time.time > time);
            yield return new WaitUntil(() => !Sound.isPlaying && Application.isFocused);

            if (this != null)
            Destroy(gameObject);
        }

        _sound = GetComponent<AudioSource>();

        StartCoroutine(DestroyEmitter());
    }

    public void FadeOut(float duration)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        IEnumerator Fade(float duration)
        {
            float start = Sound.volume;
            while (Sound.volume > 0)
            {
                yield return new WaitForEndOfFrame();
                Sound.volume -= start / duration * Time.deltaTime;
            }
            Destroy(gameObject);
        }
        StartCoroutine(Fade(duration));
    }

    public void FadeIn(float duration)
    {
        if (coroutine != null) StopCoroutine(coroutine);
        IEnumerator Fade(float duration)
        {
            while (Sound.volume < 1)
            {
                yield return new WaitForEndOfFrame();
                Sound.volume += Time.deltaTime / duration;
            }
        }
        Sound.volume = 0;
        coroutine = StartCoroutine(Fade(duration));
    }
}
