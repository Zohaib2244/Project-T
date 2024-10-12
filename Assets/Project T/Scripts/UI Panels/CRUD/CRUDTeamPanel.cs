using UnityEngine;
using Scripts.Resources;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
using Scripts.FirebaseConfig;
using Scripts.ListEntry;
using UnityEngine.Events;
using System.Linq;
namespace Scripts.UIPanels{
[System.Serializable]
public class SpeakerPanelData
{
    public GameObject Panel;
    public TMP_InputField NameInputField;
    public TMP_InputField ContactInputField;
    public TMP_InputField EmailInputField;
}

// Class to hold speaker data
[System.Serializable]
public class SpeakerData
{
    public string Name;
    public string Contact;
    public string Email;
}
public class CRUDTeamPanel : MonoBehaviour
{
    #region Singleton
    private static CRUDTeamPanel _instance;
    public static CRUDTeamPanel Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("CRUDTeamPanel instance is not initialized.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    [Header("Add Panel Objects")]
    [SerializeField] private Transform addSpeakerPanel;
    [SerializeField] private GameObject speakerPanelPrefab;
    [SerializeField] private Transform addTeamPanel;
    [SerializeField] private Button addTeamBtn;

    //Add Team Panel UI Elements
    [Header("Add Team Panel UI Elements")]
    [SerializeField] private TMP_InputField teamNameInputField;
    [SerializeField] private Button deleteTewamButton;
    [SerializeField] private Button instituteDropdownButton;
    [SerializeField] private AdvancedDropdown institutionDropdown;
    private string selectedInstituteID;
    private SpeakerTypes selectedTeamType;
    [SerializeField] private Toggle teamTypeToggle;
    public List<GameObject> speakerPanels = new List<GameObject>();
    [SerializeField] private Button saveTeamButton;
    private bool teamTypeSelectedFlag = false;

    [Header("Teams Display Panel")]
    [SerializeField] private Transform openTeamsDisplayPanel;
    [SerializeField] private GameObject openTeamEntryPrefab;
    [SerializeField] private Transform noviceTeamsDisplayPanel;
    [SerializeField] private GameObject noviceTeamEntryPrefab;
    [SerializeField] private RectTransform noOpenTeams;
    [SerializeField] private RectTransform noNoviceTeams;
    private bool genearteIDFlag = false;
    private Team selectedTeam;
    public UnityEvent DisableAllTeams;

    void OnEnable()
    {
        DeActivateAddTeamPanel();
        UpdateTeamsList();
    }
    void OnDisable()
    {
        DeActivateAddTeamPanel();
        //destroy all the speaker panels
        foreach (Transform child in openTeamsDisplayPanel)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in noviceTeamsDisplayPanel)
        {
            Destroy(child.gameObject);
        }
    }
    public void AddTeam()
    {  genearteIDFlag = true;
        DeActivateAddTeamPanel();
        DisableAllTeams?.Invoke();  
        DOVirtual.DelayedCall(0.1f, () =>
        {
            ActivateAddTeamPanel();
        });
    }

    private void ActivateAddTeamPanel()
    {
        addTeamBtn.interactable = false;
        addTeamPanel.DOScale(Vector3.one, 0.1f);
        teamNameInputField.interactable = true;
        deleteTewamButton.interactable = true;
        instituteDropdownButton.interactable = true;
        institutionDropdown.onChangedValue += OnInstitutionSelected;
        teamTypeToggle.onSelectOption1 += onOpenSelected;
        teamTypeToggle.onSelectOption2 += OnNoviceSelected;
        teamTypeToggle.Activate();

        if (AppConstants.instance.selectedTouranment.tournamentType == TournamentType.British)
        {
            // Instantiate 2 speaker panels
            for (int i = 0; i < 2; i++)
            {
                GameObject speakerPanel = Instantiate(speakerPanelPrefab, addSpeakerPanel);
                AddSpeakerPanelEntry_IF entry = speakerPanel.GetComponent<AddSpeakerPanelEntry_IF>();
                entry.SetSpeakerNumber(i + 1);
                speakerPanels.Add(speakerPanel);
                Debug.Log($"Speaker Panel {i + 1} instantiated for British tournament.");
            }
        }
        else if (AppConstants.instance.selectedTouranment.tournamentType == TournamentType.Asian)
        {
            // Instantiate 3 speaker panels
            for (int i = 0; i < 3; i++)
            {
                GameObject speakerPanel = Instantiate(speakerPanelPrefab, addSpeakerPanel);
                AddSpeakerPanelEntry_IF entry = speakerPanel.GetComponent<AddSpeakerPanelEntry_IF>();
                entry.SetSpeakerNumber(i + 1);
                speakerPanels.Add(speakerPanel);
                Debug.Log($"Speaker Panel {i + 1} instantiated for Asian tournament.");
            }
        }

        AppConstants.instance.selectedTouranment.instituitionsinTourney.ForEach(institution =>
            {
                institutionDropdown.AddOptions(institution.instituitionAbreviation.ToString());
                Debug.Log("Added institution: " + institution.instituitionAbreviation);
            });
        saveTeamButton.interactable = true;
    }

