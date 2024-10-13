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
    public void GenerateDraws()
    {
        MainRoundsPanel.Instance.selectedRound.availableTeams.Count();
        if (isTeamAutoAllocation)
        {
            Debug.Log("Available Teams: " + teams_TMP.Count);   
            foreach (var team in teams_TMP)
            {
                Debug.Log("Team Name: " + team.teamName);
            }
            matches = GenerateDrawForPrelims(teams_TMP, adjudicators_TMP, tournamentType);
        }
        else
        {
            // matches = GenerateDrawForPrelims(teams_TMP, adjudicators_TMP, tournamentType);
        }
        DrawsPanel.Instance.matches_TMP = matches;
        MainRoundsPanel.Instance.selectedRound.drawGenerated = true;
        Debug.Log("<color=green>Draws Generated Printing Matches</color>");
        // DebugPrintMatches(MainRoundsPanel.Instance.selectedRound.matches);
        DrawsPanel.Instance.SwitchDrawPanel(DrawPanelTypes.DrawDisplayPanel);
    
    }
    private List<Match> GenerateDrawForPrelims(List<Team> teams, List<Adjudicator> adjudicators, TournamentType tournamentType)
    {
        List<Match> matches = new List<Match>();
        if (tournamentType == TournamentType.British)
        {
            DrawGenerator drawGenerator = new DrawGenerator(teams.Count);
            matches = drawGenerator.GenerateDraw(teams, adjudicators, isAdjudicatorAutoAllocation, isTeamAutoAllocation);
        }
        else if (tournamentType == TournamentType.Asian)
        {
            // matches = GenerateDrawForAP(teams, adjudicators);
        }


        return matches;
    }
    public void DebugPrintMatches(List<Match> matches)
    {
        foreach (var match in matches)
        {
            Debug.Log($"<color=cyan>Match ID: {match.matchId}</color>");
            foreach (var teamEntry in match.teams)
            {
                var team = teams_TMP.FirstOrDefault(t => t.teamId == teamEntry.Key);
                if (team != null)
                {
                    var teamRoundData = team.teamRoundDatas.FirstOrDefault(trd => trd.teamRoundDataID == teamEntry.Value);
                    if (teamRoundData != null)
                    {
                        Debug.Log($"<color=green>Team ID: {team.teamId}</color>, <color=yellow>Team Name: {team.teamName}</color>, <color=orange>Speaking Position: {teamRoundData.teamPositionBritish}</color>");
                    }
                }
            }
            Debug.Log("<color=magenta>Adjudicators:</color>");
            foreach (var adjudicator in match.adjudicators)
            {
                Debug.Log($"<color=blue>{adjudicator.adjudicatorName.ToString()}</color>");
                Debug.Log($"<color=blue>{adjudicator.adjudicatorType.ToString()}</color>");
            }
            Debug.Log($"<color=red>Selected Motion: {match.selectedMotion}</color>");
        }
    }
}
    #region Draw Generation BP


public class DrawGenerator
{
    private int[,] costMatrix;
    private int numberOfTeams;
    private int numberOfPositions = 4; // OG, OO, CG, CO
    private double beta = 4.0; // Position cost exponent set to 4
    private double alpha = 1.0; // Order of Rényi entropy, default to 1 (Shannon entropy)
    private bool isAdjudicatorAutoAllocation = true;
    // private bool isTeamAutoAllocation = true;
    private Random random = new Random();

    public DrawGenerator(int numberOfTeams, double beta = 4.0, double alpha = 1.0)
    {
        this.numberOfTeams = numberOfTeams;
        this.beta = beta;
        this.alpha = alpha;
        costMatrix = new int[numberOfTeams, numberOfPositions];
    }

    public void CalculateCosts(List<Team> teams)
    {
        for (int i = 0; i < numberOfTeams; i++)
        {
            for (int j = 0; j < numberOfPositions; j++)
            {
                costMatrix[i, j] = CalculateCost(teams[i], j);
            }
        }
    }

