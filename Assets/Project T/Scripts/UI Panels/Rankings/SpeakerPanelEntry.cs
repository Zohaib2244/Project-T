using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpeakerPanelEntry : MonoBehaviour
{
    public TMP_Text rankingText;
    public TMP_Text speakerNameText;
    public TMP_Text teamNameText;
    public TMP_Text totalScoreText;
    public GameObject roundScorePrefab; // Prefab for round score text elements
    public Transform roundScoresContainer; // Container with GridLayoutGroup


    public void SetRankingData(int ranking, string speakerName, string teamName, List<float> roundScores, float totalScore)
    {
        if (rankingText != null)
            rankingText.text = ranking.ToString();

        if (speakerNameText != null)
            speakerNameText.text = speakerName;

        if (teamNameText != null)
            teamNameText.text = teamName;

        // Clear previous round scores
        foreach (Transform child in roundScoresContainer)
        {
            Destroy(child.gameObject);
        }

        // Populate round scores
        for (int i = 0; i < roundScores.Count; i++)
        {
            GameObject roundScoreObj = Instantiate(roundScorePrefab, roundScoresContainer);
            TMP_Text roundScoreText = roundScoreObj.GetComponent<TMP_Text>();
            if (roundScoreText != null)
            {
                roundScoreText.text = $"R{i + 1}: {roundScores[i]}";
                roundScoreText.enableAutoSizing = true; // Enable auto size
                roundScoreText.alignment = TextAlignmentOptions.Center; // Center align text
            }
        }

        if (totalScoreText != null)
            totalScoreText.text = totalScore.ToString();
    }
}