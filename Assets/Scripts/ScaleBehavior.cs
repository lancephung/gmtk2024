using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScaleBehavior : MonoBehaviour
{
    public static int Mass = 0;
    public static int AbsorbableMass = 3;
    public static int MaxMass = 5;

    public static int HeldMass = 0;

    [SerializeField] int _StartingMass = 0;

    [SerializeField] private bool _isDebugging = false;

    float _previousFloorY = Mathf.NegativeInfinity;

    Rigidbody2D _rigidbody;
    BoxCollider2D boxCollider;
    CapsuleCollider2D capsuleCollider;
    bool down = false;

    public static bool Dead = false;
    public static string CauseOfDeathStr = "";

    [SerializeField] private bool _freezeX = false;

    [SerializeField] private PhysicsMaterial2D _groundPhysicsMaterial; // to be applied when on the ground and slope to prevent unintended sliding
    [SerializeField] private PhysicsMaterial2D _frictionlessPhysicsMaterial; // to be applied when player is sitting on a mass or arrow block to avoid being dragged


    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        Dead = false;
        Mass = _StartingMass;
        HeldMass = 0;
        _rigidbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();

        InputSystem.actions.FindAction("Attack").started += (context) =>
        {
            if (Dead) return;
            var ray = Camera.main.ScreenPointToRay(InputSystem.actions.FindAction("MousePosition").ReadValue<Vector2>());
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null && hit.collider.transform.parent.TryGetComponent(out Arrow arrow) && Mass > 0)
            {
                //Debug.Log(hit.rigidbody.mass);
                if (arrow.IsShrink && arrow.Size <= 1) return;
                Mass--;
                arrow.Activate();
            }
        };

        InputSystem.actions.FindAction("Reset").started += (context) =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        };

        //InputSystem.actions.FindAction("Absorb").started += (context) =>
        //{
        //    Absorb();
        //};
    }

    void Die(string deathStr)
    {
        CauseOfDeathStr = deathStr;
        Dead = true;
        enabled = false;
        AudioManager.PlaySound("thump");
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
            Die("crushed by weight");
        }
        HeldMass = mass_ontop;

        var detectMassDistance = 1f; // seems to make absorbing multiple masses smoother but also appears to break one of the absorb particle animations
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.up, detectMassDistance);
        var detectMass = hits.Any(hit => hit.collider.attachedRigidbody && hit.collider.attachedRigidbody != _rigidbody && hit.collider.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic);
        down = mass_ontop > 0 || detectMass;
        animator.SetBool("Down", down);

        Absorb();

        if (crushers.Count == 0)
        {
            crushTime = 0;
        }
        else
        {
            if (crushTime > 0.25f)
            {
                //Debug.Log("crushed to death");
                Die("crushed to death");
            }
            crushTime += Time.fixedDeltaTime;
        }

        if (_freezeX)
        {
            // dont freeze x when on a slope
            _rigidbody.velocity *= Vector3.up;
        }
    }

    private void Update()
    {
        CursorManager.CanAttack = Mass > 0;
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

            if (normal.y < 0.7f || colliders.Contains(otherCollider)) continue;

            total += GetSupportedMass(otherCollider, check + 1,bodies);
        }

        return total;
    }

    void Absorb()
    {
        if (this == null || gameObject.layer == LayerMask.NameToLayer("Intangible")) return;
        var absorbDistance = .25f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.up, absorbDistance);

        foreach (RaycastHit2D hit in hits)
        {
            var body = hit.collider.attachedRigidbody;

            if (body == null || body == _rigidbody || body.bodyType == RigidbodyType2D.Kinematic || body.bodyType == RigidbodyType2D.Static) continue;
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

    List<Collider2D> crushers = new();
    float crushTime = 0;

    void OnTriggerEnter2D(Collider2D collider)
    {
        // Trigger by the small circle collider trigger between the other two colliders on the player prefab
        if (collider.isTrigger) return;
        crushers.Add(collider);
    }
    void OnTriggerExit2D(Collider2D collider)
    {
        crushers.Remove(collider);
        // Debug.Log("crushed to death");
        // Die();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.collider == capsuleCollider)
        {
            if (collision?.otherRigidbody?.bodyType == RigidbodyType2D.Static)
            {
                // Change material to have friction on ground/slope
                _rigidbody.sharedMaterial = _groundPhysicsMaterial;
                Debug.Log("found some ground");
            }
            else
            {
                // Change material to be frictionless on mass / arrow
                _rigidbody.sharedMaterial = _frictionlessPhysicsMaterial;
                Debug.Log("sitting on a mass or arrow");
            }
        }
        if (collision.otherCollider == capsuleCollider)
        {
            if (collision?.otherRigidbody?.bodyType == RigidbodyType2D.Static)
            {
                // Change material to have friction on ground/slope
                _rigidbody.sharedMaterial = _groundPhysicsMaterial;
                Debug.Log("found some ground");
            }
            else
            {
                // Change material to be frictionless on mass / arrow
                _rigidbody.sharedMaterial = _frictionlessPhysicsMaterial;
                Debug.Log("sitting on a mass or arrow");
            }
        }


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

        // Debug.Log("Fell "  + fallDistance.ToString("F2"));
        gameObject.layer = LayerMask.NameToLayer("Default");
        if (fallDistance > Mass + 1)
        {
            // immediately die
            Die("died to fall damage");
            return;
        }

        _previousFloorY = transform.position.y;
    }

    void OnCollisionStay2D(Collision2D collision)
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
        
        _previousFloorY = transform.position.y;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // reset horizontal velocity when pushed horizontally by mass?
        // resetting vertical velocity appears to give player the ability to fly when pushed by masses
        if (collision?.rigidbody?.isKinematic != true || collision?.otherRigidbody?.isKinematic != true)
        {
            _rigidbody.velocity *= Vector2.up;
            Debug.Log("stopped 1");
        }

        // reset vertical velocity when pushed horizontally by arrow
        if (collision?.rigidbody?.bodyType == RigidbodyType2D.Kinematic || collision?.otherRigidbody?.bodyType == RigidbodyType2D.Kinematic)
        {
            _rigidbody.velocity *= Vector2.up;
            Debug.Log("stopped 2");
        }

        // reset vertical velocity when pushed upwards
        if (collision.collider == capsuleCollider || collision.otherCollider == capsuleCollider)
        {
            // apparently this code also catches when a mass underneath the player starts falling and then the player starts falling
            // also catches when a mass is pushed into the player from above or the side for the purpose of absorbing mass while falling
            // fix these issues by only resetting y velocity if going upwards
            if (_rigidbody.velocity.y > .05f)
            {
                _rigidbody.velocity *= Vector2.right;
            }
            Debug.Log("stopped 3");
        }
    }
}