    private int CalculateCost(Team team, int position)
    {
        var history = GetPositionHistory(team);
        return (int)Math.Pow(PositionCostFunction(history, position), beta);
    }

    private int[] GetPositionHistory(Team team)
    {
        int[] history = new int[numberOfPositions];
        foreach (var roundData in team.teamRoundDatas)
        {
            switch (roundData.teamPositionBritish)
            {
                case TeamPositionsBritish.OG:
                    history[0]++;
                    break;
                case TeamPositionsBritish.OO:
                    history[1]++;
                    break;
                case TeamPositionsBritish.CG:
                    history[2]++;
                    break;
                case TeamPositionsBritish.CO:
                    history[3]++;
                    break;
            }
        }
        return history;
    }

    private double PositionCostFunction(int[] history, int position)
    {
        int n_h = history.Sum();
        double[] p_hat = new double[numberOfPositions];

        for (int i = 0; i < numberOfPositions; i++)
        {
            if (i == position)
            {
                p_hat[i] = (history[i] + 1) / (double)(n_h + 1);
            }
            else
            {
                p_hat[i] = history[i] / (double)(n_h + 1);
            }
        }

        double H_alpha;
        if (alpha == 1.0)
        {
            // Shannon entropy
            H_alpha = -p_hat.Sum(p => p * Math.Log(p, 2));
        }
        else
        {
            // Rényi entropy
            H_alpha = 1 / (1 - alpha) * Math.Log(p_hat.Sum(p => Math.Pow(p, alpha)), 2);
        }

        return n_h * (2 - H_alpha);
    }

 public List<Match> GenerateDraw(List<Team> teams, List<Adjudicator> adjudicators, bool isAdjudicatorAutoAllocation = true, bool isTeamAutoAllocation = true)
{
    // Log initial teams
    Debug.Log("Initial Teams: " + string.Join(", ", teams.Select(t => t.teamName)));

    List<Team> adjustedTeams = PullUp(teams);
    
    // Log teams after PullUp
    Debug.Log("Teams after PullUp: " + string.Join(", ", adjustedTeams.Select(t => t.teamName)));

    CalculateCosts(adjustedTeams);

    // Log cost matrix
    Debug.Log("Cost Matrix: ");
    for (int i = 0; i < adjustedTeams.Count; i++)
    {
        Debug.Log(string.Join(", ", Enumerable.Range(0, numberOfPositions).Select(j => costMatrix[i, j])));
    }

    int[,] shuffledCostMatrix = ShuffleMatrix(costMatrix);

    // Log shuffled cost matrix
    Debug.Log("Shuffled Cost Matrix: ");
    for (int i = 0; i < adjustedTeams.Count; i++)
    {
        Debug.Log(string.Join(", ", Enumerable.Range(0, numberOfPositions).Select(j => shuffledCostMatrix[i, j])));
    }

    int[] assignment = HungarianAlgorithm.Solve(shuffledCostMatrix);

    // Log assignment
    Debug.Log("Assignment: " + string.Join(", ", assignment));

    List<Match> matches = MapAssignmentToMatches(assignment, adjustedTeams, adjudicators);


    return matches;
}

