using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Scripts.Resources;
using UnityEngine.UI;
using System.Linq;

public class Breaks_EditPanel : MonoBehaviour
{
    #region Singleton
    public static Breaks_EditPanel Instance;
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

    #region Variables
    [Header("Side Panel")]
    [SerializeField] private Transform sidePanel;
    [SerializeField] private TMP_Text selectedOpenTeams;
    [SerializeField] private TMP_Text selectedNoviceTeams;
    [SerializeField] private TMP_Text totalNoviceTeams;
    [SerializeField] private TMP_Text totalOpenTeams;
    [SerializeField] private ButtonSelect breakParameters;

    [Header("Breaks List Panel")]
    [SerializeField] private Transform openBreaksListPanel;
    [SerializeField] private Transform openBreaksListPanelContent;
    [SerializeField] private GameObject openBreaksListPanelPrefab;
    [SerializeField] private Transform noviceBreaksListPanel;
    [SerializeField] private Transform noviceBreaksListPanelContent;
    [SerializeField] private GameObject noviceBreaksListPanelPrefab;
    [Header("Animation Vectors")]
    [SerializeField] private float openPanelInVector;
    [SerializeField] private float openPanelOutVector;
    [SerializeField] private float novicePanelInVector;
    [SerializeField] private float novicePanelOutVector;

    [Header("Buttons")]
    [SerializeField] private ButtonSelect openBreaksListPanelButtonSelect;
    [SerializeField] private ButtonSelect noviceBreaksListPanelButtonSelect;
    [SerializeField] private Toggle teamCategoryToggle;

    [SerializeField] private BreaksPanel breaksPanel;

    #endregion

    #region Essentials
    private void OnEnable()
    {
        teamCategoryToggle.onSelectOption1 = OpenOpenEligiblePanel;
        teamCategoryToggle.onSelectOption2 = OpenNoviceEligiblePanel;
        ConfigureScreen();
        teamCategoryToggle.SelectOption1();
    }
    private void OnDisable()
    {
        teamCategoryToggle.onSelectOption1 = null;
        teamCategoryToggle.onSelectOption2 = null;

        foreach (Transform child in openBreaksListPanelContent)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in noviceBreaksListPanelContent)
        {
            Destroy(child.gameObject);
        }

        openBreaksListPanel.gameObject.SetActive(false);
        noviceBreaksListPanel.gameObject.SetActive(false);

