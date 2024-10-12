using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;
using System.Collections.Generic;

public enum AnimationType
{
    PopIn,
    PopOut,
}
public class ButtonSelect : MonoBehaviour
{
    [SerializeField] private List<Button> buttons; // Array to hold all child buttons
    [SerializeField] private AnimationType animationType;
    private HashSet<Button> selectedButtons; // Set to hold the currently selected buttons
    [Header("Events")]
    public List<UnityEvent> buttonSelectEvents; // List to hold select events for each button
    public List<UnityEvent> buttonDeselectEvents; // List to hold deselect events for each button
    public UnityEvent onButtonDeselectEvent; // Event to invoke when a button is deselected

    [Header("Settings")]
    public bool autoAssignButtons = true;
    public bool sameDeselectForAll = false;
    public bool allowDeselectOnClick = true; // New boolean to control deselect on click
    public bool allowMultipleSelections = true; // New boolean to control multiple selections
    [Header("Animation Values")]
    public float popInScale = 0.9f;
    public float popOutScale = 1.3f;
    public float animationDuration = 0.2f;
    private Dictionary<Button, (UnityEvent selectEvent, UnityEvent deselectEvent)> buttonEventDict; // Dictionary to map buttons to their events

    void Start()
    {
        // Initialize the dictionary and the selected buttons set
        buttonEventDict = new Dictionary<Button, (UnityEvent, UnityEvent)>();
        selectedButtons = new HashSet<Button>();

        // Initialize the buttons list if not already done
        if (buttons == null)
        {
            buttons = new List<Button>();
        }

        if (autoAssignButtons)
        // Add all child buttons to the list
        {
            Button[] childButtons = GetComponentsInChildren<Button>();
            foreach (Button button in childButtons)
            {
                buttons.Add(button);
            }
        }
   // Add click listeners to all buttons and map events
    for (int i = 0; i < buttons.Count; i++)
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
        if (selectedButtons.Contains(button))
        {
            if (allowDeselectOnClick)
            {
                DeselectButton(button);
            }
            return;
        }

        if (!allowMultipleSelections)
        {
            DeselectAll();
        }

        // Select the new button
        SelectButton(button);

        // Invoke the select event associated with the button
        if (buttonEventDict.ContainsKey(button))
        {
            buttonEventDict[button].selectEvent?.Invoke();
        }
    }
    void PopInAnimation_Select(Button button)
    {
        button.transform.DOScale(Vector3.one * popInScale, animationDuration);
    }
    void PopInAnimation_Deselect(Button button)
    {
        button.transform.DOScale(Vector3.one, animationDuration);
    }
    void PopOutAnimation_Select(Button button)
    {
        button.transform.DOScale(Vector3.one * popOutScale, animationDuration);
    }
    void PopOutAnimation_Deselect(Button button)
    {
        button.transform.DOScale(Vector3.one, animationDuration);
    }

    void SelectButton(Button button)
    {
        selectedButtons.Add(button);
        if (animationType == AnimationType.PopIn)
        {
            PopInAnimation_Select(button);
        }
        else if (animationType == AnimationType.PopOut)
        {
            PopOutAnimation_Select(button);
        }
        if (!allowDeselectOnClick)
            button.interactable = false;
    }

