using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class Toggle : MonoBehaviour
{
    [SerializeField] private Transform Btn_1;
    [SerializeField] private Transform Btn_2;
    public Action onSelectOption1;
    public Action onSelectOption2;

    void Start()
    {
        Btn_1.gameObject.SetActive(true);
        Btn_2.gameObject.SetActive(true);

        Btn_1.localScale = Vector3.one;
        Btn_2.localScale = Vector3.one;
    }
    public void SelectOption(int index)
    {
        switch (index)
        {
            case 1:
                SelectOption1();
                break;
            case 2:
                SelectOption2();
                break;
            default:
                break;
        }
    }

    public void SelectOption1()
    {
        Btn_1.gameObject.GetComponent<Button>().interactable = false;
        //set your interactavility to true if it is false
        if (Btn_2.gameObject.GetComponent<Button>().interactable == false)
        {
            Btn_2.gameObject.GetComponent<Button>().interactable = true;
        }
        Btn_1.DOScale(Vector3.one * 1.1f, 0.1f); // Scale up slightly
        Btn_2.DOScale(Vector3.one * 0.8f, 0.1f).OnComplete(() => Btn_2.gameObject.GetComponent<Button>().interactable = true);
        onSelectOption1?.Invoke();
    }

    public void SelectOption2()
    {
        Btn_2.gameObject.GetComponent<Button>().interactable = false;
        if (Btn_1.gameObject.GetComponent<Button>().interactable == false)
        {
            Btn_1.gameObject.GetComponent<Button>().interactable = true;
        }
        Btn_2.DOScale(Vector3.one * 1.1f, 0.1f); // Scale up slightly
        Btn_1.DOScale(Vector3.one * 0.8f, 0.1f).OnComplete(() => Btn_1.gameObject.GetComponent<Button>().interactable = true);
        onSelectOption2?.Invoke();
    }
    public void DeActivate()
    {
        Btn_1.gameObject.GetComponent<Button>().interactable = false;
        Btn_2.gameObject.GetComponent<Button>().interactable = false;
        //revert their scale to
        Btn_1.DOScale(Vector3.one, 0.1f);
        Btn_2.DOScale(Vector3.one, 0.1f);
    }
    public void Activate()
    {
        Btn_1.gameObject.GetComponent<Button>().interactable = true;
        Btn_2.gameObject.GetComponent<Button>().interactable = true;
    }
    public void DisableOption(int index)
    {
        switch (index)
        {
            case 1:
                Btn_1.gameObject.GetComponent<Button>().interactable = false;
                break;
            case 2:
                Btn_2.gameObject.GetComponent<Button>().interactable = false;
                break;
            default:
                break;
        }
    }
}
