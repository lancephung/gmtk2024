using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ScaleBehavior : MonoBehaviour
{
    public static int Mass = 0;
    public int AbsorbableMass = 3;
    public int MaxExtraMass = 2;

    private float _previousFloorY = 0;

    private Rigidbody2D _rigidbody;

    private void FixedUpdate()
    {
        // Detect fall damage
        if (_rigidbody.velocity.y == 0)
        {
            if (_previousFloorY - transform.position.y > Mass + 1)
            {
                // player fell too much
                // die
                Debug.Log("It's Joever");
                return;
            }
            _previousFloorY = transform.position.y;

        }



        bool found = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.up, 1);

        bool blocked = false;

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Block-Raycast"))
            {
                blocked = true;
                break;
            }

            var body = hit.collider.attachedRigidbody;

            if (body == null || body == _rigidbody || body.bodyType == RigidbodyType2D.Static) continue;

            if (blocked) continue;
            found = true;
            transform.GetComponent<Animator>().SetBool("MassAbove", true);
        }
        if (!found)
        {
            transform.GetComponent<Animator>().SetBool("MassAbove", false);
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        Mass = 0;
        _previousFloorY = transform.position.y;
        _rigidbody = GetComponent<Rigidbody2D>();

        UserInput.Actions["Attack"].started += (context) =>
        {
            if (this == null)
            {
                return;
            }

            var ray = Camera.main.ScreenPointToRay(UserInput.Actions["MousePosition"].ReadValue<Vector2>());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null && hit.collider.TryGetComponent(out Arrow arrow) && Mass > 0)
            {
                AudioManager.PlaySound("scrape");
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
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.up, 1);

            bool blocked = false;

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Block-Raycast"))
                {
                    blocked = true;
                    break;
                }

                var body = hit.collider.attachedRigidbody;

                if (body == null || body == _rigidbody || body.bodyType == RigidbodyType2D.Static) continue;

                if (blocked) continue;

                int possibleMass = (int)(body.mass * Mathf.Max(body.gameObject.transform.localScale.x, body.gameObject.transform.localScale.z));
                int mass = Mathf.Max(0, AbsorbableMass - Mass);
                mass = Mathf.Min(mass, possibleMass);

                Mass += mass;

                if (body.mass - mass == 0)
                {
                    Destroy(body.gameObject);
                    continue;
                }
                body.mass -= mass;
            }
        };
    }
}