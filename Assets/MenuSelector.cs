using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuSelector : MonoBehaviour
{
    public List<RectTransform> options;
    public GameObject icon;
    public int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        foreach (RectTransform option in options)
        {
            if (option.name == "continue" && !GameManager.hasSave)
            {
                option.GetComponent<TMP_Text>().color = new Color(0.75f, 0.75f, 0.75f, 1);
            }

            if (option.name == "level select" && !GameManager.hasSave)
            {
                option.GetComponent<TMP_Text>().color = new Color(0.75f, 0.75f, 0.75f, 1);
            }
        }

        InputSystem.actions.FindAction("MenuDown").started += (context) => {
            currentIndex += 1;
            currentIndex %= options.Count;
            UpdatePosition();
        };

        InputSystem.actions.FindAction("MenuUp").started += (context) => {
            if (currentIndex == 0)
            {
                currentIndex = options.Count - 1;
            }
            else
            {
                currentIndex -= 1;
                currentIndex %= options.Count;
            }
            UpdatePosition();
        };

        InputSystem.actions.FindAction("MenuSelect").started += (context) => {
            Select();
        };
    }

    void UpdatePosition()
    {
        RectTransform option = options[currentIndex];
        RectTransform iconRectTransform = icon.GetComponent<RectTransform>();
        Debug.Log(option.position.y - option.rect.size.y / 2);
        iconRectTransform.localPosition = new Vector2(-235, option.localPosition.y - option.rect.size.y / 2);
    }

    void Select()
    {
    }
}
