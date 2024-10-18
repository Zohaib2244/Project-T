using System.Collections.Generic;
using Firebase.Firestore;
using Scripts.Resources;
using System;
using System.Linq;
using UnityEngine;

namespace Scripts.FirebaseConfig
{
    public static class DTOExtensions
    {
        public static Dictionary<string, object> ToDictionary(this Rounds_DTO roundDTO)
        {
            return new Dictionary<string, object>
            {
                { "roundId", roundDTO.roundId },
                { "roundType", roundDTO.roundType },
                { "roundCategory", roundDTO.roundCategory },
                { "motions", roundDTO.motions },
                { "availableTeamsIds", roundDTO.availableTeamsIds },
                { "availableAdjudicatorsIds", roundDTO.availableAdjudicatorsIds },
                { "roundState", roundDTO.roundState },
                { "swings", roundDTO.swings },
            };
        }

        public static Dictionary<string, object> ToDictionary(this Match_DTO matchDTO)
        {
            return new Dictionary<string, object>
            {
                { "matchId", matchDTO.matchId },
                { "adjudicatorsIds", matchDTO.adjudicatorsIds },
                { "selectedMotion", matchDTO.selectedMotion },
                { "teamsinMatchIds", matchDTO.teamsinMatchIds }
            };
        }

        public static Dictionary<string, object> ToDictionary(this TeamRoundData_DTO teamRoundDataDTO)
        {
            return new Dictionary<string, object>
            {
                { "teamRoundDataID", teamRoundDataDTO.teamRoundDataID },
                { "teamId", teamRoundDataDTO.teamId },
                { "teamPositionAsian", teamRoundDataDTO.teamPositionAsian },
                { "teamPositionBritish", teamRoundDataDTO.teamPositionBritish },
                { "teamScore", teamRoundDataDTO.teamScore },
                { "teamMatchRanking", teamRoundDataDTO.teamMatchRanking }
            };
        }

        public static Dictionary<string, object> ToDictionary(this SpeakerRoundData_DTO speakerRoundDataDTO)
        {
            return new Dictionary<string, object>
            {
                { "speakerRoundDataID", speakerRoundDataDTO.speakerRoundDataID },
                { "speakerId", speakerRoundDataDTO.speakerId },
                { "speakerSpeakingPosition", speakerRoundDataDTO.speakerSpeakingPosition }
            };
        }
    }


    #region Converters
    public interface IFirestoreConverter<T>
    {
        object ToFirestore(T value);
        T FromFirestore(object value);
    }
    public class AdjudicatorTypesConverter : FirestoreConverter<AdjudicatorTypes>
    {
        public override AdjudicatorTypes FromFirestore(object value)
        {
            if (value is string stringValue)
            {
                return Enum.Parse<AdjudicatorTypes>(stringValue);
            }
            throw new ArgumentException("Invalid data type for AdjudicatorTypes");
        }

        public override object ToFirestore(AdjudicatorTypes value)
        {
            return value.ToString();
        }
    }
    public class SpeakerTypesConverter : FirestoreConverter<SpeakerTypes>
    {
        public override SpeakerTypes FromFirestore(object value)
        {
            if (value is string stringValue)
            {
                return Enum.Parse<SpeakerTypes>(stringValue);
            }
            throw new ArgumentException("Invalid data type for SpeakerTypes");
        }

        public override object ToFirestore(SpeakerTypes value)
        {
            return value.ToString();
        }
    }
    public class BreakTypesConverter : FirestoreConverter<BreakTypes>
    {
        public override BreakTypes FromFirestore(object value)
        {
            if (value is string stringValue)
            {
                return Enum.TryParse<BreakTypes>(stringValue, out var result) ? result : default;
            }
            throw new ArgumentException("Invalid data type for BreakTypes");
        }

        public override object ToFirestore(BreakTypes value)
        {
            return value.ToString();
        }
    }
    public class RoundTypesConverter : FirestoreConverter<RoundTypes>
    {
        public override RoundTypes FromFirestore(object value)
        {
            if (value is string stringValue)
            {
                return Enum.Parse<RoundTypes>(stringValue);
            }
            throw new ArgumentException("Invalid data type for RoundTypes");
        }

