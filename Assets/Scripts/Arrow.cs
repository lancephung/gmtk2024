using System.Collections;
using UnityEngine;

[ExecuteAlways]
public class Arrow : MonoBehaviour
{
    BoxCollider2D collider;
    SpriteRenderer sprite;

    public Vector2 Direction = Vector2.right;

    int _Size = 1;
    public int Size { get { return _Size; }
        set
        {
            if (value == _Size || value <= 0) return;
            StartCoroutine(ChangeSize(value - _Size));
            _Size = value;
        }
    }

    IEnumerator ChangeSize(int change)
    {
        float progress = 0.0f;
        while (progress < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            float deltaTime = Mathf.Min(progress + Time.deltaTime, 1) - progress;
            float ease = change * (Mathf.Cos(Mathf.PI * progress) - Mathf.Cos(Mathf.PI * (progress + deltaTime))) * 0.5f;
            progress += deltaTime;
            
            collider.size += ease * new Vector2(Mathf.Abs(Direction.x), Mathf.Abs(Direction.y));
            sprite.size = collider.size;
            
            transform.position += ease * 0.5f * (transform.rotation * (Vector3) Direction);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
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
