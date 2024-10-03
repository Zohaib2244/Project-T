using UnityEngine;
using TMPro;
using Scripts.ListEntry;
using System.Collections.Generic;
using Scripts.Resources;

namespace Scripts.UIPanels.RoundPanels
{
public class Rounds_DrawOptionsPanel : MonoBehaviour
{
[SerializeField] private Transform DrawsOptionsPanel;

[Header("Status")]
[SerializeField] private TMP_Text swingsTxt;
[SerializeField] private TMP_Text openTeamTxt;
[SerializeField] private TMP_Text noviceTeamTxt;
[SerializeField] private TMP_Text AdjudicatorTxt;

[Header("Team Panel")]
[SerializeField] private Transform teamPanel;
[SerializeField] private Transform teamContainer;
[SerializeField] private GameObject teamPrefab;
[SerializeField] private Toggle teamAllocationToggle;

[Header("Adjudicator Panel")]
[SerializeField] private Transform adjudicatorPanel;
[SerializeField] private Transform adjudicatorContainer;
[SerializeField] private GameObject adjudicatorPrefab;
[SerializeField] private Toggle adjudicatorAllocationToggle;

    void OnEnable()
    {
        teamAllocationToggle.onSelectOption1 += OnTeamAutoAllocation;
        teamAllocationToggle.onSelectOption2 += OnTeamManualAllocation;
        adjudicatorAllocationToggle.onSelectOption1 += OnAdjudicatorAutoAllocation;
        adjudicatorAllocationToggle.onSelectOption2 += OnAdjudicatorManualAllocation;
        ConfigureScreen();
    }
    void OnDisable()
    {
        teamAllocationToggle.onSelectOption1 -= OnTeamAutoAllocation;
        teamAllocationToggle.onSelectOption2 -= OnTeamManualAllocation;
        adjudicatorAllocationToggle.onSelectOption1 -= OnAdjudicatorAutoAllocation;
        adjudicatorAllocationToggle.onSelectOption2 -= OnAdjudicatorManualAllocation;
    }

    private void ConfigureScreen()
    {
        UpdateTeamList();
        UpdateAdjudicatorList();
        ConfigureStats();
    }
    private void UpdateTeamList()
    {
        foreach (Transform child in teamContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (var team in MainRoundsPanel.Instance.selectedRound.availableTeams)
        {
            var teamObj = Instantiate(teamPrefab, teamContainer);
            teamObj.GetComponent<LE_Team_DrawOption>().SetTeam(team);
        }
    }
    private void UpdateAdjudicatorList()
    {
        foreach (Transform child in adjudicatorContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (var adjudicator in MainRoundsPanel.Instance.selectedRound.availableAdjudicators)
        {
            var adjObj = Instantiate(adjudicatorPrefab, adjudicatorContainer);
            adjObj.GetComponent<LE_Adj_DrawOption>().SetAdjudicator(adjudicator);
        }
    }
    public void ConfigureStats()
    {
        //Swings---------------------------------------
        MainRoundsPanel.Instance.swingsCount = MainRoundsPanel.Instance.selectedRound.availableTeams.Count % 4;

        if ( MainRoundsPanel.Instance.swingsCount != 0)
        {
            swingsTxt.text = MainRoundsPanel.Instance.swingsCount.ToString();
        }
        else
        {
            swingsTxt.text = "0";
        }
        //OpenTeamCount
        openTeamTxt.text = CountOpenTeams(MainRoundsPanel.Instance.selectedRound.availableTeams).ToString();
        //NoviceTeamCount
        noviceTeamTxt.text = CountNoviceTeams(MainRoundsPanel.Instance.selectedRound.availableTeams).ToString();
        //AdjudicatorCount
        AdjudicatorTxt.text = MainRoundsPanel.Instance.selectedRound.availableAdjudicators.Count.ToString();
    }
    private int CountOpenTeams(List<Team> availableTeams)
    {
        int openTeamCount = 0;
        foreach (var team in availableTeams)
        {
            if (team.teamCategory == SpeakerTypes.Open) // Assuming IsOpenCategory() is a method that checks if the team is in the open category
            {
                openTeamCount++;
            }
        }
        return openTeamCount;
    }
    private int CountNoviceTeams(List<Team> availableTeams)
    {
        int noviceTeamCount = 0;
        foreach (var team in availableTeams)
        {
            if (team.teamCategory == SpeakerTypes.Novice) // Assuming IsNoviceCategory() is a method that checks if the team is in the novice category
            {
                noviceTeamCount++;
            }
        }
        return noviceTeamCount;
    }
    private void OnTeamManualAllocation()
    {
        DrawGeneration.Instance.isTeamAutoAllocation = false;
    }
    private void OnAdjudicatorManualAllocation()
    {
        DrawGeneration.Instance.isAdjudicatorAutoAllocation = false;
    }
    private void OnTeamAutoAllocation()
    {
        DrawGeneration.Instance.isTeamAutoAllocation = true;
    }
    private void OnAdjudicatorAutoAllocation()
    {
        DrawGeneration.Instance.isAdjudicatorAutoAllocation = true;
    }
}
}