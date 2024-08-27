using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D _defaultCursor;
    [SerializeField] private Texture2D _hoverCursor; // cursor for when hovering over arrow tiles
    [SerializeField] private Texture2D _attackCursor; // onactivation
    [SerializeField] private Vector2 _cursorOffset;

    private ParticleSystem _particleSystem;

    public static Texture2D DefaultCursor;
    public static Texture2D HoverCursor;
    public static Texture2D AttackCursor;
    private static Vector2 CursorOffset;

    public static bool CanAttack = false;
    private bool pause = false;

    // Start is called before the first frame update
    void Start()
    {
        DefaultCursor = _defaultCursor;
        HoverCursor = _hoverCursor;
        AttackCursor = _attackCursor;
        CursorOffset = _cursorOffset;
        //Debug.Log(CursorOffset);
        ShowCursor(DefaultCursor);
        _particleSystem = GetComponent<ParticleSystem>();

        InputSystem.actions.FindAction("Attack").started += (context) =>
        {
            if (this == null || !CanActivateArrow()) return;
            pause = true;
            ShowCursor(AttackCursor);
            _particleSystem.Emit(15);
        };

        InputSystem.actions.FindAction("MousePosition").performed += (context) =>
        {
            UpdateCursor();
        };

        InputSystem.actions.FindAction("Attack").canceled += (context) =>
        {
            //Debug.Log("stop");
            UpdateCursor();
            pause = false;
        };
    }

    public static void ShowCursor(Texture2D texture)
    {
        Cursor.SetCursor(texture, CursorOffset, CursorMode.Auto);
    }

    private void LateUpdate()
    {
        if (CanActivateArrow() && !pause)
        {
            UpdateCursor();
        }
    }
    public void UpdateCursor()
    {
        if (CanActivateArrow())
        {
            ShowCursor(HoverCursor);
        }
        else
        {
            ShowCursor(DefaultCursor);
        }
    }

    public bool CanActivateArrow()
    {
        if (!CanAttack) return false;
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var hits = Physics2D.RaycastAll(mouseWorldPos, Vector2.zero);
        return hits.Any(hit => hit.collider.transform?.parent?.TryGetComponent(out Arrow arrow) == true);
    }

}
