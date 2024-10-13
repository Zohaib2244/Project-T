using TMPro;
using UnityEngine;

public class Team_LE_BreakDisplay : MonoBehaviour
{
    [Header("TextFields")]
    [SerializeField] private TMP_Text rankingtxt;
    [SerializeField] private TMP_Text teamNametxt;
    [SerializeField] private TMP_Text teamInstitutionAbrvtxt;
    [SerializeField] private TMP_Text teamScoretxt;
    [SerializeField] private TMP_Text teamPointstxt;

    private Team myTeam;
    public void SetTeam(Team team, int rankingIndex)
    {
        myTeam = team;

        rankingtxt.text = rankingIndex.ToString();
        teamNametxt.text = myTeam.teamName;
        teamInstitutionAbrvtxt.text = AppConstants.instance.GetInstituteAbreviation(myTeam.instituition).ToString();
        teamScoretxt.text = myTeam.totalTeamScore.ToString();
        teamPointstxt.text = myTeam.teamPoints.ToString();
    }


}