        public override object ToFirestore(RoundTypes value)
        {
            return value.ToString();
        }
    }

    public class TeamPositionsAsianConverter : FirestoreConverter<TeamPositionsAsian>
    {
        public override TeamPositionsAsian FromFirestore(object value)
        {
            if (value is string stringValue)
            {
                return Enum.Parse<TeamPositionsAsian>(stringValue);
            }
            throw new ArgumentException("Invalid data type for TeamPositionsAsian");
        }

        public override object ToFirestore(TeamPositionsAsian value)
        {
            return value.ToString();
        }
    }

    public class TeamPositionsBritishConverter : FirestoreConverter<TeamPositionsBritish>
    {
        public override TeamPositionsBritish FromFirestore(object value)
        {
            if (value is string stringValue)
            {
                return Enum.Parse<TeamPositionsBritish>(stringValue);
            }
            throw new ArgumentException("Invalid data type for TeamPositionsBritish");
        }

        public override object ToFirestore(TeamPositionsBritish value)
        {
            return value.ToString();
        }
    }
    public class RoundStatesConverter : IFirestoreConverter<RoundStates>
    {
        public object ToFirestore(RoundStates value)
        {
            return value.ToString();
        }

        public RoundStates FromFirestore(object value)
        {
            return (RoundStates)Enum.Parse(typeof(RoundStates), value.ToString());
        }
    }

    #endregion

    #region Admin Info
    [FirestoreData]
    public class Admin_DTO
    {
        [FirestoreDocumentId]
        public string UserID { get; set; }
        [FirestoreProperty]
        public AdminCategories AdminCategory { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
    }
    #endregion

    #region  Tournament Info
    [FirestoreData]
    public class SpeakerCategories_DTO
    {
        [FirestoreProperty(ConverterType = typeof(SpeakerTypesConverter))]
        public SpeakerTypes speakerType { get; set; }

        [FirestoreProperty(ConverterType = typeof(BreakTypesConverter))]
        public BreakTypes breakType { get; set; }
    }
    [FirestoreData]
    public class TournamentInfo_DTO
    {
        [FirestoreDocumentId]
        public string tournamentId { get; set; }
        [FirestoreProperty]
        public TournamentType tournamentType { get; set; }
        [FirestoreProperty]
        public string tournamentName { get; set; }
        [FirestoreProperty]
        public string tournamentShortHand { get; set; }
        [FirestoreProperty]
        public int noOfPrelims { get; set; }
        [FirestoreProperty]
        public List<string> openBreakingTeams { get; set; }
        [FirestoreProperty]
        public List<string> noviceBreakingTeams { get; set; }
        [FirestoreProperty]
        public List<SpeakerCategories_DTO> speakerCategories { get; set; }
        [FirestoreProperty]
        public bool isBreaksGenerated { get; set; }
        [FirestoreProperty]
        public int breakParameters { get; set; }
    }
    [FirestoreData]
    public class Instituitions_DTO
    {
        [FirestoreDocumentId]
        public string instituitionID { get; set; }
        [FirestoreProperty]
        public string instituitionName { get; set; }
        [FirestoreProperty]
        public string instituitionAbreviation { get; set; }
        [FirestoreProperty]
        public string tournamentId { get; set; }
    }

    [FirestoreData]
    public class Adjudicator_DTO
    {
        [FirestoreDocumentId]
        public string adjudicatorId { get; set; }
        [FirestoreProperty]
        public string adjudicatorName { get; set; }
        [FirestoreProperty]
        public string institutionID { get; set; }
        [FirestoreProperty]
        public string adjudicatorEmail { get; set; }
        [FirestoreProperty]
        public string adjudicatorPhone { get; set; }
        [FirestoreProperty]
        public AdjudicatorTypes adjudicatorType { get; set; }
    }
    #endregion

    #region Rounds Info
    [FirestoreData]
    public class Rounds_DTO
    {
        [FirestoreDocumentId]
        public string roundId { get; set; }

        [FirestoreProperty(ConverterType = typeof(RoundTypesConverter))]
        public RoundTypes roundType { get; set; }

        [FirestoreProperty]
        public int roundCategory { get; set; }

        [FirestoreProperty]
        public Dictionary<string, string> motions { get; set; }

