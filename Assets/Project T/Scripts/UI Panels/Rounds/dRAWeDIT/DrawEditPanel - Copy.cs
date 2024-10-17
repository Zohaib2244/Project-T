using System.Linq;
using System.Collections.Generic;
using Scripts.Resources;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System;

[Serializable]
public class Team_TMP
{
    public string TeamID;
    public string TeamName;
    public TeamPositionsBritish TeamPosition;

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
    public List<Team_TMP> teamsInMatch = new List<Team_TMP>(4);
    public List<Adjudicator> adjudicatorsInMatch = new List<Adjudicator>();
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

public class TeamBtn
{
    public Button btn;
    public Team_TMP team;

    public TeamBtn(Button btn, Team_TMP team)
    {
        this.btn = btn;
        this.team = team;
    }
}
public class AdjBtn
{
    public Button btn;
    public Adjudicator adj;

    public AdjBtn(Button btn, Adjudicator adj)
    {
        this.btn = btn;
        this.adj = adj;
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
    
    private Team_TMP replacementTeam1;
    private Team_TMP replacementTeam2;
     private List<Team_TMP> replacementTeams = new List<Team_TMP>();
    private Adjudicator replacementJudge1;
    private Adjudicator replacementJudge2;

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
       
    
    public void AddReplaceableTeam(TeamBtn teamBtn)
    {
        Team_TMP team = teamBtn.team;
        if (replacementTeams.Count == 2)
        {
            // Remove the oldest team (first in the list)
            replacementTeams.RemoveAt(0);
        }
    
        // Add the new team to the list
        replacementTeams.Add(team);
    
        // Assign the teams to replacementTeam1 and replacementTeam2
        replacementTeam1 = replacementTeams.Count > 0 ? replacementTeams[0] : null;
        replacementTeam2 = replacementTeams.Count > 1 ? replacementTeams[1] : null;
    
        Debug.Log($"Replacement teams updated: {replacementTeam1?.TeamID}, {replacementTeam2?.TeamID}");
    }
    public void RemoveReplaceableTeam(TeamBtn teamBtn)
    {
        Team_TMP team = teamBtn.team;
        if (replacementTeams.Contains(team))
        {
            replacementTeams.Remove(team);
        }
    
        // Assign the teams to replacementTeam1 and replacementTeam2
        replacementTeam1 = replacementTeams.Count > 0 ? replacementTeams[0] : null;
        replacementTeam2 = replacementTeams.Count > 1 ? replacementTeams[1] : null;
    
        Debug.Log($"Replacement teams updated: {replacementTeam1?.TeamID}, {replacementTeam2?.TeamID}");
    }
    private void ReplaceJudges(Adjudicator judge1, Adjudicator judge2)
    {
        replacementJudge1 = judge1;
        replacementJudge2 = judge2;
    }
    private void ReplaceTeams()
    {
        if (replacementTeam1 != null && replacementTeam2 != null)
        {
            Team_TMP temp = replacementTeam1;
            replacementTeam1 = replacementTeam2;
            replacementTeam2 = temp;
        }
    }
    private void ReplaceJudges()
    {
        if (replacementJudge1 != null && replacementJudge2 != null)
        {
            Adjudicator temp = replacementJudge1;
            replacementJudge1 = replacementJudge2;
            replacementJudge2 = temp;
        }
    }

    
}
