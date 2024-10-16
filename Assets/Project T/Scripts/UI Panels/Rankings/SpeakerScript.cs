using System.Collections;
using System.Collections.Generic;
using Scripts.Resources;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerScript : MonoBehaviour
{
    public GameObject speakerPanelEntryPrefab; // Prefab for SpeakerPanelEntry
    public Transform scrollViewContent; // Container for the ScrollView content
    public Button openButton; // Reference to the Open button
    public Button noviceButton; // Reference to the Novice button

    // Start is called before the first frame update
    void Start()
    {
        // Add OnClick listeners to the buttons
        openButton.onClick.AddListener(() => RefreshSpeakerRankings(SpeakerTypes.Open));
        noviceButton.onClick.AddListener(() => RefreshSpeakerRankings(SpeakerTypes.Novice));

        // Call the method to refresh speaker rankings on start with default category
        RefreshSpeakerRankings(SpeakerTypes.Open);
    }

    // Method to refresh and print speaker rankings
    public void RefreshSpeakerRankings(SpeakerTypes category)
    {
        // Call GetSpeakerRankings from AppConstants instance
        List<AppConstants.SpeakerRanking> speakerRankings = AppConstants.instance.GetSpeakerRankings(category);

        // Clear previous entries
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // Populate the ScrollView with speaker rankings
        if (speakerRankings != null && speakerRankings.Count > 0)
        {
            foreach (var ranking in speakerRankings)
            {
                GameObject entryObj = Instantiate(speakerPanelEntryPrefab, scrollViewContent);
                SpeakerPanelEntry entry = entryObj.GetComponent<SpeakerPanelEntry>();
                if (entry != null)
                {
                    entry.SetRankingData(ranking.Ranking, ranking.SpeakerName, ranking.TeamName, ranking.RoundScores, ranking.TotalScore);
                }
            }
        }
        else
        {
            Debug.Log("No speaker rankings found.");
        }
    }

    // Optional: Refresh rankings when the panel becomes active (if necessary)
    private void OnEnable()
    {
        RefreshSpeakerRankings(SpeakerTypes.Open);
    }
}