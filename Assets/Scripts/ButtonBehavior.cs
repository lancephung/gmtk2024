using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    [SerializeField] private List<Arrow> _targets = new();
    private Animator _animator;
    private bool _isActive = false;

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
        if (_isActive) return;

        if (collision.TryGetComponent(out Rigidbody2D rigidbody))
        {
            _isActive = true;
            _animator.SetBool("Press", true);
            // activate anim
            foreach (var target in _targets)
            {
                // do the thing
                target.Activate();

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        _animator.SetBool("Press", false);
    }
}
