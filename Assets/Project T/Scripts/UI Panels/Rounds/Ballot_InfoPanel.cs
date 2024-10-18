using Scripts.Resources;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Scripts.FirebaseConfig;
using Scripts.UIPanels;
using System.Linq;
using System;
using UnityEngine.UI;

public class Ballot_InfoPanel : MonoBehaviour
{
    #region Singleton
    public static Ballot_InfoPanel Instance;
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
    //OG Variables----------------------------------------------------
    // [SerializeField] private TMP_Text matchNotxt;

    #region Variables
    [Header("OG Variables")]
    [SerializeField] private TMP_Text OG_posText;
    [SerializeField] private TMP_Text OG_teamNameText;
    [SerializeField] private TMP_Text OG_speakerNameText_2;
    [SerializeField] private AdvancedDropdown OG_speakerNameDropdown_1;
    [SerializeField] private TMP_InputField OG_speakerScore_1;
    [SerializeField] private TMP_InputField OG_speakerScore_2;
    [SerializeField] private TMP_Text OG_totalScore;
    [SerializeField] private bool OG_ironman;
    //OO Variables----------------------------------------------------  
    [Header("OO Variables")]
    [SerializeField] private TMP_Text OO_posText;
    [SerializeField] private TMP_Text OO_teamNameText;
    [SerializeField] private TMP_Text OO_speakerNameText_2;
    [SerializeField] private AdvancedDropdown OO_speakerNameDropdown_1;
    [SerializeField] private TMP_InputField OO_speakerScore_1;
    [SerializeField] private TMP_InputField OO_speakerScore_2;
    [SerializeField] private TMP_Text OO_totalScore;
    [SerializeField] private bool OO_ironman;
    //CG Variables----------------------------------------------------
    [Header("CG Variables")]
    [SerializeField] private TMP_Text CG_posText;
    [SerializeField] private TMP_Text CG_teamNameText;
    [SerializeField] private TMP_Text CG_speakerNameText_2;
    [SerializeField] private AdvancedDropdown CG_speakerNameDropdown_1;
    [SerializeField] private TMP_InputField CG_speakerScore_1;
    [SerializeField] private TMP_InputField CG_speakerScore_2;
    [SerializeField] private TMP_Text CG_totalScore;
    [SerializeField] private bool CG_ironman;
    //CO Variables----------------------------------------------------
    [Header("CO Variables")]
    [SerializeField] private TMP_Text CO_posText;
    [SerializeField] private TMP_Text CO_teamNameText;
    [SerializeField] private TMP_Text CO_speakerNameText_2;
    [SerializeField] private AdvancedDropdown CO_speakerNameDropdown_1;
    [SerializeField] private TMP_InputField CO_speakerScore_1;
    [SerializeField] private TMP_InputField CO_speakerScore_2;
    [SerializeField] private TMP_Text CO_totalScore;
    [SerializeField] private bool CO_ironman;


