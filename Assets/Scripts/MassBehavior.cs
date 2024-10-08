using Unity.VisualScripting;
using UnityEngine;

public class MassBehavior : MonoBehaviour
{
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _collider;
    public bool FreezeX;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //_rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);
        //if (_rigidbody.velocity.y < -.3)

        //ContactPoint2D[] contacts = new ContactPoint2D[8];
        //List<Collider2D> colliders = new();

        //for (int i = 0; i < _collider.GetContacts(contacts); i++)
        //{
        //    var contact = contacts[i];

        //    var normal = contact.normal;
        //    if (contact.collider != _collider) normal *= -1;

        //    var otherCollider = contact.collider == _collider ? contact.otherCollider : contact.collider;

        //    if (normal.y < 0.7f || colliders.Contains(otherCollider)) continue;
        //    Debug.Log("Ground");

        //}
        //{
        //    Debug.Log("fgsdfsdfsdf");
        //    _rigidbody.velocity *= new Vector2(0, 1);
        //}
        if (FreezeX)
        {
            _rigidbody.velocity *= new Vector2(0, 1);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // reset velocity when stop colliding with arrow
        if (!collision.rigidbody || !collision.otherRigidbody) return;
        if (collision.rigidbody.bodyType == RigidbodyType2D.Kinematic || collision.otherRigidbody.bodyType == RigidbodyType2D.Kinematic)
        {
            // only reset velocity in the direction the arrow is pushing
            if (collision.transform.parent.TryGetComponent(out Arrow arrow))
            {
                // dont reset velocity when being pushed down (and likely falling)
                if (arrow.Direction.y != -1)
                {
                    _rigidbody.velocity *= new Vector2(arrow.AbsoluteDirection.y, arrow.AbsoluteDirection.x);
                }
                
            }
            

        }
        if (collision.collider.TryGetComponent(out MassBehavior mass1) && collision.otherCollider.TryGetComponent(out MassBehavior mass2))
        {
            _rigidbody.velocity *= Vector2.up;
        }

        // detect mass getting flung upwards
        if (collision.rigidbody.bodyType == RigidbodyType2D.Dynamic && collision.otherRigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            // reset y velocity if being shot upwards up
            if (_rigidbody.velocity.y > .1f)
            {
                _rigidbody.velocity *= Vector2.right;
            }

        }
    }
}
