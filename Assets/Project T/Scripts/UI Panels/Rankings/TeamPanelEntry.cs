using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TeamPanelEntry : MonoBehaviour
{
    public TMP_Text rankingText;
    public TMP_Text teamNameText;
    public TMP_Text institutionText;
    public TMP_Text teamPointsText;
    public TMP_Text teamScoresText;
    public HorizontalLayoutGroup positionsGridLayout;
    public TMP_Text positionTextPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRankingData(int ranking, string teamName, string institution, List<string> positions, int teamPoints, float teamScores)
    {
        if (rankingText != null)
            rankingText.text = ranking.ToString();

        if (teamNameText != null)
            teamNameText.text = teamName;

        if (institutionText != null)
            institutionText.text = institution;

        if (teamPointsText != null)
            teamPointsText.text = teamPoints.ToString();

        if (teamScoresText != null)
            teamScoresText.text = ((int)teamScores).ToString(); // Convert float to int

        // Clear existing children in the GridLayoutGroup
        foreach (Transform child in positionsGridLayout.transform)
        {
            Destroy(child.gameObject);
        }

        // Populate the GridLayoutGroup with positions
        foreach (string position in positions)
        {
            TMP_Text positionTextInstance = Instantiate(positionTextPrefab, positionsGridLayout.transform);
            positionTextInstance.text = position.ToString();
            positionTextInstance.enableAutoSizing = true; // Enable auto size
            positionTextInstance.alignment = TextAlignmentOptions.Center; // Center align text
        }
    }
}