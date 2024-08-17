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
            if (value == _Size || value < 0) return;
            StartCoroutine(UpdateSize(value - _Size));
            _Size = value;
        }
    }

    IEnumerator UpdateSize(int change)
    {
        Vector2 initialSize = collider.size;
        Vector2 targetSize = initialSize + new Vector2(Mathf.Abs(Direction.x), Mathf.Abs(Direction.y)) * change;

        Vector3 initialPosition = transform.position;
        Vector3 targetPosition = initialPosition + 0.5f * (transform.rotation * (Vector3)Direction) * change;

        float progress = 0.0f;
        float duration = 0.5f;
        float exponent = 6.0f;

        push?.Play();
        while (progress < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            progress += Time.deltaTime / duration;
            float easedProgress = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(progress), exponent);

            collider.size = Vector2.Lerp(initialSize, targetSize, easedProgress);
            sprite.size = collider.size;

            transform.position = Vector3.Lerp(initialPosition, targetPosition, easedProgress);
            push.transform.position = transform.position;
        }
        push?.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        collider.size = targetSize;
        sprite.size = targetSize;
        transform.position = targetPosition;
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
            Vector2 check = collider.size * Direction;
            int currentSize = Mathf.RoundToInt(Mathf.Max(check.x, check.y));
            if (currentSize != _Size)
            {
                collider.size += (_Size - currentSize) * Direction;
                transform.position += (_Size - currentSize) * 0.5f * (Vector3) Direction;
            }
        }

        #endif

        sprite.size = collider.size;
    }
}
