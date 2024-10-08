using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Scripts.UIPanels;
using UnityEngine.UI;
namespace Scripts.ListEntry
{
public class AdjudicatorListEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text adjudicatorName;
    [SerializeField] private TMP_Text adjudicatorInstitute;
    [SerializeField] private ColorBlock myColorBlock;
    [SerializeField] private Button myBtn;

    Adjudicator myAdjudicator;
    Instituitions myInstitution;

    private bool isSelected = false;

    void Start()
    {
        myColorBlock = myBtn.colors;
        CRUDAdjudicatorPanel.Instance.DisableAllAdjudicators.AddListener(Deselect);
    }

    public void SetAdjudicator(Adjudicator adjudicator)
    {
        Debug.Log("Setting Adjudicator");
        myAdjudicator = adjudicator;
        myInstitution = AppConstants.instance.GetInstituitionsFromID(myAdjudicator.instituitionID);
        Debug.Log("Setting Adjudicator");
        adjudicatorName.text = adjudicator.adjudicatorName;
        adjudicatorInstitute.text = myInstitution.instituitionAbreviation;
    }


    public void Select()
    {
        
        if (isSelected)
        {
            Deselect();
            CRUDAdjudicatorPanel.Instance.DeactivateAdjudicatorPanel();
            CRUDAdjudicatorPanel.Instance.DisableAllAdjudicators?.Invoke();
        }
        else
        {
            CRUDAdjudicatorPanel.Instance.DisableAllAdjudicators?.Invoke();
            DOVirtual.DelayedCall(0.1f, () =>
            {
                isSelected = true;
                myColorBlock.normalColor = new Color32(0xE6, 0xFF, 0xD1, 0xFF);
                myBtn.colors = myColorBlock;
                transform.DOScale(Vector3.one * 0.9f, 0.1f);
                CRUDAdjudicatorPanel.Instance.ShowAdjudicatorInfo(myAdjudicator, myInstitution);
            });
        }
    }
    private void Deselect()
    {
        isSelected = false;
        myColorBlock.normalColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
        myBtn.colors = myColorBlock;
            transform.DOScale(Vector3.one, 0.1f);
    }
    private void OnDestroy() {
        Debug.Log("Adjudicator List Entry Destroyed");
    }
}
}