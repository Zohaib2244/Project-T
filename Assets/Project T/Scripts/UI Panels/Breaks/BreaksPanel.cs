using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Scripts.Resources;
using System.Linq;

public class BreaksPanel : MonoBehaviour
{
    //     #region Singleton
    // public static BreaksPanel Instance;
    // private void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //     }
    //     else
    //     {
    //         Destroy(this);
    //     }
    // }
    // #endregion

    [Header("Breaks Panels")]
    [SerializeField] private Transform breaksEditorPanel;
    [SerializeField] private Transform breaksDisplayPanel;
    [SerializeField] private Toggle teamCategoryToggle;
    public bool isOpenEligibility = false;
    public bool isNoviceEligibility = false;
    public List<Team> Eligible_openTeams_TMP = new List<Team>();
    public List<Team> Eligible_noviceTeams_TMP = new List<Team>();
    public List<Team> breakingOpenTeams = new List<Team>();
    public List<Team> breakingNoviceTeams = new List<Team>();
    public BreakParameters breakParameters;
    void OnEnable()
    {
        breakParameters = AppConstants.instance.selectedTouranment.breakParam;
        teamCategoryToggle.DeActivate();
        GetAllEligibleOpenTeams();
        GetAllEligibleNoviceTeams();
        ShowAppropriatePanel();
    }

