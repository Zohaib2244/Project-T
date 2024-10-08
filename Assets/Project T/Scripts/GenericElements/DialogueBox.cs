using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class DialogueBox : MonoBehaviour
{
    public static DialogueBox Instance { get; private set; }

    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private float visiblePositionY;
    [SerializeField] private float hiddenPositionY;
    [SerializeField] private float animationDuration = 0.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowDialogueBox(string text, Color textColor)
    {
        dialogueText.text = text;
        dialogueText.color = textColor;

        if (dialogueBox.activeSelf)
        {
            // If the dialogue box is already active, update the text and color without restarting the animation
            DOVirtual.DelayedCall(2f, () => HideDialogueBox());
        }
        else
        {
            dialogueBox.SetActive(true);
            dialogueBox.transform.DOLocalMoveY(visiblePositionY, animationDuration).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                // Automatically hide the dialogue box after 2 seconds
                DOVirtual.DelayedCall(2f, () => HideDialogueBox());
            });
        }
    }

    public void HideDialogueBox()
    {
        dialogueBox.transform.DOLocalMoveY(hiddenPositionY, animationDuration).SetEase(Ease.InCubic).OnComplete(() =>
        {
            dialogueBox.SetActive(false);
        });
    }
}