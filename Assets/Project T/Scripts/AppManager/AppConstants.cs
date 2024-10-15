using System.Collections.Generic;
using UnityEngine;
using Scripts.Resources;
using Firebase.Firestore;
using Scripts.FirebaseConfig;
using System.Linq;
using System;
using System.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;

#region Data Classes
#region Admin Info

public class Admin
{
    public string UserID { get; set; }

    public AdminCategories AdminCategory { get; set; }
    public string Name { get; set; }

    // Constructor
    public Admin(string userID, AdminCategories adminCategory, string name)
    {
        UserID = userID;
        AdminCategory = adminCategory;
        Name = name;
    }

    // Parameterless constructor
    public Admin()
    {
    }
}
#endregion

#region  Tournament Info
public class SpeakerCategories
{

    public SpeakerTypes speakerType { get; set; }

    public BreakTypes breakType { get; set; }
}
public class TournamentInfo
{

    public string tournamentId { get; set; }

    public TournamentType tournamentType { get; set; }

    public string tournamentName { get; set; }
    public string tournamentShortHand { get; set; }
    public int noOfPrelims { get; set; }
    public List<Instituitions> instituitionsinTourney { get; set; }
    public List<Adjudicator> adjudicatorsInTourney { get; set; }
    public List<SpeakerCategories> speakerCategories { get; set; }
    public List<Team> teamsInTourney { get; set; }
    public List<string> openBreakingTeams { get; set; }
    public List<string> noviceBreakingTeams { get; set; }
    public List<Rounds> preLimsInTourney { get; set; }
    public List<Rounds> noviceBreaksInTourney { get; set; }
    public List<Rounds> openBreaksInTourney { get; set; }
    public BreakParameters breakParam { get; set; }
    public bool isBreaksGenerated { get; set;}

    public TournamentInfo()
    {
        speakerCategories = new List<SpeakerCategories>
            {
                new SpeakerCategories { speakerType = SpeakerTypes.Open, breakType = BreakTypes.QF },
            };
        instituitionsinTourney = new List<Instituitions>();
        teamsInTourney = new List<Team>();
        adjudicatorsInTourney = new List<Adjudicator>();
        preLimsInTourney = new List<Rounds>();
        noviceBreaksInTourney = new List<Rounds>();
        openBreaksInTourney = new List<Rounds>();
        isBreaksGenerated = false;
        breakParam = BreakParameters.TeamPoints;
    }
}
public class Instituitions : TournamentInfo
{
    public string instituitionID { get; set; }

    public string instituitionName { get; set; }

    public string instituitionAbreviation { get; set; }
    public Instituitions()
    {
        instituitionID = "";
        instituitionName = "Instituition Name";
        instituitionAbreviation = "IN";
    }
}
public class Adjudicator : TournamentInfo
{

    public string adjudicatorID;

    public string adjudicatorName;
    public string adjudicatorEmail;
    public string adjudicatorPhone;
    public bool available = false;
    public AdjudicatorTypes adjudicatorType;
    public string instituitionID;
    public Adjudicator()
    {
        adjudicatorID = "";
        adjudicatorName = "Adjudicator Name";
        adjudicatorEmail = "smth@gmail.com";
        adjudicatorPhone = "00000000000";
        adjudicatorType = AdjudicatorTypes.Normie;
        instituitionID = "";
    }
}
#endregion

#region Team Info
public class Team : TournamentInfo
{

    public string teamId;

    public string teamName;

    public SpeakerTypes teamCategory;
    public string instituition;
    public List<TeamRoundData> teamRoundDatas;
    public List<Speaker> speakers;
    public bool available = false;
    public float totalTeamScore;
    public int teamPoints;
    public bool isEligibleforBreak;
    public Team()
    {
        teamId = "";
        teamName = "Team Name";
        teamCategory = SpeakerTypes.Open;
        instituition = "";
        speakers = new List<Speaker>();
        teamRoundDatas = new List<TeamRoundData>();
        totalTeamScore = 0;
        teamPoints = 0;
        isEligibleforBreak = false;
    }
}
public class Speaker : Team
{

    public string speakerId { get; set; }

    public string speakerName { get; set; }

    public string speakerEmail { get; set; }

    public string speakerContact { get; set; }

    public Speaker()
    {
        speakerId = "";
        speakerName = "Speaker Name";
        speakerEmail = "";
        speakerContact = "";
    }
}

