using System.Collections.Generic;
using UnityEngine;

public class ButtonBehavior : MonoBehaviour
{
    [SerializeField] private List<Arrow> _targets = new();
    private Animator _animator;
    private int _touching = 0;
    //public bool Loggabble = false; // for debug messages
    public bool IsActive => _touching > 0;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _touching = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if (!Loggabble) return;
        //Debug.Log("button touching: " + _touching);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.attachedRigidbody) return;
        if (!IsActive)
        {
            _animator.SetBool("Press", true);
            _targets.ForEach(t => t.Activate());

        }
        _touching++;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.attachedRigidbody) return;
        _touching--;
        if (IsActive) return;
        _animator.SetBool("Press", false);
        _targets.ForEach(t => t.Deactivate());
    }
}
