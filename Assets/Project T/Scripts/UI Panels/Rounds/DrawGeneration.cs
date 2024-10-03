using System.Collections;
using System.Collections.Generic;
using Scripts.Resources;
using Scripts.UIPanels;
using UnityEngine;
using System;
using System.Linq;
using Random = System.Random;


public class DrawGeneration : MonoBehaviour
{
    #region Singleton
    public static DrawGeneration Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    #endregion
    public bool isAdjudicatorAutoAllocation = false;
    public bool isTeamAutoAllocation = false; 

    private List<Team> teams_TMP = new List<Team>();
    private List<Adjudicator> adjudicators_TMP = new List<Adjudicator>();
    private List<Match> matches = new List<Match>();
    private TournamentType tournamentType;

    void OnEnable()
    {
        tournamentType = AppConstants.instance.selectedTouranment.tournamentType;
        teams_TMP = MainRoundsPanel.Instance.selectedRound.availableTeams;
        adjudicators_TMP = MainRoundsPanel.Instance.selectedRound.availableAdjudicators;
    }
    private List<Match> GenerateDrawForPrelims(List<Team> teams, List<Adjudicator> adjudicators, TournamentType tournamentType)
    {
        List<Match> matches = new List<Match>();
        if (tournamentType == TournamentType.British)
        {
            matches = GenerateDrawForBP(teams, adjudicators);
        }
        else if (tournamentType == TournamentType.Asian)
        {
            // matches = GenerateDrawForAP(teams, adjudicators);
        }
        return matches;
    }
    #region Draw Generation BP
  private List<Match> GenerateDrawForBP(List<Team> teams, List<Adjudicator> adjudicators)
    {
        List<Match> matches = new List<Match>();
        Dictionary<int, List<Team>> brackets = new Dictionary<int, List<Team>>();

        // Step 1: Divide teams into raw brackets by the number of their wins
        foreach (var team in teams)
        {
            int wins = GetTeamWins(team);
            if (!brackets.ContainsKey(wins))
            {
                brackets[wins] = new List<Team>();
            }
            brackets[wins].Add(team);
        }

        // Step 2: Resolve odd brackets by pulling up teams from the next bracket down
        List<int> sortedKeys = brackets.Keys.OrderByDescending(k => k).ToList();
        for (int i = 0; i < sortedKeys.Count; i++)
        {
            int key = sortedKeys[i];
            if (brackets[key].Count % 4 != 0)
            {
                int teamsNeeded = 4 - (brackets[key].Count % 4);
                if (i + 1 < sortedKeys.Count)
                {
                    int nextKey = sortedKeys[i + 1];
                    List<Team> pullUpTeams = brackets[nextKey].Take(teamsNeeded).ToList();
                    brackets[nextKey].RemoveAll(t => pullUpTeams.Contains(t));
                    brackets[key].AddRange(pullUpTeams);
                }
            }
        }

        // Step 3: Generate matches
        foreach (var bracket in brackets.Values)
        {
            // Shuffle teams to randomize matches
            Random rand = new Random();
            List<Team> shuffledTeams = bracket.OrderBy(t => rand.Next()).ToList();

            // Pair teams using the slide method
            for (int i = 0; i < shuffledTeams.Count; i += 4)
            {
                if (i + 3 >= shuffledTeams.Count) break; // Ensure there are enough teams for a match

                Match match = new Match();
                match.matchId = Guid.NewGuid().ToString();

                // Assign sides based on the number of times teams have affirmed
                List<Team> matchTeams = shuffledTeams.Skip(i).Take(4).ToList();
                matchTeams = AssignSides(matchTeams);

                for (int j = 0; j < 4; j++)
                {
                    Team team = matchTeams[j];
                    TeamPositionsBritish position = (TeamPositionsBritish)j;
                    TeamRoundData teamRoundData = new TeamRoundData
                    {
                        teamId = team.teamId,
                        teamPositionBritish = position
                    };

                    match.teams.Add(team.teamId, teamRoundData.teamRoundDataID);
                }

                // Assign at least one adjudicator to the match
                if (adjudicators.Count > 0)
                {
                    match.adjudicators[0] = adjudicators[rand.Next(adjudicators.Count)];
                }

                matches.Add(match);
            }
        }

        return matches;
    }
private List<Team> AssignSides(List<Team> teams)
{
    Random rand = new Random();
    List<Team> sortedTeams = teams.OrderBy(t => GetAffirmCount(t)).ToList();

    for (int i = 0; i < sortedTeams.Count; i++)
    {
        Team team = sortedTeams[i];
        TeamRoundData trd = team.teamRoundDatas.FirstOrDefault(tr => tr.roundID == MainRoundsPanel.Instance.selectedRound.roundId);
        if (trd != null)
        {
            switch (i % 4)
            {
                case 0:
                    trd.teamPositionBritish = TeamPositionsBritish.OG;
                    break;
                case 1:
                    trd.teamPositionBritish = TeamPositionsBritish.OO;
                    break;
                case 2:
                    trd.teamPositionBritish = TeamPositionsBritish.CG;
                    break;
                case 3:
                    trd.teamPositionBritish = TeamPositionsBritish.CO;
                    break;
            }
        }
    }

    if (GetAffirmCount(sortedTeams[0]) == GetAffirmCount(sortedTeams[1]))
    {
        sortedTeams = sortedTeams.OrderBy(t => rand.Next()).ToList();
    }

    return sortedTeams;
}

private int GetAffirmCount(Team team)
{
    return team.teamRoundDatas.Count(trd => trd.teamPositionBritish == TeamPositionsBritish.OG || trd.teamPositionBritish == TeamPositionsBritish.CG || trd.teamPositionAsian == TeamPositionsAsian.Gov);
}

private int GetTeamWins(Team team)
{
    return team.teamRoundDatas.Count(trd => trd.teamScore > 0);
}




#endregion
















}