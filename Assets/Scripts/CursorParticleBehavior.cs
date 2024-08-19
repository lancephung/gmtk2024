using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.ParticleSystem;

public class CursorParticleBehavior : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    void Update()
    {
        transform.position = _camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
    }
}