    void DeselectButton(Button button)
    {
        selectedButtons.Remove(button);
        button.interactable = true;
        if (animationType == AnimationType.PopIn)
        {
            PopInAnimation_Deselect(button);
        }
        else if (animationType == AnimationType.PopOut)
        {
            PopOutAnimation_Deselect(button);
        }
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

    #region Public Methods
    public void DeselectAll()
    {
        if (selectedButtons != null && selectedButtons.Count > 0)
        {        // Create a copy of the selectedButtons collection
            var selectedButtonsCopy = new List<Button>(selectedButtons);

            // Iterate over the copy
            foreach (var button in selectedButtonsCopy)
            {
                // Perform the deselection logic
                DeselectButton(button);
            }
        }
    }
    public void SelectOption(int id)
    {
        if (id < 0 || id >= buttons.Count)
        {
            Debug.LogError("Invalid button ID");
            return;
        }

        Button button = buttons[id];
        OnButtonClick(button);
    }
    public void SelectOption(Button button)
    {
        OnButtonClick(button);
    }
    public void DisableInteractability(int id)
    {
        if (id < 0 || id >= buttons.Count)
        {
            Debug.LogError("Invalid button ID");
            return;
        }

        Button button = buttons[id];
        button.interactable = false;
    }

    public void EnableInteractability(int id)
    {
        if (id < 0 || id >= buttons.Count)
        {
            Debug.LogError("Invalid button ID");
            return;
        }

        Button button = buttons[id];
        button.interactable = true;
    }

    public void DisableAllInteractability()
    {
        foreach (var button in buttons)
        {
            button.interactable = false;
        }
    }

    public void EnableAllInteractability()
    {
        foreach (var button in buttons)
        {
            button.interactable = true;
        }
    }


    public void AddOption(Button btn, UnityAction selectAction, UnityAction deselectAction)
    {
        buttons.Add(btn);
        btn.onClick.AddListener(() => OnButtonClick(btn));
        UnityEvent deselectEvent = new UnityEvent();
        deselectEvent.AddListener(deselectAction);
        buttonDeselectEvents.Add(deselectEvent);
        UnityEvent selectEvent = new UnityEvent();
        selectEvent.AddListener(selectAction);
        buttonSelectEvents.Add(selectEvent);
        if (buttonEventDict == null)
        {
            Debug.Log("Button Event Dict is null");
            buttonEventDict = new Dictionary<Button, (UnityEvent, UnityEvent)>();
        }
        buttonEventDict.Add(btn, (selectEvent, deselectEvent));
    }
    public void AddOption(Button btn, UnityAction action, int actionType)
    {
        buttons.Add(btn);
        btn.onClick.AddListener(() => OnButtonClick(btn));

        if (actionType == 0)
        {
            UnityEvent selectEvent = new UnityEvent();
            selectEvent.AddListener(action);
            buttonSelectEvents.Add(selectEvent);
            if (buttonEventDict == null)
            {
                Debug.Log("Button Event Dict is null");
                buttonEventDict = new Dictionary<Button, (UnityEvent, UnityEvent)>();
            }
            buttonEventDict[btn] = (selectEvent, buttonEventDict.ContainsKey(btn) ? buttonEventDict[btn].deselectEvent : null);
        }
        else if (actionType == 1)
        {
            UnityEvent deselectEvent = new UnityEvent();
            deselectEvent.AddListener(action);
            buttonDeselectEvents.Add(deselectEvent);
            if (buttonEventDict == null)
            {
                Debug.Log("Button Event Dict is null");
                buttonEventDict = new Dictionary<Button, (UnityEvent, UnityEvent)>();
            }
            buttonEventDict[btn] = (buttonEventDict.ContainsKey(btn) ? buttonEventDict[btn].selectEvent : null, deselectEvent);
        }
    }
    public void AddOption(Button btn)
    {
        btn.onClick.AddListener(() => OnButtonClick(btn));
        buttons.Add(btn);
        if (buttonEventDict == null)
        {
            Debug.Log("Button Event Dict is null");
            buttonEventDict = new Dictionary<Button, (UnityEvent, UnityEvent)>();
        }
    }
    public void RemoveAllButtons()
    {
        if (buttons != null)
        {
            buttons.Clear();
        }

        if (buttonSelectEvents != null)
        {
            buttonSelectEvents.Clear();
        }

        if (buttonDeselectEvents != null)
        {
            buttonDeselectEvents.Clear();
        }

        if (buttonEventDict != null)
        {
            buttonEventDict.Clear();
        }
    }
    #endregion
}