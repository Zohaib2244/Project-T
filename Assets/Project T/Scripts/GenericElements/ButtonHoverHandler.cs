using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonHoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CanvasGroup image1CanvasGroup;
    [SerializeField] private CanvasGroup image2CanvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    private void Start()
    {
        // Ensure only the first image is visible initially
        image1CanvasGroup.alpha = 1f;
        image2CanvasGroup.alpha = 0f;
        image1CanvasGroup.gameObject.SetActive(true);
        image2CanvasGroup.gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Fade out the first image and fade in the second image
        image1CanvasGroup.DOFade(0f, fadeDuration);
        image2CanvasGroup.DOFade(1f, fadeDuration);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Fade in the first image and fade out the second image
        image1CanvasGroup.DOFade(1f, fadeDuration);
        image2CanvasGroup.DOFade(0f, fadeDuration);
    }
}