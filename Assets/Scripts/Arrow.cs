using System.Collections;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public bool IsShrink = false;
    //[SerializeField] private bool isShrink = false;
    BoxCollider2D collider;
    SpriteRenderer sprite;

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

        while (progress < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            progress += Time.deltaTime / duration;
            float easedProgress = 1.0f - Mathf.Pow(1.0f - Mathf.Clamp01(progress), exponent);

            collider.size = Vector2.Lerp(initialSize, targetSize, easedProgress);
            sprite.size = collider.size;

            transform.position = Vector3.Lerp(initialPosition, targetPosition, easedProgress);
        }

        collider.size = targetSize;
        sprite.size = targetSize;
        transform.position = targetPosition;
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
        sprite.size = collider.size;
    }
}
