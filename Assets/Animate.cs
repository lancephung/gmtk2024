using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animate : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    public bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
        animator = transform.GetComponent<Animator>();
    }

    void Delete()
    {
        GameObject.Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Mass", rb.mass);
        if (transform.CompareTag("dying") && dead == false)
        {
            dead = true;
            animator.SetTrigger("Die");
        }

    }
}
