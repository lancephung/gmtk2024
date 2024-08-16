using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static Dictionary<string, InputAction> Actions = new();

    static UserInput _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        } else {
            _instance = this;
        }
        
        // foreach (PlayerAction action in Enum.GetValues(typeof(PlayerAction)))
        // {
        //     string name = Enum.GetName(typeof(PlayerAction), action);
        //     Actions[name] = InputSystem.actions.FindAction(name);
        // }

        foreach (InputAction action in InputSystem.actions)
        {
            Actions[action.name] = action;
        }
    }
}