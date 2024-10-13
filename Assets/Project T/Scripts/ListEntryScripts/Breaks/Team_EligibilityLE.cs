using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Scripts.Resources;

public class Team_EligibilityLE : MonoBehaviour
{
    [Header("TextFields")]
    [SerializeField] private TMP_Text teamName;
    [SerializeField] private TMP_Text teamInstitution;

    [Header("Attendance Sprites")]
    [SerializeField] private Sprite eligibilityMarkedSprite;
    [SerializeField] private Sprite eligibilityNotMarkedSprite;
    [SerializeField] private Image eligibilityImage;

    private Team myTeam;
    private bool eligibilityMarked = false;

    public void Initialize(Team team)
    {
        myTeam = team;
        teamName.text = myTeam.teamName;
        teamInstitution.text = myTeam.instituition;
    }

    public void MarkEligibility()
    {
        Debug.Log("Marking eligibility");
        eligibilityMarked = true;
        eligibilityImage.sprite = eligibilityMarkedSprite;
        if(myTeam.teamCategory == SpeakerTypes.Open)
            Breaks_EditPanel.Instance.AddOpenTeamAsEligible(myTeam);
        else if(myTeam.teamCategory == SpeakerTypes.Novice)
            Breaks_EditPanel.Instance.AddNoviceTeamAsEligible(myTeam);
    }
    public void UnMarkEligibility()
    {
        Debug.Log("Unmarking eligibility");
        eligibilityMarked = false;
        eligibilityImage.sprite = eligibilityNotMarkedSprite;
        if(myTeam.teamCategory == SpeakerTypes.Open)
            Breaks_EditPanel.Instance.RemoveOpenTeamAsEligible(myTeam);
        else if(myTeam.teamCategory == SpeakerTypes.Novice)
            Breaks_EditPanel.Instance.RemoveNoviceTeamAsEligible(myTeam);
    }
}