#endregion

#region Rounds Info
public class Rounds : TournamentInfo
{
    public string roundId;
    public RoundTypes roundType;
    public RoundCategory roundCategory;
    public Dictionary<string, string> motions;
    public List<Team> availableTeams;
    public List<Adjudicator> availableAdjudicators;
    public RoundStates roundState;
    public List<TeamRoundData> swings;
    public List<Match> matches;
        public bool motionAdded = false;
        public bool teamAttendanceAdded = false;
        public bool AdjudicatorAttendanceAdded = false;
        public bool drawGenerated = false;
        public bool ballotsAdded = false;
        public bool isSilent = false;
        public bool isPublic = false;

    // FOR DRAWS : be like the number of matches ibn this round, based upon the draw and the data in matches will be set by the generate draw function
    public Rounds()
    {
        roundId = "";
        roundType = RoundTypes.PreLim;
        roundCategory = RoundCategory.PreLim;
        motions = new Dictionary<string, string>();
        availableTeams = new List<Team>();
        availableAdjudicators = new List<Adjudicator>();
        swings = new List<TeamRoundData>();
        matches = new List<Match>();

    }
}
public class Match : Rounds
{
    public string matchId;
    public Dictionary<string, string> teams; // store team id and team round data id
    public Adjudicator[] adjudicators;
    public bool ballotEntered = false;
    public string selectedMotion;
    public Match()
    {
        matchId = "";
        teams = new Dictionary<string, string>();
        adjudicators = new Adjudicator[1];
        selectedMotion = "";
    }
}
public class TeamRoundData : Team
{
    public string teamRoundDataID;
    public RoundCategory roundType;
    public string roundID;
    public string matchID;
    public TeamPositionsAsian teamPositionAsian;
    public TeamPositionsBritish teamPositionBritish;
    public float teamScore;
    public int teamMatchRanking;
    public List<SpeakerRoundData> speakersInRound;
    public TeamRoundData()
    {
        teamRoundDataID = "";
        teamId = "";
        teamPositionAsian = TeamPositionsAsian.Gov;
        teamPositionBritish = TeamPositionsBritish.OG;
        teamScore = 0;
        teamMatchRanking = 0;
        speakersInRound = new List<SpeakerRoundData>();
    }
}
public class SpeakerRoundData : TeamRoundData
{
    public string speakerId;
    public int speakerSpeakingPosition;
    public float speakerScore;

    public SpeakerRoundData()
    {
        speakerId = "";
        speakerSpeakingPosition = 0;
        speakerScore = 0;
    }
    public SpeakerRoundData(Team team, int speakingPosition)
    {
        if (team.speakers != null && team.speakers.Count > speakingPosition)
        {
            var speaker = team.speakers[speakingPosition];
            speakerId = speaker.speakerId;
            speakerSpeakingPosition = speakingPosition;
            speakerScore = 0;
        }
        else
        {
            speakerId = "";
            speakerSpeakingPosition = speakingPosition;
            speakerScore = 0;
        }
    }
}

#endregion
#endregion

public class AppConstants : MonoBehaviour
{
    #region Singleton
    public static AppConstants instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Public Variables
    public List<TournamentInfo> tournaments;
    public TournamentInfo selectedTouranment;
    public Admin selectedAdmin;
    public Rounds selectedRound;
    #endregion

    private void Start()
    {
        selectedAdmin = new Admin();
        tournaments = new List<TournamentInfo>();
    }

    #region DataClass Functions
    #region Tournament Functions
    public async Task AddTournament(TournamentInfo tournament)
    {
        if (tournaments == null)
        {
            tournaments = new List<TournamentInfo>();
        }
        tournaments.Add(tournament);
        selectedTouranment = tournament;
        FirestoreManager.FireInstance.SaveTournamentToFireStore(tournament);
        Debug.Log("Prelims in Tournament: " + tournament.preLimsInTourney.Count);
        Debug.Log("Novice Breaks in Tournament: " + tournament.noviceBreaksInTourney.Count);
        Debug.Log("Open Breaks in Tournament: " + tournament.openBreaksInTourney.Count);
        await Task.Delay(1500);
        await SaveAllRound(tournament);
    }
    public string GenerateTournamentID(string tournamentShortHand)
    {
        int numberOfTournaments = tournaments.Count;
        return $"TOURNEY_{numberOfTournaments}_{tournamentShortHand}";
    }
    public void RemoveTournament(TournamentInfo tournament)
    {
        tournaments.Remove(tournament);
    }
    public bool CheckTournamentExists()
    {
        if (tournaments == null)
            return false;
        return true;
    }
    public TournamentInfo GetTournament(string tournamentId)
    {
        return tournaments.Find(t => t.tournamentId == tournamentId);
    }
    public async Task UpdateTournamentInfo(TournamentInfo tournament)
    {
        TournamentInfo _tournament = tournaments.Find(t => t.tournamentId == tournament.tournamentId);
        _tournament = tournament;
        await AddTournament(_tournament);
    }
    #endregion

