using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Scripts.Resources;
using UnityEngine.Events;
using Scripts.FirebaseConfig;
using Scripts.ListEntry;
using System.Linq;

namespace Scripts.UIPanels{
public class CRUDAdjudicatorPanel : MonoBehaviour
{
    #region Singleton
    private static CRUDAdjudicatorPanel _instance;
    public static CRUDAdjudicatorPanel Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("CRUDAdjudicatorPanel instance is not initialized.");
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
    [Header("Panel List")]
    [SerializeField] private Transform adjudicatorListContent;
    [SerializeField] private Transform capListContent;
    [SerializeField] private GameObject adjudicatorListPrefab;
    [SerializeField] private Button addTeamBtn;
    [SerializeField] private RectTransform noAdjText;
    [SerializeField] private RectTransform noCapText;


    [Header("Adjudicator Panel")]
    [SerializeField] private Transform addAdjudicatorPanel;
    [SerializeField] private TMP_InputField adjudicatorNameInputField;
    [SerializeField] private TMP_InputField adjudicatorContactInputField;
    [SerializeField] private TMP_InputField adjudicatorEmailInputField;
    [SerializeField] private Toggle adjudicatorTypeToggle;
    [SerializeField] private Button institutionDropdownBtn;
    [SerializeField] private AdvancedDropdown institutionDropdown;
    [SerializeField] private Button saveBtn;
    [SerializeField] private Button deleteBtn;

    private Adjudicator selectedAdjudicator;
    private Instituitions selectedAdjudicatorInstitution;
    private AdjudicatorTypes selectedAdjudicatorType;
    private bool adjudicatorTypeSelected = false;
    private bool genearteIDFlag = false;
    public UnityEvent DisableAllAdjudicators = new UnityEvent();