    public void SaveTeam()
    {
        try
        {
            saveTeamButton.interactable = false;
            Debug.Log("SaveTeam button clicked. Starting to create new team.");

            Team newTeam = new Team();
            newTeam.teamName = teamNameInputField.text;
            newTeam.instituition = selectedInstituteID;
            newTeam.teamCategory = selectedTeamType;
            if(genearteIDFlag)
                newTeam.teamId = AppConstants.instance.GenerateTeamID(newTeam.teamName);
            else
                newTeam.teamId = selectedTeam.teamId;

            Debug.Log($"Team details: Name={newTeam.teamName}, InstitutionID={newTeam.instituition}, Category={newTeam.teamCategory}");

            // Get all Speaker objects from the speaker panels
            List<Speaker> speakers = new List<Speaker>();
            int counter = 0;
            foreach (GameObject speakerPanel in speakerPanels)
            {
                AddSpeakerPanelEntry_IF entry = speakerPanel.GetComponent<AddSpeakerPanelEntry_IF>();
                if (entry != null)
                {
                    Debug.Log("Found AddSpeakerPanelEntry_IF component.");
                    Speaker speaker = entry.GetSpeaker;
                    if(genearteIDFlag)
                        speaker.speakerId = AppConstants.instance.GenerateSpeakerID(speaker, newTeam.teamId, counter);
                    else
                        speaker.speakerId = selectedTeam.speakers[counter].speakerId;
                    speakers.Add(speaker);
                    Debug.Log("Speaker added to the list.");
                }
                else
                {
                    Debug.LogWarning("AddSpeakerPanelEntry_IF component not found on speaker panel.");
                }
                counter++;
            }
            newTeam.speakers = speakers;

            Debug.Log($"Total speakers added: {speakers.Count}");

            if (!ValidateTeamInfoToAdd(newTeam))
            {
                Debug.Log("<color=red>Invalid team data</color>");
                saveTeamButton.interactable = true;
                return;
            }
            genearteIDFlag = false;
            Loading.Instance.ShowLoadingScreen();
            if(genearteIDFlag)
                FirestoreManager.FireInstance.SaveTeamToFireStore(newTeam, OnTeamAddSuccess, OnTeamAddFailure);
            else
                FirestoreManager.FireInstance.UpdateTeamInFireStore(newTeam, OnTeamAddSuccess, OnTeamAddFailure);
        }
        catch (System.Exception e)
        {
            Debug.Log("<color=red>Error saving team:" + e + "</color>");
            saveTeamButton.interactable = true;
        }
    }

    public void ShowTeamInfo(Team team)
    {
        genearteIDFlag = false;
        Debug.Log("Team Selected Info : " + team.teamName);
        Debug.Log("Team Selected Category : " + team.teamCategory);

        DeActivateAddTeamPanel();
        selectedTeam = team;
        DOVirtual.DelayedCall(0.2f, () =>
        {
            ActivateAddTeamPanel();
            DOVirtual.DelayedCall(0.1f, () =>
            {
                teamNameInputField.text = team.teamName;

                // Check if AppConstants.instance is null


                var institution = AppConstants.instance.GetInstituitionsFromID(team.instituition);

                // Get the institution abbreviation
                string instituteAbrv = institution.instituitionAbreviation;
                // string instituteAbrv = AppConstants.instance.GetInstituitionsFromID(team.instituition).instituitionAbreviation;
               
                int index = institutionDropdown.optionsList.FindIndex(x => x.nameText == instituteAbrv);
                 if(institutionDropdown.ratioShrinking == 0)
                    institutionDropdown.ratioShrinking = 1;
                institutionDropdown.SelectOption(index);

                if (team.teamCategory == SpeakerTypes.Open)
                {
                    teamTypeToggle.SelectOption1();;
                }
                else if (team.teamCategory == SpeakerTypes.Novice)
                {
                    teamTypeToggle.SelectOption2();
                }
                Debug.Log("Going To Access Speaker Panel");
                //add speaker info too
                for (int i = 0; i < selectedTeam.speakers.Count; i++)
                {
                    AddSpeakerPanelEntry_IF entry = speakerPanels[i].GetComponent<AddSpeakerPanelEntry_IF>();
                    entry.SetSpeakerInfo(team.speakers[i]);
                }
                addTeamBtn.interactable = true;
                Debug.Log("Team Info Updated");
            });
        });
    }





    #region Private Script Functions

