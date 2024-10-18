using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;


public class DrawEditListEntry : MonoBehaviour
{
    public BritishMatch_TMP myMatch;
    public List<Button> _teamBTNS = new List<Button>();
    public List<Button> _adjBTNS = new List<Button>();

    public List<TeamBtn> teamBtns = new List<TeamBtn>();

    public List<AdjBtn> adjBtns = new List<AdjBtn>();

    void OnEnable()
    {
        DrawEditPanel.Instance.DeselectAllTeams.AddListener(DeselectExtraTeams);
        DrawEditPanel.Instance.ReplaceTeamsEvent.AddListener(DeselectExtraTeams);
        DrawEditPanel.Instance.ReplaceTeamsEvent.AddListener(AddTextToButton);
    
        DrawEditPanel.Instance.DeselectAllJudges.AddListener(DeselectExtraAdjudicators);
        DrawEditPanel.Instance.ReplaceJudgesEvent.AddListener(DeselectExtraAdjudicators);
        DrawEditPanel.Instance.ReplaceJudgesEvent.AddListener(AddTextToButton);
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
            Debug.Log("AdjID: " + adjBtn.adj.adjudicatorID);
        }
        DrawEditPanel.Instance.adjBtns.AddRange(adjBtns);
        AddTextToButton();
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
        if (adjBtn.isSelected)
        {
            DeselectAdj(adjBtn);
            DrawEditPanel.Instance.DeselectAllJudges?.Invoke();
            return;
        }
        adjBtn.isSelected = true;
        SelectedAnimation(adjBtn.btn.transform, adjBtn.btn);
        DrawEditPanel.Instance.AddReplaceableAdjudicator(adjBtn); // Pass the Adjudicator_TMP object associated with the button
        DeselectExtraAdjudicators();
    }

    private void DeselectAdj(AdjBtn adjBtn)
    {
        if (adjBtn.isSelected)
        {
            DrawEditPanel.Instance.RemoveReplaceableAdjudicator(adjBtn);
            adjBtn.isSelected = false;
            DeselctedAnimation(adjBtn.btn.transform, adjBtn.btn);
        }
    }
    private void DeselectExtraAdjudicators()
    {
        foreach (var adjBtn in adjBtns)
        {
            if (adjBtn.adj != DrawEditPanel.Instance.replacementJudge1 && adjBtn.adj != DrawEditPanel.Instance.replacementJudge2)
            {
                DeselectAdj(adjBtn);
            }
        }
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
        teamBtn.isSelected = true;
        SelectedAnimation(teamBtn.btn.transform, teamBtn.btn);
        DrawEditPanel.Instance.AddReplaceableTeam(teamBtn); // Pass the Team_TMP object associated with the button
        DeselectExtraTeams();
    }
    private void DeselectTeam(TeamBtn teamBtn)
    {
        if (teamBtn.isSelected)
        {
            DrawEditPanel.Instance.RemoveReplaceableTeam(teamBtn);
            teamBtn.isSelected = false;
            DeselctedAnimation(teamBtn.btn.transform, teamBtn.btn);
        }
    }
    #endregion




    private void SelectedAnimation(Transform transform, Button btn)
    {
        //           ColorBlock colorBlock = btn.colors;
        // colorBlock.normalColor = new Color32(0xFA, 0xC2, 0x9C, 0xFF);
        // btn.colors = colorBlock;
        transform.DOScale(Vector3.one * 0.8f, 0.2f);


            // Force the button to refresh its state
    btn.OnPointerExit(null);
    btn.OnPointerEnter(null);
    }

    private void DeselctedAnimation(Transform transform, Button btn)
    {   
        //      ColorBlock colorBlock = btn.colors;
        // colorBlock.normalColor = Color.white;
        // btn.colors = colorBlock;
        transform.DOScale(Vector3.one, 0.2f);


            // Force the button to refresh its state
    btn.OnPointerExit(null);
    btn.OnPointerEnter(null);
    }


}