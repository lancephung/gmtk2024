using UnityEngine;

public class PhysicsSound : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //if (rb.velocity.y != 0)
        //{
        //    for (int i = 0; i < collision.contactCount; i++)
        //    {
        //        var contact = collision.GetContact(i);
        //        if (contact.normal.y >= Mathf.Cos(Mathf.PI / 4))
        //        {
        //            AudioManager.PlaySound("knock loud");
        //        }
        //    }
        //}
    }

    private void Update()
    {
    }

}