    private List<Team> PullUp(List<Team> teams)
    {
        // Calculate winning points for each team
        foreach (var team in teams)
        {
            team.totalTeamScore = CalculateWinningPoints(team);
        }

        // Group teams by their winning points to form brackets
        var brackets = teams.GroupBy(t => (int)t.totalTeamScore).OrderByDescending(g => g.Key).ToList();
        List<Team> adjustedTeams = new List<Team>();
        Random random = new Random();

        // Create a new list to store the modified brackets
        var updatedBrackets = new List<IGrouping<int, Team>>(brackets);

        foreach (var bracket in brackets)
        {
            // Log bracket teams
            Debug.Log("Bracket: " + string.Join(", ", bracket.Select(t => t.teamName)));

            // Shuffle teams within each bracket
            var shuffledBracket = bracket.OrderBy(t => random.Next()).ToList();
            adjustedTeams.AddRange(shuffledBracket);
        }

        // Log adjusted teams
        Debug.Log("Adjusted Teams: " + string.Join(", ", adjustedTeams.Select(t => t.teamName)));

        return adjustedTeams;
    }
    private int CalculateWinningPoints(Team team)
    {
        int winningPoints = 0;
        foreach (var trd in team.teamRoundDatas)
        {
            switch (trd.teamMatchRanking)
            {
                case 1:
                    winningPoints += 3;
                    break;
                case 2:
                    winningPoints += 2;
                    break;
                case 3:
                    winningPoints += 1;
                    break;
                case 4:
                    winningPoints += 0;
                    break;
            }
        }
        return winningPoints;
    }
    private int[,] ShuffleMatrix(int[,] matrix)
        {
            int[] rowIndices = Enumerable.Range(0, matrix.GetLength(0)).OrderBy(x => random.Next()).ToArray();
            int[] colIndices = Enumerable.Range(0, matrix.GetLength(1)).OrderBy(x => random.Next()).ToArray();

            int[,] shuffledMatrix = new int[matrix.GetLength(0), matrix.GetLength(1)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    shuffledMatrix[i, j] = matrix[rowIndices[i], colIndices[j]];
                }
            }

            return shuffledMatrix;
        }
      private List<Match> MapAssignmentToMatches(int[] assignment, List<Team> teams, List<Adjudicator> adjudicators)
    {
        int numberOfRooms = teams.Count / 4;
        List<Match> matches = new List<Match>();
    
        for (int i = 0; i < numberOfRooms; i++)
        {
            Match match = new Match
            {
                matchId = AppConstants.instance.GenerateMatchID(MainRoundsPanel.Instance.selectedRound.roundId, i),
                teams = new Dictionary<string, string>(),
                adjudicators = AssignAdjudicators(adjudicators, numberOfRooms, i), // Assign adjudicators based on conditions
                selectedMotion = "Motion for the debate" // Placeholder for the motion
            };
    
            for (int j = 0; j < 4; j++)
            {
                int teamIndex = assignment[i * 4 + j];
                Team team = teams[teamIndex];
    
                // Check if a TRD already exists for the current round
                TeamRoundData teamRoundData = team.teamRoundDatas.FirstOrDefault(trd => trd.roundID == MainRoundsPanel.Instance.selectedRound.roundId);
    
                if (teamRoundData == null)
                {
                    Debug.Log($"<color=red>TeamRoundData not found for team {team.teamName} in round {MainRoundsPanel.Instance.selectedRound.roundId}</color>");
                    // Create a new TRD if it doesn't exist
                    teamRoundData = new TeamRoundData
                    {
                        teamId = team.teamId,
                        teamName = team.teamName,
                        teamCategory = team.teamCategory,
                        speakers = team.speakers,
                        teamRoundDataID = AppConstants.instance.GenerateTRDID(MainRoundsPanel.Instance.selectedRound.roundId, team.teamId),
                        roundType = MainRoundsPanel.Instance.selectedRound.roundCategory,
                        roundID = MainRoundsPanel.Instance.selectedRound.roundId,
                        matchID = match.matchId,
                        teamPositionBritish = (TeamPositionsBritish)j,
                        teamScore = 0,
                        teamMatchRanking = 0,
                        speakersInRound = new List<SpeakerRoundData>()
                    };
    
                    for (int k = 0; k < team.speakers.Count; k++)
                    {
                        SpeakerRoundData speakerRoundData = new SpeakerRoundData(team, k);
                        teamRoundData.speakersInRound.Add(speakerRoundData);
                    }
    
                    team.teamRoundDatas.Add(teamRoundData);
                }
                else
                {
                    Debug.Log($"<color=green>TeamRoundData found for team {team.teamName} in round {MainRoundsPanel.Instance.selectedRound.roundId}</color>");
                    // Update the existing TRD, preserving additional information
                    teamRoundData.matchID = match.matchId;
                    teamRoundData.teamPositionBritish = (TeamPositionsBritish)j;
    
                    // Preserve existing scores and rankings
                    // teamRoundData.teamScore = teamRoundData.teamScore;
                    teamRoundData.teamMatchRanking = 0;
                    teamRoundData.teamScore = 0;
                    teamRoundData.speakersInRound.Clear();
                    teamRoundData.speakersInRound = new List<SpeakerRoundData>();

                    for (int k = 0; k < team.speakers.Count; k++)
                    {
                        SpeakerRoundData speakerRoundData = new SpeakerRoundData(team, k);
                        teamRoundData.speakersInRound.Add(speakerRoundData);
                    }
                }
                match.teams[team.teamId] = teamRoundData.teamRoundDataID;
            }
    
            matches.Add(match);
        }
    
        return matches;
    }
    
    private Adjudicator[] AssignAdjudicators(List<Adjudicator> adjudicators, int totalMatches, int matchIndex)
    {
        if (isAdjudicatorAutoAllocation)
        {
            List<Adjudicator> assignedAdjudicators = new List<Adjudicator>();
    
            // Separate CAP and Normie adjudicators
            List<Adjudicator> capAdjudicators = adjudicators.Where(a => a.adjudicatorType == AdjudicatorTypes.CAP).ToList();
            List<Adjudicator> normieAdjudicators = adjudicators.Where(a => a.adjudicatorType == AdjudicatorTypes.Normie).ToList();
    
            // Assign CAP adjudicators to the first matches
            if (matchIndex < capAdjudicators.Count)
            {
                assignedAdjudicators.Add(capAdjudicators[matchIndex]);
            }
            else if (matchIndex < capAdjudicators.Count + normieAdjudicators.Count)
            {
                assignedAdjudicators.Add(normieAdjudicators[matchIndex - capAdjudicators.Count]);
            }
    
            // Assign remaining Normie adjudicators randomly
            if (matchIndex == totalMatches - 1 && normieAdjudicators.Count > 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(normieAdjudicators.Count);
                assignedAdjudicators.Add(normieAdjudicators[randomIndex]);
            }
    
            return assignedAdjudicators.ToArray();
        }
        else
        {
            // Manual allocation, return an empty array
            return new Adjudicator[0];
        }
    }
