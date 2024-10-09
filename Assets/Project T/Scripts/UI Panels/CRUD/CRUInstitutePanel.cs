using UnityEngine;
using TMPro;
using Scripts.FirebaseConfig;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using Scripts.ListEntry;
using System.Linq;
using System;

namespace Scripts.UIPanels
{
public class CRUInstitutePanel : MonoBehaviour
{
    #region Singleton
    private static CRUInstitutePanel _instance;
    public static CRUInstitutePanel Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("CRUInstitutePanel instance is not initialized.");
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

    [SerializeField] private Transform addInstitutetionPanel;
    [SerializeField] private TMP_InputField instituteNameInputField;
    [SerializeField] private TMP_InputField institutitionAbreviationInputField;
    [SerializeField] private Button saveInstituteButton;
    // [SerializeField] private Button deleteInstituteButton;


    [SerializeField] private Transform instituteListContent;
    [SerializeField] private GameObject instituteListPrefab;
    [SerializeField] private TMP_Text noAdjudicatortxt;


    [SerializeField] private Button addNewInstituteButton;

    public UnityEvent DisableAllSpeakers;

    public bool isGenerateIDFlag = false;
    private bool isUpdateFlag = false;
    private Instituitions selectedInstitute = new Instituitions();

    void OnEnable()
    {
        RefreshInstituteList();
        DeactiveInstitutePanel();
    }
    void OnDisable()
    {
        DeactiveInstitutePanel();
        foreach(Transform child in instituteListContent)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowInstituteData(Instituitions institute)
    {
        isUpdateFlag = true;
        DeactiveInstitutePanel();
        DOVirtual.DelayedCall(0.2f, () =>
        {
            selectedInstitute = institute;
            ActivateInstitutePanel();
            DOVirtual.DelayedCall(0.2f, () =>
            {
                instituteNameInputField.text = institute.instituitionName;
                institutitionAbreviationInputField.text = institute.instituitionAbreviation;
                isGenerateIDFlag = false;
            });
        });
    }

    public void AddNewInstitute()
    {
        isUpdateFlag = false;
        DeactiveInstitutePanel();
        DOVirtual.DelayedCall(0.2f, () =>
        {
            ActivateInstitutePanel();
            isGenerateIDFlag = true;
        });
    }
public void SaveInstitute()
{
    Debug.Log("Adding Institute");
    saveInstituteButton.interactable = false;

    // Input validation
    if (string.IsNullOrEmpty(instituteNameInputField.text) || string.IsNullOrEmpty(institutitionAbreviationInputField.text))
    {
        // Debug.LogError("Institute name or abbreviation is empty.");
        DialogueBox.Instance.ShowDialogueBox("Institute name or abbreviation cannot be empty.", Color.red);
        saveInstituteButton.interactable = true;
        return;
    }

    if (institutitionAbreviationInputField.text.Length > 5)
    {
        // Debug.LogError("Institute abbreviation is too long.");
        DialogueBox.Instance.ShowDialogueBox("Institute abbreviation cannot be more than 5 characters.", Color.red);
        saveInstituteButton.interactable = true;
        return;
    }

    if (!isUpdateFlag)
    {
        if (selectedInstitute == null)
        {
            Debug.Log("Selected Institute is null");
            selectedInstitute = new Instituitions();
        }

        // Format the abbreviation to uppercase
        string abbreviation = institutitionAbreviationInputField.text.ToUpper();

        // Format the institution name
        string institutionName = FormatInstitutionName(instituteNameInputField.text);

        selectedInstitute.instituitionName = institutionName;
        selectedInstitute.instituitionAbreviation = abbreviation;

        selectedInstitute.instituitionID = AppConstants.instance.InstituteIDGenerator(abbreviation);

        Loading.Instance.ShowLoadingScreen();
        FirestoreManager.FireInstance.SaveInstituteToFireStore(selectedInstitute, SaveInstituteOnSuccess, SaveInstituteOnFailed);
        Debug.Log("Institute Added");
    }
    else
    {
        // Format the abbreviation to uppercase
        string abbreviation = institutitionAbreviationInputField.text.ToUpper();

        // Format the institution name
        string institutionName = FormatInstitutionName(instituteNameInputField.text);

        selectedInstitute.instituitionName = institutionName;
        selectedInstitute.instituitionAbreviation = abbreviation;

        FirestoreManager.FireInstance.UpdateInstitutionInFireStore(selectedInstitute, SaveInstituteOnSuccess, SaveInstituteOnFailed);
    }
}


private string FormatInstitutionName(string name)
{
    string[] words = name.Split(' ');
    string[] exceptions = { "of", "the", "and" };
    for (int i = 0; i < words.Length; i++)
    {
        if (Array.Exists(exceptions, e => e.Equals(words[i], StringComparison.OrdinalIgnoreCase)))
        {
            words[i] = words[i].ToLower();
        }
        else
        {
            words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
        }
    }
    return string.Join(" ", words);
}



    public void RefreshInstituteList()
    {
        foreach (Transform child in instituteListContent)
        {
            Destroy(child.gameObject);
        }
        FirestoreManager.FireInstance.GetAllInstituitionsFromFirestore(UpdateInstituteOnSuccess, UpdateInstituteOnFailed);
    }

    private void UpdateInstituteOnSuccess()
    {
        Loading.Instance.HideLoadingScreen();
        noAdjudicatortxt.gameObject.SetActive(false);
    
        // Sort institutes by their names
        var sortedInstitutes = AppConstants.instance.selectedTouranment.instituitionsinTourney
            .OrderBy(institute => institute.instituitionName)
            .ToList();
    
        // Iterate through sorted institutes and instantiate list entries
        foreach (Instituitions institute in sortedInstitutes)
        {
            GameObject instituteListEntry = Instantiate(instituteListPrefab, instituteListContent);
            if (instituteListEntry.TryGetComponent<InstituteListEntry>(out var instituteListEntryComponent))
            {
                instituteListEntryComponent.SetInstituteData(institute);
            }
            // Debug info about the added institute
            Debug.Log("Institute Added: " + institute.instituitionName);
        }
    
        DialogueBox.Instance.ShowDialogueBox("Institute List Updated", Color.green);
    }
    private void UpdateInstituteOnFailed()
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Failed to update Institute List", Color.red);
        FirestoreManager.FireInstance.GetAllInstituitionsFromFirestore(UpdateInstituteOnSuccess, UpdateInstituteOnFailed);
    }
    private void SaveInstituteOnSuccess()
    {
        Loading.Instance.HideLoadingScreen();
        DialogueBox.Instance.ShowDialogueBox("Institute Saved", Color.green);
        isGenerateIDFlag = false;
        DeactiveInstitutePanel();
        RefreshInstituteList();
        
    }
    private void SaveInstituteOnFailed()
    {
        Loading.Instance.HideLoadingScreen();
        saveInstituteButton.interactable = true;
        DialogueBox.Instance.ShowDialogueBox("Failed to save Institute", Color.red);
        Debug.Log("Failed to save institute");
    }
    public void DeactiveInstitutePanel()
    {
        selectedInstitute = null;
        addInstitutetionPanel.DOScale(Vector3.one * 0.8f, 0.2f);
        instituteNameInputField.text = "NAME";
        institutitionAbreviationInputField.text = "";
        saveInstituteButton.interactable = false;
        addNewInstituteButton.interactable = true;
        instituteNameInputField.interactable = false;
        institutitionAbreviationInputField.interactable = false;
        // deleteInstituteButton.interactable = false;
    }
    private void ActivateInstitutePanel()
    {
        addInstitutetionPanel.DOScale(Vector3.one, 0.2f);
        instituteNameInputField.text = "";
        institutitionAbreviationInputField.text = "";
        saveInstituteButton.interactable = true;
        // addNewInstituteButton.interactable = false;
        instituteNameInputField.interactable = true;
        institutitionAbreviationInputField.interactable = true;
        // deleteInstituteButton.interactable = true;
    }
}
}