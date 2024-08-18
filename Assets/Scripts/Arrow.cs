using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class Arrow : MonoBehaviour
{
    public bool IsShrink = false;
    //[SerializeField] private bool isShrink = false;
    BoxCollider2D collider;
    SpriteRenderer sprite;
    ParticleSystem push;

    public Vector2 Direction = Vector2.right;

    [SerializeField] private int _Size = 1;
    public int Size { get { return _Size; }
        set
        {
            if (value == _Size || value <= 0) return;
            StartCoroutine(UpdateSize(value - _Size));
            _Size = value;
        }
    }

    public void Activate()
    {
        AudioManager.PlaySound("scrape");
        Size += IsShrink ? -1 : 1;
    }

    float EasingFunction(float progress)
    {
        return -(Mathf.Cos(Mathf.PI * progress) + 1) * 0.5f;
    }

    IEnumerator UpdateSize(int change)
    {
        float progress = 0.0f;

        float duration = 0.7f;

        push.Play();
        while (progress < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            float deltaTime = Mathf.Min(progress + Time.deltaTime / duration, 1) - progress;
            float ease = change * (EasingFunction(progress + deltaTime) - EasingFunction(progress));
            progress += deltaTime;
            
            collider.size += ease * new Vector2(Mathf.Abs(Direction.x), Mathf.Abs(Direction.y));
            sprite.size = collider.size;
            
            transform.position += ease * 0.5f * (transform.rotation * (Vector3) Direction);
            push.transform.localPosition += ease * 0.5f * (transform.rotation * (Vector3) Direction);
        }
        push.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        push = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        #if UNITY_EDITOR

        if (!Application.isPlaying)
        {
            Direction.x = Mathf.Clamp(Direction.x, -1, 1);
            Direction.y = Mathf.Clamp(Direction.y, -1, 1);

            transform.position = new Vector2(Mathf.Round(transform.position.x * 2) * 0.5f, Mathf.Round(transform.position.y * 2) * 0.5f);
            var absolute = new Vector2(Mathf.Abs(Direction.x), Mathf.Abs(Direction.y));
            var check = collider.size * absolute;
            int currentSize = Mathf.RoundToInt(Mathf.Max(check.x, check.y));
            // Debug.Log(currentSize);
            if (currentSize != _Size)
            {
                collider.size += (_Size - currentSize) * absolute;
                transform.position += (_Size - currentSize) * 0.5f * (Vector3) Direction;
                push.transform.position += (_Size - currentSize) * 0.5f * (Vector3) Direction;
            }
        }

        #endif

        sprite.size = collider.size;
    }
}
