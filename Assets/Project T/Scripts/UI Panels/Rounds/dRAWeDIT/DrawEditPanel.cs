using System.Linq;
using System.Collections.Generic;
using Scripts.Resources;
using Scripts.UIPanels;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class Team_TMP
{
    string TeamID;
    string TeamName;
    TeamPositionsBritish TeamPosition;

    public Team_TMP(string teamID, string teamRoundDataID)
    {
        Team team = AppConstants.instance.GetTeamFromID(teamID);
        TeamRoundData teamRoundData = AppConstants.instance.GetTeamRoundData(teamID,teamRoundDataID);
    
        TeamID = team.teamId;
        TeamName = team.teamName;
        TeamPosition = teamRoundData.teamPositionBritish;
    }
}
public class BritishMatch_TMP
{
    List<Team_TMP> teamsInMatch = new List<Team_TMP>();
        

    public BritishMatch_TMP(Match match)
    {
        foreach (var team in match.teams)
        {
            Team_TMP team_TMP = new Team_TMP(team.Key, team.Value);
            teamsInMatch.Add(team_TMP);
        }
    }
}
public class Draw
{
    public List<BritishMatch_TMP> matches = new List<BritishMatch_TMP>();

    public Draw(List<Match> matches)
    {
        foreach (var match in matches)
        {
            BritishMatch_TMP britishMatch_TMP = new BritishMatch_TMP(match);
            this.matches.Add(britishMatch_TMP);
        }
    }
}
public class DrawEditPanel : MonoBehaviour
{
    #region Singleton
    private static DrawEditPanel _instance;

    public static DrawEditPanel Instance

    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("DrawEditPanel instance is not initialized.");
            }
            return _instance;
        }
    }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }
    #endregion

    [SerializeField] private Transform matchesListContent;
    [SerializeField] private GameObject matchEntryPrefab;

[SerializeField] private List<string> allAvailableTeams = new List<string>();
    [SerializeField] private List<string> allAvailableJudges = new List<string>();
    [SerializeField] private Draw draw;
    private void OnEnable() {
        draw = new Draw(DrawsPanel.Instance.matches_TMP);
        foreach (var match in DrawsPanel.Instance.matches_TMP)
        {
            foreach (var team in match.teams)
            {
                if (!allAvailableTeams.Contains(team.Key))
                {
                    allAvailableTeams.Add(team.Key);
                }
            }
            foreach (var judge in match.adjudicators)
            {
                if (!allAvailableJudges.Contains(judge.adjudicatorID))
                {
                    allAvailableJudges.Add(judge.adjudicatorID);
                }
            }
        }
    }

    
}
