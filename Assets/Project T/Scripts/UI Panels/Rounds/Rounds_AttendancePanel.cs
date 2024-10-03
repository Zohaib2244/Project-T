using DG.Tweening;
using System.Collections.Generic;
using Scripts.UIPanels;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Rounds_AttendancePanel : MonoBehaviour
{
    #region Singleton
    public static Rounds_AttendancePanel Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion
    [Header("Panels")]
    [SerializeField] private Transform teamAtdPanel;
    [SerializeField] private Transform teamAtdPanelContent;
    [SerializeField] private GameObject teamAtdPanelPrefab;
    [SerializeField] private Transform adjudicatorAtdPanel;
    [SerializeField] private Transform adjudicatorAtdPanelContent;
    [SerializeField] private GameObject adjudicatorAtdPanelPrefab;
    [Header("Animation Vectors")]
    [SerializeField] private float teamPanelInVector;
    [SerializeField] private float teamPanelOutVector;
    [SerializeField] private float adjudicatorPanelInVector;
    [SerializeField] private float adjudicatorPanelOutVector;
    
    [Header("Buttons")]
    [SerializeField] private ButtonSelect teamAttendanceButtonSelect;
    [SerializeField] private ButtonSelect adjudicatorAttendanceButtonSelect;
    [SerializeField] private Toggle attendanceToggle;
    public bool isTeamAttendance = false;
    public bool isAdjudicatorAttendance = false;    
    private List<Team> AvailableTeams_tmp;
    private List<Adjudicator> AvailableAdjudicators_tmp;


#region Essentials
    private void OnEnable() 
    {
        attendanceToggle.onSelectOption1 = OpenAdjudicatorAttendancePanel;
        attendanceToggle.onSelectOption2 = OpenTeamAttendancePanel;
    }
    private void OnDisable() {
        if (isTeamAttendance)
        {
            SaveTeamAttendance();
        }
        if (isAdjudicatorAttendance)
        {
            SaveAdjudicatorAttendance();
        }
        teamAtdPanel.gameObject.SetActive(false);
        adjudicatorAtdPanel.gameObject.SetActive(false);
    }
#endregion



#region UI Configuration

public void OpenTeamAttendancePanel()
{
    if (adjudicatorAtdPanel.gameObject.activeSelf)
    {
        // Animate the adjudicator panel downwards
        adjudicatorAtdPanel.transform.DOLocalMoveY(adjudicatorPanelOutVector, 0.5f).OnComplete(() =>
        {
            adjudicatorAtdPanel.gameObject.SetActive(false);
        });
    }

    // Set the team panel active and animate it upwards
    teamAtdPanel.gameObject.SetActive(true);
    teamAtdPanel.transform.localPosition = new Vector3(0, -Screen.height, 0); // Start below the screen
    teamAtdPanel.transform.DOLocalMoveY(teamPanelInVector, 0.5f);
    // Update the team list
    UpdateTeamList();
}
public void OpenAdjudicatorAttendancePanel()
{
    if (teamAtdPanel.gameObject.activeSelf)
    {
        // Animate the team panel downwards
        teamAtdPanel.transform.DOLocalMoveY(teamPanelOutVector, 0.5f).OnComplete(() =>
        {
            teamAtdPanel.gameObject.SetActive(false);
        });
    }

    // Set the adjudicator panel active and animate it upwards
    adjudicatorAtdPanel.gameObject.SetActive(true);
    adjudicatorAtdPanel.transform.localPosition = new Vector3(0, -Screen.height, 0); // Start below the screen
    adjudicatorAtdPanel.transform.DOLocalMoveY(adjudicatorPanelInVector, 0.5f);
    // Update the adjudicator list
    UpdateAdjudicatorList();
}


private void UpdateTeamList()
{
    // Clear existing team entries
    foreach (Transform child in teamAtdPanelContent)
    {
        Destroy(child.gameObject);
    }
    teamAttendanceButtonSelect.RemoveAllButtons();
    // Get the list of available teams from selectedRound
    var availableTeams = MainRoundsPanel.Instance.selectedRound.availableTeams;
    
    // Instantiate team list entries and check for attendance
    foreach (Team team in AppConstants.instance.selectedTouranment.teamsInTourney)
    {
        GameObject teamEntry = Instantiate(teamAtdPanelPrefab, teamAtdPanelContent);
        TeamListEntry_Attendance teamListEntry = teamEntry.GetComponent<TeamListEntry_Attendance>();
        teamListEntry.Initialize(team);
        teamAttendanceButtonSelect.AddOption(teamEntry.GetComponent<Button>(), teamListEntry.MarkAttendance, teamListEntry.UnMarkAttendance);

        // Check if the team exists in availableTeams and mark attendance if it does
        if (availableTeams.Any(t => t.teamId == team.teamId))
        {
            teamAttendanceButtonSelect.SelectOption(teamEntry.GetComponent<Button>());
        }
    }
    foreach (var team in availableTeams)
    {
        Debug.Log("<color=orange>Called From Update Team List Available Team: " + team.teamName + "</color>");
    }
}
private void UpdateAdjudicatorList()
{
    // Clear existing adjudicator entries
    foreach (Transform child in adjudicatorAtdPanelContent)
    {
        Destroy(child.gameObject);
    }
    adjudicatorAttendanceButtonSelect.RemoveAllButtons();
    // Get the list of available adjudicators from selectedRound
    var availableAdjudicators = MainRoundsPanel.Instance.selectedRound.availableAdjudicators;

    // Instantiate adjudicator list entries and check for attendance
    foreach (Adjudicator adjudicator in AppConstants.instance.selectedTouranment.adjudicatorsInTourney)
    {
        GameObject adjudicatorEntry = Instantiate(adjudicatorAtdPanelPrefab, adjudicatorAtdPanelContent);
        AdjudicatorListEntry_Attendance adjudicatorListEntry = adjudicatorEntry.GetComponent<AdjudicatorListEntry_Attendance>();
        adjudicatorListEntry.Initialize(adjudicator);
        adjudicatorAttendanceButtonSelect.AddOption(adjudicatorEntry.GetComponent<Button>(), adjudicatorListEntry.MarkAttendance, adjudicatorListEntry.UnMarkAttendance);

        // Check if the adjudicator exists in availableAdjudicators and mark attendance if it does
        if (availableAdjudicators.Any(a => a.adjudicatorID == adjudicator.adjudicatorID))
        {
            adjudicatorAttendanceButtonSelect.SelectOption(adjudicatorEntry.GetComponent<Button>());
        }
    }
}


#endregion

#region Attendance Functions
    public void AddTeamAsAvailable(Team team)
    {
        if (AvailableTeams_tmp == null)
        {
            AvailableTeams_tmp = new List<Team>();
        }
        AvailableTeams_tmp.Add(team);
        team.available = true;
    }
    public void RemoveTeamAsAvailable(Team team)
    {
        if (AvailableTeams_tmp == null)
        {
            return;
        }
        AvailableTeams_tmp.Remove(team);
        team.available = false;
    }
    public void AddAdjudicatorAsAvailable(Adjudicator adjudicator)
    {
        if (AvailableAdjudicators_tmp == null)
        {
            AvailableAdjudicators_tmp = new List<Adjudicator>();
        }
        AvailableAdjudicators_tmp.Add(adjudicator);
        adjudicator.available = true;
    }
    public void RemoveAdjudicatorAsAvailable(Adjudicator adjudicator)
    {
        if (AvailableAdjudicators_tmp == null)
        {
            return;
        }
        AvailableAdjudicators_tmp.Remove(adjudicator);
        adjudicator.available = false;
    }
    public void SaveTeamAttendance()
    {
        // Create a HashSet to track unique team IDs (as strings)
        HashSet<string> uniqueTeamIds = new HashSet<string>();
        List<Team> uniqueTeams = new List<Team>();
    
        // Filter the list to remove duplicates
        foreach (var team in AvailableTeams_tmp)
        {
            if (uniqueTeamIds.Add(team.teamId))
            {
                uniqueTeams.Add(team);
            }
        }
    
        // Assign the filtered list to selectedRound
        MainRoundsPanel.Instance.selectedRound.availableTeams.Clear();
        MainRoundsPanel.Instance.selectedRound.availableTeams = uniqueTeams;
        MainRoundsPanel.Instance.selectedRound.teamAttendanceAdded = true;
        foreach (var team in MainRoundsPanel.Instance.selectedRound.availableTeams)
        {
            Debug.Log("<color=green>Called From Save Team Attendance Available Team: " + team.teamName + "</color>");
        }
        foreach (var team in AvailableTeams_tmp)
        {
            Debug.Log("<color=red>Called From Save Team Attendance Available Team TMP: " + team.teamName + "</color>");
        }
        isTeamAttendance = true;
        AvailableTeams_tmp.Clear();
    }
    
    public void SaveAdjudicatorAttendance()
    {
        // Create a HashSet to track unique adjudicator IDs (as strings)
        HashSet<string> uniqueAdjudicatorIds = new HashSet<string>();
        List<Adjudicator> uniqueAdjudicators = new List<Adjudicator>();
    
        // Filter the list to remove duplicates
        foreach (var adjudicator in AvailableAdjudicators_tmp)
        {
            if (uniqueAdjudicatorIds.Add(adjudicator.adjudicatorID))
            {
                uniqueAdjudicators.Add(adjudicator);
            }
        }
    
        // Assign the filtered list to selectedRound
        MainRoundsPanel.Instance.selectedRound.availableAdjudicators.Clear();
        MainRoundsPanel.Instance.selectedRound.availableAdjudicators = uniqueAdjudicators;
        MainRoundsPanel.Instance.selectedRound.AdjudicatorAttendanceAdded = true;
        isAdjudicatorAttendance = true;
        AvailableAdjudicators_tmp.Clear();
    }
    #endregion
}