        [FirestoreProperty]
        public List<string> availableTeamsIds { get; set; }

        [FirestoreProperty]
        public List<string> availableAdjudicatorsIds { get; set; }

        [FirestoreProperty]
        public RoundStates roundState { get; set; }

        [FirestoreProperty]
        public int swings { get; set; }
        [FirestoreProperty]
        public bool motionAdded { get; set; }
        [FirestoreProperty]
        public bool teamAttendanceAdded { get; set; }
        [FirestoreProperty]
        public bool AdjudicatorAttendanceAdded { get; set; }
        [FirestoreProperty]
        public bool drawGenerated { get; set; }
        [FirestoreProperty]
        public bool ballotsAdded { get; set; }
        [FirestoreProperty]
        public bool isPublic { get; set; }
        [FirestoreProperty]
        public bool isSilent { get; set; }
    }

    [FirestoreData]
    public class Match_DTO
    {
        [FirestoreDocumentId]
        public string matchId { get; set; }
        [FirestoreProperty]
        public string[] adjudicatorsIds { get; set; }
        [FirestoreProperty]
        public string selectedMotion { get; set; }
        [FirestoreProperty]
        public Dictionary<string, string> teamsinMatchIds { get; set; }
        [FirestoreProperty]
        public bool ballotEntered { get; set; }
    }

    [FirestoreData]
    public class TeamRoundData_DTO
    {
        [FirestoreDocumentId]
        public string teamRoundDataID { get; set; }
        [FirestoreProperty]
        public int roundCategory { get; set; }
        [FirestoreProperty]
        public string roundId { get; set; }
        [FirestoreProperty]
        public string matchId { get; set; }
        [FirestoreProperty]
        public string teamId { get; set; }
        [FirestoreProperty(ConverterType = typeof(TeamPositionsAsianConverter))]
        public TeamPositionsAsian teamPositionAsian { get; set; }
        [FirestoreProperty(ConverterType = typeof(TeamPositionsBritishConverter))]
        public TeamPositionsBritish teamPositionBritish { get; set; }
        [FirestoreProperty]
        public float teamScore { get; set; }
        [FirestoreProperty]
        public int teamMatchRanking { get; set; }
        [FirestoreProperty]
        public List<SpeakerRoundData_DTO> speakersInRound { get; set; } // Add this property
    }

    [FirestoreData]
    public class SpeakerRoundData_DTO
    {
        [FirestoreDocumentId]
        public string speakerRoundDataID { get; set; }
        [FirestoreProperty]
        public string speakerId { get; set; }
        [FirestoreProperty]
        public int speakerSpeakingPosition { get; set; }
        [FirestoreProperty]
        public float speakerScore { get; set; }
    }
    #endregion



    #region Team Info
    [FirestoreData]
    public class Team_DTO
    {
        [FirestoreDocumentId]
        public string teamId { get; set; }
        [FirestoreProperty]
        public string tournamentId { get; set; }
        [FirestoreProperty]
        public string instituitionID { get; set; }
        [FirestoreProperty]
        public string teamName { get; set; }
        [FirestoreProperty]
        public SpeakerTypes teamCategory { get; set; }
        [FirestoreProperty]
        public float totalTeamScore { get; set; }
        [FirestoreProperty]
        public int teamPoints { get; set; }
        [FirestoreProperty]
        public bool isEligibleforBreak { get; set; }
    }

    [FirestoreData]
    public class Speaker_DTO
    {
        [FirestoreDocumentId]
        public string speakerId { get; set; }
        [FirestoreProperty]
        public string speakerTeamId { get; set; }
        [FirestoreProperty]
        public string tournamentId { get; set; }
        [FirestoreProperty]
        public string speakerName { get; set; }
        [FirestoreProperty]
        public string speakerEmail { get; set; }
        [FirestoreProperty]
        public string speakerContact { get; set; }
    }
    #endregion

