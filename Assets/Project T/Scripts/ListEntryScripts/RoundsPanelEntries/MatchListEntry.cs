using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Scripts.Resources;

public class MatchListEntry : MonoBehaviour
{
    [SerializeField] private TMP_Text matchNo;
    [SerializeField] private TMP_Text team1NameOG;
    [SerializeField] private TMP_Text team2NameOO;
    [SerializeField] private TMP_Text team1NameCG;
    [SerializeField] private TMP_Text team2NameCO;
    [SerializeField] private TMP_Text adjudicatorNames;

    private Match match;

    public void SetMatch(Match match, int _matchNo)
    {
        Debug.Log("SetMatch called");
        this.match = match;
        matchNo.text = _matchNo.ToString();

        foreach (var teamEntry in match.teams)
        {

            Debug.Log("<color=lightblue>Team Entry Key: " + teamEntry.Key + "</color>");
            Debug.Log("<color=lightblue>Team Entry Value: " + teamEntry.Value + "</color>");
            // Debug.Log("<color=lightblue>Team Round Data: " + AppConstants.instance.GetTeamFromID(teamEntry.Key).teamRoundDatas.Count + "</color>");
            foreach(var trd in AppConstants.instance.GetTeamFromID(teamEntry.Key).teamRoundDatas)
            {
                Debug.Log("<color=lightblue>Team Round Data ID: " + trd.teamRoundDataID + "</color>");
                Debug.Log("<color=lightblue>Team Position: " + trd.teamPositionBritish + "</color>");
            }
            
            var team = AppConstants.instance.selectedTouranment.teamsInTourney.FirstOrDefault(t => t.teamId == teamEntry.Key);
            if (team != null)
            {
                Debug.Log("Team Name: " + team.teamName);
                var teamRoundData = team.teamRoundDatas.FirstOrDefault(trd => trd.teamRoundDataID == teamEntry.Value);
                if (teamRoundData != null)
                {
                    Debug.Log("Team Position: " + teamRoundData.teamPositionBritish);
                    switch (teamRoundData.teamPositionBritish)
                    {
                        case TeamPositionsBritish.OG:
                        Debug.Log("Team Position: OG Team Name: " + team.teamName);
                            team1NameOG.text = team.teamName;
                            break;
                        case TeamPositionsBritish.OO:
                        Debug.Log("Team Position: OO Team Name: " + team.teamName);
                            team2NameOO.text = team.teamName;
                            break;
                        case TeamPositionsBritish.CG:
                        Debug.Log("Team Position: CG Team Name: " + team.teamName);
                            team1NameCG.text = team.teamName;
                            break;
                        case TeamPositionsBritish.CO:
                        Debug.Log("Team Position: CO Team Name: " + team.teamName);
                            team2NameCO.text = team.teamName;
                            break;
                    }
                }
                else
                Debug.Log("Team Round Data is null");
            }
        }

        List<string> capAdjudicatorNamesList = new List<string>();
        List<string> normieAdjudicatorNamesList = new List<string>();
        Debug.Log("Match Adjucators Count: " + match.adjudicators.Count());
        foreach (var adjudicator in match.adjudicators)
        {
            if (adjudicator.adjudicatorType == AdjudicatorTypes.CAP)
            {
                capAdjudicatorNamesList.Add(adjudicator.adjudicatorName);
            }
            else if(adjudicator.adjudicatorType == AdjudicatorTypes.Normie)
            {
                normieAdjudicatorNamesList.Add(adjudicator.adjudicatorName);
            }
        }
        
        // Combine CAP adjudicators and normie adjudicators, each on a new line
        List<string> allAdjudicatorNamesList = new List<string>();
        allAdjudicatorNamesList.AddRange(capAdjudicatorNamesList);
        allAdjudicatorNamesList.AddRange(normieAdjudicatorNamesList);
        
        adjudicatorNames.text = string.Join("\n", allAdjudicatorNamesList);
        Debug.Log("Match No: " + _matchNo);
        foreach (var adjudicator in match.adjudicators)
        {
            Debug.Log("Adjudicator Name: " + adjudicator.adjudicatorName);
        }
    }
    public Match GetMatch()
    {
        return match;
    }
}