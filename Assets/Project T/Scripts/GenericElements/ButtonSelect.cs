using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections.Generic;

public class ButtonSelect : MonoBehaviour
{
    [SerializeField] private Button[] buttons; // Array to hold all child buttons
    private Button selectedButton; // Reference to the currently selected button
    public List<UnityEvent> buttonSelectEvents; // List to hold select events for each button
    public List<UnityEvent> buttonDeselectEvents; // List to hold deselect events for each button
    public bool sameDeselectForAll = false;
    public bool allowDeselectOnClick = true; // New boolean to control deselect on click
    public UnityEvent onButtonDeselectEvent; // Event to invoke when a button is deselected
    private Dictionary<Button, (UnityEvent selectEvent, UnityEvent deselectEvent)> buttonEventDict; // Dictionary to map buttons to their events

    void Start()
    {
        // Get all Button components in the children of this GameObject
        buttons = GetComponentsInChildren<Button>();
        // Initialize the dictionary
        buttonEventDict = new Dictionary<Button, (UnityEvent, UnityEvent)>();

        // Add click listeners to all buttons and map events
        for (int i = 0; i < buttons.Length; i++)
        {
            Button button = buttons[i];
            button.onClick.AddListener(() => OnButtonClick(button));

            UnityEvent selectEvent = i < buttonSelectEvents.Count ? buttonSelectEvents[i] : null;
            UnityEvent deselectEvent = i < buttonDeselectEvents.Count ? buttonDeselectEvents[i] : null;
            buttonEventDict[button] = (selectEvent, deselectEvent);
        }
        DeselectAll();
    }

    void OnButtonClick(Button button)
    {
        if (selectedButton == button)
        {
            if (allowDeselectOnClick)
            {
                selectedButton = null;
                DeselectButton(button);
            }
            return;
        }
        // Deselect all buttons
        foreach (var btn in buttonEventDict.Keys)
        {
            DeselectButton(btn);
        }

        // Select the new button
        SelectButton(button);

        // Invoke the select event associated with the button
        if (buttonEventDict.ContainsKey(button))
        {
            buttonEventDict[button].selectEvent?.Invoke();
        }
    }

    void SelectButton(Button button)
    {
        selectedButton = button;
        button.transform.DOScale(Vector3.one * 0.9f, 0.1f);
        if(!allowDeselectOnClick)
            button.interactable = false;
    }

    void DeselectButton(Button button)
    {
        button.interactable = true;
        button.transform.DOScale(Vector3.one, 0.1f);

        if (sameDeselectForAll)
        {
            // Invoke the global deselect event
            onButtonDeselectEvent?.Invoke();
        }
        else
        {
            // Invoke the specific button deselect event
            if (buttonEventDict.ContainsKey(button))
            {
                buttonEventDict[button].deselectEvent?.Invoke();
            }
        }
    }
    public void DeselectAll()
    {
        foreach (var button in buttons)
        {
            DeselectButton(button);
        }
        selectedButton = null;
    }

    public void SelectOption(int id)
    {
        if (id < 0 || id >= buttons.Length)
        {
            Debug.LogError("Invalid button ID");
            return;
        }

        Button button = buttons[id];
        OnButtonClick(button);
    }
    public void DisableInteractability(int id)
    {
        if (id < 0 || id >= buttons.Length)
        {
            Debug.LogError("Invalid button ID");
            return;
        }

        Button button = buttons[id];
        button.interactable = false;
    }
    public void EnableInteractability(int id)
    {
        if (id < 0 || id >= buttons.Length)
        {
            Debug.LogError("Invalid button ID");
            return;
        }

        Button button = buttons[id];
        button.interactable = true;
    }
}