    #region DTO Converters
    public class DTOConverter
    {
        //make singelton
        private static DTOConverter instance;
        public static DTOConverter Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DTOConverter();
                }
                return instance;
            }
        }

        //Institution DTO Converter -------------------------------------------------------------------------------------------------
        public Instituitions_DTO InstituitionsToDTO(Instituitions instituitions)
        {
            return new Instituitions_DTO
            {
                instituitionID = instituitions.instituitionID,
                instituitionName = instituitions.instituitionName,
                instituitionAbreviation = instituitions.instituitionAbreviation,
                tournamentId = instituitions.tournamentId
            };
        }
        public Instituitions DTOToInstituitions(Instituitions_DTO instituitions)
        {
            return new Instituitions
            {
                instituitionID = instituitions.instituitionID,
                instituitionName = instituitions.instituitionName,
                instituitionAbreviation = instituitions.instituitionAbreviation,
            };
        }
        //TournamentInfo DTO Converter -------------------------------------------------------------------------------------------------
        public TournamentInfo_DTO TournamentInfoToDTO(TournamentInfo tournamentInfo)
        {
            List<SpeakerCategories_DTO> speakerCategories = new List<SpeakerCategories_DTO>();
            foreach (var speakerCategory in tournamentInfo.speakerCategories)
            {
                speakerCategories.Add(new SpeakerCategories_DTO
                {
                    speakerType = speakerCategory.speakerType,
                    breakType = speakerCategory.breakType
                });
            }
            return new TournamentInfo_DTO
            {
                tournamentId = tournamentInfo.tournamentId,
                tournamentType = tournamentInfo.tournamentType,
                tournamentName = tournamentInfo.tournamentName,
                tournamentShortHand = tournamentInfo.tournamentShortHand,
                noOfPrelims = tournamentInfo.noOfPrelims,
                speakerCategories = speakerCategories,
                isBreaksGenerated = tournamentInfo.isBreaksGenerated,
                openBreakingTeams = tournamentInfo.openBreakingTeams,
                noviceBreakingTeams = tournamentInfo.noviceBreakingTeams,
                breakParameters = (int)tournamentInfo.breakParam
            };
        }
        public TournamentInfo DTOToTournamentInfo(TournamentInfo_DTO tournamentInfo)
        {

            var speakerCategories = new List<SpeakerCategories>();
            foreach (var speakerCategory in tournamentInfo.speakerCategories)
            {
                var speakerType = speakerCategory.speakerType;
                var breakType = speakerCategory.breakType;

                speakerCategories.Add(new SpeakerCategories
                {
                    speakerType = speakerType,
                    breakType = breakType
                });
            }
            return new TournamentInfo
            {
                tournamentId = tournamentInfo.tournamentId,
                tournamentType = tournamentInfo.tournamentType,
                tournamentName = tournamentInfo.tournamentName,
                tournamentShortHand = tournamentInfo.tournamentShortHand,
                noOfPrelims = tournamentInfo.noOfPrelims,
                speakerCategories = speakerCategories,
                isBreaksGenerated = tournamentInfo.isBreaksGenerated,
                openBreakingTeams = tournamentInfo.openBreakingTeams,
                noviceBreakingTeams = tournamentInfo.noviceBreakingTeams,
                breakParam = (BreakParameters)tournamentInfo.breakParameters
            };
        }

        //Speaker DTO Converter -------------------------------------------------------------------------------------------------
        public Speaker_DTO SpeakerToDTO(Speaker speaker)
        {
            return new Speaker_DTO
            {
                speakerId = speaker.speakerId,
                speakerName = speaker.speakerName,
                speakerEmail = speaker.speakerEmail,
                speakerContact = speaker.speakerContact
            };
        }
        public Speaker DTOToSpeaker(Speaker_DTO speaker)
        {
            return new Speaker
            {
                speakerId = speaker.speakerId,
                speakerName = speaker.speakerName,
                speakerEmail = speaker.speakerEmail,
                speakerContact = speaker.speakerContact
            };
        }
        //Team DTO Converter -------------------------------------------------------------------------------------------------
        public Team_DTO TeamToDTO(Team team)
        {
            return new Team_DTO
            {
                teamId = team.teamId,
                tournamentId = team.tournamentId,
                instituitionID = team.instituition,
                teamName = team.teamName,
                teamCategory = team.teamCategory,
                totalTeamScore = team.totalTeamScore,
                teamPoints = team.teamPoints,
                isEligibleforBreak = team.isEligibleforBreak
            };
        }
        public Team DTOToTeam(Team_DTO team)
        {
            return new Team
            {
                teamId = team.teamId,
                tournamentId = team.tournamentId,
                instituition = team.instituitionID,
                teamName = team.teamName,
                teamCategory = team.teamCategory,
                totalTeamScore = team.totalTeamScore,
                teamPoints = team.teamPoints,
                isEligibleforBreak = team.isEligibleforBreak
            };
        }

        //Adjudicator DTO Converter -------------------------------------------------------------------------------------------------
        public Adjudicator_DTO AdjudicatorToDTO(Adjudicator adjudicator)
        {
            return new Adjudicator_DTO
            {
                adjudicatorId = adjudicator.adjudicatorID,
                adjudicatorName = adjudicator.adjudicatorName,
                adjudicatorEmail = adjudicator.adjudicatorEmail,
                adjudicatorPhone = adjudicator.adjudicatorPhone,
                adjudicatorType = adjudicator.adjudicatorType,
                institutionID = adjudicator.instituitionID,
            };
        }
        public Adjudicator DTOToAdjudicator(Adjudicator_DTO adjudicator)
        {
            return new Adjudicator
            {
                adjudicatorID = adjudicator.adjudicatorId,
                adjudicatorName = adjudicator.adjudicatorName,
                adjudicatorEmail = adjudicator.adjudicatorEmail,
                adjudicatorPhone = adjudicator.adjudicatorPhone,
                adjudicatorType = adjudicator.adjudicatorType,
                instituitionID = adjudicator.institutionID
            };
        }
        //Rounds DTO Converter -------------------------------------------------------------------------------------------------
        public Rounds_DTO RoundsToDTO(Rounds rounds)
        {
            return new Rounds_DTO
            {
                roundId = rounds.roundId,
                roundType = rounds.roundType,
                roundCategory = (int)rounds.roundCategory,
                motions = rounds.motions,
                availableTeamsIds = rounds.availableTeams.ConvertAll(team => team.teamId),
                availableAdjudicatorsIds = rounds.availableAdjudicators.ConvertAll(adj => adj.adjudicatorID),
                roundState = rounds.roundState,
                swings = rounds.swings.Count,
                motionAdded = rounds.motionAdded,
                teamAttendanceAdded = rounds.teamAttendanceAdded,
                AdjudicatorAttendanceAdded = rounds.AdjudicatorAttendanceAdded,
                drawGenerated = rounds.drawGenerated,
                ballotsAdded = rounds.ballotsAdded,
                isPublic = rounds.isPublic,
                isSilent = rounds.isSilent
            };
        }

        public Rounds DTOToRounds(Rounds_DTO rounds)
        {
            // Calculate availableTeams before the return statement
            // Add debug logs to check the values of rounds and rounds.availableTeamsIds
            if (rounds == null)
            {
                Debug.LogError("rounds is null");
                return null; // Return null or handle the error as appropriate
            }
            List<Team> availableTeams = new List<Team>();
            if (rounds.availableTeamsIds != null)
            {
                Debug.Log("rounds.availableTeamsIds is not null");
                availableTeams = AppConstants.instance.selectedTouranment?.teamsInTourney?
                .Where(team => rounds.availableTeamsIds.Contains(team.teamId))
                .ToList() ?? new List<Team>();

                // return null; // Return null or handle the error as appropriate
            }
                return new Rounds
            {
                roundId = rounds?.roundId,
                roundType = rounds?.roundType ?? default,
                roundCategory = (RoundCategory)(rounds?.roundCategory ?? 0),
                motions = rounds?.motions ?? new Dictionary<string, string>(),
                availableTeams = availableTeams, // Assign the calculated availableTeams here
                availableAdjudicators = AppConstants.instance.selectedTouranment?.adjudicatorsInTourney?
                    .Where(adjudicator => rounds?.availableAdjudicatorsIds?.Contains(adjudicator.adjudicatorID) ?? false)
                    .ToList() ?? new List<Adjudicator>(),
                roundState = rounds?.roundState ?? default,
                swings = new List<TeamRoundData>(),
                motionAdded = rounds?.motionAdded ?? false,
                teamAttendanceAdded = rounds?.teamAttendanceAdded ?? false,
                AdjudicatorAttendanceAdded = rounds?.AdjudicatorAttendanceAdded ?? false,
                drawGenerated = rounds?.drawGenerated ?? false,
                ballotsAdded = rounds?.ballotsAdded ?? false,
                isPublic = rounds?.isPublic ?? false,
                isSilent = rounds?.isSilent ?? false
            };
        }
        // Match DTO Converter
        public Match_DTO MatchToDTO(Match match)
        {
            if (match == null)
            {
                return null;
            }

            return new Match_DTO
            {
                matchId = match.matchId,
                adjudicatorsIds = match.adjudicators.Select(adj => adj.adjudicatorID).ToArray(),
                selectedMotion = match.selectedMotion,
                teamsinMatchIds = match.teams, // Assuming Match class also has Dictionary<string, string>
                ballotEntered = match.ballotEntered
            };
        }

        public Match DTOToMatch(Match_DTO match)
        {
            return new Match
            {
                matchId = match.matchId,
                adjudicators = AppConstants.instance.selectedTouranment.adjudicatorsInTourney
            .Where(adjudicator => match.adjudicatorsIds.Contains(adjudicator.adjudicatorID))
            .ToArray(),
                selectedMotion = match.selectedMotion,
                teams = match.teamsinMatchIds,
                ballotEntered = match.ballotEntered
            };
        }

        // TeamRoundData DTO Converter
        public TeamRoundData_DTO TeamRoundDataToDTO(TeamRoundData teamRoundData)
        {
            var teamRoundDataDTO = new TeamRoundData_DTO
            {
                teamRoundDataID = teamRoundData.teamRoundDataID,
                roundId = teamRoundData.roundID,
                matchId = teamRoundData.matchID,
                roundCategory = (int)teamRoundData.roundType,
                teamId = teamRoundData.teamId,
                teamPositionAsian = teamRoundData.teamPositionAsian,
                teamPositionBritish = teamRoundData.teamPositionBritish,
                teamScore = teamRoundData.teamScore,
                teamMatchRanking = teamRoundData.teamMatchRanking
            };

            if (teamRoundData.speakersInRound != null)
            {
                teamRoundDataDTO.speakersInRound = teamRoundData.speakersInRound
                    .Select(speakerRoundData => SpeakerRoundDataToDTO(speakerRoundData))
                    .ToList();
            }

            return teamRoundDataDTO;
        }