    #region Debug Functions
    public void DebugTournamentInfo()
    {
        if (tournaments == null)
        {
            Debug.Log("No tournaments found!");
            return;
        }
        foreach (TournamentInfo tournament in tournaments)
        {
            Debug.Log("Tournament ID: " + tournament.tournamentId);
            Debug.Log("Tournament Name: " + tournament.tournamentName);
            Debug.Log("Tournament Type: " + tournament.tournamentType);
            Debug.Log("Number of Prelims: " + tournament.noOfPrelims);
            foreach (SpeakerCategories speakerCategory in tournament.speakerCategories)
            {
                Debug.Log("Speaker Type: " + speakerCategory.speakerType);
                Debug.Log("Break Type: " + speakerCategory.breakType);
            }
        }
    }

    public void DebugAdminInfo()
    {
        Debug.Log("<color = blue>Admin Name: " + selectedAdmin.Name + "</color>");
        Debug.Log("<color = blue>Admin Category: " + selectedAdmin.AdminCategory + "</color>");
    }

    #endregion

    #region Institute Functions
    public string InstituteIDGenerator(string abrv)
    {
        string selectedTournamentID = selectedTouranment.tournamentId.ToString();
        int numberOfThisInstitute = selectedTouranment.instituitionsinTourney.Count();
        string instituteAbbreviation = abrv;
        return $"I_{selectedTournamentID}_{numberOfThisInstitute}_{instituteAbbreviation}";
    }
    public Instituitions GetInstituitionsFromID(string instituteID)
    {
        return selectedTouranment.instituitionsinTourney.Find(i => i.instituitionID == instituteID);
    }
    public string GetInstituteAbreviation(string instituteID)
    {
        return GetInstituitionsFromID(instituteID).instituitionAbreviation;
    }
    #endregion

