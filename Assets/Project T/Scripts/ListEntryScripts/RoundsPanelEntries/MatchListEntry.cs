using System.Collections;
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
        this.match = match;
        matchNo.text = _matchNo.ToString();

        foreach (var teamEntry in match.teams)
        {
            var team = AppConstants.instance.selectedTouranment.teamsInTourney.FirstOrDefault(t => t.teamId == teamEntry.Key);
            if (team != null)
            {
                var teamRoundData = team.teamRoundDatas.FirstOrDefault(trd => trd.teamRoundDataID == teamEntry.Value);
                if (teamRoundData != null)
                {
                    switch (teamRoundData.teamPositionBritish)
                    {
                        case TeamPositionsBritish.OG:
                            team1NameOG.text = team.teamName;
                            break;
                        case TeamPositionsBritish.OO:
                            team2NameOO.text = team.teamName;
                            break;
                        case TeamPositionsBritish.CG:
                            team1NameCG.text = team.teamName;
                            break;
                        case TeamPositionsBritish.CO:
                            team2NameCO.text = team.teamName;
                            break;
                    }
                }
            }
        }

        List<string> capAdjudicatorNamesList = new List<string>();
        List<string> normieAdjudicatorNamesList = new List<string>();
        
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