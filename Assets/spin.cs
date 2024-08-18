using UnityEngine;

public class spin : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 300 * Time.deltaTime, 0);
    }
}
