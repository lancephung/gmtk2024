using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScaleBehavior : MonoBehaviour
{
    public static int Mass = 0;
    public static int AbsorbableMass = 3;
    public static int MaxMass = 5;

    public static int HeldMass = 0;

    float _previousFloorY = 0;

    Rigidbody2D _rigidbody;
    BoxCollider2D boxCollider;
    CapsuleCollider2D capsuleCollider;
    bool down = false;

    Animator animator;
    

    // Start is called before the first frame update
    void Start()
    {
        Mass = 0;
        HeldMass = 0;
        _previousFloorY = transform.position.y;
        _rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();

        InputSystem.actions.FindAction("Attack").started += (context) =>
        {
            var ray = Camera.main.ScreenPointToRay(InputSystem.actions.FindAction("MousePosition").ReadValue<Vector2>());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null && hit.collider.TryGetComponent(out Arrow arrow) && Mass > 0)
            {
                //Debug.Log(hit.rigidbody.mass);
                if (arrow.IsShrink && arrow.Size <= 1) return;
                Mass--;
                arrow.Activate();
            }
        };

        //InputSystem.actions.FindAction("Absorb").started += (context) =>
        //{
        //    Absorb();
        //};
    }

    void Die()
    {
        enabled = false;
        AudioManager.PlaySound("death");
        foreach (var particle in GetComponentsInChildren<ParticleSystem>())
        {
            particle.Play();
        }
    }

    private void FixedUpdate()
    {
        //Debug.Log("Supporting a mass of: " + GetTotalMassSupportedAt(transform.position) + ", Absorbed mass of: " + Mass);
        int mass_ontop = GetSupportedMass(boxCollider);
        if (mass_ontop + Mass > MaxMass)
        {
            // Die by being crushed
            Die();
        }

        down = mass_ontop > 0;
        HeldMass = mass_ontop;
        animator.SetBool("Down", down);

        Absorb();

    }


    int GetSupportedMass(Collider2D collider, int check = 0, List<Rigidbody2D> bodies = null)
    {
        bodies ??= new();

        int total = 0;
        Rigidbody2D body = collider.attachedRigidbody;
        if (body == null || check >= 10 || body.CompareTag("dying") || body.bodyType == RigidbodyType2D.Static || body.isKinematic || !body.simulated) return 0;
        if (body != _rigidbody && !bodies.Contains(body)) 
        {
            total += (int) body.mass;
            bodies.Add(body);
        } 

        ContactPoint2D[] contacts = new ContactPoint2D[8];
        
        List<Collider2D> colliders = new();
        for (int i = 0; i < body.GetContacts(contacts); i++)
        {
            var contact = contacts[i];

            var normal = contact.normal;
            if (contact.collider != collider) normal *= -1;

            var otherCollider = contact.collider == collider ? contact.otherCollider : contact.collider;

            if (normal.y < 0.1f || colliders.Contains(otherCollider)) continue;

            total += GetSupportedMass(otherCollider, check + 1,bodies);
        }

        return total;
    }

    void Absorb()
    {
        if (this == null) return;
        var absorbDistance = .25f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.up, absorbDistance);

        foreach (RaycastHit2D hit in hits)
        {
            var body = hit.collider.attachedRigidbody;

            if (body == null || body == _rigidbody || body.bodyType == RigidbodyType2D.Static) continue;
            if (body.CompareTag("dying")) continue;

            int mass = Mathf.Max(0, AbsorbableMass - Mass);
            if (mass == 0) continue;
            AudioManager.PlaySound("absorb");

            mass = Mathf.Min(mass, (int) body.mass);
            //Debug.Log("Mass");
            Mass += mass;
            down = false;
            if (body.mass - mass == 0)
            {
                //body.GetComponent<Animator>().SetTrigger("Die");
                //Destroy(body.gameObject);
                body.tag = "dying";

                continue;
            }
            body.mass -= mass;
        }
    }


    void OnTriggerEnter2D(Collider2D collider)
    {
        // Trigger by the small circle collider trigger between the other two colliders on the player prefab
        if (collider.isTrigger) return;
        Debug.Log("crushed to death");
        Die();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != capsuleCollider && collision.otherCollider != capsuleCollider) return;
        bool check = false;
        for (int i = 0; i < collision.contactCount; i++)
        {
            var contact = collision.GetContact(i);

            var normal = contact.normal;
            // if (collision.collider != capsuleCollider) normal *= -1;

            // Debug.Log((collision.otherCollider == capsuleCollider) + " " + normal);
            if (normal.y > 0.5f)
            {
                check = true;
                break;
            }
        }

        if (!check) return;

        var fallDistance = Mathf.Round(_previousFloorY - transform.position.y);

        Debug.Log("Fell "  + fallDistance.ToString("F2"));
        if (fallDistance > Mass + 1)
        {
            // immediately die
            Die();
            return;
        }

        _previousFloorY = transform.position.y;
    }
}