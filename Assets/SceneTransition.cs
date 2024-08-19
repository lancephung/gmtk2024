using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        GetComponent<Animator>().SetTrigger("Entry");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
