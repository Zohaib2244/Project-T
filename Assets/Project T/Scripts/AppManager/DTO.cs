using System.Collections.Generic;
using Firebase.Firestore;
using Scripts.Resources;
using System;
using System.Linq;

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
    public class _RoundTypesConverter : IFirestoreConverter<_RoundTypes>
    {
        public object ToFirestore(_RoundTypes value)
        {
            return value.ToString();
        }

        public _RoundTypes FromFirestore(object value)
        {
            return (_RoundTypes)Enum.Parse(typeof(_RoundTypes), value.ToString());
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
        public int noOfPrelims { get; set; }
        [FirestoreProperty]
        public List<SpeakerCategories_DTO> speakerCategories { get; set; }
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
        public List<Match> matches { get; set; }
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
    }

    [FirestoreData]
    public class TeamRoundData_DTO
    {
        [FirestoreDocumentId]
        public string teamRoundDataID { get; set; }
        [FirestoreProperty(ConverterType = typeof(_RoundTypesConverter))]
        public _RoundTypes roundType { get; set; }
        public int roundNumber { get; set; }
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
        public string speakerPhone { get; set; }
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
                noOfPrelims = tournamentInfo.noOfPrelims,
                speakerCategories = speakerCategories
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
                noOfPrelims = tournamentInfo.noOfPrelims,
                speakerCategories = speakerCategories,
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
                speakerPhone = speaker.speakerPhone
            };
        }
        public Speaker DTOToSpeaker(Speaker_DTO speaker)
        {
            return new Speaker
            {
                speakerId = speaker.speakerId,
                speakerName = speaker.speakerName,
                speakerEmail = speaker.speakerEmail,
                speakerPhone = speaker.speakerPhone
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
                motions = rounds.motions,
                availableTeamsIds = rounds.availableTeams.ConvertAll(team => team.teamId),
                availableAdjudicatorsIds = rounds.availableAdjudicators.ConvertAll(adj => adj.adjudicatorID),
                roundState = rounds.roundState,
                swings = rounds.swings.Count
            };
        }

        public Rounds DTOToRounds(Rounds_DTO rounds)
        {
            return new Rounds
            {
                roundId = rounds.roundId,
                roundType = rounds.roundType,
                motions = rounds.motions,
                availableTeams = AppConstants.instance.selectedTouranment.teamsInTourney
                    .Where(team => rounds.availableTeamsIds.Contains(team.teamId))
                    .ToList(),

                availableAdjudicators = AppConstants.instance.selectedTouranment.adjudicatorsInTourney
                    .Where(adjudicator => rounds.availableAdjudicatorsIds.Contains(adjudicator.adjudicatorID))
                    .ToList(),
                roundState = rounds.roundState,
                swings = new List<TeamRoundData>() // Populate this list as needed
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
                teamsinMatchIds = match.teams // Assuming Match class also has Dictionary<string, string>
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
                teams = match.teamsinMatchIds
            };
        }

        // TeamRoundData DTO Converter
        public TeamRoundData_DTO TeamRoundDataToDTO(TeamRoundData teamRoundData)
        {
            return new TeamRoundData_DTO
            {
                teamRoundDataID = teamRoundData.teamRoundDataID,
                roundNumber = teamRoundData.roundNumber,
                roundType = teamRoundData.roundType,
                teamId = teamRoundData.teamId,
                teamPositionAsian = teamRoundData.teamPositionAsian,
                teamPositionBritish = teamRoundData.teamPositionBritish,
                teamScore = teamRoundData.teamScore,
                teamMatchRanking = teamRoundData.teamMatchRanking
            };
        }

        public TeamRoundData DTOToTeamRoundData(TeamRoundData_DTO teamRoundData)
        {
            return new TeamRoundData
            {
                teamRoundDataID = teamRoundData.teamRoundDataID,
                roundNumber = teamRoundData.roundNumber,
                roundType = teamRoundData.roundType,
                teamId = teamRoundData.teamId,
                teamPositionAsian = teamRoundData.teamPositionAsian,
                teamPositionBritish = teamRoundData.teamPositionBritish,
                teamScore = teamRoundData.teamScore,
                teamMatchRanking = teamRoundData.teamMatchRanking
            };
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