using System.Collections;
using UnityEngine;

public class SoundBehavior : MonoBehaviour
{
    AudioSource baudio;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        IEnumerator DestroyEmitter()
        {
            float time = Time.time + 10.0f;
            yield return new WaitUntil(() => baudio.isPlaying || Time.time > time);
            yield return new WaitUntil(() => !baudio.isPlaying);

            if (this != null)
            Destroy(gameObject);
        }

        baudio = GetComponent<AudioSource>();

        StartCoroutine(DestroyEmitter());
    }

    public void FadeOut(float duration)
    {
        IEnumerator Loop()
        {
            float start = baudio.volume;
            while (baudio.volume > 0)
            {
                yield return new WaitForEndOfFrame();
                baudio.volume -= start / duration * Time.deltaTime;
                Debug.Log(baudio.volume);
            }
            Destroy(gameObject);
        }

        StartCoroutine(Loop());
    }
}
