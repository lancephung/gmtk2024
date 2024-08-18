using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuSelector : MonoBehaviour
{
    public List<RectTransform> options;
    public GameObject icon;
    public Vector2 goal;
    public int currentIndex;
    public bool active;
    public Color activeColor;
    public Color inactiveColor;
    public Color lockedColor;
    public CanvasGroup cg;
    public float x;
    public bool autoX;
    public float fresh;

    // Start is called before the first frame update
    void Start()
    {
        fresh = 0;
        RectTransform iconRectTransform = icon.GetComponent<RectTransform>();
        x = iconRectTransform.localPosition.x;
        options = new List<RectTransform>();
        foreach (Transform child in transform)
        {
            if (child.GetComponent<RectTransform>() != null && child != icon.transform)
            {
                options.Add(child.GetComponent<RectTransform>());
            }
        }

        UpdateColors();

        InputSystem.actions.FindAction("MenuDown").performed += (context) => {
            if (this == null || !active) { 
                return;
            }
            currentIndex += 1;
            currentIndex %= options.Count;
            UpdatePosition();
        };

        InputSystem.actions.FindAction("MenuUp").performed += (context) => {
            if (this == null || !active) { 
                return;
            }
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
            if (this == null || !active || fresh < 100)
            {
                return;
            } else
            {
                Debug.Log("Hit");
            }
            Select();
        };

        foreach (RectTransform option in options)
        {
            EventTrigger eventTrigger = option.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerEnter
            };
            pointerEnterEntry.callback.AddListener((data) =>
            {
                if (!active) return;
                currentIndex = options.IndexOf(option);
                UpdatePosition();
            });
            eventTrigger.triggers.Add(pointerEnterEntry);

            EventTrigger.Entry pointerClickEntry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            pointerClickEntry.callback.AddListener((data) =>
            {
                if (!active) return;

                Select();
            });
            eventTrigger.triggers.Add(pointerClickEntry);
        }
    }

    void UpdatePosition()
    {
        RectTransform option = options[currentIndex];
        RectTransform iconRectTransform = icon.GetComponent<RectTransform>();

        if (autoX)
        {
            x = option.localPosition.x - 50;
        }

        goal = new Vector2(x, option.localPosition.y - option.rect.size.y / 2);

    }

    public void SwitchTo(string MenuName)
    {
        cg.alpha = 0;
        cg.GetComponentInParent<Canvas>().sortingOrder = -1;
        active = false;
        MenuSelector menu = GameObject.Find(MenuName).GetComponentInChildren<MenuSelector>();
        menu.cg.alpha = 1;
        menu.cg.GetComponentInParent<Canvas>().sortingOrder = 10;
        menu.active = true;
        menu.fresh = 0;
        menu.currentIndex = 0;
        menu.UpdatePosition();
    }

    void Select()
    {
        if (!active || fresh < 100)
        {
            return;
        }
        string selected = options[currentIndex].name;
        //Debug.Log(currentIndex, this);
        switch (selected)
        {
            case "new game":
                GameManager.hasSave = true;
                GameManager.level = 1;
                GameManager.highest_level = 1;
                SceneManager.LoadScene("Level 1", LoadSceneMode.Single);
                break;
            case "continue":
                if (!GameManager.hasSave) return;
                SceneManager.LoadScene("Level " + GameManager.level.ToString(), LoadSceneMode.Single);
                break;
            case "level select":
                SwitchTo("level screen");
                return;
            case "settings":
                SwitchTo("settings screen");
                break;
            case "credits":
                SwitchTo("credits screen");
                break;
            case "main menu":
                SceneManager.LoadScene("Menu", LoadSceneMode.Single);
                break;
            case "back":
                SwitchTo("main screen");
                return;
            case "retry":
                SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                break;
            case "resume":
                transform.GetComponentInParent<Pause>().open = false;
                break;
            default:

                if (selected.StartsWith("PLevel"))
                {
                    if (int.Parse(selected.Substring(selected.Length - 2)) <= GameManager.highest_level)
                    {
                        Debug.Log(selected);
                        SceneManager.LoadScene(selected.Substring(1), LoadSceneMode.Single);
                    }
                }
                break;
        }

    }

    void UpdateColors()
    {
        if (!active) return;
        foreach (RectTransform option in options)
        {
            if (option.name.StartsWith("PLevel"))
            {
                if (int.Parse(option.name.Substring(option.name.Length - 2)) > GameManager.highest_level)
                {
                    Debug.Log(option.name);
                    option.GetComponentInChildren<Image>().color = lockedColor;
                    continue;
                }
            }

            if (options[currentIndex].name == option.name)
            {
                if (options[currentIndex].name.StartsWith("PLevel"))
                {
                    option.GetComponentInChildren<Image>().color = activeColor;
                    continue;
                }
                option.GetComponentInChildren<TMP_Text>().color = activeColor;

            }
            else
            {
                if (option.name.StartsWith("PLevel"))
                {
                    option.GetComponentInChildren<Image>().color = inactiveColor;
                    continue;
                }
                option.GetComponentInChildren<TMP_Text>().color = inactiveColor;
            }

            if (option.name == "continue" && !GameManager.hasSave)
            {
                option.GetComponentInChildren<TMP_Text>().color = lockedColor;
            }
            else if (option.name == "continue" && GameManager.hasSave)
            {
                option.GetComponentInChildren<TMP_Text>().text = "continue | level " + GameManager.level.ToString();
            }
        }
    }

    void Update()
    {
        UpdateColors();
        if (goal != Vector2.zero)
        {
            icon.GetComponent<RectTransform>().localPosition = Vector2.Lerp(icon.GetComponent<RectTransform>().localPosition, goal, 0.2f);
        }

        if (active && fresh <= 100)
        {
            fresh += 1.0f;
        }
    }
}
