using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TeamPanelEntry : MonoBehaviour
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
    public TMP_Text round1PositionText;
    public TMP_Text round2PositionText;
    public TMP_Text round3PositionText;
    public TMP_Text round4PositionText;
    public TMP_Text teamPointsText;
    public TMP_Text teamScoresText;

    public void SetRankingData(int ranking, string teamName, string institution, int totalSpeakerPoints, int round1Position, int round2Position, int round3Position, int round4Position, int teamPoints, string teamScores)
    {
        if (rankingText != null)
            rankingText.text = ranking.ToString();

        if (teamNameText != null)
            teamNameText.text = teamName;

        if (institutionText != null)
            institutionText.text = institution;

        if (round1PositionText != null)
            round1PositionText.text = round1Position.ToString();

        if (round2PositionText != null)
            round2PositionText.text = round2Position.ToString();

        if (round3PositionText != null)
            round3PositionText.text = round3Position.ToString();

        if (round4PositionText != null)
            round4PositionText.text = round4Position.ToString();

        if (teamPointsText != null)
            teamPointsText.text = teamPoints.ToString("F2"); // Format as needed

        if (teamScoresText != null)
            teamScoresText.text = teamScores;
    }
}
