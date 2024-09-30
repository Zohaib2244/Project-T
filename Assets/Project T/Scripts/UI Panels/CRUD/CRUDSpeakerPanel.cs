using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using Scripts.Resources;
using Scripts.FirebaseConfig;
using UnityEngine.Events;
using Scripts.ListEntry;
namespace Scripts.UIPanels
{public class CRUDSpeakerPanel : MonoBehaviour
{
    #region Singleton
    private static CRUDSpeakerPanel _instance;
    public static CRUDSpeakerPanel Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("CRUDSpeakerPanel instance is not initialized.");
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
    
    #region Variables
    private Team selectedTeam;
    private Speaker selectedSpeaker;
    private int selectedSpeakerIndex;

    private Team replacementTeam;
    private Speaker replacementSpeaker;

    [Header("Panel Info")]
    [SerializeField] private Transform openSpeakerPanel;
    [SerializeField] private Transform noviceSpeakerPanel;
    [SerializeField] private Transform openSpeakerListContent;
    [SerializeField] private Transform noviceSpeakerListContent;
    [SerializeField] private GameObject speakerListEntryPrefab;

    [Header("Speaker Panel")]
    [SerializeField] private Transform speakerPanel;
    [SerializeField] private TMP_InputField speakerName;
    [SerializeField] private TMP_Text speakerTeamName;
    [SerializeField] private TMP_InputField speakerPhone;
    [SerializeField] private TMP_InputField speakerEmail;


    [SerializeField] private Image openCatImg;
    [SerializeField] private Image noviceCatImg;
    [SerializeField] private Button saveBtn;
    [SerializeField] private Button replaceSpeakerBtn;

    public bool isReplaceModeOn;
    public UnityEvent OnSpeakerListEntryDeselect;
    public UnityEvent OnReplaceModeEnable;
    public UnityEvent OnReplaceModeDisable;
    public UnityEvent DisableAllReplaceable;
    #endregion
    private void OnEnable()
    {
        UpdateSpeaekrsList();
        DeActivateSpeakerPanel();
    }
    private void OnDisable()
    {
        DeActivateSpeakerPanel();
        DisableReplaceMode();
    }

    #region Feature Functions
    public void ShowSelectedSpeakerInfo(Team team, int indexSpeaker)
    {
        DOVirtual.DelayedCall(0, () => DeActivateSpeakerPanel()).OnComplete(() =>
        {
            DOVirtual.DelayedCall(0.2f, () => ActivateSpeakerPanel()).OnComplete(() =>
            {
                DOVirtual.DelayedCall(0.2f, () =>
                {
                    selectedTeam = team;
                    selectedSpeakerIndex = indexSpeaker;
                    selectedSpeaker = team.speakers[selectedSpeakerIndex];

                    speakerName.text = selectedSpeaker.speakerName.ToString();
                    speakerPhone.text = selectedSpeaker.speakerPhone.ToString();
                    speakerEmail.text = selectedSpeaker.speakerEmail.ToString();
                    speakerTeamName.text = selectedTeam.teamName.ToString();

                    if (selectedTeam.teamCategory == SpeakerTypes.Novice)
                    {
                        openCatImg.gameObject.SetActive(false);
                        noviceCatImg.gameObject.SetActive(true);
                        noviceCatImg.transform.DOScale(Vector3.one * 1.2f, 0.2f).OnComplete(() =>
                        {
                            noviceCatImg.transform.DOScale(Vector3.one, 0.2f);
                        });
                    }
                    else
                    {
                        noviceCatImg.gameObject.SetActive(false);
                        openCatImg.gameObject.SetActive(true);
                        openCatImg.transform.DOScale(Vector3.one * 1.2f, 0.2f).OnComplete(() =>
                        {
                            openCatImg.transform.DOScale(Vector3.one, 0.2f);
                        });

                    }
                });
            });
        });
    }

    public void ReplaceSpeakerWith(Team team, int indexSpeaker)
    {
        replacementTeam = null;
        replacementSpeaker = null;
        replacementTeam = team;
        replacementSpeaker = team.speakers[indexSpeaker];

    }
    private void UpdateSpeaekrsList()
    {
        foreach (Transform child in openSpeakerListContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in noviceSpeakerListContent)
        {
            Destroy(child.gameObject);
        }
        AppConstants.instance.selectedTouranment.teamsInTourney.ForEach(team =>
        {
            for (int i = 0; i < team.speakers.Count; i++)
            {
                var speaker = team.speakers[i];
                if (team.teamCategory == SpeakerTypes.Novice)
                {
                    GameObject speakerObj = Instantiate(speakerListEntryPrefab, noviceSpeakerListContent);
                    speakerObj.GetComponent<SpeakerPanelSpeakerListEntry>().SetSpeakerListEntry(team, i);
                }
                else if (team.teamCategory == SpeakerTypes.Open)
                {
                    GameObject speakerObj = Instantiate(speakerListEntryPrefab, openSpeakerListContent);
                    speakerObj.GetComponent<SpeakerPanelSpeakerListEntry>().SetSpeakerListEntry(team, i);
                }
            }
        });
    }
    #endregion

    #region Button Click Events
    public void SaveBtnClick()
    {
        saveBtn.interactable = false;
        replaceSpeakerBtn.interactable = false;
        if (!isReplaceModeOn)
        {
            selectedSpeaker.speakerName = speakerName.text;
            selectedSpeaker.speakerPhone = speakerPhone.text;
            selectedSpeaker.speakerEmail = speakerEmail.text;
            Debug.Log("Selected Speaker: " + selectedSpeaker.speakerId);
            Debug.Log("Selected Team: " + selectedTeam.teamId);
            FirestoreManager.FireInstance.UpdateSpeakerInFireStore(selectedSpeaker, selectedTeam.teamId, OnUpdateSpeakerSuccess, OnUpdateSpeakerFailed);
        }
        else if (isReplaceModeOn)
        {
            FirestoreManager.FireInstance.ReplaceSpeakersInFirestore(selectedSpeaker, replacementSpeaker, selectedTeam.teamId, replacementTeam.teamId, OnSpeakerReplaceSuccess, OnSpeakerReplaceFailed);
        }

    }
    public void ReplaceBtnClick()
    {
        isReplaceModeOn = !isReplaceModeOn;
        if (isReplaceModeOn)
        {
            EnableReplaceMode();
        }
        else
        {
            DisableReplaceMode();
        }
    }

    #endregion
    #region Utility Functions
    private void EnableReplaceMode()
    {   
        OnReplaceModeEnable?.Invoke();
        speakerPanel.GetComponent<Image>().color = new Color32(252, 218, 255, 255); // FCDAFF in RGBA
        speakerPanel.DOScale(Vector3.one * 1.2f, 0.2f);
        openSpeakerPanel.DOScale(Vector3.one * 1.05f, 0.2f);
        noviceSpeakerPanel.DOScale(Vector3.one * 1.05f, 0.2f);
        isReplaceModeOn = true;
        replaceSpeakerBtn.transform.DOScale(Vector3.one * 0.8f, 0.2f);
        speakerName.interactable = false;
        speakerPhone.interactable = false;
        speakerEmail.interactable = false;
    }
    private void DisableReplaceMode()
    {
        OnReplaceModeDisable?.Invoke();
        speakerPanel.GetComponent<Image>().color = new Color32(255, 255, 255, 255); // FCDAFF in RGBA
        speakerPanel.DOScale(Vector3.one, 0.2f);
        openSpeakerPanel.DOScale(Vector3.one, 0.2f);
        noviceSpeakerPanel.DOScale(Vector3.one, 0.2f);
        isReplaceModeOn = false;
        replaceSpeakerBtn.transform.DOScale(Vector3.one, 0.2f);
        speakerName.interactable = true;
        speakerPhone.interactable = true;
        speakerEmail.interactable = true;
        replacementSpeaker = null;
        replacementTeam = null;
    }
    private void ActivateSpeakerPanel()
    {
        speakerPanel.DOScale(Vector3.one, 0.2f);
        speakerName.placeholder.GetComponent<TMP_Text>().text = "";
        speakerPhone.placeholder.GetComponent<TMP_Text>().text = "";
        speakerEmail.placeholder.GetComponent<TMP_Text>().text = "";
        speakerTeamName.text = "";

        speakerName.interactable = true;
        speakerPhone.interactable = true;
        speakerEmail.interactable = true;
        saveBtn.interactable = true;
        replaceSpeakerBtn.interactable = true;
    }
    private void DeActivateSpeakerPanel()
    {
        isReplaceModeOn = false;
        speakerPanel.DOScale(Vector3.one * 0.8f, 0.2f);
        speakerName.text = "";
        speakerPhone.text = "";
        speakerEmail.text = "";
        speakerTeamName.text = "";

        speakerName.interactable = false;
        speakerPhone.interactable = false;
        speakerEmail.interactable = false;
        replaceSpeakerBtn.interactable = false;
        saveBtn.interactable = false;
        selectedTeam = null;
        selectedSpeaker = null;

        openCatImg.gameObject.SetActive(false);
        noviceCatImg.gameObject.SetActive(false);
    }


    #endregion

    #region Success & Fail Events
    private void OnSpeakerReplaceSuccess()
    {
        DeActivateSpeakerPanel();
        FirestoreManager.FireInstance.GetAllTeamsFromFirestore(OnGetAllTeamsSuccess, OnGetAllTeamsFailed);
    }
    private void OnSpeakerReplaceFailed()
    {
        Debug.Log("<color = red> Failed to replace speaker</color>");
    }
    private void OnGetAllTeamsSuccess()
    {
        UpdateSpeaekrsList();
    }
    private void OnGetAllTeamsFailed()
    {
        Debug.Log("<color = red> Failed to get all teams</color>");
    }
    private void OnUpdateSpeakerSuccess()
    {
        DeActivateSpeakerPanel();
    }
    private void OnUpdateSpeakerFailed()
    {
        Debug.Log("<color = red> Failed to update speaker</color>");
    }

    #endregion
}
}