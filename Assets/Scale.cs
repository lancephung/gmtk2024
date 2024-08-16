using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBehavior : MonoBehaviour
{
    public List<Rigidbody2D> Bodies = new();

    Coroutine routine;

    [SerializeField] LineRenderer line;
    [SerializeField] LineRenderer line1;

    Dictionary<Transform, (Vector3, Vector3)> sizes = new();

    IEnumerator ScaleLoop(Transform obj)
    {
        line.enabled = true;
        line1.enabled = true;

        
        Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(UserInput.Actions["MousePosition"].ReadValue<Vector2>());
        Vector2 position = obj.position;
        float distance = (worldMousePos - position).magnitude;
        Vector3 scale = obj.transform.localScale;
        
        Vector3 maxScale;
        Vector3 minScale;
        if (!sizes.ContainsKey(obj))
        {
            maxScale = obj.transform.localScale * 2;
            minScale = obj.transform.localScale * 0.5f;
            if (obj.CompareTag("Horizontal"))
            {
                maxScale.y = obj.transform.localScale.y;
                minScale.y = obj.transform.localScale.y;
            }
            if (obj.CompareTag("Vertical"))
            {
                maxScale.x = obj.transform.localScale.x;
                minScale.x = obj.transform.localScale.x;
            }
            sizes[obj] = (maxScale, minScale);
        }
        else
        {
            maxScale = sizes[obj].Item1;
            minScale = sizes[obj].Item2;
        }

        while (UserInput.Actions["Attack"].IsPressed())
        {
            yield return new WaitForEndOfFrame();

            worldMousePos = Camera.main.ScreenToWorldPoint(UserInput.Actions["MousePosition"].ReadValue<Vector2>());
            
            obj.localScale = Vector3.Max(Vector3.Min(maxScale, (worldMousePos - position).magnitude / distance * scale), minScale);
            
            line.SetPositions(new Vector3[]{position, worldMousePos});
        }
        line.enabled = false;
        line1.enabled = false;
    }    

    // Start is called before the first frame update
    void Start()
    {
        UserInput.Actions["Attack"].started += (context) =>
        {
            if (routine != null)
            {
                StopCoroutine(routine);
            }

            Debug.Log(this);
            var mousePos = Camera.main.ScreenToWorldPoint(UserInput.Actions["MousePosition"].ReadValue<Vector2>());
            var difference = mousePos - transform.position;
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, difference, Mathf.Infinity);
            if (hit.collider != null)
            {
                Debug.Log(hit.collider);
                line1.SetPositions(new Vector3[]{hit.point, transform.position});
                StartCoroutine(ScaleLoop(hit.collider.transform));
            }
        };
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.attachedRigidbody != null)
        {
            Bodies.Add(collider.attachedRigidbody);
        }
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.attachedRigidbody != null)
        {
            Bodies.Remove(collider.attachedRigidbody);
        }
    }
}