// Hungarian Algorithm Implementation (Placeholder)
    public static class HungarianAlgorithm
    {
        public static int[] Solve(int[,] costMatrix)
        {
            int rows = costMatrix.GetLength(0);
            int cols = costMatrix.GetLength(1);
            int dim = Math.Max(rows, cols);

            int[,] cost = new int[dim, dim];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    cost[i, j] = costMatrix[i, j];

            int[] u = new int[dim];
            int[] v = new int[dim];
            int[] p = new int[dim];
            int[] way = new int[dim];

            for (int i = 1; i < dim; ++i)
            {
                p[0] = i;
                int j0 = 0;
                int[] minv = new int[dim];
                bool[] used = new bool[dim];
                for (int j = 0; j < dim; ++j)
                    minv[j] = int.MaxValue;

                do
                {
                    used[j0] = true;
                    int i0 = p[j0], delta = int.MaxValue, j1 = 0;
                    for (int j = 1; j < dim; ++j)
                    {
                        if (!used[j])
                        {
                            int cur = cost[i0, j] - u[i0] - v[j];
                            if (cur < minv[j])
                            {
                                minv[j] = cur;
                                way[j] = j0;
                            }
                            if (minv[j] < delta)
                            {
                                delta = minv[j];
                                j1 = j;
                            }
                        }
                    }
                    for (int j = 0; j < dim; ++j)
                    {
                        if (used[j])
                        {
                            u[p[j]] += delta;
                            v[j] -= delta;
                        }
                        else
                        {
                            minv[j] -= delta;
                        }
                    }
                    j0 = j1;
                } while (p[j0] != 0);

                do
                {
                    int j1 = way[j0];
                    p[j0] = p[j1];
                    j0 = j1;
                } while (j0 != 0);
            }

            int[] result = new int[rows];
            for (int j = 1; j < dim; ++j)
            {
                if (p[j] < rows)
                {
                    result[p[j]] = j;
                }
            }

            return result;
        }
    }
}

#endregion