    [SerializeField] private Button saveBtn;
    private Match myMatch;
    private TeamRoundData OG_trd;
    private TeamRoundData OO_trd;
    private TeamRoundData CG_trd;
    private TeamRoundData CO_trd;
    #endregion
    void OnEnable()
    {
        OG_speakerScore_1.onValueChanged.AddListener(delegate { UpdateScoresAndPositions(); });
        OG_speakerScore_2.onValueChanged.AddListener(delegate { UpdateScoresAndPositions(); });
        OO_speakerScore_1.onValueChanged.AddListener(delegate { UpdateScoresAndPositions(); });
        OO_speakerScore_2.onValueChanged.AddListener(delegate { UpdateScoresAndPositions(); });
        CG_speakerScore_1.onValueChanged.AddListener(delegate { UpdateScoresAndPositions(); });
        CG_speakerScore_2.onValueChanged.AddListener(delegate { UpdateScoresAndPositions(); });
        CO_speakerScore_1.onValueChanged.AddListener(delegate { UpdateScoresAndPositions(); });
        CO_speakerScore_2.onValueChanged.AddListener(delegate { UpdateScoresAndPositions(); });
        // Call the function to assign TRD to appropriate variables
        AddDropdownListeners();
    }
    void OnDisable()
    {
        ClearScreen();
        OG_speakerScore_1.onValueChanged.RemoveAllListeners();
        OG_speakerScore_2.onValueChanged.RemoveAllListeners();
        OO_speakerScore_1.onValueChanged.RemoveAllListeners();
        OO_speakerScore_2.onValueChanged.RemoveAllListeners();
        CG_speakerScore_1.onValueChanged.RemoveAllListeners();
        CG_speakerScore_2.onValueChanged.RemoveAllListeners();
        CO_speakerScore_1.onValueChanged.RemoveAllListeners();
        CO_speakerScore_2.onValueChanged.RemoveAllListeners();
        
        OG_speakerNameDropdown_1.onChangedValue -= OnOGSpeakerSelected;
        OO_speakerNameDropdown_1.onChangedValue -= OnOOSpeakerSelected;
        CG_speakerNameDropdown_1.onChangedValue -= OnCGSpeakerSelected;
        CO_speakerNameDropdown_1.onChangedValue -= OnCOSpeakerSelected;

        OG_trd = null;
        OO_trd = null;
        CG_trd = null;
        CO_trd = null;
    }

    #region Essentials
    public void DisplayMatchBallot(Match match)
    {
        myMatch = match;
        AssignTeamRoundData();
        ClearScreen();
        ConfigureScreen();
        PopulateDropdowns();
      
        if (myMatch.ballotEntered)
        {
            Debug.Log("<color=yellow>Ballot already entered</color>");
            // UpdateSpeakerScores();
        }
    }
    public async void SaveBallot()
    {
        myMatch.ballotEntered = true;

        // Save data from input fields to TeamRoundData objects
        SaveTeamRoundDataFromInput(OG_trd, OG_speakerScore_1, OG_speakerScore_2);
        SaveTeamRoundDataFromInput(OO_trd, OO_speakerScore_1, OO_speakerScore_2);
        SaveTeamRoundDataFromInput(CG_trd, CG_speakerScore_1, CG_speakerScore_2);
        SaveTeamRoundDataFromInput(CO_trd, CO_speakerScore_1, CO_speakerScore_2);

        SaveTeamRoundData(OG_trd);
        SaveTeamRoundData(OO_trd);
        SaveTeamRoundData(CG_trd);
        SaveTeamRoundData(CO_trd);
        //debug info of all trd
        DebugTeamRoundData(OG_trd, "OG");
        DebugTeamRoundData(OO_trd, "OO");
        DebugTeamRoundData(CG_trd, "CG");
        DebugTeamRoundData(CO_trd, "CO");

        Loading.Instance.ShowLoadingScreen();
        // Rounds_BallotsPanel.Instance.CloseBallotInfo();
        await FirestoreManager.FireInstance.UpdateMatchAtFirestore(MainRoundsPanel.Instance.selectedRound.roundCategory.ToString(), MainRoundsPanel.Instance.selectedRound.roundId, myMatch, OnUpdateMatchesSuccess, OnUpdateMatchesFail);
    }
    private void ConfigureScreen()
    {
        // Set the team names
        OG_teamNameText.text = OG_trd != null ? AppConstants.instance.GetTeamFromID(OG_trd.teamId.ToString()).teamName : "";
        OO_teamNameText.text = OO_trd != null ? AppConstants.instance.GetTeamFromID(OO_trd.teamId.ToString()).teamName : "";
        CG_teamNameText.text = CG_trd != null ? AppConstants.instance.GetTeamFromID(CG_trd.teamId.ToString()).teamName : "";
        CO_teamNameText.text = CO_trd != null ? AppConstants.instance.GetTeamFromID(CO_trd.teamId.ToString()).teamName : "";
    }
    private void ClearScreen()
    {
        OG_posText.text = "-";
        OG_teamNameText.text = "";
        OG_speakerNameText_2.text = "";
        OG_speakerScore_1.text = "0";
        OG_speakerScore_2.text = "0";
        OG_totalScore.text = "0";

        OO_posText.text = "-";
        OO_teamNameText.text = "";
        OO_speakerNameText_2.text = "";
        OO_speakerScore_1.text = "0";
        OO_speakerScore_2.text = "0";
        OO_totalScore.text = "0";

        CG_posText.text = "-";
        CG_teamNameText.text = "";
        CG_speakerNameText_2.text = "";
        CG_speakerScore_1.text = "0";
        CG_speakerScore_2.text = "0";
        CG_totalScore.text = "0";

        CO_posText.text = "-";
        CO_teamNameText.text = "";
        CO_speakerNameText_2.text = "0";
        CO_speakerScore_1.text = "0";
        CO_speakerScore_2.text = "0";
        CO_totalScore.text = "0";

    }
    private void CheckSaveButtonInteractability()
    {
        if (OG_speakerScore_1.text == "" || OG_speakerScore_2.text == "" || OO_speakerScore_1.text == "" || OO_speakerScore_2.text == "" || CG_speakerScore_1.text == "" || CG_speakerScore_2.text == "" || CO_speakerScore_1.text == "" || CO_speakerScore_2.text == "")
        {
            saveBtn.interactable = false;
        }
        else
        {
            saveBtn.interactable = true;
        }
    }
   #endregion

