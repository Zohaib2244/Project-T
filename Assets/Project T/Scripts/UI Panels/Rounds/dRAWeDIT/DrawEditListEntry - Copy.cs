using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;


public class DrawEditListEntry : MonoBehaviour
{
    public BritishMatch_TMP myMatch;
    private List<Button> _teamBTNS = new List<Button>();
    private List<Button> _adjBTNS = new List<Button>();
   
    public List<TeamBtn> teamBtns = new List<TeamBtn>();

    public List<AdjBtn> adjBtns = new List<AdjBtn>();

    private bool isReplaceable = false;
    public void SetMatch(BritishMatch_TMP match)
    {
        myMatch = match;
        for (int i = 0; i < myMatch.teamsInMatch.Count; i++)
        {
            TeamBtn teamBtn = new TeamBtn(_teamBTNS[i], myMatch.teamsInMatch[i]);
            teamBtn.btn.onClick.AddListener(() => OnTeamBtnClick(teamBtn));
            teamBtns.Add(teamBtn);
        }

        for (int i = 0; i < myMatch.adjudicatorsInMatch.Count; i++)
        {
            AdjBtn adjBtn = new AdjBtn(_adjBTNS[i], myMatch.adjudicatorsInMatch[i]);
            adjBtn.btn.onClick.AddListener(() => OnAdjBtnClick(adjBtn));
            adjBtns.Add(adjBtn);
        }
    }

    public void AddTextToButton()
    {
        for (int i = 0; i < myMatch.teamsInMatch.Count; i++)
        {
            _teamBTNS[i].GetComponentInChildren<Text>().text = myMatch.teamsInMatch[i].TeamName;
        }

        for (int i = 0; i < myMatch.adjudicatorsInMatch.Count; i++)
        {
            _adjBTNS[i].GetComponentInChildren<Text>().text = myMatch.adjudicatorsInMatch[i].adjudicatorName;
        }
    }

    public void OnAdjBtnClick(AdjBtn adjBtn)
    {
        
    }



    private void OnTeamBtnClick(TeamBtn teamBtn)
    {
        isReplaceable = !isReplaceable;
        if (isReplaceable)
        {
            DeselctedAnimation();
            isReplaceable = false;
            DrawEditPanel.Instance.RemoveReplaceableTeam(teamBtn);
        }
        else
        {
            SelectedAnimation();
            isReplaceable = true;
            DrawEditPanel.Instance.AddReplaceableTeam(teamBtn); // Pass the Team_TMP object associated with the button
        }
    }
    private void SelectedAnimation()
    {
        transform.DOScale(Vector3.one * 0.9f, 0.2f);
    }

    private void DeselctedAnimation()
    {
        transform.DOScale(Vector3.one, 0.2f);
    }


}