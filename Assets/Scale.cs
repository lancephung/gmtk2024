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
    [SerializeField] LineRenderer line1;

    public float mass = 0;

    IEnumerator ScaleLoop(Transform obj)
    {
        line.enabled = true;
        line1.enabled = true;

        
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

            if (obj.CompareTag("Horizontal")) localScale.y = scale.y;
            if (obj.CompareTag("Vertical")) localScale.x = scale.x;
            
            obj.localScale = localScale;
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
