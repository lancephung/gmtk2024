using System.Collections;
using System.Collections.Generic;
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
        //if (rb.velocity.y != 0 && collision.otherCollider.GetComponentInParent<ScaleBehavior>() != null)
        //{
        //    AudioManager.PlaySound("thump");
        //}
    }

    private void Update()
    {
    }

}
