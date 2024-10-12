using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Scripts.Resources;

public class TeamListEntry_Attendance : MonoBehaviour
{
    [Header("TextFields")]
    [SerializeField, Tooltip("We Enter Team Name Here")] private TMP_Text teamName;
    [SerializeField] private TMP_Text teamInstitution;
    [SerializeField] private TMP_Text speaker1;
    [SerializeField] private TMP_Text speaker2;
    [SerializeField] private TMP_Text speaker3;

    [Header("Attendance Sprites")]
    [SerializeField] private Sprite attendanceMarkedSprite;
    [SerializeField] private Sprite attendanceNotMarkedSprite;
    [SerializeField] private Image attendanceImage;

    private Team myTeam;
    public Instituitions myInstitute;
    private bool attendanceMarked = false;



    public void Initialize(Team team)
    {
        myTeam = team;
        myInstitute = AppConstants.instance.GetInstituitionsFromID(myTeam.instituition);
        teamName.text = myTeam.teamName;
        teamInstitution.text = myInstitute.instituitionAbreviation;

        if (AppConstants.instance.selectedTouranment.tournamentType == TournamentType.British)
        {
            Destroy(speaker3.gameObject);
            speaker1.text = team.speakers[0].speakerName;
            speaker2.text = team.speakers[1].speakerName;
        }
        else if(AppConstants.instance.selectedTouranment.tournamentType == TournamentType.Asian)
        {
            speaker1.text = team.speakers[0].speakerName;
            speaker2.text = team.speakers[1].speakerName;
            speaker3.text = team.speakers[2].speakerName;
        }
    }
    public void MarkAttendance()
    {
        Debug.Log("Marking attendance");
        attendanceMarked = true;
        attendanceImage.sprite = attendanceMarkedSprite;
        Rounds_AttendancePanel.Instance.AddTeamAsAvailable(myTeam);
    }
    public void UnMarkAttendance()
    {
        Debug.Log("Unmarking attendance");
        attendanceMarked = false;
        attendanceImage.sprite = attendanceNotMarkedSprite;
        Rounds_AttendancePanel.Instance.RemoveTeamAsAvailable(myTeam);
    }
}
