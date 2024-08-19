using UnityEngine;

public class AnimateMass : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    public bool IsDead = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Mass", rb.mass);
        if (transform.CompareTag("dying") && !IsDead)
        {
            IsDead = true;
            animator.SetTrigger("Die");
        }

    }
}