        Debug.Log("Eligible Open Teams: " + breaksPanel.Eligible_openTeams_TMP.Count);
        Debug.Log("Eligible Novice Teams: " + breaksPanel.Eligible_noviceTeams_TMP.Count);
    }
    #endregion

    #region UI Configuration
    private void ConfigureScreen()
    {
        totalOpenTeams.text = "/" + AppConstants.instance.GetEligibleTeamsForBreaks(SpeakerTypes.Open).Count.ToString();
        totalNoviceTeams.text ="/" +  AppConstants.instance.GetEligibleTeamsForBreaks(SpeakerTypes.Novice).Count.ToString();
    }
    public void OpenOpenEligiblePanel()
    {
        if (noviceBreaksListPanel.gameObject.activeSelf)
            noviceBreaksListPanel.DOLocalMoveY(novicePanelOutVector, 0.5f).OnComplete(() => noviceBreaksListPanel.gameObject.SetActive(false));

        openBreaksListPanel.gameObject.SetActive(true);
        openBreaksListPanel.localPosition = new Vector3(0, -Screen.height, 0);
        openBreaksListPanel.DOLocalMoveY(openPanelInVector, 0.5f).OnComplete(() => UpdateOpenTeamList());
    }
    public void OpenNoviceEligiblePanel()
    {
        if (openBreaksListPanel.gameObject.activeSelf)
            openBreaksListPanel.DOLocalMoveY(openPanelOutVector, 0.5f).OnComplete(() => openBreaksListPanel.gameObject.SetActive(false));

        noviceBreaksListPanel.gameObject.SetActive(true);
        noviceBreaksListPanel.localPosition = new Vector3(0, -Screen.height, 0);
        noviceBreaksListPanel.DOLocalMoveY(novicePanelInVector, 0.5f).OnComplete(() => UpdateNoviceTeamList());
    }

    private void UpdateOpenTeamList()
    {
        foreach (Transform child in openBreaksListPanelContent)
        {
            Destroy(child.gameObject);
        }

        openBreaksListPanelButtonSelect.RemoveAllButtons();
        var eligibleTems = AppConstants.instance.GetEligibleTeamsForBreaks(SpeakerTypes.Open);
        Debug.Log("Available Teams: " + eligibleTems.Count);
        foreach (Team team in AppConstants.instance.selectedTouranment.teamsInTourney)
        {
            GameObject teamObj = Instantiate(openBreaksListPanelPrefab, openBreaksListPanelContent);
            Team_EligibilityLE teamEligibilityLE = teamObj.GetComponent<Team_EligibilityLE>();
            teamEligibilityLE.Initialize(team);
            openBreaksListPanelButtonSelect.AddOption(teamObj.GetComponent<Button>(), teamEligibilityLE.MarkEligibility, teamEligibilityLE.UnMarkEligibility);
        
            if(eligibleTems.Any(x => x.teamId == team.teamId))
            {
                teamEligibilityLE.MarkEligibility();
            }
        }
    }
    private void UpdateNoviceTeamList()
    {
        foreach (Transform child in noviceBreaksListPanelContent)
        {
            Destroy(child.gameObject);
        }

        noviceBreaksListPanelButtonSelect.RemoveAllButtons();
        var eligibleTems = AppConstants.instance.GetEligibleTeamsForBreaks(SpeakerTypes.Novice);
        Debug.Log("Available Teams: " + eligibleTems.Count);
        foreach (Team team in AppConstants.instance.selectedTouranment.teamsInTourney)
        {
            GameObject teamObj = Instantiate(noviceBreaksListPanelPrefab, noviceBreaksListPanelContent);
            Team_EligibilityLE teamEligibilityLE = teamObj.GetComponent<Team_EligibilityLE>();
            teamEligibilityLE.Initialize(team);
            noviceBreaksListPanelButtonSelect.AddOption(teamObj.GetComponent<Button>(), teamEligibilityLE.MarkEligibility, teamEligibilityLE.UnMarkEligibility);

            if(eligibleTems.Any(x => x.teamId == team.teamId))
            {
                teamEligibilityLE.MarkEligibility();
            }
        }
    }
    #endregion
    #region Eligibility Functions
    public void AddOpenTeamAsEligible(Team team)
    {
        if (!breaksPanel.Eligible_openTeams_TMP.Any(x => x.teamId == team.teamId))
        {
            team.isEligibleforBreak = true;
            breaksPanel.Eligible_openTeams_TMP.Add(team);
            selectedOpenTeams.text = "/" + breaksPanel.Eligible_openTeams_TMP.Count.ToString();
        }
    }
    public void RemoveOpenTeamAsEligible(Team team)
    {
        if (breaksPanel.Eligible_openTeams_TMP.Any(x => x.teamId == team.teamId))
        {
            team.isEligibleforBreak = false;
            breaksPanel.Eligible_openTeams_TMP.Remove(team);
            selectedOpenTeams.text = "/" + breaksPanel.Eligible_openTeams_TMP.Count.ToString();
        }
    }
    public void AddNoviceTeamAsEligible(Team team)
    {
        if (!breaksPanel.Eligible_noviceTeams_TMP.Any(x => x.teamId == team.teamId))
        {
            team.isEligibleforBreak = true;
           breaksPanel. Eligible_noviceTeams_TMP.Add(team);
            selectedNoviceTeams.text = "/" + breaksPanel.Eligible_noviceTeams_TMP.Count.ToString();
        }
    }
    public void RemoveNoviceTeamAsEligible(Team team)
    {
        if (breaksPanel.Eligible_noviceTeams_TMP.Any(x => x.teamId == team.teamId))
        {
            team.isEligibleforBreak = false;
            breaksPanel.Eligible_noviceTeams_TMP.Remove(team);
            selectedNoviceTeams.text = "/" + breaksPanel.Eligible_noviceTeams_TMP.Count.ToString();
        }
    }
    #endregion
    #region BreakParameters
    public void SelectBreakParameters(int id)
    {
        breaksPanel.breakParameters = (BreakParameters)id;
    }
    #endregion
}