    public void ShowAppropriatePanel()
    {
        if (!AppConstants.instance.selectedTouranment.isBreaksGenerated)
            ShowBreaksEditorPanel();
        else
            ShowBreaksDisplayPanel();
    }
    public void ShowBreaksEditorPanel()
    {
        if (breaksDisplayPanel.gameObject.activeSelf)
            breaksDisplayPanel.gameObject.SetActive(false);
        breaksEditorPanel.localScale = Vector3.zero;
        breaksEditorPanel.gameObject.SetActive(true);
        breaksEditorPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    public void ShowBreaksDisplayPanel()
    {
        if (breaksEditorPanel.gameObject.activeSelf)
            breaksEditorPanel.gameObject.SetActive(false);
        breaksDisplayPanel.localScale = Vector3.zero;
        breaksDisplayPanel.gameObject.SetActive(true);
        breaksDisplayPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }
    private void GetAllEligibleOpenTeams()
    {
        if (AppConstants.instance.selectedTouranment.openBreakingTeams != null)
        {
            foreach (string teamid in AppConstants.instance.selectedTouranment.openBreakingTeams)
            {
                Team team = AppConstants.instance.GetTeamFromID(teamid);
                if (team != null && !Eligible_openTeams_TMP.Contains(team))
                {
                    Eligible_openTeams_TMP.Add(team);
                }
            }
        }
    }
    private void GetAllEligibleNoviceTeams()
    {
        if (AppConstants.instance.selectedTouranment.noviceBreakingTeams != null)
        {
            foreach (string teamid in AppConstants.instance.selectedTouranment.noviceBreakingTeams)
            {
                Team team = AppConstants.instance.GetTeamFromID(teamid);
                if (team != null && !Eligible_noviceTeams_TMP.Contains(team))
                {
                    Eligible_noviceTeams_TMP.Add(team);
                }
            }
        }
    }
    public void CalculateBreaks()
    {
        breakingOpenTeams = CalculateOpenBreaks();
        breakingNoviceTeams = CalculateNoviceBreaks();
    }
    List<Team> CalculateOpenBreaks()
    {
        if (breakParameters == BreakParameters.TeamPoints)
        {
            Eligible_openTeams_TMP.Sort((team1, team2) => team1.teamPoints.CompareTo(team2.teamPoints));
        }
        else if (breakParameters == BreakParameters.TeamScore)
        {
            Eligible_openTeams_TMP.Sort((team1, team2) => team1.totalTeamScore.CompareTo(team2.totalTeamScore));
        }

        // Iterate through each SpeakerCategory in the tournament
        foreach (var speakerCategory in AppConstants.instance.selectedTouranment.speakerCategories)
        {
            if (speakerCategory.speakerType == SpeakerTypes.Open)
            {    // Select breaking teams based on the break parameter
                foreach (var team in Eligible_openTeams_TMP)
                {
                    switch (speakerCategory.breakType)
                    {
                        case BreakTypes.QF:
                            if (Eligible_openTeams_TMP.Count < 16)
                                return Eligible_openTeams_TMP.Take(16).ToList();
                            else
                                DialogueBox.Instance.ShowDialogueBox("Not enough teams to break to Quarter Finals", Color.red);
                            break;
                        case BreakTypes.SF:
                            if (Eligible_openTeams_TMP.Count < 8)
                                return Eligible_openTeams_TMP.Take(8).ToList();
                            else
                                DialogueBox.Instance.ShowDialogueBox("Not enough teams to break to Semi Finals", Color.red);
                            break;
                        case BreakTypes.F:
                            if (Eligible_openTeams_TMP.Count < 4)
                                return Eligible_openTeams_TMP.Take(4).ToList();
                            else
                                DialogueBox.Instance.ShowDialogueBox("Not enough teams to break to Finals", Color.red);
                            break;
                        default:
                            return new List<Team>();
                    }
                }
            }
        }
        return null;
    }
    List<Team> CalculateNoviceBreaks()
    {
        if (breakParameters == BreakParameters.TeamPoints)
        {
            Eligible_noviceTeams_TMP.Sort((team1, team2) => team1.teamPoints.CompareTo(team2.teamPoints));
        }
        else if (breakParameters == BreakParameters.TeamScore)
        {
            Eligible_noviceTeams_TMP.Sort((team1, team2) => team1.totalTeamScore.CompareTo(team2.totalTeamScore));
        }

        // Iterate through each SpeakerCategory in the tournament
        foreach (var speakerCategory in AppConstants.instance.selectedTouranment.speakerCategories)
        {
            if (speakerCategory.speakerType == SpeakerTypes.Novice)
            {
                // Select breaking teams based on the break parameter
                foreach (var team in Eligible_noviceTeams_TMP)
                {
                    switch (speakerCategory.breakType)
                    {
                        case BreakTypes.QF:
                            if (Eligible_noviceTeams_TMP.Count < 16)
                                return Eligible_noviceTeams_TMP.Take(16).ToList();
                            else
                                DialogueBox.Instance.ShowDialogueBox("Not enough teams to break to Quarter Finals", Color.red);
                            break;
                        case BreakTypes.SF:
                            if (Eligible_noviceTeams_TMP.Count < 8)
                                return Eligible_noviceTeams_TMP.Take(8).ToList();
                            else
                                DialogueBox.Instance.ShowDialogueBox("Not enough teams to break to Semi Finals", Color.red);
                            break;
                        case BreakTypes.F:
                            if (Eligible_noviceTeams_TMP.Count < 4)
                                return Eligible_noviceTeams_TMP.Take(4).ToList();
                            else
                                DialogueBox.Instance.ShowDialogueBox("Not enough teams to break to Finals", Color.red);
                            break;
                        default:
                            return new List<Team>();
                    }
                }
            }
        }
        return new List<Team>();
    }
    public void SaveBreaks()
    {
        AppConstants.instance.selectedTouranment.isBreaksGenerated = true;
        if (AppConstants.instance.selectedTouranment.speakerCategories.Count == 1)
            AppConstants.instance.selectedTouranment.openBreakingTeams = breakingOpenTeams.Select(x => x.teamId).ToList();
        else if (AppConstants.instance.selectedTouranment.speakerCategories.Count == 2)
        {
            AppConstants.instance.selectedTouranment.openBreakingTeams = breakingOpenTeams.Select(x => x.teamId).ToList();
            AppConstants.instance.selectedTouranment.noviceBreakingTeams = breakingNoviceTeams.Select(x => x.teamId).ToList();
        }
    }
}
