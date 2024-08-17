using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particles : MonoBehaviour
{
    public Rigidbody2D rb;
    public ParticleSystem dust;

    // Start is called before the first frame update
    void Start()
    {
        rb = transform.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity != Vector2.zero)
        {
            dust.Play();
            dust.emissionRate = 10;
        }
        else
        {
            dust.emissionRate = 0;
        }
    }
}