    #region TRD Configuration
    public void UpdateSpeakerScores()
    {
        // Update speaker scores for each team
        UpdateTeamSpeakerScores(OG_trd, OG_speakerScore_1, OG_speakerScore_2);
        UpdateTeamSpeakerScores(OO_trd, OO_speakerScore_1, OO_speakerScore_2);
        UpdateTeamSpeakerScores(CG_trd, CG_speakerScore_1, CG_speakerScore_2);
        UpdateTeamSpeakerScores(CO_trd, CO_speakerScore_1, CO_speakerScore_2);
    }

    private TeamRoundData GetTeamRoundData(Match match, TeamPositionsBritish teamPosition)
    {
        string teamRoundDataID;
        if (match.teams.TryGetValue(teamPosition.ToString(), out teamRoundDataID))
        {
            // Assuming you have a method to get TeamRoundData by ID
            return GetTeamRoundDataByID(teamRoundDataID);
        }
        return null;
    }

    private void UpdateTeamSpeakerScores(TeamRoundData trd, TMP_InputField score1, TMP_InputField score2)
    {
        if (trd != null && trd.speakersInRound != null)
        {
            score1.text = trd.speakersInRound.Count > 0 ? trd.speakersInRound[0].speakerScore.ToString() : "0";
            score2.text = trd.speakersInRound.Count > 1 ? trd.speakersInRound[1].speakerScore.ToString() : "0";
        }
        else
        {
            score1.text = "0";
            score2.text = "0";
        }
    }

    private TeamRoundData GetTeamRoundDataByID(string teamRoundDataID)
    {
        // Implement this method based on your data retrieval logic
        return new TeamRoundData();
    }