public TeamRoundData DTOToTeamRoundData(TeamRoundData_DTO teamRoundData)
{
    var teamRoundDataEntity = new TeamRoundData
    {
        teamRoundDataID = teamRoundData.teamRoundDataID,
        roundID = teamRoundData.roundId,
        matchID = teamRoundData.matchId,
        roundType = (RoundCategory)teamRoundData.roundCategory,
        teamId = teamRoundData.teamId,
        teamPositionAsian = teamRoundData.teamPositionAsian,
        teamPositionBritish = teamRoundData.teamPositionBritish,
        teamScore = teamRoundData.teamScore,
        teamMatchRanking = teamRoundData.teamMatchRanking
    };

    if (teamRoundData.speakersInRound != null)
    {
        teamRoundDataEntity.speakersInRound = teamRoundData.speakersInRound
            .Select(speakerRoundDataDTO => DTOToSpeakerRoundData(speakerRoundDataDTO))
            .ToList();
    }

    return teamRoundDataEntity;
}

        // SpeakerRoundData DTO Converter 
        public SpeakerRoundData_DTO SpeakerRoundDataToDTO(SpeakerRoundData speakerRoundData)
        {
            return new SpeakerRoundData_DTO
            {
                speakerRoundDataID = speakerRoundData.teamRoundDataID,
                speakerId = speakerRoundData.speakerId,
                speakerSpeakingPosition = speakerRoundData.speakerSpeakingPosition,
                speakerScore = speakerRoundData.speakerScore
            };
        }

        public SpeakerRoundData DTOToSpeakerRoundData(SpeakerRoundData_DTO speakerRoundData)
        {
            return new SpeakerRoundData
            {
                teamRoundDataID = speakerRoundData.speakerRoundDataID,
                speakerId = speakerRoundData.speakerId,
                speakerSpeakingPosition = speakerRoundData.speakerSpeakingPosition,
                speakerScore = speakerRoundData.speakerScore
            };
        }
    }
    #endregion
}