    private void onOpenSelected()
    {
        teamTypeSelectedFlag = true;
        selectedTeamType = SpeakerTypes.Open;
    }
    private void OnNoviceSelected()
    {
        teamTypeSelectedFlag = true;
        selectedTeamType = SpeakerTypes.Novice; // Replace 'Default' with an appropriate value`
    }
    private void OnInstitutionSelected(int id)
    {
        // Assuming AppConstants.selectedTournament.institutesintourney is a list of institution names
        if (id >= 0 && id < AppConstants.instance.selectedTouranment.instituitionsinTourney.Count)
        {
            selectedInstituteID = AppConstants.instance.selectedTouranment.instituitionsinTourney[id].instituitionID;
            Debug.Log("Selected Institution: " + selectedInstituteID);
        }
        else
        {
            Debug.LogError("Invalid institution ID: " + id);
        }
    }
    private bool ValidateTeamInfoToAdd(Team team)
    {
        if (string.IsNullOrEmpty(team.teamName))
        {
            return false;
        }
        if (string.IsNullOrEmpty(team.instituition))
        {
            return false;
        }
        if (!teamTypeSelectedFlag)
        {
            return false;
        }
        foreach (Speaker speaker in team.speakers)
        {
            if (!ValidateSpeakerInfo(speaker))
            {
                return false;
            }
        }
        return true;
    }
    private bool ValidateSpeakerInfo(Speaker speaker)
    {
        if (string.IsNullOrEmpty(speaker.speakerName))
        {
            return false;
        }
        if (string.IsNullOrEmpty(speaker.speakerContact))
        {
            return false;
        }
        if (string.IsNullOrEmpty(speaker.speakerEmail))
        {
            return false;
        }
        return true;
    }
    public void DeActivateAddTeamPanel()
    {
        selectedTeam = null;
        selectedInstituteID = "";
        addTeamBtn.interactable = true;
        teamNameInputField.text = "";
        teamNameInputField.interactable = false;
        deleteTewamButton.interactable = false;
        institutionDropdown.SetDefaultText();
        institutionDropdown.DeleteAllOptions();

        instituteDropdownButton.interactable = false;
        teamTypeToggle.DeActivate();
        foreach (var speakerPanel in speakerPanels)
        {
            Destroy(speakerPanel);
            Debug.Log("Speaker Panel destroyed");
        }
        speakerPanels.Clear();
        saveTeamButton.interactable = false;
        //animate addteam panel to be a lil small less scale
        addTeamPanel.DOScale(Vector3.one * 0.8f, 0.1f);
    }
    private async void OnTeamAddSuccess()
    {
        DialogueBox.Instance.ShowDialogueBox("Team Saved", Color.green);
        DeActivateAddTeamPanel();
        await FirestoreManager.FireInstance.GetAllTeamsFromFirestore(OnAllTeamsUpdatedSuccess, OnAllTeamsUpdatedFailure);
        UpdateTeamsList();
    }
    private void OnTeamAddFailure()
    {
        DialogueBox.Instance.ShowDialogueBox("Failed to save Team", Color.red);
        Loading.Instance.HideLoadingScreen();
        saveTeamButton.interactable = true;
    }

    private void OnAllTeamsUpdatedSuccess()
    {
        Loading.Instance.HideLoadingScreen();
        UpdateTeamsList();
    }
    private async void OnAllTeamsUpdatedFailure()
    {
        await FirestoreManager.FireInstance.GetAllTeamsFromFirestore(OnAllTeamsUpdatedSuccess, OnAllTeamsUpdatedFailure);
        Loading.Instance.HideLoadingScreen();
    }
        private void UpdateTeamsList()
        {
            // Clear existing team list entries
            foreach (Transform child in openTeamsDisplayPanel)
            {
                Destroy(child.gameObject);
            }
            foreach (Transform child in noviceTeamsDisplayPanel)
            {
                Destroy(child.gameObject);
            }

    if (AppConstants.instance == null)
    {
        Debug.Log("AppConstants.instance is null.");
        return;
    }

    if (AppConstants.instance.selectedTouranment == null)
    {
        Debug.Log("AppConstants.instance.selectedTouranment is null.");
        return;
    }

    if (AppConstants.instance.selectedTouranment.teamsInTourney != null)
    {
        Debug.Log("AppConstants.instance.selectedTouranment.teamsInTourney count is: "+ AppConstants.instance.selectedTouranment.teamsInTourney.Count);
    }

    foreach(Team team in AppConstants.instance.selectedTouranment.teamsInTourney)
    {
       if (team == null)
{
    Debug.Log("Team object is null.");
}
else if (team.teamName == null)
{
    Debug.Log("Team name is null for team: " + team.ToString());
}
else
{
    Debug.Log("Team Name: " + team.teamName);
}
    }

    // Sort teams by their names
    var sortedTeams = AppConstants.instance.selectedTouranment.teamsInTourney
        .OrderBy(team => team.teamName)
        .ToList();

            // Iterate through sorted teams and instantiate list entries
            foreach (Team team in sortedTeams)
            {
                if (team.teamCategory == SpeakerTypes.Open)
                {
                    GameObject teamEntry = Instantiate(openTeamEntryPrefab, openTeamsDisplayPanel);
                    TeamListEntry entry = teamEntry.GetComponent<TeamListEntry>();
                    entry.SetTeam(team);
                    noOpenTeams.gameObject.SetActive(false);
                }
                else if (team.teamCategory == SpeakerTypes.Novice)
                {
                    Debug.Log("Novice Team Found");
                    GameObject teamEntry = Instantiate(noviceTeamEntryPrefab, noviceTeamsDisplayPanel);
                    TeamListEntry entry = teamEntry.GetComponent<TeamListEntry>();
                    entry.SetTeam(team);
                    noNoviceTeams.gameObject.SetActive(false);
                }
            }
        }
        #endregion

    }
}