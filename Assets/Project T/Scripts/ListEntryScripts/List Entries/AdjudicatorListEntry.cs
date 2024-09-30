using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Scripts.UIPanels;
namespace Scripts.ListEntry
{
public class AdjudicatorListEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text adjudicatorName;
    [SerializeField] private TMP_Text adjudicatorInstitute;

    Adjudicator myAdjudicator;
    Instituitions myInstitution;

    private bool isSelected = false;

    void Start()
    {
        CRUDAdjudicatorPanel.Instance.DisableAllAdjudicators.AddListener(Deselect);
    }

    public void SetAdjudicator(Adjudicator adjudicator)
    {
        Debug.Log("Setting Adjudicator");
        myAdjudicator = adjudicator;
        myInstitution = AppConstants.instance.GetInstituitionsFromID(myAdjudicator.instituitionID);
        Debug.Log("Setting Adjudicator");
        adjudicatorName.text = adjudicator.adjudicatorName;
        adjudicatorInstitute.text = myInstitution.instituitionName;
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
            isSelected = true;
            transform.DOScale(Vector3.one * 0.9f, 0.1f);
            CRUDAdjudicatorPanel.Instance.ShowAdjudicatorInfo(myAdjudicator, myInstitution);
        }
    }
    private void Deselect()
    {
        isSelected = false;
            transform.DOScale(Vector3.one, 0.1f);
    }
    private void OnDestroy() {
        Debug.Log("Adjudicator List Entry Destroyed");
    }
}
}