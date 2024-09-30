using UnityEngine;
using UnityEngine.Events;
using DG.Tweening; // Import DoTween namespace
using UnityEngine.UI;
public class ButtonClickAnim : MonoBehaviour
{
    public UnityEvent actions;
    public float animationDuration = 0.1f;
    public float scaleFactor = 1.1f;
    private Button button;

    void Start()
    {
        // Automatically detect the button component
        button = GetComponent<Button>();

        if (button != null)
        {
            // Add the ButtonClick method as a listener to the button's onClick event
            button.onClick.AddListener(ButtonClick);
        }
        else
        {
            Debug.LogError("Button component not found on this GameObject.");
        }
    }
    public void ButtonClick()
    {
        AnimateButtonClick();
        Debug.Log("Button Clicked Once");
    }

    private void AnimateButtonClick()
    {
        button.interactable = false;
        // Store the original scale
        Vector3 originalScale = transform.localScale;

        // Scale up and then scale down
        transform.DOScale(originalScale * scaleFactor, animationDuration)
            .OnComplete(() => 
            {
                transform.DOScale(originalScale, animationDuration)
                    .OnComplete(() => 
                    {
                        button.interactable = true;
                        actions?.Invoke();
                    });
            });
    }
}