    private void SaveTeamRoundDataFromInput(TeamRoundData trd, TMP_InputField score1, TMP_InputField score2)
    {
        if (trd != null)
        {
            trd.speakersInRound[0].speakerScore = float.TryParse(score1.text, out float score1Value) ? score1Value : 0;
            trd.speakersInRound[1].speakerScore = float.TryParse(score2.text, out float score2Value) ? score2Value : 0;
            trd.teamScore = trd.speakersInRound[0].speakerScore + trd.speakersInRound[1].speakerScore;
        }
    }
    private void DebugTeamRoundData(TeamRoundData trd, string teamPosition)
    {
        if (trd != null)
        {
            Debug.Log($"{teamPosition} TeamRoundData:");
            Debug.Log($"teamRoundDataID: {trd.teamRoundDataID}");
            Debug.Log($"teamId: {trd.teamId}");
            Debug.Log($"teamPositionAsian: {trd.teamPositionAsian}");
            Debug.Log($"teamPositionBritish: {trd.teamPositionBritish}");
            Debug.Log($"teamScore: {trd.teamScore}");
            Debug.Log($"teamMatchRanking: {trd.teamMatchRanking}");
            Debug.Log($"speakersInRound Count: {trd.speakersInRound.Count}");
            for (int i = 0; i < trd.speakersInRound.Count; i++)
            {
                var speaker = trd.speakersInRound[i];
                Debug.Log($"Speaker {i + 1}:");
                Debug.Log($"  speakerId: {speaker.speakerId}");
                Debug.Log($"  speakerSpeakingPosition: {speaker.speakerSpeakingPosition}");
                Debug.Log($"  speakerScore: {speaker.speakerScore}");
            }
        }
        else
        {
            Debug.Log($"{teamPosition} TeamRoundData is null");
        }
    }
    private void SaveTeamRoundData(TeamRoundData trd)
    {
        var team = AppConstants.instance.selectedTouranment.teamsInTourney.Find(t => t.teamId == trd.teamId);
        if (team != null)
        {
            var existingRoundData = team.teamRoundDatas.Find(r => r.teamRoundDataID == trd.teamRoundDataID);
            if (existingRoundData != null)
            {
                // Replace the existing round data
                int index = team.teamRoundDatas.IndexOf(existingRoundData);
                team.teamRoundDatas[index] = trd;
            }
            else
            {
                // Add the new round data
                team.teamRoundDatas.Add(trd);
            }
        }
    }
    void AssignTeamRoundData()
    {
        foreach (var teamEntry in myMatch.teams)
        {
            // Find the corresponding team in the tournament
            var team = AppConstants.instance.selectedTouranment.teamsInTourney.Find(t => t.teamId == teamEntry.Key);
            if (team != null)
            {
                // Retrieve the team's round data
                var teamRoundData = team.teamRoundDatas.Find(trd => trd.teamRoundDataID == teamEntry.Value);
                if (teamRoundData != null)
                {
                    // Assign the TRD to the appropriate variable based on BritishTeamPositions
                    switch (teamRoundData.teamPositionBritish)
                    {
                        case TeamPositionsBritish.OG:
                            OG_trd = teamRoundData;
                            break;
                        case TeamPositionsBritish.OO:
                            OO_trd = teamRoundData;
                            break;
                        case TeamPositionsBritish.CG:
                            CG_trd = teamRoundData;
                            break;
                        case TeamPositionsBritish.CO:
                            CO_trd = teamRoundData;
                            break;
                    }
                }
            }
        }
    }
    #endregion
    #region Toggle Configuration
    public void UpdateOGIronman(bool value)
    {
        OG_ironman = value;
    }
    public void UpdateOOIronman(bool value)
    {
        OO_ironman = value;
    }
    public void UpdateCGIronman(bool value)
    {
        CG_ironman = value;
    }
    public void UpdateCOIronman(bool value)
    {
        CO_ironman = value;
    }
    #endregion
    #region Dropdown Configuration
    void PopulateDropdowns()
    {
        PopulateDropdown(OG_trd, OG_speakerNameDropdown_1, OG_speakerNameText_2, OG_speakerScore_1, OG_speakerScore_2);
        PopulateDropdown(OO_trd, OO_speakerNameDropdown_1, OO_speakerNameText_2, OO_speakerScore_1, OO_speakerScore_2);
        PopulateDropdown(CG_trd, CG_speakerNameDropdown_1, CG_speakerNameText_2, CG_speakerScore_1, CG_speakerScore_2);
        PopulateDropdown(CO_trd, CO_speakerNameDropdown_1, CO_speakerNameText_2, CO_speakerScore_1, CO_speakerScore_2);
    }

