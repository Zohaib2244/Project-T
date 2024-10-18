using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class DrawEditListEntry : MonoBehaviour
{
    public BritishMatch_TMP myMatch;
    private List<Button> _teamBTNS = new List<Button>();
    private List<Button> _adjBTNS = new List<Button>();

    public List<TeamBtn> teamBtns = new List<TeamBtn>();

    public List<AdjBtn> adjBtns = new List<AdjBtn>();

    void OnEnable()
    {
        DrawEditPanel.Instance.DeselectAllTeams.AddListener(DeselectExtraTeams);
        DrawEditPanel.Instance.ReplaceTeamsEvent.AddListener(DeselectExtraTeams);
        DrawEditPanel.Instance.ReplaceTeamsEvent.AddListener(AddTextToButton);
    }

    public void SetMatch(BritishMatch_TMP match)
    {
        myMatch = match;
        for (int i = 0; i < myMatch.teamsInMatch.Count; i++)
        {
            TeamBtn teamBtn = new TeamBtn(_teamBTNS[i], myMatch.teamsInMatch[i]);
            teamBtn.btn.onClick.AddListener(() => OnTeamBtnClick(teamBtn));
            teamBtns.Add(teamBtn);
        }
        DrawEditPanel.Instance.teamBtns.AddRange(teamBtns);

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
            _teamBTNS[i].GetComponentInChildren<TMP_Text>().text = myMatch.teamsInMatch[i].TeamName;
        }

        for (int i = 0; i < myMatch.adjudicatorsInMatch.Count; i++)
        {
            _adjBTNS[i].GetComponentInChildren<TMP_Text>().text = myMatch.adjudicatorsInMatch[i].adjudicatorName;
        }
    }

    public void OnAdjBtnClick(AdjBtn adjBtn)
    {

    }

    


    private void DeselectExtraTeams()
    {
        foreach (var teamBtn in teamBtns)
        {
            if(teamBtn.team != DrawEditPanel.Instance.replacementTeam1 && teamBtn.team != DrawEditPanel.Instance.replacementTeam2)
            {
                DeselectTeam(teamBtn);
            }
        }
    }
    #region Team Functions
    private void OnTeamBtnClick(TeamBtn teamBtn)
    {
        if (teamBtn.isSelected)
        {
            DeselectTeam(teamBtn);
            DrawEditPanel.Instance.DeselectAllTeams?.Invoke();
            return;
        }
        // DrawEditPanel.Instance.DeselectAllTeams?.Invoke();
        teamBtn.isSelected = true;
        SelectedAnimation(teamBtn.btn.transform);
        DrawEditPanel.Instance.AddReplaceableTeam(teamBtn); // Pass the Team_TMP object associated with the button

    }
    private void DeselectTeam(TeamBtn teamBtn)
    {
        if (teamBtn.isSelected)
        {
            DrawEditPanel.Instance.RemoveReplaceableTeam(teamBtn);
            teamBtn.isSelected = false;
            DeselctedAnimation(teamBtn.btn.transform);
        }
    }
    #endregion




    private void SelectedAnimation(Transform transform)
    {
        transform.DOScale(Vector3.one * 0.9f, 0.2f);
    }

    private void DeselctedAnimation(Transform transform)
    {
        transform.DOScale(Vector3.one, 0.2f);
    }


}