using System.Linq;
using System.Collections.Generic;
using Scripts.Resources;
using Scripts.UIPanels;
using UnityEngine;

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

    [SerializeField] public List<string> unassignedTeamIds = new List<string>();
    [SerializeField] private List<string> assignedTeamIds = new List<string>();

    [SerializeField] private List<string> unassignedAdjudicatorIds = new List<string>();
    [SerializeField] private List<string> assignedAdjudicatorIds = new List<string>();

void OnEnable()
{
    AssignTeams();
    AssignAdjudicators();
}
void OnDisable()
{
    assignedTeamIds.Clear();
    unassignedTeamIds.Clear();
    assignedAdjudicatorIds.Clear();
    unassignedAdjudicatorIds.Clear();
}
    void UpdateMatchesList()
    {
        // Clear the list
        foreach (Transform child in matchesListContent)
        {
            Destroy(child.gameObject);
        }
    
        // Loop through the matches in mainroundspanel.selectedround.matches
        foreach (var match in DrawsPanel.Instance.matches_TMP)
        {
            // Instantiate matchEntryPrefab for each match
            GameObject matchEntry = Instantiate(matchEntryPrefab, matchesListContent);
    
            // Access the DrawEditListEntry script on the instantiated prefab
            DrawEditListEntry drawEditListEntry = matchEntry.GetComponent<DrawEditListEntry>();
            drawEditListEntry.SetMatch(match);
        }
    }

    public void AddTeamToMatch(string teamId, string matchId, TeamPositionsBritish teamPosition)
    {
        // Get the match from the matches_TMP list
        var match = DrawsPanel.Instance.matches_TMP.Find(m => m.matchId == matchId);
        if (match == null)
        {
            Debug.LogError("AddTeamToMatch: Match not found.");
            return;
        }

        // Get the team from the teamsInTourney list
        var team = AppConstants.instance.selectedTouranment.teamsInTourney.Find(t => t.teamId == teamId);
        if (team == null)
        {
            Debug.LogError("AddTeamToMatch: Team not found.");
            return;
        }

        // Check if the team is already assigned to a match
        if (assignedTeamIds.Contains(teamId))
        {
            Debug.LogWarning("AddTeamToMatch: Team is already assigned to a match.");
            return;
        }

        // Assign Team To A Match
        match.teams.Add(teamId, teamPosition);
        assignedTeamIds.Add(teamId);
        unassignedTeamIds.Remove(teamId);
    }

    void AssignTeams()
    {
        foreach (var team in MainRoundsPanel.Instance.selectedRound.availableTeams)
        {
            if (!CheckIfTeamIsAssigned(team.teamId, MainRoundsPanel.Instance.selectedRound.matches))
            {
                unassignedTeamIds.Add(team.teamId);
            }
            else
            {
                assignedTeamIds.Add(team.teamId);
            }
        }
    }
    void AssignAdjudicators()
    {
        foreach (var adj in MainRoundsPanel.Instance.selectedRound.availableAdjudicators)
        {
            if (!CheckIfAdjudicatorIsAssigned(adj, MainRoundsPanel.Instance.selectedRound.matches))
            {
                unassignedAdjudicatorIds.Add(adj.adjudicatorID);
            }
            else
            {
                assignedAdjudicatorIds.Add(adj.adjudicatorID);
            }
        }
    }
    public bool CheckIfTeamIsAssigned(string teamId, List<Match> matches)
    {
        foreach (var match in matches)
        {
            if (match.teams.ContainsKey(teamId))
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckIfAdjudicatorIsAssigned(Adjudicator adj, List<Match> matches)
    {
        foreach (var match in matches)
        {
            if (match.adjudicators.Contains(adj))
            {
                return true;
            }
        }
        return false;
    }
}
