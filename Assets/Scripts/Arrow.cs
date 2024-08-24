using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Arrow : MonoBehaviour
{
    public bool IsShrink = false;
    public bool UseLinear;
    public int boost;
    //[SerializeField] private bool isShrink = false;
    //BoxCollider2D collider;
    //SpriteRenderer sprite;
    //ParticleSystem push;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Rigidbody2D _frontRB;
    [SerializeField] private Rigidbody2D _middleRB;
    [SerializeField] private float _animationDuration = .2f;
    [SerializeField] private AnimationCurve _activationAnimationCurve;


    public bool ButtonToggle = false; // for arrows that are toggled by a button like the escalator in level 11

    public float DistPerActivation => ButtonToggle ? 1.5f : 1;
    public int ActivateDir => IsShrink ? -1 : 1;

    public Vector2 Direction = Vector2.right;
    //private Rigidbody2D _rigidbody;

    [SerializeField] private float _Size = 1;
    public float Size { get { return _Size; }
        set
        {
            if (value == _Size || value <= 0) return;
            if (this == null) return;
            float target = Mathf.Max(1, value);
            StartCoroutine(UpdateSize(_Size, target));

            _Size = target;
        }
    }

    public void Activate()
    {
        AudioManager.PlaySound("scrape");
        Size += DistPerActivation * ActivateDir;
    }

    public void Deactivate()
    {
        AudioManager.PlaySound("scrape");
        Size -= DistPerActivation * ActivateDir;
    }

    float EasingFunction(float progress)
    {
        if (UseLinear)
        {
            return -(Mathf.Cos(Mathf.PI * progress) + 1) * 0.5f;
        }
        else
        {
            return 1 - Mathf.Pow(1 - progress, 3);
        }
    }

    IEnumerator UpdateSize(float startSize, float endSize)
    {
        var startOffset = (Vector3)Direction * (startSize - 1);
        var changeOffset = (Vector3)Direction * (endSize - startSize);
        var changeSizeOffset = new Vector3(Mathf.Abs(Direction.x), Mathf.Abs(Direction.y)) * (endSize - startSize);
        var startSizeOffset = new Vector3(Mathf.Abs(Direction.x), Mathf.Abs(Direction.y)) * (startSize - 1);


        var defaultSize = Vector3.Max(new Vector3(1, 1), new Vector3(Mathf.Abs(Direction.x), Mathf.Abs(Direction.y)));

        float animationProgress = 0.0f;
        //push.Play();
        while (animationProgress < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            if (this == null) yield break;
            float deltaTime = Mathf.Min(animationProgress + Time.deltaTime / _animationDuration, 1) - animationProgress;
            animationProgress += deltaTime;

            float progress = _activationAnimationCurve.Evaluate(animationProgress);
            _frontRB.MovePosition(transform.position + startOffset + (changeOffset * progress));
            _middleRB.MovePosition(transform.position + .5f * (startOffset + (changeOffset * progress)));
            _spriteRenderer.size = defaultSize + startSizeOffset + (changeSizeOffset * progress);

            //push.transform.localPosition += dist;
        }
        _frontRB.MovePosition(transform.position + startOffset + changeOffset);
        _middleRB.MovePosition(transform.position + .5f * (startOffset + changeOffset));

        //push.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    // Start is called before the first frame update
    void Start()
    {
        //collider = GetComponent<BoxCollider2D>();
        //sprite = _middleObj.GetComponent<SpriteRenderer>();
        //push = GetComponentInChildren<ParticleSystem>();
        //_rigidbody = GetComponent<Rigidbody2D>();

        //collider.edgeRadius = 0.1f;
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
            var check = _spriteRenderer.size * absolute;
            int currentSize = Mathf.RoundToInt(Mathf.Max(check.x, check.y));
            // Debug.Log(currentSize);
            if (currentSize != _Size)
            {
                _spriteRenderer.size += (_Size - currentSize) * absolute;
                transform.position += (_Size - currentSize) * 0.5f * (Vector3) Direction;
                //push.transform.position += (_Size - currentSize) * 0.5f * (Vector3) Direction;
            }
        }

        #endif

        //collider.size = sprite.size - 0.2f * Vector2.one;
    }
}
