using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class DialogueBox : MonoBehaviour
{

    //make singelton
    private static DialogueBox _instance;
    public static DialogueBox Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("DialogueBox instance is not initialized.");
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // Set the hidden and visible positions

        // Initialize the dialogue box to be hidden
        dialogueBox.transform.position = hiddenPosition;
    }


     public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public float animationDuration = 0.5f;
    public Vector3 hiddenPosition;
    public Vector3 visiblePosition;


    public void ShowDialogueBox(string text, Color textColor)
    {
        dialogueText.text = text;
        dialogueText.color = textColor;
        dialogueBox.SetActive(true);
        dialogueBox.transform.DOMoveY(visiblePosition.y, animationDuration).SetEase(Ease.OutCubic).SetRelative(false).OnComplete(() =>
        {
            // Automatically hide the dialogue box after 2 seconds
            DOVirtual.DelayedCall(2f, () => HideDialogueBox());
        });}
        
        public void HideDialogueBox()
        {
            dialogueBox.transform.DOMoveY(hiddenPosition.y, animationDuration).SetEase(Ease.InCubic).SetRelative(false).OnComplete(() =>
            {
                dialogueBox.SetActive(false);
            });
        }
}
