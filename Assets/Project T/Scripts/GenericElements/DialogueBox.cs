using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

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
    
        // Assuming dialogueBox has an Image component
        Image dialogueBoxImage = dialogueBox.GetComponent<Image>();
        if (dialogueBoxImage != null)
        {
            // Set the color based on the input color
            if (textColor == Color.red)
            {
                dialogueBoxImage.color = new Color32(224, 134, 134, 255); // E08686
            }
            else if (textColor == Color.green)
            {
                dialogueBoxImage.color = new Color32(102, 171, 131, 255); // 66AB83
            }
            else if (textColor == Color.yellow)
            {
                dialogueBoxImage.color = new Color32(215, 183, 100, 255); // D7B764
            }
            else
            {
                dialogueBoxImage.color = textColor; // Default to the provided color
            }
        }
        else
        {
            Debug.LogError("DialogueBox does not have an Image component.");
        }
    
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