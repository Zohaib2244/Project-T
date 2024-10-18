using System.Linq;
using System.Collections.Generic;
using Scripts.Resources;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System;


#region TMP Classes
[Serializable]
public class Team_TMP
{
    public string TeamID;
    public string TeamName;
    public TeamPositionsBritish TeamPosition;
    public TeamRoundData teamRoundData;
    public Team_TMP(string teamID, string teamRoundDataID)
    {
        Team team = AppConstants.instance.GetTeamFromID(teamID);
        teamRoundData = AppConstants.instance.GetTeamRoundData(teamID, teamRoundDataID);

        TeamID = team.teamId;
        TeamName = team.teamName;
        TeamPosition = teamRoundData.teamPositionBritish;
    }
    public Team_TMP(string teamID, string TeamName, TeamPositionsBritish TeamPosition)
    {
        TeamID = teamID;
        this.TeamName = TeamName;
        this.TeamPosition = TeamPosition;
    }
}

[Serializable]
public class BritishMatch_TMP
{
    public string matchID;
    public List<Team_TMP> teamsInMatch = new List<Team_TMP>(4);
    public List<Adjudicator> adjudicatorsInMatch = new List<Adjudicator>();
    public BritishMatch_TMP(Match match)
    {
        matchID = match.matchId;
        foreach (var team in match.teams)
        {
            Team_TMP team_TMP = new Team_TMP(team.Key, team.Value);
            teamsInMatch.Add(team_TMP);
        }
        // Define the desired order of team positions
        var desiredOrder = new List<string> { "OG", "OO", "CG", "CO" };

        // Sort the teamsInMatch list based on the desired order
        teamsInMatch.Sort((team1, team2) =>
        {
            int index1 = desiredOrder.IndexOf(team1.TeamPosition.ToString());
            int index2 = desiredOrder.IndexOf(team2.TeamPosition.ToString());
            return index1.CompareTo(index2);
        });

        foreach (var adj in match.adjudicators)
        {
            adjudicatorsInMatch.Add(adj);
        }
    }
    public Match ToMatch()
    {
        Match match = new Match();
        match.matchId = matchID;

        foreach (var team_TMP in teamsInMatch)
        {
            Team team = AppConstants.instance.GetTeamFromID(team_TMP.TeamID);
            // TeamRoundData teamRoundData = team_TMP.teamRoundData;
            // if (teamRoundData == null)
            // {
            //     Debug.LogError($"TeamRoundData for team {team_TMP.TeamID} in match {matchID} is null.");
            // }
            Debug.Log($"<color=yellow>TeamRoundData for team {team_TMP.TeamID} in match {matchID} is {team_TMP.teamRoundData.teamPositionBritish}</color>");
            team_TMP.teamRoundData.teamPositionBritish = team_TMP.TeamPosition;
            team_TMP.teamRoundData.matchID = matchID;
                        // Debug.Log($"TeamRoundData for team {team_TMP.TeamID} in match {matchID} is {teamRoundData.teamPositionBritish}");

            match.teams.Add(team_TMP.TeamID, team_TMP.teamRoundData.teamRoundDataID);
        }

       for(int i=0; i<adjudicatorsInMatch.Count; i++)
        {
            match.adjudicators[i] = adjudicatorsInMatch[i];
        }

        Debug.Log($"New adjudicators count: {match.adjudicators.Length}");
        return match;
    }
}
[Serializable]
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
[Serializable]
public class TeamBtn
{
    public Button btn;
    public Team_TMP team;
    public bool isSelected;
    public bool isReplaceable;
    public TeamBtn(Button btn, Team_TMP team)
    {
        this.btn = btn;
        this.team = team;
        isSelected = false;
        isReplaceable = false;
    }

}
[Serializable]
public class AdjBtn
{
    public Button btn;
    public Adjudicator adj;
    public bool isSelected;
    public bool isReplaceable;
    public AdjBtn(Button btn, Adjudicator adj)
    {
        this.btn = btn;
        this.adj = adj;
        isSelected = false;
        isReplaceable = false;
    }
}
#endregion
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

    #region Variables
    [SerializeField] private Transform matchesListContent;
    [SerializeField] private GameObject matchEntryPrefab;

    [SerializeField] private List<string> allAvailableTeams = new List<string>();
    [SerializeField] private List<string> allAvailableJudges = new List<string>();
    [SerializeField] private Draw draw;

    public Team_TMP replacementTeam1;
    public Team_TMP replacementTeam2;
    private List<Team_TMP> replacementTeams = new List<Team_TMP>();
    private List<Adjudicator> replacementJudges = new List<Adjudicator>();
    public Adjudicator replacementJudge1;
    public Adjudicator replacementJudge2;
    public UnityEvent DeselectAllTeams = new UnityEvent();
    public UnityEvent DeselectAllJudges = new UnityEvent();
    public UnityEvent ReplaceTeamsEvent = new UnityEvent();
    public UnityEvent ReplaceJudgesEvent = new UnityEvent();

    public List<TeamBtn> teamBtns = new List<TeamBtn>();
    public List<AdjBtn> adjBtns = new List<AdjBtn>();

    #endregion
    #region Essentials
    private void OnEnable()
    {
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
        UpdateMatchesList();
    }

    private void UpdateMatchesList()
    {
        foreach (Transform child in matchesListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (var match in draw.matches)
        {
            GameObject matchEntry = Instantiate(matchEntryPrefab, matchesListContent);
            matchEntry.GetComponent<DrawEditListEntry>().SetMatch(match);
        }
    }
    public void Replace()
    {
        ReplaceTeams();
        ReplaceJudges();
    }
    public void SaveDraws()
    {
        List<Match> matches = new List<Match>();
        foreach (var matchTMP in draw.matches)
        {
            Match match = matchTMP.ToMatch();
            matches.Add(match);
        }
        DrawsPanel.Instance.matches_TMP.Clear();
        DrawsPanel.Instance.matches_TMP = matches;
        DrawsPanel.Instance.SwitchDrawPanel(DrawPanelTypes.DrawDisplayPanel);
    }
    #endregion
    #region Replace Team    
    public void AddReplaceableTeam(TeamBtn teamBtn)
    {
        Team_TMP team = teamBtn.team;

        // Check if the list already contains two teams
        if (replacementTeams.Count == 2)
        {
            // Remove the first team and set its isReplaceable to false
            Team_TMP removedTeam = replacementTeams[0];
            replacementTeams.RemoveAt(0);
            TeamBtn removedTeamBtn = teamBtns.FirstOrDefault(tb => tb.team == removedTeam);
            if (removedTeamBtn != null)
            {
                removedTeamBtn.isReplaceable = false;
            }
        }

        // Add the new team to the list
        replacementTeams.Add(team);

        // Update the isReplaceable status
        if (replacementTeams.Count > 0)
        {
            TeamBtn firstTeamBtn = teamBtns.FirstOrDefault(tb => tb.team == replacementTeams[0]);
            if (firstTeamBtn != null)
            {
                firstTeamBtn.isReplaceable = true;
            }
        }

        if (replacementTeams.Count > 1)
        {
            TeamBtn secondTeamBtn = teamBtns.FirstOrDefault(tb => tb.team == replacementTeams[1]);
            if (secondTeamBtn != null)
            {
                secondTeamBtn.isReplaceable = false;
            }
        }

        // Assign the teams to replacementTeam1 and replacementTeam2
        replacementTeam1 = replacementTeams.Count > 0 ? replacementTeams[0] : null;
        replacementTeam2 = replacementTeams.Count > 1 ? replacementTeams[1] : null;
        // Set the normal color of the button to green

        Debug.Log($"Replacement teams updated: {replacementTeam1?.TeamID}, {replacementTeam2?.TeamID}");
    }
    public void RemoveReplaceableTeam(TeamBtn teamBtn)
    {
        Team_TMP team = teamBtn.team;
        if (replacementTeams.Contains(team))
        {
            replacementTeams.Remove(team);
        }

        // Update the isReplaceable status
        if (replacementTeams.Count > 0)
        {
            TeamBtn firstTeamBtn = teamBtns.FirstOrDefault(tb => tb.team == replacementTeams[0]);
            if (firstTeamBtn != null)
            {
                firstTeamBtn.isReplaceable = true;
            }
        }

        if (replacementTeams.Count > 1)
        {
            TeamBtn secondTeamBtn = teamBtns.FirstOrDefault(tb => tb.team == replacementTeams[1]);
            if (secondTeamBtn != null)
            {
                secondTeamBtn.isReplaceable = false;
            }
        }

        // Assign the teams to replacementTeam1 and replacementTeam2
        replacementTeam1 = replacementTeams.Count > 0 ? replacementTeams[0] : null;
        replacementTeam2 = replacementTeams.Count > 1 ? replacementTeams[1] : null;
        // Set the normal color of the button to green
        Debug.Log($"Replacement teams updated: {replacementTeam1?.TeamID}, {replacementTeam2?.TeamID}");
    }
    private void ReplaceTeams()
    {
        if (replacementTeam1 != null && replacementTeam2 != null)
        {

            // Find the TeamBtn instances for replacementTeam1 and replacementTeam2
            TeamBtn teamBtn1 = teamBtns.FirstOrDefault(tb => tb.team == replacementTeam1);
            TeamBtn teamBtn2 = teamBtns.FirstOrDefault(tb => tb.team == replacementTeam2);


            // Swap the team details directly without using a temp variable
            (replacementTeam1.TeamID, replacementTeam2.TeamID) = (replacementTeam2.TeamID, replacementTeam1.TeamID);
            (replacementTeam1.TeamName, replacementTeam2.TeamName) = (replacementTeam2.TeamName, replacementTeam1.TeamName);
            (replacementTeam1.teamRoundData, replacementTeam2.teamRoundData) = (replacementTeam2.teamRoundData, replacementTeam1.teamRoundData);
            // (replacementTeam1.teamRoundData.matchID, replacementTeam2.teamRoundData.matchID) = (replacementTeam2.teamRoundData.matchID, replacementTeam1.teamRoundData.matchID);
            // Update the isReplaceable property
            if (teamBtn1 != null)
            {
                teamBtn1.isReplaceable = false;
                Debug.Log($"<color=red>Updated teamBtn1.isReplaceable to false</color>");
            }
            if (teamBtn2 != null)
            {
                teamBtn2.isReplaceable = false;
                Debug.Log($"<color=red>Updated teamBtn2.isReplaceable to false</color>");
            }

            replacementTeam1 = null;
            replacementTeam2 = null;
            replacementTeams.Clear();
            // Invoke the event
            ReplaceTeamsEvent?.Invoke();
        }
    }
    #endregion
    #region Replace Judge
    public void AddReplaceableAdjudicator(AdjBtn adjudicatorBtn)
    {
        Adjudicator adj = adjudicatorBtn.adj;

        // Check if the list already contains two judges
        if (replacementJudges.Count == 2)
        {
            // Remove the first judge and set its isReplaceable to false
            Adjudicator removedJudge = replacementJudges[0];
            replacementJudges.RemoveAt(0);
            AdjBtn removedJudgeBtn = adjBtns.FirstOrDefault(ab => ab.adj == removedJudge);
            if (removedJudgeBtn != null)
            {
                removedJudgeBtn.isReplaceable = false;
            }
        }

        // Add the new judge to the list
        replacementJudges.Add(adj);

        // Update the isReplaceable status
        if (replacementJudges.Count > 0)
        {
            AdjBtn firstJudgeBtn = adjBtns.FirstOrDefault(ab => ab.adj == replacementJudges[0]);
            if (firstJudgeBtn != null)
            {
                firstJudgeBtn.isReplaceable = true;
            }
        }

        if (replacementJudges.Count > 1)
        {
            AdjBtn secondJudgeBtn = adjBtns.FirstOrDefault(ab => ab.adj == replacementJudges[1]);
            if (secondJudgeBtn != null)
            {
                secondJudgeBtn.isReplaceable = false;
            }
        }

        // Assign the judges to replacementJudge1 and replacementJudge2
        replacementJudge1 = replacementJudges.Count > 0 ? replacementJudges[0] : null;
        replacementJudge2 = replacementJudges.Count > 1 ? replacementJudges[1] : null;

        // Debug.Log($"Replacement judges updated: {replacementJudge1?.adjudicatorID}, {replacementJudge2?.adjudicatorID}");
    }
    public void RemoveReplaceableAdjudicator(AdjBtn adjBtn)
    {
        Adjudicator adj = adjBtn.adj;
        if (replacementJudges.Contains(adj))
        {
            replacementJudges.Remove(adj);
        }

        // Update the isReplaceable status
        if (replacementJudges.Count > 0)
        {
            AdjBtn firstJudgeBtn = adjBtns.FirstOrDefault(ab => ab.adj == replacementJudges[0]);
            if (firstJudgeBtn != null)
            {
                firstJudgeBtn.isReplaceable = true;
            }
        }

        if (replacementJudges.Count > 1)
        {
            AdjBtn secondJudgeBtn = adjBtns.FirstOrDefault(ab => ab.adj == replacementJudges[1]);
            if (secondJudgeBtn != null)
            {
                secondJudgeBtn.isReplaceable = false;
            }
        }

        // Assign the judges to replacementJudge1 and replacementJudge2
        replacementJudge1 = replacementJudges.Count > 0 ? replacementJudges[0] : null;
        replacementJudge2 = replacementJudges.Count > 1 ? replacementJudges[1] : null;

        Debug.Log($"Replacement judges updated: {replacementJudge1?.adjudicatorID}, {replacementJudge2?.adjudicatorID}");
    }
    private void ReplaceJudges()
    {
        if (replacementJudge1 != null && replacementJudge2 != null)
        {
            // Find the AdjBtn instances for replacementJudge1 and replacementJudge2
            AdjBtn adjBtn1 = adjBtns.FirstOrDefault(ab => ab.adj == replacementJudge1);
            AdjBtn adjBtn2 = adjBtns.FirstOrDefault(ab => ab.adj == replacementJudge2);

            // Swap the judges
            (replacementJudge1.adjudicatorID, replacementJudge2.adjudicatorID) = (replacementJudge2.adjudicatorID, replacementJudge1.adjudicatorID);
            (replacementJudge1.adjudicatorName, replacementJudge2.adjudicatorName) = (replacementJudge2.adjudicatorName, replacementJudge1.adjudicatorName);


            // Update the isReplaceable property
            if (adjBtn1 != null)
            {
                adjBtn1.isReplaceable = false;
            }
            if (adjBtn2 != null)
            {
                adjBtn2.isReplaceable = false;
            }
            replacementJudge1 = null;
            replacementJudge2 = null;
            replacementJudges.Clear();
            // Invoke the event
            ReplaceJudgesEvent?.Invoke();
        }
    }
    #endregion

}
