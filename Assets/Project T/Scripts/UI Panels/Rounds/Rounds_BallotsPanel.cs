using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Scripts.UIPanels;
using System.Linq;

public class Rounds_BallotsPanel : MonoBehaviour
{
    [SerializeField] private Transform ballotsPanel;
    [SerializeField] private Transform ballotsContent;
    [SerializeField] private GameObject ballotEntryPrefab;


    void OnEnable()
    {
        ballotsPanel.localScale = Vector3.zero; // Start from zero scale
        ballotsPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            UpdateBallotsList();
        }); // Animate to full scale with a pop effect
    }
    void OnDisable()
    {
        // Clear existing entries
        foreach (Transform child in ballotsContent)
        {
            Destroy(child.gameObject);
        }
    }


    private void UpdateBallotsList()
    {
        // Clear existing entries
        foreach (Transform child in ballotsContent)
        {
            Destroy(child.gameObject);
        }

        // Instantiate new entries and set match data
        for (int i = 0; i < DrawsPanel.Instance.matches_TMP.Count; i++)
        {
            var match = DrawsPanel.Instance.matches_TMP[i];
            var drawEntry = Instantiate(ballotEntryPrefab, ballotsContent);
            drawEntry.GetComponent<MatchListEntry>().SetMatch(match, i + 1); // Pass match object and match number (index + 1)
        }
    }
    public void SaveBallots()
    {
       List<Match> allMatches = GetBallotPrefabs();
        
            // Get the selected round
            var selectedRound = MainRoundsPanel.Instance.selectedRound;
        
            if (selectedRound == null)
            {
                Debug.LogWarning("No selected round found.");
                return;
            }

            selectedRound.matches.Clear();
            // Save the draw prefabs to the selected round
            selectedRound.matches = allMatches;
    }
    
    // Helper method to retrieve all ballot prefabs
    private List<Match> GetBallotPrefabs()
    {
        List<Match> matches = new List<Match>();

        // Assuming the parent GameObject containing all ballot prefabs is named "BallotsContainer"
        GameObject ballotsContainer = GameObject.Find("BallotsContainer");

        if (ballotsContainer != null)
        {
            // Iterate through each child of the ballotsContainer
            foreach (Transform child in ballotsContainer.transform)
            {
                // Get the BallotListEntry component and retrieve the match
            if (child.TryGetComponent<BallotListEntry>(out BallotListEntry ballotEntry))
                {if (ballotEntry != null)
                {
                    Match match = ballotEntry.GetSavedBallot();
                    matches.Add(match);
                }}
            }
        }
        else
        {
            Debug.LogWarning("BallotsContainer not found in the scene.");
        }

        return matches;
    }

    
}
