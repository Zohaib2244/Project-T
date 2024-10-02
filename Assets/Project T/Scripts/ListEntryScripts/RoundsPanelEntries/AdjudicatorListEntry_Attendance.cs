using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Scripts.Resources;

public class AdjudicatorListEntry_Attendance : MonoBehaviour
{
    [Header("TextFields")]
    [SerializeField] TMP_Text adjudicatorName;
    [SerializeField] TMP_Text adjudicatorInstitution;
    [SerializeField] TMP_Text adjudicatorCategory;
     [Header("Attendance Sprites")]
    [SerializeField] private Sprite attendanceMarkedSprite;
    [SerializeField] private Sprite attendanceNotMarkedSprite;
    [SerializeField] private Image attendanceImage;

    private Adjudicator myAdjudicator;
    private Instituitions myInstitute;


    public void Initialize(Adjudicator adjudicator)
    {
        myAdjudicator = adjudicator;
        myInstitute = AppConstants.instance.GetInstituitionsFromID(adjudicator.instituitionID);
        adjudicatorName.text = adjudicator.adjudicatorName;
        adjudicatorInstitution.text = myInstitute.instituitionAbreviation;
        if(myAdjudicator.adjudicatorType == AdjudicatorTypes.Normie)
            adjudicatorCategory.text = "Adjudicator";
        else if(myAdjudicator.adjudicatorType == AdjudicatorTypes.CAP)
            adjudicatorCategory.text = "CAP";
    }
    public void MarkAttendance()
    {
        attendanceImage.sprite = attendanceMarkedSprite;
        Rounds_AttendancePanel.Instance.AddAdjudicatorAsAvailable(myAdjudicator);
    }
    public void UnMarkAttendance()
    {
        attendanceImage.sprite = attendanceNotMarkedSprite;
        Rounds_AttendancePanel.Instance.RemoveAdjudicatorAsAvailable(myAdjudicator);
    }
}
