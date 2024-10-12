using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scripts.UI;
using Scripts.Resources;
using Scripts.FirebaseConfig;
using Scripts.ListEntry;
using TMPro;

namespace Scripts.UIPanels
{
public class TournamentSelectionPanel : MonoBehaviour
{
    [SerializeField] private Button createTournamentBtn;
    [SerializeField] private Button britishParliamentBtn;
    [SerializeField] private Button asianParliamentBtn;
    [SerializeField] private TMP_Text tournamentName_1;
    [SerializeField] private TMP_Text tournamentName_2;
    [SerializeField] private TMP_Text tournamentType_1;
    [SerializeField] private TMP_Text tournamentType_2;
    void OnEnable()
    {
        createTournamentBtn.interactable = false;
        britishParliamentBtn.interactable = false;
        asianParliamentBtn.interactable = false;
        FirestoreManager.FireInstance.UpdateTournamentsFromFirestore(UpdateUI, UpdateTournamentOnFail);
    }

    public void CreateTournament()
    {
        MainUIManager.Instance.SwitchPanel(Panels.TournamentInfoPanel);
    }
    public void OpenTournamentHome(int tournamentTypeindex)
    {
        //convertt int to enum
        TournamentType tournamentType = (TournamentType)tournamentTypeindex;
        if (tournamentType == TournamentType.British)
        {
            AppConstants.instance.selectedTouranment = AppConstants.instance.tournaments[0];
            MainUIManager.Instance.SwitchPanel(Panels.TournamentHomePanel);
        }
        else if (tournamentType == TournamentType.Asian)
        {
            AppConstants.instance.selectedTouranment = AppConstants.instance.tournaments[1];
            MainUIManager.Instance.SwitchPanel(Panels.TournamentHomePanel);
        }
    }
    private void UpdateTournamentOnFail()
    {
        createTournamentBtn.interactable = true;
    }
    private void UpdateUI()
    {
        if (!AppConstants.instance.CheckTournamentExists())
        {
            Debug.Log("Tournaments list is null");
            createTournamentBtn.interactable = true;
            britishParliamentBtn.interactable = false;
            asianParliamentBtn.interactable = false;
        }
        else
        {
            if(britishParliamentBtn.TryGetComponent(out TournamentInfoCard britishTournamentCard))
            {
                britishTournamentCard.SetTournamentInfo();
            }
            if(asianParliamentBtn.TryGetComponent(out TournamentInfoCard asianTournamentCard))
            {
                asianTournamentCard.SetTournamentInfo();
            }
            if(AppConstants.instance.tournaments.Count == 0)
            {
                createTournamentBtn.interactable = true;
                britishParliamentBtn.interactable = false;
                asianParliamentBtn.interactable = false;
            }
            else if(AppConstants.instance.tournaments.Count == 1 )
            {
                createTournamentBtn.interactable = true;
                britishParliamentBtn.interactable = true;
                asianParliamentBtn.interactable = false;
                tournamentName_1.text = AppConstants.instance.tournaments[0].tournamentName;
                tournamentType_1.text = AppConstants.instance.tournaments[0].tournamentType.ToString();
            }
            else if(AppConstants.instance.tournaments.Count == 2)
            {
                createTournamentBtn.interactable = false;
                britishParliamentBtn.interactable = true;
                asianParliamentBtn.interactable = true;
                tournamentName_1.text = AppConstants.instance.tournaments[0].tournamentName;
                tournamentType_1.text = AppConstants.instance.tournaments[0].tournamentType.ToString();
            }
            }
    }
}
}