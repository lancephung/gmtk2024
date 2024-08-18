using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    [SerializeField] private List<Rigidbody2D> _targets = new();
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D rigidbody))
        {
            //Debug.Log("deez nuts");
            _animator.ResetTrigger("Release");
            _animator.SetTrigger("Press");
            // activate anim
            foreach (var target in _targets)
            {
                // do the thing

            }

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _animator.ResetTrigger("Press");
        _animator.SetTrigger("Release");

    }
}
