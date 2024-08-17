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
        sprite.size = collider.size;
    }
}