    void PopulateDropdown(TeamRoundData trd, AdvancedDropdown dropdown, TMP_Text speakerNameText_2, TMP_InputField speaker1ScoreText, TMP_InputField speaker2ScoreText)
    {
        if (trd != null)
        {
            var team = AppConstants.instance.selectedTouranment.teamsInTourney.Find(t => t.teamId == trd.teamId);
            if (team != null)
            {
                dropdown.DeleteAllOptions();
                string[] options = new string[team.speakers.Count];
                for (int i = 0; i < team.speakers.Count; i++)
                {
                    options[i] = team.speakers[i].speakerName;
                }
    
                dropdown.AddOptions(options);
    
                if (trd.speakersInRound[0].speakerId == "0")
                {
                    dropdown.SetDefaultText();
                    speakerNameText_2.text = "";
                    speaker1ScoreText.text = "";
                    speaker2ScoreText.text = "";
                }
                else
                {
                    var speaker1 = team.speakers.Find(s => s.speakerId == trd.speakersInRound[0].speakerId);
                    var speaker2 = team.speakers.Find(s => s.speakerId == trd.speakersInRound[1].speakerId);
    
                    if (speaker1 != null)
                    {
                        dropdown.ratioShrinking = 1;
                        dropdown.SelectOption(0);
                        speaker1ScoreText.text = trd.speakersInRound[0].speakerScore.ToString();
                    }
    
                    if (speaker2 != null)
                    {
                        speaker2ScoreText.text = trd.speakersInRound[1].speakerScore.ToString();
                    }
                }
            }
        }
    }
    void AddDropdownListeners()
    {
        OG_speakerNameDropdown_1.onChangedValue += OnOGSpeakerSelected;
        OO_speakerNameDropdown_1.onChangedValue += OnOOSpeakerSelected;
        CG_speakerNameDropdown_1.onChangedValue += OnCGSpeakerSelected;
        CO_speakerNameDropdown_1.onChangedValue += OnCOSpeakerSelected;
    }
    #endregion

    #region Speaker Selected Callbacks
    void OnOGSpeakerSelected(int index)
    {
        var team = AppConstants.instance.selectedTouranment.teamsInTourney.Find(t => t.teamId == OG_trd.teamId);
        if (team != null)
        {
            var selectedSpeaker = team.speakers[index];
            OG_trd.speakersInRound[0].speakerId = selectedSpeaker.speakerId;

            // Find the other speaker
            var otherSpeaker = team.speakers.Find(s => s.speakerId != selectedSpeaker.speakerId);
            if (otherSpeaker != null)
            {
                OG_trd.speakersInRound[1].speakerId = otherSpeaker.speakerId;
                OG_speakerNameText_2.text = otherSpeaker.speakerName;
            }
            else if(OG_ironman)
            {
                OG_trd.speakersInRound[1].speakerId = selectedSpeaker.speakerId;
                OG_speakerNameText_2.text = selectedSpeaker.speakerName;
            }
        }

    }
    void OnOOSpeakerSelected(int index)
    {
        var team = AppConstants.instance.selectedTouranment.teamsInTourney.Find(t => t.teamId == OO_trd.teamId);
        if (team != null)
        {
            var selectedSpeaker = team.speakers[index];
            OO_trd.speakersInRound[0].speakerId = selectedSpeaker.speakerId;

            // Find the other speaker
            var otherSpeaker = team.speakers.Find(s => s.speakerId != selectedSpeaker.speakerId);
            if (otherSpeaker != null)
            {
                OO_trd.speakersInRound[1].speakerId = otherSpeaker.speakerId;
                OO_speakerNameText_2.text = otherSpeaker.speakerName;
            }
            else if(OO_ironman)
            {
                OO_trd.speakersInRound[1].speakerId = selectedSpeaker.speakerId;
                OO_speakerNameText_2.text = selectedSpeaker.speakerName;
            }
        }
    }

