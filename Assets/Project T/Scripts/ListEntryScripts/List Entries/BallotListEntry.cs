using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Scripts.Resources;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine.AI;

public class BallotListEntry : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Image ballotBtn;
    [SerializeField] private Image ballotTextBg;
    [SerializeField] private Sprite enteredBallotBtnSprite;
    [SerializeField] private Sprite notEnteredBallotBtnSprite;
    [SerializeField] private Sprite enteredBallotTextBgSprite;
    [SerializeField] private Sprite notEnteredBallotTextBgSprite;

    [Header("Text Elements")]
    [SerializeField] private TMP_Text matchNo;
    [SerializeField] private TMP_Text teamName1stPos;
    [SerializeField] private TMP_Text teamName2ndPos;
    [SerializeField] private TMP_Text teamName3rdPos;
    [SerializeField] private TMP_Text teamName4thPos;
    [SerializeField] private TMP_Text adjudicatorNames;

    private Match match;

    public void SetMatch(Match match, int matchIndex)
    {
        this.match = match;

        matchNo.text = matchIndex.ToString();
        if (match.ballotEntered == false)
        {
            foreach (var teamEntry in match.teams)
            {
                var team = AppConstants.instance.selectedTouranment.teamsInTourney.Find(t => t.teamId == teamEntry.Key);
                if (team != null)
                {
                    var teamRoundData = team.teamRoundDatas.Find(trd => trd.teamRoundDataID == teamEntry.Value);
                    if (teamRoundData != null)
                    {
                        switch (teamRoundData.teamPositionBritish)
                        {
                            case TeamPositionsBritish.OG:
                                teamName1stPos.text = team.teamName;
                                break;
                            case TeamPositionsBritish.OO:
                                teamName2ndPos.text = team.teamName;
                                break;
                            case TeamPositionsBritish.CG:
                                teamName3rdPos.text = team.teamName;
                                break;
                            case TeamPositionsBritish.CO:
                                teamName4thPos.text = team.teamName;
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
                else if (adjudicator.adjudicatorType == AdjudicatorTypes.Normie)
                {
                    normieAdjudicatorNamesList.Add(adjudicator.adjudicatorName);
                }
            }

            adjudicatorNames.text = "CAP: " + string.Join(", ", capAdjudicatorNamesList.ToArray()) + "\n" + "Adjudicators: " + string.Join(", ", normieAdjudicatorNamesList.ToArray());
        }
        else if (match.ballotEntered == true)
        {
            BallotEntered();
        }
    }
    public void BallotEntered()
    {
        UpdateTeamPositions();
        ballotBtn.sprite = enteredBallotBtnSprite;
        ballotTextBg.sprite = enteredBallotTextBgSprite;
        match.ballotEntered = true;
    }
    public void UpdateTeamPositions()
    {
        // Iterate through each team in the match
        foreach (var teamEntry in match.teams)
        {
            // Find the corresponding team in the tournament
            var team = AppConstants.instance.GetTeamFromID(teamEntry.Key);
            if (team != null)
            {
                // Retrieve the team's round data
                var teamRoundData = team.teamRoundDatas.Find(trd => trd.teamRoundDataID == teamEntry.Value);
                if (teamRoundData != null)
                {
                    // Check the team's ranking and update the appropriate text field
                    switch (teamRoundData.teamMatchRanking)
                    {
                        case 1:
                            teamName1stPos.text = team.teamName;
                            break;
                        case 2:
                            teamName2ndPos.text = team.teamName;
                            break;
                        case 3:
                            teamName3rdPos.text = team.teamName;
                            break;
                        case 4:
                            teamName4thPos.text = team.teamName;
                            break;
                    }
                }
            }
        }
    }
    public Match GetSavedBallot()
    {
            return match;
    }
    public void AddBallot()
    {
        Rounds_BallotsPanel.Instance.ShowBallotInfo(match);
    }
}
