using UnityEngine;
using TMPro;
using Scripts.FirebaseConfig;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using Scripts.ListEntry;


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
    public void SaveInsitute()
    {
        Debug.Log("Adding Institute");
        if (instituteNameInputField.text != "" && institutitionAbreviationInputField.text != "" && !isUpdateFlag)
        {

            if (selectedInstitute == null)
            {
                Debug.Log("Selected Institute is null");
                selectedInstitute = new Instituitions();
            }
            selectedInstitute.instituitionName = instituteNameInputField.text;
            selectedInstitute.instituitionAbreviation = institutitionAbreviationInputField.text;

            selectedInstitute.instituitionID = AppConstants.instance.InstituteIDGenerator(selectedInstitute.instituitionAbreviation);

            FirestoreManager.FireInstance.SaveInstituteToFireStore(selectedInstitute, SaveInstituteOnSuccess, SaveInstituteOnFailed);
            Debug.Log("Institute Added");
        }
        else if (isUpdateFlag)
        {
            selectedInstitute.instituitionName = instituteNameInputField.text;
            selectedInstitute.instituitionAbreviation = institutitionAbreviationInputField.text;
            FirestoreManager.FireInstance.UpdateInstitutionInFireStore(selectedInstitute, SaveInstituteOnSuccess, SaveInstituteOnFailed);
        }
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
        foreach (Instituitions institute in AppConstants.instance.selectedTouranment.instituitionsinTourney)
        {
            GameObject instituteListEntry = Instantiate(instituteListPrefab, instituteListContent);
            if (instituteListEntry.TryGetComponent<InstituteListEntry>(out var instituteListEntryComponent))
                instituteListEntryComponent.SetInstituteData(institute);
            //debug info about the added institute
            Debug.Log("Institute Added: " + institute.instituitionName);
        }
        Debug.Log("Institute List Updated");
    }
    private void UpdateInstituteOnFailed()
    {
        FirestoreManager.FireInstance.GetAllInstituitionsFromFirestore(UpdateInstituteOnSuccess, UpdateInstituteOnFailed);
    }
    private void SaveInstituteOnSuccess()
    {
        isGenerateIDFlag = false;
        DeactiveInstitutePanel();
        RefreshInstituteList();

    }
    private void SaveInstituteOnFailed()
    {
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
        addNewInstituteButton.interactable = false;
        instituteNameInputField.interactable = true;
        institutitionAbreviationInputField.interactable = true;
        // deleteInstituteButton.interactable = true;
    }
}
}