    #region Team Functions
    public string GenerateTeamID(string teamName)
    {
        string selectedTournamentID = selectedTouranment.tournamentId.ToString();
        int numberOfThisTeam = selectedTouranment.teamsInTourney.Count();
        string _teamName = teamName.Length >= 3 ? teamName.Substring(0, 3) : teamName;
        return $"T_{selectedTournamentID}_{numberOfThisTeam}_{_teamName}";
    }
    public string GenerateSpeakerID(Speaker speaker, string team_id, int index)
    {
        string teamID = team_id;
        string speakerName = speaker.speakerName.Length >= 3 ? speaker.speakerName.Substring(0, 3) : speaker.speakerName;
        return $"S{teamID}_{index}_{speakerName}";
    }
    internal void AddTournament(TournamentInfo_DTO tournament)
    {
        throw new NotImplementedException();
    }
    public Team GetTeamFromID(string teamID)
    {
        return selectedTouranment.teamsInTourney.Find(t => t.teamId == teamID);
    }
    public List<Team> GetEligibleTeamsForBreaks(SpeakerTypes speakerType)
    {
        return selectedTouranment.teamsInTourney.Where(t => t.teamCategory == speakerType).ToList();
    }
    public TeamRoundData GetTeamRoundData(string teamId, string teamrounddataid)
    {
        return GetTeamFromID(teamId).teamRoundDatas.Find(trd => trd.teamRoundDataID == teamrounddataid);
    }
    public List<Team> GetTeamsFromIDs(List<string> teamIDs)
    {
        List<Team> teams = new List<Team>();
        foreach (string teamID in teamIDs)
        {
            teams.Add(GetTeamFromID(teamID));
        }
        return teams;
    }
    public void CalculateTeamPoints()
    {
        foreach (Team team in selectedTouranment.teamsInTourney)
        {
            int totalPoints = 0;
    
            foreach (var trd in team.teamRoundDatas)
            {
                if(trd.roundType == RoundCategory.PreLim)
                {
                    switch (trd.teamMatchRanking)
                    {
                        case 1:
                            totalPoints += 3;
                            break;
                        case 2:
                            totalPoints += 2;
                            break;
                        case 3:
                            totalPoints += 1;
                            break;
                        case 4:
                            totalPoints += 0;
                            break;
                        default:
                            Debug.LogWarning("Invalid position: " + trd.teamMatchRanking);
                            break;
                    }
                }
            }
    
            team.teamPoints = totalPoints;
        }
    }
    public void CalculateTeamScore()
    {
        foreach (Team team in selectedTouranment.teamsInTourney)
        {
            float totalScore = 0;
    
            foreach (var roundData in team.teamRoundDatas)
            {
                if (roundData.roundType == RoundCategory.PreLim)
                {
                    foreach (var speakerScore in roundData.speakersInRound)
                    {
                        totalScore += speakerScore.speakerScore;
                    }
                }
            }
    
            team.totalTeamScore = totalScore;
        }
    }
public TeamRoundData GetTeamsSpeakerRoundDataFromMatch(string matchID, string teamID)
{
    // Find the match with the given matchID in the selected round
    Match match = selectedRound.matches.Find(m => m.matchId == matchID);
    
    // Check if the match is found
    if (match == null)
    {
        Debug.LogError($"Match with ID {matchID} not found.");
        return null;
    }

    // Check if the team exists in the match
    if (!match.teams.ContainsKey(teamID))
    {
        Debug.LogError($"Team with ID {teamID} not found in match {matchID}.");
        return null;
    }

    // Get the TeamRoundData for the team
    TeamRoundData trd = GetTRDFromID(teamID, match.teams[teamID]);

    // Return the TeamRoundData
    return trd;
}
    #endregion
    #region Adjudicator Functions
    public string GenerateAdjudicatorID(string adjName, string adjInstitute)
    {
        string tournamentid = selectedTouranment.tournamentId;
        int adjCount = selectedTouranment.adjudicatorsInTourney.Count;
        string adjNameShort = adjName.Length >= 3 ? adjName.Substring(0, 3) : adjName;

        return $"A_{tournamentid}_{adjInstitute}_{adjCount}_{adjNameShort}";
    }
    #endregion
    #region Round Functions
    //make a function to get current round
    public Rounds GetCurrentRound()
    {
        // Check preLimsInTourney for the selected round
        Rounds selectedRound = selectedTouranment.preLimsInTourney?.FirstOrDefault(round => round.roundState == RoundStates.InProgress);
        if (selectedRound != null)
        {
            return selectedRound;
        }

        // Check noviceBreaksInTourney for the selected round
        selectedRound = selectedTouranment.noviceBreaksInTourney?.FirstOrDefault(round => round.roundState == RoundStates.InProgress);
        if (selectedRound != null)
        {
            return selectedRound;
        }

        // Check openBreaksInTourney for the selected round
        selectedRound = selectedTouranment.openBreaksInTourney?.FirstOrDefault(round => round.roundState == RoundStates.InProgress);
        if (selectedRound != null)
        {
            return selectedRound;
        }

        // Return null if no selected round is found
        return null;
    }


