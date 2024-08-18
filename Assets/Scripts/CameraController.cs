using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    Vector3 start;
    Camera camera;

    Transform follow;
    // Start is called before the first frame update
    void Start()
    {
        start = transform.position;
        camera = GetComponent<Camera>();
        
        if (follow == null)
        {
            follow = FindFirstObjectByType<ScaleBehavior>().transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 size = new Vector2(camera.pixelWidth, camera.pixelHeight);
        // transform.position = Vector3.Lerp(transform.position, start + (Vector3) Vector2.Scale((Mouse.current.position.value - size / 2) / size, new Vector2(2, 2)), 0.01f);
        transform.position = follow.position + (Vector3) Vector2.Scale((Mouse.current.position.value - size / 2) / size, new Vector2(0, 0)) + new Vector3(0, 2, -10);
        // transform.position.Set(transform.position.x, transform.position.y, -10);
    }
}
