using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D _defaultCursor;
    [SerializeField] private Texture2D _attackCursor; // cursor for when hovering over arrow tiles
    [SerializeField] private Vector2 _cursorOffset;

    public static Texture2D DefaultCursor;
    public static Texture2D AttackCursor;
    private static Vector2 CursorOffset;

    // Start is called before the first frame update
    void Start()
    {
        DefaultCursor = _defaultCursor;
        AttackCursor = _attackCursor;
        CursorOffset = _cursorOffset;
        ShowCursor(_defaultCursor);
    }

    public static void ShowCursor(Texture2D texture)
    {
        Cursor.SetCursor(texture, CursorOffset, CursorMode.Auto);
    }

    private void Update()
    {
        //Debug.Log("Is this even working");
        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero);
        if (hit.collider && hit.collider.gameObject.TryGetComponent(out Arrow arrow))
        {
            //Debug.Log("Detection");
            ShowCursor(AttackCursor);
        } else
        {
            Debug.Log("Nor");
            ShowCursor(DefaultCursor);
        }
    }

}
