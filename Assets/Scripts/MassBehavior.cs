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
        // problem occurs where stacked masses get pulled along with the bottom one, likely due to friction

        // reset velocity when stop colliding with arrow
        if (collision.rigidbody.isKinematic || collision.otherRigidbody.isKinematic)
        {
            _rigidbody.velocity *= 0;
        }
    }
}