    void OnEnable()
    {
        institutionDropdown.onChangedValue += OnInstitutionSelected;
        adjudicatorTypeToggle.onSelectOption1 += CapAdjTypeSelected;
        adjudicatorTypeToggle.onSelectOption2 += NormieAdjTypeSelected;
        DeactivateAdjudicatorPanel();
        UpdateTeamsList();
        
    }
    void OnDisable()
    {
        DeactivateAdjudicatorPanel();
        institutionDropdown.onChangedValue -= OnInstitutionSelected;
        adjudicatorTypeToggle.onSelectOption1 -= CapAdjTypeSelected;
        adjudicatorTypeToggle.onSelectOption2 -= NormieAdjTypeSelected;
        //destroy list entries
        foreach (Transform child in capListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in adjudicatorListContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void AddNewAdjudicator()
    {
        DisableAllAdjudicators?.Invoke();
        genearteIDFlag = true;
        selectedAdjudicator = new Adjudicator();
        DOVirtual.DelayedCall(0.2f, () =>
        {
            DeactivateAdjudicatorPanel();
            DOVirtual.DelayedCall(0.2f, () =>
            {
                ActivacteAdjudicatorPanel();
            });
        });
    }
    private void CapAdjTypeSelected()
    {
        Debug.Log("CAP selected");
        selectedAdjudicator.adjudicatorType = AdjudicatorTypes.CAP;
        adjudicatorTypeSelected = true;
    }
    private void NormieAdjTypeSelected()
    {
        Debug.Log("Normie selected");
        selectedAdjudicator.adjudicatorType = AdjudicatorTypes.Normie;
        adjudicatorTypeSelected = true;
    }
    public void SaveAdjudicator()
    {
        try
        {
            saveBtn.interactable = false;
            Debug.Log("Adjudicator Type: " + selectedAdjudicator.adjudicatorType);
            selectedAdjudicator.adjudicatorName = adjudicatorNameInputField.text;
            selectedAdjudicator.adjudicatorPhone = adjudicatorContactInputField.text;
            selectedAdjudicator.adjudicatorEmail = adjudicatorEmailInputField.text;
            selectedAdjudicator.instituitionID = selectedAdjudicatorInstitution.instituitionID;
            if (genearteIDFlag)
            {
                Debug.Log("Generating ID for adjudicator: " + selectedAdjudicator.adjudicatorName);
                selectedAdjudicator.adjudicatorID = AppConstants.instance.GenerateAdjudicatorID(selectedAdjudicator.adjudicatorName, selectedAdjudicator.instituitionID);
                if (!ValidateAdjInfo())
                {
                    DialogueBox.Instance.ShowDialogueBox("Invalid adjudicator info", Color.red);
                    saveBtn.interactable = true;
                    return;
                }
                genearteIDFlag = false;
                Loading.Instance.ShowLoadingScreen();
                FirestoreManager.FireInstance.SaveAdjudicatorToFireStore(selectedAdjudicator, OnAddAdjudicatorSuccess, OnAddAdjudicatorFailure);
            }
            else
            {
                if (!ValidateAdjInfo())
                {
                    DialogueBox.Instance.ShowDialogueBox("Invalid adjudicator info", Color.red);
                    saveBtn.interactable = true;
                    return;
                }
                Debug.Log("Adjudicator Type: " + selectedAdjudicator.adjudicatorType);
                Loading.Instance.ShowLoadingScreen();
                FirestoreManager.FireInstance.UpdateAdjudicatorInFireStore(selectedAdjudicator, OnAdjudicatorUpdateSuccess, OnAdjudicatorUpdateFailure);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("<color=red>Error saving team:" + e + "</color>");
            saveBtn.interactable = true;
        }
    }
    private bool ValidateAdjInfo()
    {
        if (string.IsNullOrEmpty(selectedAdjudicator.adjudicatorName))
        {
            Debug.Log("<color=red>Invalid adjudicator name</color>");
            DialogueBox.Instance.ShowDialogueBox("Invalid adjudicator name", Color.red);
            return false;
        }
        else if (string.IsNullOrEmpty(selectedAdjudicator.adjudicatorPhone))
        {
            Debug.Log("<color=red>Invalid adjudicator phone</color>");
            DialogueBox.Instance.ShowDialogueBox("Invalid adjudicator phone", Color.red);
            return false;
        }
        else if (!selectedAdjudicator.adjudicatorPhone.StartsWith("0"))
        {
            Debug.Log("<color=red>Adjudicator phone must start with 0</color>");
            DialogueBox.Instance.ShowDialogueBox("Adjudicator phone must start with 0", Color.red);
            return false;
        }
        else if (string.IsNullOrEmpty(selectedAdjudicator.adjudicatorEmail))
        {
            Debug.Log("<color=red>Invalid adjudicator email</color>");
            DialogueBox.Instance.ShowDialogueBox("Invalid adjudicator email", Color.red);
            return false;
        }
        else if (!selectedAdjudicator.adjudicatorEmail.Contains("@"))
        {
            Debug.Log("<color=red>Invalid adjudicator email format</color>");
            DialogueBox.Instance.ShowDialogueBox("Invalid adjudicator email format", Color.red);
            return false;
        }
        else if (!adjudicatorTypeSelected)
        {
            Debug.Log("<color=red>Invalid adjudicator type</color>");
            DialogueBox.Instance.ShowDialogueBox("Invalid adjudicator type", Color.red);
            return false;
        }
        else if (selectedAdjudicatorInstitution == null)
        {
            Debug.Log("<color=red>Invalid adjudicator institution</color>");
            DialogueBox.Instance.ShowDialogueBox("Invalid adjudicator institution", Color.red);
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ShowAdjudicatorInfo(Adjudicator adj, Instituitions inst)
    {   selectedAdjudicatorInstitution = null;
        selectedAdjudicatorInstitution = inst;
        selectedAdjudicator = null;
        selectedAdjudicator = adj;
        Debug.Log("Selected Adjudicator Type: " + selectedAdjudicator.adjudicatorType);
        DeactivateAdjudicatorPanel();
        DOVirtual.DelayedCall(0.2f, () =>
        {
            ActivacteAdjudicatorPanel();
            DOVirtual.DelayedCall(0.2f, () =>
           {
               adjudicatorNameInputField.text = selectedAdjudicator.adjudicatorName;
               adjudicatorContactInputField.text = selectedAdjudicator.adjudicatorPhone;
               adjudicatorEmailInputField.text = selectedAdjudicator.adjudicatorEmail;
               genearteIDFlag = false;
               if (selectedAdjudicator.adjudicatorType == AdjudicatorTypes.CAP)
               {
                   adjudicatorTypeToggle.SelectOption1();
               }
               else if (selectedAdjudicator.adjudicatorType == AdjudicatorTypes.Normie)
               {
                   adjudicatorTypeToggle.SelectOption2();
               }
                // string instituteAbrv = AppConstants.instance.GetInstituitionsFromID(team.instituition).instituitionAbreviation;
                int index = institutionDropdown.optionsList.FindIndex(x => x.nameText == selectedAdjudicatorInstitution.instituitionAbreviation);
                Debug.Log("Institution index: " + index);
                if(institutionDropdown.ratioShrinking == 0)
                    institutionDropdown.ratioShrinking = 1;
                institutionDropdown.SelectOption(index);
           });
        });
    }
    private void OnInstitutionSelected(int id)
    {
        // Assuming AppConstants.selectedTournament.institutesintourney is a list of institution names
        if (id >= 0 && id < AppConstants.instance.selectedTouranment.instituitionsinTourney.Count)
        {
            selectedAdjudicatorInstitution = AppConstants.instance.selectedTouranment.instituitionsinTourney[id];
        }
        else
        {
            Debug.LogError("Invalid institution ID: " + id);
        }
    }










    //---------------------------------------------------------------------------------------------------
    private void ActivacteAdjudicatorPanel()
    {
        adjudicatorTypeSelected = false;
        addAdjudicatorPanel.DOScale(Vector3.one, 0.2f);
        adjudicatorNameInputField.interactable = true;
        adjudicatorContactInputField.interactable = true;
        adjudicatorEmailInputField.interactable = true;
        saveBtn.interactable = true;
        deleteBtn.interactable = true;
        adjudicatorContactInputField.text = "";
        adjudicatorEmailInputField.text = "";
        adjudicatorNameInputField.text = "";
        institutionDropdownBtn.interactable = true;

        AppConstants.instance.selectedTouranment.instituitionsinTourney.ForEach(institution =>
            {
                institutionDropdown.AddOptions(institution.instituitionAbreviation.ToString());
                Debug.Log("Added institution: " + institution.instituitionAbreviation);
            });
        adjudicatorTypeToggle.Activate();
    }
    public void DeactivateAdjudicatorPanel()
    {
        adjudicatorTypeSelected = false;
        addAdjudicatorPanel.DOScale(Vector3.one * 0.8f, 0.2f);
        adjudicatorTypeToggle.DeActivate();
        saveBtn.interactable = false;
        deleteBtn.interactable = false;
        adjudicatorContactInputField.text = "";
        adjudicatorEmailInputField.text = "";
        adjudicatorNameInputField.text = "";
        adjudicatorNameInputField.interactable = false;
        adjudicatorContactInputField.interactable = false;
        adjudicatorEmailInputField.interactable = false;
        institutionDropdownBtn.interactable = false;
        institutionDropdown.DeleteAllOptions();
        institutionDropdown.SetDefaultText();
    }
    private void UpdateTeamsList()
    {
        Debug.Log("Updating adjudicator list");
    
        // Clear existing adjudicator list entries
        foreach (Transform child in capListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in adjudicatorListContent)
        {
            Destroy(child.gameObject);
        }
    
        // Sort adjudicators by their names
        var sortedAdjudicators = AppConstants.instance.selectedTouranment.adjudicatorsInTourney
            .OrderBy(adjudicator => adjudicator.adjudicatorName)
            .ToList();
    
        // Iterate through sorted adjudicators and instantiate list entries
        foreach (Adjudicator adjudicator in sortedAdjudicators)
        {
            if (adjudicator.adjudicatorType == AdjudicatorTypes.CAP)
            {
                GameObject adjudicatorEntry = Instantiate(adjudicatorListPrefab, capListContent);
                AdjudicatorListEntry entry = adjudicatorEntry.GetComponent<AdjudicatorListEntry>();
                entry.SetAdjudicator(adjudicator);
                noCapText.gameObject.SetActive(false);
            }
            else if (adjudicator.adjudicatorType == AdjudicatorTypes.Normie)
            {
                GameObject adjudicatorEntry = Instantiate(adjudicatorListPrefab, adjudicatorListContent);
                AdjudicatorListEntry entry = adjudicatorEntry.GetComponent<AdjudicatorListEntry>();
                entry.SetAdjudicator(adjudicator);
                noAdjText.gameObject.SetActive(false);
            }
        }
    }
    #region Callbacks
    private void OnGetAllAdjudicatorsSuccess()
    {
        UpdateTeamsList();
    }
    private void OnGetAllAdjudicatorsFailure()
    {
        Debug.LogError("Error getting adjudicators: ");
    }
    private async void OnAddAdjudicatorSuccess()
    {
        Loading.Instance.HideLoadingScreen();
        DeactivateAdjudicatorPanel();
        await FirestoreManager.FireInstance.GetAllAdjudicatorsFromFirestore(OnGetAllAdjudicatorsSuccess, OnGetAllAdjudicatorsFailure);
    }
    private void OnAddAdjudicatorFailure()
    {
        DialogueBox.Instance.ShowDialogueBox("Failed to save adjudicator", Color.red);
        Loading.Instance.HideLoadingScreen();
        saveBtn.interactable = true;
    }
    private async void OnAdjudicatorUpdateSuccess()
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Adjudicator updated", Color.green);
        await FirestoreManager.FireInstance.GetAllAdjudicatorsFromFirestore(OnGetAllAdjudicatorsSuccess, OnGetAllAdjudicatorsFailure);
  
    }
    private void OnAdjudicatorUpdateFailure()
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Failed to update adjudicator", Color.red);
        saveBtn.interactable = true;
        // Debug.LogError("Error updating adjudicator: ");
    }
    #endregion
}
}