using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Scripts.Resources;


namespace Scripts.ListEntry
{public class TournamentInfoCard : MonoBehaviour
{
    [SerializeField] private TMP_Text tournamentName;
    [SerializeField] private TMP_Text noOfPrelimss; 
    [SerializeField] private TMP_Text noOfTeams;
    [SerializeField] private TMP_Text speakercategories;

    [SerializeField] private bool isBritish;
    public void SetTournamentInfo()
    {
         TournamentInfo tournamentInfo = null;
        if(isBritish)
        {
           tournamentInfo  = AppConstants.instance.GetTournament("b01");
        }
        else
        {
            tournamentInfo = AppConstants.instance.GetTournament("a01");
        }
        if (tournamentInfo != null)
        {
            tournamentName.text = tournamentInfo.tournamentName;
            noOfPrelimss.text = tournamentInfo.noOfPrelims.ToString();
            noOfTeams.text = "x";
            if(tournamentInfo.speakerCategories.Count == 1)
            {
                speakercategories.text = "Open";

            }
            else if(tournamentInfo.speakerCategories.Count == 2)
            {
                speakercategories.text = "Open | Novice";
            }
        }
        else
        {
            tournamentName.text = "Name...";
            noOfPrelimss.text = "0";
            noOfTeams.text = "0";
            speakercategories.text = "";
        }
    }

}
}