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

    private Coroutine _animationRoutine;

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

            if (_animationRoutine != null)
            {
                StopCoroutine(_animationRoutine);
            }
            _animationRoutine = StartCoroutine(UpdateSize(target));

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

    IEnumerator UpdateSize(float endSize)
    {
        var startPos = _frontRB.transform.position;
        var endPos = transform.position + (Vector3)Direction * (endSize - 1);

        var startSpritePos = _middleRB.transform.position;
        var endSpritePos = transform.position + .5f * (Vector3)Direction * (endSize - 1);

        var startSpriteSize = _spriteRenderer.size;
        var endSpriteSize = (new Vector3(Mathf.Abs(Direction.x), Mathf.Abs(Direction.y)) * endSize) + (Direction.x == 0 ? new Vector3(1, 0) : new Vector3(0, 1));

        float animationProgress = 0.0f;
        //push.Play();
        while (animationProgress < 1.0f)
        {
            yield return new WaitForEndOfFrame();
            if (this == null) yield break;
            float deltaTime = Mathf.Min(animationProgress + Time.deltaTime / _animationDuration, 1) - animationProgress;
            animationProgress += deltaTime;

            float progress = _activationAnimationCurve.Evaluate(animationProgress);
            _frontRB.MovePosition(startPos + ((endPos - startPos) * progress));
            _middleRB.transform.position = startSpritePos + ((endSpritePos - startSpritePos) * progress);
            _spriteRenderer.size = startSpriteSize + (((Vector2)endSpriteSize - startSpriteSize) * progress);

            //push.transform.localPosition += dist;
        }
        _frontRB.MovePosition(startPos + (endPos - startPos));
        _middleRB.transform.position = endSpritePos;

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

            var absDir = new Vector2(Mathf.Abs(Direction.x), Mathf.Abs(Direction.y));
            var check = _spriteRenderer.size * absDir + (Direction.x == 0 ? new Vector2(1, 0) : new Vector2(0, 1));
            int currentSize = Mathf.RoundToInt(Mathf.Max(check.x, check.y));
            // Debug.Log(currentSize);
            if (currentSize != _Size)
            {
                _spriteRenderer.size = _Size * absDir + (Direction.x == 0 ? new Vector2(1, 0) : new Vector2(0, 1));
                _frontRB.transform.localPosition = (_Size - 1) * (Vector3)Direction;
                _middleRB.transform.localPosition = (_Size - 1) * (Vector3)Direction * .5f;
                //push.transform.position += (_Size - currentSize) * 0.5f * (Vector3) Direction;
            }
        }

        #endif

        //collider.size = sprite.size - 0.2f * Vector2.one;
    }
}
