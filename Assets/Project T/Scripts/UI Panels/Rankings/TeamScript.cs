using System.Collections;
using System.Collections.Generic;
using Scripts.Resources;
using UnityEngine;
using UnityEngine.UI;

public class TeamScript : MonoBehaviour
{
    public GameObject teamPanelEntryPrefab; // Prefab to instantiate
    public Transform scrollViewContent; // Parent container for the instantiated prefabs
    public Button openButton; // Button for Open speaker type
    public Button noviceButton; // Button for Novice speaker type

    // Start is called before the first frame update
    void Start()
    {
        // Attach event listeners to buttons
        openButton.onClick.AddListener(() => RefreshTeamRankings(SpeakerTypes.Open));
        noviceButton.onClick.AddListener(() => RefreshTeamRankings(SpeakerTypes.Novice));

        // Call the method to refresh team rankings on start with default parameter
        RefreshTeamRankings(SpeakerTypes.Open);
    }

    // Method to refresh and populate team rankings
    public void RefreshTeamRankings(SpeakerTypes speakerType)
    {
        // Call GetTeamRankings from AppConstants instance
        List<AppConstants.TeamRanking> teamRankings = AppConstants.instance.GetTeamRankings(speakerType);

        // Print each team ranking to the console
        foreach (var ranking in teamRankings)
        {
            Debug.Log($"Ranking: {ranking.Ranking}, TeamName: {ranking.TeamName}, Institution: {ranking.Institution}, Positions: {string.Join(", ", ranking.Positions)}, Points: {ranking.Points}, Scores: {string.Join(", ", ranking.Scores)}");
        }
        
        // Populate the team rankings in the scroll view
        PopulateTeamRankings(teamRankings);
    }

    // Method to populate the team rankings in the scroll view
    private void PopulateTeamRankings(List<AppConstants.TeamRanking> teamRankings)
    {
        // Clear existing entries
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        if (teamRankings != null && teamRankings.Count > 0)
        {
            foreach (var ranking in teamRankings)
            {
                GameObject entry = Instantiate(teamPanelEntryPrefab, scrollViewContent);
                TeamPanelEntry teamPanelEntry = entry.GetComponent<TeamPanelEntry>();

                if (teamPanelEntry != null)
                {
                    teamPanelEntry.SetRankingData(
                        ranking.Ranking,
                        ranking.TeamName,
                        ranking.Institution,
                        ranking.Positions,
                        ranking.Points,
                        ranking.Scores
                    );
                }
            }
        }
        else
        {
            Debug.Log("No team rankings found.");
        }
    }

    // Update is called once per frame (if needed)
    void Update()
    {

    }

    // Optional: Refresh rankings when the panel becomes active (if necessary)
    private void OnEnable()
    {
        RefreshTeamRankings(SpeakerTypes.Open);
    }
}