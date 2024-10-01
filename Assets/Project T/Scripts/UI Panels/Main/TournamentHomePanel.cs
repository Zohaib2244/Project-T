using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Scripts.Resources;
using Scripts.UI;
using Scripts.FirebaseConfig;
namespace Scripts.UIPanels
{
public class TournamentHomePanel : MonoBehaviour
{

    #region Private Variables
    [SerializeField] private TMP_Text _tournamentName;
    [SerializeField] private Toggle tournamentCategory;
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
        FirestoreManager.FireInstance.GetAllInstituitionsFromFirestore();
        FirestoreManager.FireInstance.GetAllAdjudicatorsFromFirestore();
        AppConstants.instance.GetAllRounds();
        await FirestoreManager.FireInstance.GetAllTeamsFromFirestore();
    }
    private void ConfigureTournament()
    {
        _tournamentName.text = AppConstants.instance.selectedTouranment.tournamentName;
        if (AppConstants.instance.selectedTouranment.tournamentType == TournamentType.British)
        {
            tournamentCategory.SelectOption1();
        }
        else if (AppConstants.instance.selectedTouranment.tournamentType == TournamentType.Asian)
        {
            tournamentCategory.SelectOption2();
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
}
}