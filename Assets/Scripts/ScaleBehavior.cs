using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ScaleBehavior : MonoBehaviour
{
    public List<Rigidbody2D> Bodies = new();

    public int Mass = 0;
    public int AbsorbableMass = 3;
    public int MaxExtraMass = 2;

    // Start is called before the first frame update
    void Start()
    {
        UserInput.Actions["Attack"].started += (context) =>
        {
            if (this == null)
            {
                Debug.Log("wtf");
                return;
            }

            var ray = Camera.main.ScreenPointToRay(UserInput.Actions["MousePosition"].ReadValue<Vector2>());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null && hit.collider.TryGetComponent(out Arrow arrow) && Mass > 0)
            {
                Mass -= 1;

                if (!arrow) return;

                if (arrow.IsShrink)
                {
                    arrow.Size -= 1;
                }
                else
                {
                    arrow.Size += 1;
                }
            }
        };

        UserInput.Actions["Absorb"].started += (context) =>
        {
            var ray = Camera.main.ScreenPointToRay(UserInput.Actions["MousePosition"].ReadValue<Vector2>());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            //if (hit.collider != null && hit.collider.TryGetComponent(out Arrow arrow) && arrow.Size > 1)
            //{
            //    Mass += 1;
            //    arrow.Size -= 1;
            //    return;
            //}
            for (int i = 0; i < Bodies.Count; i++)
            {
                Debug.Log("thing");
                Debug.Log(i);
                var body = Bodies[i];

                if (body.bodyType == RigidbodyType2D.Static) continue;

                int possibleMass = (int)(body.mass * Mathf.Max(body.gameObject.transform.localScale.x, body.gameObject.transform.localScale.z));
                int mass = Mathf.Max(0, AbsorbableMass - Mass);
                mass = Mathf.Min(mass, possibleMass);
                //Debug.Log(mass);
                Mass += mass;
                //Debug.Log(body.mass - mass);
                if (body.mass - mass == 0)
                {
                    Destroy(body.gameObject);
                    continue;
                }
                body.mass -= mass;

            }
        };
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.attachedRigidbody != null)
        {
            Bodies.Add(collider.attachedRigidbody);
            Debug.Log("gotchu");
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