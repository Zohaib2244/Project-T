using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RankingItemUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public TMP_Text rankingText;
    public TMP_Text teamNameText;
    public TMP_Text institutionText;
    public TMP_Text totalSpeakerPointsText;
    public TMP_Text teamPointsText;

    public void setRankingData(int ranking, string teamName, string institution, int totalSpeakerPoints, int teamPoints)
    {
        if (rankingText != null)
            rankingText.text = ranking.ToString();

        if (teamNameText != null)
            teamNameText.text = teamName;

        if (institutionText != null)
            institutionText.text = institution;

        if (totalSpeakerPointsText != null)
            totalSpeakerPointsText.text = totalSpeakerPoints.ToString("F2"); // Format as needed

        if (teamPointsText != null)
            teamPointsText.text = teamPoints.ToString("F2"); // Format as needed
    }
}
