using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Scripts.UIPanels;

namespace Scripts.ListEntry
{
public class InstituteListEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text instituteNameText;
    [SerializeField] private TMP_Text instituteAbreviationText;
    [SerializeField] private Button myBtn;
    private Instituitions myInstitute;
    private ColorBlock myColorBlock;
    private bool isSelected;

    void Start()
    {
        CRUInstitutePanel.Instance.DisableAllSpeakers.AddListener(DeSelect);
        //get the buttons color block
        myColorBlock = myBtn.colors;
    }

    public void SetInstituteData(Instituitions institute)
    {
        myInstitute = institute;
        instituteNameText.text = myInstitute.instituitionName;
        instituteAbreviationText.text = myInstitute.instituitionAbreviation;
    }
    public void Select()
    {
        if (isSelected)
        {
            // Deselect the button
            DeSelect();
            CRUInstitutePanel.Instance.DeactiveInstitutePanel();
        }
        else
        {
            // Select the button
            CRUInstitutePanel.Instance.DisableAllSpeakers?.Invoke();
            DOVirtual.DelayedCall(0.1f, () =>
            {
                isSelected = true;
                myColorBlock.normalColor = new Color32(0xE6, 0xFF, 0xD1, 0xFF);
                myBtn.colors = myColorBlock;
                transform.DOScale(Vector3.one * 0.9f, 0.1f);
                CRUInstitutePanel.Instance.ShowInstituteData(myInstitute);
            });
        }
    }

    private void DeSelect()
    {
        if (isSelected)
        {
            // myBtn.interactable = true;
            myColorBlock.normalColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF); // White color in hex
            myBtn.colors = myColorBlock;
            isSelected = false;
            transform.DOScale(Vector3.one, 0.1f);
        }
    }
}
}