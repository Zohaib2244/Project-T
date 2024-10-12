using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeakerPanelEntry : MonoBehaviour
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
    public TMP_Text speakerNameText;
    public TMP_Text teamNameText;
    public TMP_Text round1ScoreText;
    public TMP_Text round2ScoreText;
    public TMP_Text round3ScoreText;
    public TMP_Text round4ScoreText;
    public TMP_Text totalScoreText;

    public void SetRankingData(int ranking, string speakerName, string teamName, int round1Score, int round2Score, int round3Score, int round4Score, int totalScore)
    {
        if (rankingText != null)
            rankingText.text = ranking.ToString();

        if (speakerNameText != null)
            speakerNameText.text = speakerName;

        if (teamNameText != null)
            teamNameText.text = teamName;

        if (round1ScoreText != null)
            round1ScoreText.text = round1Score.ToString();

        if (round2ScoreText != null)
            round2ScoreText.text = round2Score.ToString();

        if (round3ScoreText != null)
            round3ScoreText.text = round3Score.ToString();

        if (round4ScoreText != null)
            round4ScoreText.text = round4Score.ToString();

        if (totalScoreText != null)
            totalScoreText.text = totalScore.ToString();
    }
}
