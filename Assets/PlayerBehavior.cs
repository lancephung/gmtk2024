using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerBehavior : MonoBehaviour
{
    public List<Rigidbody2D> Bodies = new();

    Coroutine routine;

    [SerializeField] LineRenderer line;

    public float mass = 0;

    IEnumerator ScaleLoop(Transform obj)
    {
        line.enabled = true;
        
        Vector2 worldMousePos = Camera.main.ScreenToWorldPoint(UserInput.Actions["MousePosition"].ReadValue<Vector2>());
        Vector2 position = obj.position;
        float distance = (worldMousePos - position).magnitude;
        Vector3 scale = obj.transform.localScale;

        float startMass = mass;

        while (UserInput.Actions["Attack"].IsPressed())
        {
            yield return new WaitForEndOfFrame();

            worldMousePos = Camera.main.ScreenToWorldPoint(UserInput.Actions["MousePosition"].ReadValue<Vector2>());
            
            float factor = (worldMousePos - position).magnitude / distance;
            if (factor > 1)
            {
                factor = Mathf.Min(factor, startMass + 1);
            }
            else
            {
                factor = Mathf.Max(factor, 1 / (startMass + 1));
            }
            Vector3 localScale = factor * scale;
            mass = startMass - factor + 1;

            if (obj.CompareTag("Horizontal") || obj.CompareTag("ExpandRight") || obj.CompareTag("ExpandLeft")) localScale.y = scale.y;
            if (obj.CompareTag("ExpandRight")) obj.position = position + (localScale.x - scale.x) * 0.5f * Vector2.right;
            if (obj.CompareTag("ExpandLeft")) obj.position = position - (localScale.x - scale.x) * 0.5f * Vector2.right;

            if (obj.CompareTag("Vertical") || obj.CompareTag("ExpandUp") || obj.CompareTag("ExpandDown")) localScale.x = scale.x;
            if (obj.CompareTag("ExpandUp")) obj.position = position + (localScale.y - scale.y) * 0.5f * Vector2.up;
            if (obj.CompareTag("ExpandDown")) obj.position = position - (localScale.y - scale.y) * 0.5f * Vector2.up;
            
            obj.localScale = localScale;
            line.SetPositions(new Vector3[]{position, worldMousePos});
        }
        line.enabled = false;
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
            
            var ray = Camera.main.ScreenPointToRay(UserInput.Actions["MousePosition"].ReadValue<Vector2>());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                StartCoroutine(ScaleLoop(hit.collider.transform));
            }
        };

        UserInput.Actions["Absorb"].started += (context) =>
        {
            for (int i = 0; i < Bodies.Count; i++)
            {
                var body = Bodies[i];

                if (body.bodyType == RigidbodyType2D.Static) continue;

                mass += body.mass * Mathf.Max(body.gameObject.transform.localScale.x, body.gameObject.transform.localScale.z);

                Destroy(body.gameObject);
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
