using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Resources;
using Scripts.UI;
using Scripts.FirebaseConfig;
using System.Threading.Tasks;
namespace Scripts.UIPanels
{
public class TournamentHomePanel : MonoBehaviour
{

    #region Private Variables
    [SerializeField] private TMP_Text _tournamentName;
    [SerializeField] private Toggle tournamentSelection;
    [SerializeField] private TMP_Text adminName;
    [SerializeField] private TMP_Text adminCategory;
    [SerializeField] private Button roundsPanelBtnl;
    #endregion

    #region Public Variables
    #endregion

    private async void OnEnable()
    {
        ConfigureTournament();
        UpdateAdminInfo();
        Loading.Instance.ShowLoadingScreen();
    
        // Run Firestore calls in parallel
        var institutionsTask = FirestoreManager.FireInstance.GetAllInstituitionsFromFirestore();
        var adjudicatorsTask = FirestoreManager.FireInstance.GetAllAdjudicatorsFromFirestore();
        var roundsTask = AppConstants.instance.GetAllRounds();
        var teamsTask = FirestoreManager.FireInstance.GetAllTeamsFromFirestore();
    
        // Await all tasks to complete
        await Task.WhenAll(institutionsTask, adjudicatorsTask, roundsTask, teamsTask);
        tournamentSelection.onSelectOption1 = ShowTournament1Info;
        tournamentSelection.onSelectOption2 = ShowTournament2Info;
        Loading.Instance.HideLoadingScreen();
    }

    public void ShowTournament1Info()
    {
        AppConstants.instance.selectedTouranment = AppConstants.instance.tournaments[0];
         _tournamentName.text = AppConstants.instance.selectedTouranment.tournamentName;
         Debug.Log("Tournament 1 selected");
        // ConfigureTournament();
    }
    public void ShowTournament2Info()
    {
        AppConstants.instance.selectedTouranment = AppConstants.instance.tournaments[1];
        _tournamentName.text = AppConstants.instance.selectedTouranment.tournamentName;
        Debug.Log("Tournament 2 selected");
        // ConfigureTournament();
    }


    private void ConfigureTournament()
    {
        // _tournamentName.text = AppConstants.instance.selectedTouranment.tournamentName;
        if(AppConstants.instance.tournaments[0].tournamentId == AppConstants.instance.selectedTouranment.tournamentId)
        {
            tournamentSelection.SelectOption1();
        }
        else
        {
            tournamentSelection.SelectOption2();
        }
    }

    public void SwitchTournament(int tournamentTypeindex)
    {
        TournamentType tournamentType = (TournamentType)tournamentTypeindex;
        if (tournamentType == TournamentType.British)
        {
            AppConstants.instance.selectedTouranment = AppConstants.instance.tournaments.Find(t => t.tournamentId == "b01");
            ConfigureTournament();
        }
        else if (tournamentType == TournamentType.Asian)
        {
            AppConstants.instance.selectedTouranment = AppConstants.instance.tournaments.Find(t => t.tournamentId == "a01");
            ConfigureTournament();
        }
    }

    public void UpdateAdminInfo()
    {
        try
        {
            adminName.text = AppConstants.instance.selectedAdmin.Name.ToString();
            adminCategory.text = AppConstants.instance.selectedAdmin.AdminCategory.ToString();

            if(AppConstants.instance.selectedAdmin.AdminCategory == AdminCategories.Super)
            {
                roundsPanelBtnl.interactable = true;
            }
            else
            {
                roundsPanelBtnl.interactable = false;
            }
        }
        catch (System.Exception)
        {
            if (AppConstants.instance.selectedAdmin == null)
            {
                Debug.Log("Admin is null");
            }
        }
    }
    #region Panel Switchers
    public void OpenHomePanel()
    {
        MainUIManager.Instance.SwitchPanel(Panels.TournamentHomePanel);
    }
    public void OpenTeamsPanel()
    {
        MainUIManager.Instance.SwitchPanel(Panels.CRUDTeamPanel);
    }
    public void OpenInstitutionsPanel()
    {
        MainUIManager.Instance.SwitchPanel(Panels.CRUInstitutionPanel);
    }
    public void OpenSPeakersPanel()
    {
        MainUIManager.Instance.SwitchPanel(Panels.CRUDSpeakerPanel);
    }
    public void OpenAdjudicator()
    {
        MainUIManager.Instance.SwitchPanel(Panels.CRUDAdjudicatorPanel);
    }
    public void OpenRoundsPanel()
    {
        MainUIManager.Instance.SwitchPanel(Panels.RoundsPanel);
    }
    public void OpenRankingsPanel()
    {
        MainUIManager.Instance.SwitchPanel(Panels.RankingsPanel);
    }
    #endregion
}
}