    public string GenerateRoundID(string roundType)
    {
        string selectedTournamentID = selectedTouranment.tournamentId.ToString();
        int numberOfThisRound = 0;
        switch (roundType)
        {
            case "PreLim":
                numberOfThisRound = selectedTouranment.preLimsInTourney.Count();
                break;
            case "NoviceBreak":
                numberOfThisRound = selectedTouranment.noviceBreaksInTourney.Count();
                break;
            case "OpenBreak":
                numberOfThisRound = selectedTouranment.openBreaksInTourney.Count();
                break;
        }
        return $"R_{selectedTournamentID}_{numberOfThisRound}_{roundType}";
    }
    public async Task SaveAllRound(TournamentInfo selectedTouranment)
    {
        foreach (Rounds round in selectedTouranment.preLimsInTourney)
        {
            Debug.Log("Saving Prelim Rounds");
            await FirestoreManager.FireInstance.SaveRoundAsync(RoundCategory.PreLim.ToString(), round);
        }
        foreach (Rounds round in selectedTouranment.noviceBreaksInTourney)
        {
            await FirestoreManager.FireInstance.SaveRoundAsync(RoundCategory.NoviceBreak.ToString(), round);
        }
        foreach (Rounds round in selectedTouranment.openBreaksInTourney)
        {
            await FirestoreManager.FireInstance.SaveRoundAsync(RoundCategory.OpenBreak.ToString(), round);
        }
        return;
    }
    public async Task GetAllRounds()
    {
        selectedTouranment.preLimsInTourney = await FirestoreManager.FireInstance.GetAllRoundsFromFirestore(RoundCategory.PreLim.ToString());
        selectedTouranment.noviceBreaksInTourney = await FirestoreManager.FireInstance.GetAllRoundsFromFirestore(RoundCategory.NoviceBreak.ToString());
        selectedTouranment.openBreaksInTourney = await FirestoreManager.FireInstance.GetAllRoundsFromFirestore(RoundCategory.OpenBreak.ToString());
        Debug.Log("Prelims in Tournament: " + selectedTouranment.preLimsInTourney.Count);
        Debug.Log("Novice Breaks in Tournament: " + selectedTouranment.noviceBreaksInTourney.Count);
        Debug.Log("Open Breaks in Tournament: " + selectedTouranment.openBreaksInTourney.Count);
    }
    public void PrintRankings()
{
    List<Rounds> allRounds = new List<Rounds>();
    allRounds.AddRange(selectedTouranment.preLimsInTourney);
    allRounds.AddRange(selectedTouranment.noviceBreaksInTourney);
    allRounds.AddRange(selectedTouranment.openBreaksInTourney);

    foreach (Rounds round in allRounds)
    {
        Debug.Log($"Round ID: {round.roundId}, Round Type: {round.roundType}");

        var teamRankings = round.availableTeams
            .Select(team => new
            {
                Team = team,
                TotalSpeakerPoints = team.teamRoundDatas.Sum(trd => trd.speakersInRound.Sum(srd => srd.speakerScore)),
                TeamPoints = team.teamRoundDatas.Sum(trd => trd.teamScore)
            })
            .OrderByDescending(tr => tr.TeamPoints)
            .ThenByDescending(tr => tr.TotalSpeakerPoints)
            .ToList();

        for (int i = 0; i < teamRankings.Count; i++)
        {
            var ranking = teamRankings[i];
            Debug.Log($"Rank: {i + 1}, Team Name: {ranking.Team.teamName}, Institution: {ranking.Team.instituition}, Total Speaker Points: {ranking.TotalSpeakerPoints}, Team Points: {ranking.TeamPoints}");
        }
    }
}
    public string GenerateMatchID(string roundID, int matchCount)
    {
        string selectedTournamentID = selectedTouranment.tournamentId.ToString();
        return $"M_{selectedTournamentID}_{roundID}_{matchCount}";
    }
    public string GenerateTRDID(string teamID, string roundID)
    {
        string selectedTournamentID = selectedTouranment.tournamentId.ToString();
        int numberOfThisTRD = selectedTouranment.teamsInTourney.Count();
        return $"TRD_{selectedTournamentID}_{teamID}_{roundID}_{numberOfThisTRD}";
    }
    public string GenerateSRDID(string speakerID, string roundID)
    {
        string selectedTournamentID = selectedTouranment.tournamentId.ToString();
        int numberOfThisSRD = selectedTouranment.teamsInTourney.Count();
        return $"SRD_{selectedTournamentID}_{speakerID}_{roundID}_{numberOfThisSRD}";
    }
    public TeamRoundData GetTRDFromID(string teamId, string TRDId)
    {
        return GetTeamFromID(teamId).teamRoundDatas.Find(trd => trd.teamRoundDataID == TRDId);
    }
    public void AddTRD(TeamRoundData trd, string teamId)
    {
        Team myTeam = GetTeamFromID(teamId);
        // Assuming you have a collection to store TeamRoundData, e.g., a dictionary
        var existingTRD = myTeam.teamRoundDatas.FirstOrDefault(t => t.teamRoundDataID == teamId);
        if (existingTRD != null)
        {
            // Update existing TeamRoundData
            int index = myTeam.teamRoundDatas.IndexOf(existingTRD);
            myTeam.teamRoundDatas[index] = trd;
        }
        else
        {
            myTeam.teamRoundDatas.Add(trd);
        }
    }
    
    #endregion
   
   
   
    #endregion
}