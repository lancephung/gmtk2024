using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Unity.VisualScripting;

public class Pause : MonoBehaviour
{
    public bool open = false;
    public CanvasGroup cg;
    public MenuSelector menu;
    // Start is called before the first frame update
    void Start()
    {
        cg = gameObject.GetComponent<CanvasGroup>();
        InputSystem.actions.FindAction("MenuESC").started += (context) => {
            open = !open;
            menu.active = open;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            cg.alpha = Mathf.Lerp(cg.alpha, 1, 0.2f);
        } 
        else
        {
            cg.alpha = Mathf.Lerp(cg.alpha, 0, 0.2f);
        }
    }
}