    void OnCGSpeakerSelected(int index)
    {
        var team = AppConstants.instance.selectedTouranment.teamsInTourney.Find(t => t.teamId == CG_trd.teamId);
        if (team != null)
        {
            var selectedSpeaker = team.speakers[index];
            CG_trd.speakersInRound[0].speakerId = selectedSpeaker.speakerId;

            // Find the other speaker
            var otherSpeaker = team.speakers.Find(s => s.speakerId != selectedSpeaker.speakerId);
            if (otherSpeaker != null)
            {
                CG_trd.speakersInRound[1].speakerId = otherSpeaker.speakerId;
                CG_speakerNameText_2.text = otherSpeaker.speakerName;
            }
            else if
            (CG_ironman)
            {
                CG_trd.speakersInRound[1].speakerId = selectedSpeaker.speakerId;
                CG_speakerNameText_2.text = selectedSpeaker.speakerName;
            }
        }
    }
    void OnCOSpeakerSelected(int index)
    {
        var team = AppConstants.instance.selectedTouranment.teamsInTourney.Find(t => t.teamId == CO_trd.teamId);
        if (team != null)
        {
            var selectedSpeaker = team.speakers[index];
            CO_trd.speakersInRound[0].speakerId = selectedSpeaker.speakerId;

            // Find the other speaker
            var otherSpeaker = team.speakers.Find(s => s.speakerId != selectedSpeaker.speakerId);
            if (otherSpeaker != null)
            {
                CO_trd.speakersInRound[1].speakerId = otherSpeaker.speakerId;
                CO_speakerNameText_2.text = otherSpeaker.speakerName;
            }
            else if(CO_ironman)
            {
                CO_trd.speakersInRound[1].speakerId = selectedSpeaker.speakerId;
                CO_speakerNameText_2.text = selectedSpeaker.speakerName;
            }
        }
    }
    #endregion

    #region Update Scores and Positions
    void UpdateScoresAndPositions()
    {
        UpdateTotalScore(OG_trd, OG_speakerScore_1, OG_speakerScore_2, OG_totalScore);
        UpdateTotalScore(OO_trd, OO_speakerScore_1, OO_speakerScore_2, OO_totalScore);
        UpdateTotalScore(CG_trd, CG_speakerScore_1, CG_speakerScore_2, CG_totalScore);
        UpdateTotalScore(CO_trd, CO_speakerScore_1, CO_speakerScore_2, CO_totalScore);

        UpdatePositions();
        CheckSaveButtonInteractability();
    }

    void UpdateTotalScore(TeamRoundData trd, TMP_InputField score1, TMP_InputField score2, TMP_Text totalScoreText)
    {
        if (trd != null)
        {
            int score1Value = int.TryParse(score1.text, out score1Value) ? score1Value : 0;
            int score2Value = int.TryParse(score2.text, out score2Value) ? score2Value : 0;
            int totalScore = score1Value + score2Value;
            totalScoreText.text = totalScore.ToString();
            trd.teamScore = totalScore;
        }
    }
    void UpdatePositions()
    {
        List<(TeamRoundData trd, TMP_Text posText)> teams = new List<(TeamRoundData, TMP_Text)>
        {
            (OG_trd, OG_posText),
            (OO_trd, OO_posText),
            (CG_trd, CG_posText),
            (CO_trd, CO_posText)
        };
    
        // Filter out any null TeamRoundData or TMP_Text entries
        teams = teams.Where(t => t.trd != null && t.posText != null).ToList();
    
        // Sort the teams by teamScore, handling potential null values
        teams.Sort((a, b) =>
        {
            if (a.trd == null || b.trd == null)
            {
                throw new InvalidOperationException("TeamRoundData cannot be null.");
            }
    
            if (float.IsNaN(a.trd.teamScore) && float.IsNaN(b.trd.teamScore)) return 0;
            if (float.IsNaN(a.trd.teamScore)) return 1;
            if (float.IsNaN(b.trd.teamScore)) return -1;
            return b.trd.teamScore.CompareTo(a.trd.teamScore);
        });
    
        string[] positions = { "1st", "2nd", "3rd", "4th" };
        for (int i = 0; i < teams.Count; i++)
        {
            // Check for ties
            if (i > 0 && teams[i].trd.teamScore == teams[i - 1].trd.teamScore)
            {
                teams[i].posText.text = "-";
                teams[i - 1].posText.text = "-"; // Ensure the previous team also shows "-"
            }
            else
            {
                teams[i].posText.text = positions[i];
            }
            teams[i].trd.teamMatchRanking = i + 1;
        }
    }
    #endregion

    #region CallBacks
    private void OnUpdateMatchesSuccess()
    {
        DialogueBox.Instance.ShowDialogueBox("Ballot saved successfully", Color.green);
        Rounds_BallotsPanel.Instance.CloseBallotInfo();
    }
    private void OnUpdateMatchesFail()
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Failed to save ballot", Color.red);
    }

    #endregion
}
