using DG.Tweening;
using System.Collections.Generic;
using Scripts.UIPanels;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using Scripts.FirebaseConfig;
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
    private List<GameObject> adjudicatorEntries = new List<GameObject>();


#region Essentials
    private void OnEnable() 
    {
        attendanceToggle.onSelectOption1 = OpenAdjudicatorAttendancePanel;
        attendanceToggle.onSelectOption2 = OpenTeamAttendancePanel;
    }
    private void OnDisable() {
        // if (isTeamAttendance)
        // {
        //     SaveTeamAttendance();
        // }
        // if (isAdjudicatorAttendance)
        // {
        //     SaveAdjudicatorAttendance();
        // }
        foreach (Transform child in teamAtdPanelContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in adjudicatorAtdPanelContent)
        {
            Destroy(child.gameObject);
        }
        MainRoundsPanel.Instance.SaveRound();
        teamAtdPanel.gameObject.SetActive(false);
        adjudicatorAtdPanel.gameObject.SetActive(false);
                Debug.Log("Count of available adjudicators: " + MainRoundsPanel.Instance.selectedRound.availableAdjudicators.Count);
        Debug.Log("Count of available teams: " + MainRoundsPanel.Instance.selectedRound.availableTeams.Count);
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
    teamAtdPanel.transform.DOLocalMoveY(teamPanelInVector, 0.5f).OnComplete(() =>UpdateTeamList());
    // Update the team list
    
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
    adjudicatorAtdPanel.transform.DOLocalMoveY(adjudicatorPanelInVector, 0.5f).OnComplete(() => UpdateAdjudicatorList());
    // Update the adjudicator list
    // UpdateAdjudicatorList();
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
    Debug.Log("Available Teams Count: " + availableTeams.Count);
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
    Debug.Log("Available Adjudicators Count: " + availableAdjudicators.Count);

    // First, add all GameObjects
    foreach (Adjudicator adjudicator in AppConstants.instance.selectedTouranment.adjudicatorsInTourney)
    {
        GameObject adjudicatorEntry = Instantiate(adjudicatorAtdPanelPrefab, adjudicatorAtdPanelContent);
        AdjudicatorListEntry_Attendance adjudicatorListEntry = adjudicatorEntry.GetComponent<AdjudicatorListEntry_Attendance>();
        adjudicatorListEntry.Initialize(adjudicator);
        adjudicatorAttendanceButtonSelect.AddOption(adjudicatorEntry.GetComponent<Button>(), adjudicatorListEntry.MarkAttendance, adjudicatorListEntry.UnMarkAttendance);

        if(availableAdjudicators.Any(a => a.adjudicatorID == adjudicator.adjudicatorID))
        {
            if (adjudicatorEntry.TryGetComponent<Button>(out var button))
            {
                adjudicatorAttendanceButtonSelect.SelectOption(button);
            }
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
    public async void SaveTeamAttendance()
    {
        Loading.Instance.ShowLoadingScreen();
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
        List<string> uniqueTeamIdsList = uniqueTeamIds.ToList();
        await FirestoreManager.FireInstance.SaveTeamAttendanceToFirestore(MainRoundsPanel.Instance.selectedRound.roundCategory.ToString(), MainRoundsPanel.Instance.selectedRound.roundId, uniqueTeamIdsList,uniqueTeams, OnTeamAttendanceAddedSuccess, OnTeamAttendanceAddedFailure);
    }
    
    public async void SaveAdjudicatorAttendance()
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
    
    await FirestoreManager.FireInstance.SaveAdjudicatorAttendanceToFirestore(MainRoundsPanel.Instance.selectedRound.roundCategory.ToString(), MainRoundsPanel.Instance.selectedRound.roundId, uniqueAdjudicatorIds.ToList(), uniqueAdjudicators, OnAdjudicatorAttendanceAddedSuccess, OnAdjudicatorAttendanceAddedFailure);
       
    }
    #endregion
#region Firestore Callbacks
    public void OnTeamAttendanceAddedSuccess(List<Team> uniqueTeams)
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Team Attendance Added Successfully.", Color.green);
                // Assign the filtered list to selectedRound
        MainRoundsPanel.Instance.selectedRound.availableTeams.Clear();
        MainRoundsPanel.Instance.selectedRound.availableTeams = uniqueTeams;
        MainRoundsPanel.Instance.selectedRound.teamAttendanceAdded = true;
        foreach (var team in MainRoundsPanel.Instance.selectedRound.availableTeams)
        {
            Debug.Log("<color=green>Called From Save Team Attendance Available Team: " + team.teamName + "</color>");
        }
        // foreach (var team in AvailableTeams_tmp)
        // {
        //     Debug.Log("<color=red>Called From Save Team Attendance Available Team TMP: " + team.teamName + "</color>");
        // }
        isTeamAttendance = true;
        AvailableTeams_tmp.Clear();
        MainRoundsPanel.Instance.UpdatePanelSwitcherButtonsStates();
    }
    public void OnTeamAttendanceAddedFailure()
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Failed to add team attendance.", Color.red);
    }
    public void OnAdjudicatorAttendanceAddedSuccess(List<Adjudicator> uniqueAdjudicators)
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Adjudicator Attendance Added Successfully.", Color.green);
         // Assign the filtered list to selectedRound
        MainRoundsPanel.Instance.selectedRound.availableAdjudicators.Clear();
        MainRoundsPanel.Instance.selectedRound.availableAdjudicators = uniqueAdjudicators;
        MainRoundsPanel.Instance.selectedRound.AdjudicatorAttendanceAdded = true;
        foreach (var adjudicator in MainRoundsPanel.Instance.selectedRound.availableAdjudicators)
        {
            Debug.Log("<color=green>Called From Save Adjudicator Attendance Available Adjudicator: " + adjudicator.adjudicatorName + "</color>");
        }
        // foreach (var adjudicator in AvailableAdjudicators_tmp)
        // {
        //     Debug.Log("<color=red>Called From Save Adjudicator Attendance Available Adjudicator TMP: " + adjudicator.adjudicatorName + "</color>");
        // }
        isAdjudicatorAttendance = true;
        AvailableAdjudicators_tmp.Clear();
        MainRoundsPanel.Instance.UpdatePanelSwitcherButtonsStates();
    }
    public void OnAdjudicatorAttendanceAddedFailure()
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Failed to add adjudicator attendance.", Color.red);
    }

#endregion
}
