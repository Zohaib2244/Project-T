using UnityEngine;
using Firebase.Firestore;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Scripts.Resources;
using Unity.VisualScripting;

namespace Scripts.FirebaseConfig
{
    public class FirestoreManager : MonoBehaviour
    {
        private static FirestoreManager _instance;

        public static FirestoreManager FireInstance
        {
            get
            {
                if (_instance == null)
                {
                    Debug.LogError("FirestoreManager instance is not initialized.");
                }
                return _instance;
            }
        }
        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #region Reference Getters
        // Collection Reference Getters --------------------------------------------------------------------------------
        protected CollectionReference GetInstituteCollectionReference()
        {
            return FirebaseConnector.Instance.Db.Collection("tournaments").Document(AppConstants.instance.selectedTouranment.tournamentId).Collection("instituitions");
        }
        protected CollectionReference GetTouranmentCollectionReference()
        {
            return FirebaseConnector.Instance.Db.Collection("tournaments");
        }
        protected CollectionReference GetTeamCollectionReference()
        {
            return FirebaseConnector.Instance.Db.Collection("tournaments").Document(AppConstants.instance.selectedTouranment.tournamentId).Collection("teams");
        }
        protected CollectionReference GetSpeakerCollectionReference(string teamId)
        {
            return GetTeamDocumentReference(teamId).Collection("speakers");
        }
        protected CollectionReference GetAdjudicatorCollectionReference()
        {
            return FirebaseConnector.Instance.Db.Collection("tournaments").Document(AppConstants.instance.selectedTouranment.tournamentId).Collection("adjudicators");
        }
        protected CollectionReference GetRoundCollectionReference(string roundType)
        {
            if (roundType == RoundCategory.PreLim.ToString())
            {
                Debug.Log("PreLim");
                return GetTournamentDocumentReference(AppConstants.instance.selectedTouranment.tournamentId).Collection("PreLim");
            }
            else if (roundType == RoundCategory.OpenBreak.ToString())
            {
                Debug.Log("OpenBreak");
                return GetTournamentDocumentReference(AppConstants.instance.selectedTouranment.tournamentId).Collection("OpenBreak");
            }
            else
            {
                Debug.Log("NoviceBreak");
                return GetTournamentDocumentReference(AppConstants.instance.selectedTouranment.tournamentId).Collection("NoviceBreak");
            }
        }
        protected CollectionReference GetMatchCollectionReference(string roundType, string roundId)
        {
            return GetRoundDocumentReference(roundType, roundId).Collection("matches");
        }
        protected CollectionReference GetTeamRoundDataCollectionReference(string teamId)
        {
            return GetTeamDocumentReference(teamId).Collection("roundData");
        }
        protected CollectionReference GetSpeakerRoundDataCollectionReference(string teamId, string trd_id)
        {
            return GetTeamRoundDataDocumentReference(teamId, trd_id).Collection("speakers");
        }




        // Document Reference Getters -----------------------------------------------------------------------------------
        protected DocumentReference GetSpeakerRoundDataDocumentReference(string teamId, string trd_id, string speakerId)
        {
            return GetSpeakerRoundDataCollectionReference(teamId, trd_id).Document(speakerId);
        }
        protected DocumentReference GetTeamRoundDataDocumentReference(string teamId, string teamRoundDataId)
        {
            return GetTeamRoundDataCollectionReference(teamId).Document(teamRoundDataId);
        }
        protected DocumentReference GetMatchDocumentReference(string roundType, string roundId, string matchId)
        {
            return GetMatchCollectionReference(roundType, roundId).Document(matchId);
        }
        protected DocumentReference GetRoundDocumentReference(string roundType, string roundId)
        {
            return GetRoundCollectionReference(roundType).Document(roundId);
        }
        protected DocumentReference GetTeamDocumentReference(string teamId)
        {
            return GetTeamCollectionReference().Document(teamId);
        }
        protected DocumentReference GetTournamentDocumentReference(string tournamentId)
        {
            return GetTouranmentCollectionReference().Document(tournamentId);
        }
        protected DocumentReference GetInstitutionDocumentReference(string instituitionId)
        {
            return GetInstituteCollectionReference().Document(instituitionId);
        }
        protected DocumentReference GetSpeakerDocumentReference(string teamId, string speakerId)
        {
            return GetSpeakerCollectionReference(teamId).Document(speakerId);
        }
        protected DocumentReference GetAdjudicatorDocumentReference(string adjudicatorId)
        {
            return GetAdjudicatorCollectionReference().Document(adjudicatorId);
        }
        #endregion



        #region Admin Functions
        public async void GetAdminFromFirestore(string userId, UnityAction<Admin> onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                Debug.Log("<color=yellow>Retrieving admin info...</color>");
                DialogueBox.Instance.ShowDialogueBox("Retrieving admin info...", Color.yellow);

                DocumentReference docRef = FirebaseConnector.Instance.Db.Collection("Admin_Categorizer").Document(userId);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    Admin_DTO admin_dto = snapshot.ConvertTo<Admin_DTO>();
                    Admin admin = new Admin(admin_dto.UserID, admin_dto.AdminCategory, admin_dto.Name);
                    onSuccess?.Invoke(admin);
                    Debug.Log("<color=green>Admin info retrieved successfully.</color>");
                    DialogueBox.Instance.ShowDialogueBox("Admin info retrieved successfully.", Color.green);
                }
                else
                {
                    Debug.LogError("No such document!");
                    DialogueBox.Instance.ShowDialogueBox("No such document!", Color.red);
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                DialogueBox.Instance.ShowDialogueBox("Firebase is not ready. Please wait for Firebase to initialize.", Color.red);
                onFailure?.Invoke();
            }
        }
        #endregion

        #region Tournament Functions
    public async void SaveTournamentToFireStore(TournamentInfo tournament, UnityAction onSuccess = null, UnityAction onFailure = null)
    {
        if (FirebaseConnector.Instance.isFirebaseReady)
        {
            try
            {
                Debug.Log("<color=orange>Trying to Save Tournament.</color>");
                DialogueBox.Instance.ShowDialogueBox("Trying to Save Tournament.", Color.yellow);
                Debug.Log("<color=lightblue>Tournament ID: " + tournament.tournamentId + "</color>");
                var tournamentInfoDTO = DTOConverter.Instance.TournamentInfoToDTO(tournament);
                DocumentReference docRef = GetTouranmentCollectionReference().Document(tournamentInfoDTO.tournamentId);
                await docRef.SetAsync(tournamentInfoDTO);

                Debug.Log("<color=green>Tournament saved successfully.</color>");
                DialogueBox.Instance.ShowDialogueBox("Tournament saved successfully.", Color.green);

                onSuccess?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"SaveTournamentToFireStore encountered an error: {ex}");
                DialogueBox.Instance.ShowDialogueBox("Failed to save tournament.", Color.red);

                onFailure?.Invoke();
            }
        }
        else
        {
            Debug.Log("<color=orange>Firebase is not ready. Please wait for Firebase to initialize.</color>");
            DialogueBox.Instance.ShowDialogueBox("Firebase is not ready. Please wait for Firebase to initialize.", Color.red);

            onFailure?.Invoke();
        }
    }

    public async void UpdateTournamentsFromFirestore(UnityAction onSuccess = null, UnityAction onFailure = null)
    {
        if (FirebaseConnector.Instance.isFirebaseReady)
        {
            try
            {
                Debug.Log("<color=orange>Retrieving tournaments from Firestore.</color>");
                // DialogueBox.Instance.ShowDialogueBox("Retrieving tournaments from Firestore.", Color.yellow);

                QuerySnapshot snapshot = await GetTouranmentCollectionReference().GetSnapshotAsync();
                Debug.Log($"<color=blue>Retrieved {snapshot.Count} tournament documents from Firestore.</color>");

                // Check the count of documents
                if (snapshot.Count == 0)
                {
                    Debug.Log("<color=orange>No tournaments found in Firestore.</color>");
                    // DialogueBox.Instance.ShowDialogueBox("No tournaments found in Firestore.", Color.red);

                    onFailure?.Invoke();
                    return;
                }

                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    AppConstants.instance.tournaments.Add(DTOConverter.Instance.DTOToTournamentInfo(document.ConvertTo<TournamentInfo_DTO>()));
                }

                Debug.Log("<color=green>Tournaments updated successfully.</color>");
                // DialogueBox.Instance.ShowDialogueBox("Tournaments updated successfully.", Color.green);

                onSuccess?.Invoke();
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"UpdateTournamentsFromFirestore encountered an error: {ex}");
                // DialogueBox.Instance.ShowDialogueBox("Failed to update tournaments.", Color.red);

                onFailure?.Invoke();
            }
        }
        else
        {
            Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
            // DialogueBox.Instance.ShowDialogueBox("Firebase is not ready. Please wait for Firebase to initialize.", Color.red);

            onFailure?.Invoke();
        }
    }
public async Task UpdateTournamentBreakingTeams(UnityAction onSuccess = null, UnityAction onFailure = null)
{
    if (FirebaseConnector.Instance.isFirebaseReady)
    {
        try
        {
            // Get the reference to the selected tournament
            var selectedTournament = AppConstants.instance.selectedTouranment;
            if (selectedTournament == null)
            {
                Debug.LogError("Selected tournament is null.");
                onFailure?.Invoke();
                return;
            }

            // Create a dictionary to hold the updated fields
            var updates = new Dictionary<string, object>
            {
                { "noviceBreakingTeams", selectedTournament.noviceBreakingTeams },
                { "openBreakingTeams", selectedTournament.openBreakingTeams },
                 { "breakParameters", Convert.ToInt32(selectedTournament.breakParam) },
                {"isBreaksGenerated", selectedTournament.isBreaksGenerated}
            };

            // Get the document reference for the selected tournament
            var tournamentDocRef = GetTournamentDocumentReference(selectedTournament.tournamentId);

            // Update the tournament document in Firestore
            await tournamentDocRef.UpdateAsync(updates);

            Debug.Log("Tournament breaking teams updated successfully.");
            onSuccess?.Invoke();
        }
        catch (Exception ex)
        {
            Debug.LogError("UpdateTournamentBreakingTeams encountered an error: " + ex);
            onFailure?.Invoke();
        }
    }
    else
    {
        Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
        onFailure?.Invoke();
    }
}

        #endregion

        #region Institution Functions
               public async void SaveInstituteToFireStore(Instituitions institute, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    Debug.Log("<color=orange>Trying to Save Institute.</color>");
                    // DialogueBox.Instance.ShowDialogueBox("Trying to Save Institute.", Color.yellow);
        
                    if (AppConstants.instance.selectedTouranment.instituitionsinTourney.Any(i => i.instituitionAbreviation == institute.instituitionAbreviation))
                    {
                        Debug.LogError("Institute with the same name already exists.");
                        // DialogueBox.Instance.ShowDialogueBox("Institute with the same name already exists.", Color.red);
                        onFailure?.Invoke();
                    }
                    else
                    {
                        Instituitions_DTO instituition_dto = DTOConverter.Instance.InstituitionsToDTO(institute);
                        Debug.Log("<color=lightblue>Trying to Save Institute.</color>");
                        await GetInstitutionDocumentReference(institute.instituitionID).SetAsync(instituition_dto);
                        Debug.Log("<color=green>Institute saved successfully.</color>");
                        // DialogueBox.Instance.ShowDialogueBox("Institute saved successfully.", Color.green);
                        onSuccess?.Invoke();
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"SaveInstituteToFireStore encountered an error: {ex}");
                    // DialogueBox.Instance.ShowDialogueBox("Failed to save institute.", Color.red);
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                // DialogueBox.Instance.ShowDialogueBox("Firebase is not ready. Please wait for Firebase to initialize.", Color.red);
                onFailure?.Invoke();
            }
        }
        
        public async Task GetAllInstituitionsFromFirestore(UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    Debug.Log("<color=orange>Retrieving institutions from Firestore.</color>");
        
                    QuerySnapshot snapshot = await GetInstituteCollectionReference().GetSnapshotAsync();
                    Debug.Log($"<color=blue>Retrieved {snapshot.Count} institution documents from Firestore.</color>");
        
                    AppConstants.instance.selectedTouranment.instituitionsinTourney.Clear();
                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        Debug.Log("Institutions adding to the list");
                        AppConstants.instance.selectedTouranment.instituitionsinTourney.Add(DTOConverter.Instance.DTOToInstituitions(document.ConvertTo<Instituitions_DTO>()));
                    }
                    Debug.Log("<color=green>Institutions updated successfully.</color>");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.Log("GetAllInstituitionsFromFirestore encountered an error: " + ex);
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.Log("Firebase is not ready. Please wait for Firebase to initialize.");
                onFailure?.Invoke();
            }
        }
        
        public async void UpdateInstitutionInFireStore(Instituitions institute, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    Debug.Log("<color=orange>Trying to Update Institute.</color>");
                    DialogueBox.Instance.ShowDialogueBox("Trying to Update Institute.", Color.yellow);
        
                    Instituitions_DTO instituition_dto = DTOConverter.Instance.InstituitionsToDTO(institute);
                    await GetInstitutionDocumentReference(instituition_dto.instituitionID).SetAsync(instituition_dto);
                    Debug.Log("<color=green>Institute updated successfully.</color>");
                    DialogueBox.Instance.ShowDialogueBox("Institute updated successfully.", Color.green);
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"UpdateInstituteInFireStore encountered an error: {ex}");
                    DialogueBox.Instance.ShowDialogueBox("Failed to update institute.", Color.red);
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                DialogueBox.Instance.ShowDialogueBox("Firebase is not ready. Please wait for Firebase to initialize.", Color.red);
                onFailure?.Invoke();
            }
        }
        #endregion

        #region Team Functions
        public async void SaveTeamToFireStore(Team team, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    Team_DTO team_dto = DTOConverter.Instance.TeamToDTO(team);
                    DocumentReference teamDocRef = GetTeamDocumentReference(team_dto.teamId);

                    // Save the team document
                    await teamDocRef.SetAsync(team_dto);
                    Debug.Log("<color=green>Team saved successfully.</color>");

                    // Save each speaker as a document in the speakers subcollection
                    CollectionReference speakersCollectionRef = GetSpeakerCollectionReference(team_dto.teamId);
                    foreach (var speaker in team.speakers)
                    {
                        Speaker_DTO speaker_dto = DTOConverter.Instance.SpeakerToDTO(speaker);
                        DocumentReference speakerDocRef = GetSpeakerDocumentReference(team_dto.teamId, speaker_dto.speakerId);
                        await speakerDocRef.SetAsync(speaker_dto);
                        Debug.Log($"<color=green>Speaker {speaker_dto.speakerName} saved successfully.</color>");
                    }

                    // Retrieve the number of preliminary rounds from the selected tournament
                    int numberOfPrelimRounds = AppConstants.instance.selectedTouranment.preLimsInTourney.Count;

                    // Save each team round data for the number of preliminary rounds
                    if (team.teamRoundDatas != null)
                    {
                        for (int i = 0; i < numberOfPrelimRounds && i < team.teamRoundDatas.Count; i++)
                        {
                            var teamRoundData = team.teamRoundDatas[i];
                            TeamRoundData_DTO teamRoundDataDTO = DTOConverter.Instance.TeamRoundDataToDTO(teamRoundData);
                            DocumentReference teamRoundDataDocRef = GetTeamRoundDataDocumentReference(team_dto.teamId, teamRoundDataDTO.teamRoundDataID);
                            await teamRoundDataDocRef.SetAsync(teamRoundDataDTO);
                            Debug.Log($"<color=green>Team round data {teamRoundDataDTO.teamRoundDataID} saved successfully.</color>");
                        }
                    }

                    AppConstants.instance.selectedTouranment.teamsInTourney.Add(team);
                    onSuccess?.Invoke();
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"SaveTeamToFireStore encountered an error: {ex}");
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                onFailure?.Invoke();
            }
        }
                  public async Task GetAllTeamsFromFirestore(UnityAction onSuccess = null, UnityAction onFailure = null)
            {
                if (FirebaseConnector.Instance.isFirebaseReady)
                {
                    try
                    {
                        QuerySnapshot teamSnapshot = await GetTeamCollectionReference().GetSnapshotAsync();
                        Debug.Log($"<color=blue>Retrieved {teamSnapshot.Count} team documents from Firestore.</color>");
                        
                        // Clear existing teams
                        AppConstants.instance.selectedTouranment.teamsInTourney.Clear();
                        
                        // Create a list of tasks to process each team document in parallel
                        var tasks = new List<Task>();
                        var processedTeamIds = new HashSet<string>();
                        
                        foreach (var teamDoc in teamSnapshot.Documents)
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                try
                                {
                                    Debug.Log($"Processing team document {teamDoc.Id}");
                                    Team_DTO team_dto = teamDoc.ConvertTo<Team_DTO>();
                                    Team team = DTOConverter.Instance.DTOToTeam(team_dto);
                    
                                    // Fetch speakers for the team
                                    QuerySnapshot speakersSnapshot = await GetSpeakerCollectionReference(team_dto.teamId).GetSnapshotAsync();
                                    foreach (var speakerDoc in speakersSnapshot.Documents)
                                    {
                                        Speaker_DTO speaker_dto = speakerDoc.ConvertTo<Speaker_DTO>();
                                        Speaker speaker = DTOConverter.Instance.DTOToSpeaker(speaker_dto);
                                        team.speakers.Add(speaker);
                                    }
                    
                                    // Fetch team round data for the team
                                    QuerySnapshot teamRoundDataSnapshot = await GetTeamRoundDataCollectionReference(team_dto.teamId).GetSnapshotAsync();
                                    Debug.Log($"<color=blue>Retrieved {teamRoundDataSnapshot.Count} team round data documents for team ID {team_dto.teamId} from Firestore.</color>");
                                    foreach (var document in teamRoundDataSnapshot.Documents)
                                    {
                                        TeamRoundData_DTO teamRoundDataDTO = document.ConvertTo<TeamRoundData_DTO>();
                                        if (teamRoundDataDTO.speakersInRound != null)
                                            Debug.Log($"<color=lightblue> SRD in DTO : + " + teamRoundDataDTO.speakersInRound.Count + "</color>");
                                        else
                                            Debug.Log($"<color=lightblue> SRD in DTO : + " + "NULL" + "</color>");
                
                                        TeamRoundData teamRoundData = DTOConverter.Instance.DTOToTeamRoundData(teamRoundDataDTO);
                                        team.teamRoundDatas.Add(teamRoundData);
                                        Debug.Log($"<color=green>Added TeamRoundData object with ID {teamRoundDataDTO.teamRoundDataID} to team with ID {team_dto.teamId}.</color>");
                                    }
                
                                    lock (processedTeamIds)
                                    {
                                        if (!processedTeamIds.Contains(team_dto.teamId))
                                        {
                                            AppConstants.instance.selectedTouranment.teamsInTourney.Add(team);
                                            processedTeamIds.Add(team_dto.teamId);
                                            Debug.Log($"<color=green>Added team with ID {team_dto.teamId} to teamsInTourney.</color>");
                                        }
                                        else
                                        {
                                            Debug.LogWarning($"<color=yellow>Duplicate team ID detected: {team_dto.teamId}. Skipping addition.</color>");
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError($"Error processing team document {teamDoc.Id}: {ex}");
                                }
                            }));
                        }
                        
                        // Wait for all tasks to complete
                        await Task.WhenAll(tasks);
                        
                        Debug.Log($"<color=green>Teams updated successfully. Total teams added: {AppConstants.instance.selectedTouranment.teamsInTourney.Count}</color>");
                        onSuccess?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("UpdateTeamsFromFirestore encountered an error: " + ex);
                        onFailure?.Invoke();
                    }
                }
                else
                {
                    Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                    onFailure?.Invoke();
                }
            }        public async void UpdateTeamInFireStore(Team team, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    Team_DTO team_dto = DTOConverter.Instance.TeamToDTO(team);
                    DocumentReference teamDocRef = GetTeamDocumentReference(team_dto.teamId);

                    // Update the team document
                    await teamDocRef.SetAsync(team_dto);
                    Debug.Log("<color=green>Team updated successfully.</color>");

                    // Update each speaker in the speakers subcollection
                    CollectionReference speakersCollectionRef = teamDocRef.Collection("speakers");
                    foreach (var speaker in team.speakers)
                    {
                        Speaker_DTO speaker_dto = DTOConverter.Instance.SpeakerToDTO(speaker);
                        DocumentReference speakerDocRef = GetSpeakerDocumentReference(team_dto.teamId, speaker_dto.speakerId);
                        await speakerDocRef.SetAsync(speaker_dto);
                        Debug.Log($"<color=green>Speaker {speaker_dto.speakerName} updated successfully.</color>");
                    }

                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"UpdateTeamInFireStore encountered an error: {ex}");
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                onFailure?.Invoke();
            }
        }
        public async void DeleteTeamFromFirestore(string teamId, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    DocumentReference teamDocRef = GetTeamDocumentReference(teamId);
                    await teamDocRef.DeleteAsync();
                    Debug.Log("<color=green>Team deleted successfully.</color>");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"DeleteTeamFromFirestore encountered an error: {ex}");
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                onFailure?.Invoke();
            }
        }
        public async Task UpdateAllTeamsScoreandPointatFirestore(UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    Debug.Log("<color=orange>Updating all teams' scores and points.</color>");
        
                    // Get all teams
                    var teams = AppConstants.instance.selectedTouranment.teamsInTourney;
        
                    // Create a list to hold the tasks
                    var updateTasks = new List<Task>();
        
                    foreach (var team in teams)
                    {
                        // Convert the team to DTO
                        Team_DTO team_dto = DTOConverter.Instance.TeamToDTO(team);
        
                        // Get the document reference for the team
                        DocumentReference teamDocRef = GetTeamDocumentReference(team.teamId);
        
                        // Create the update task and add it to the list
                        var updateTask = teamDocRef.UpdateAsync(new Dictionary<string, object>
                        {
                            { "totalTeamScore", team.totalTeamScore },
                            { "teamPoints", team.teamPoints }
                        }).ContinueWith(task =>
                        {
                            if (task.IsCompletedSuccessfully)
                            {
                                Debug.Log($"<color=green>Team {team.teamId} updated successfully with total score {team.totalTeamScore} and total points {team.teamPoints}.</color>");
                            }
                            else
                            {
                                Debug.LogError($"Failed to update team {team.teamId}: {task.Exception}");
                            }
                        });
        
                        updateTasks.Add(updateTask);
                    }
        
                    // Wait for all tasks to complete
                    await Task.WhenAll(updateTasks);
        
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"UpdateAllTeamsScoreandPointatFirestore encountered an error: {ex}");
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                onFailure?.Invoke();
            }
        }
                public async void ReplaceSpeakersInFirestore(Speaker speaker1, Speaker speaker2, string speaker1TeamID, string speaker2TeamID, UnityAction<Task>? onSuccess = null, UnityAction? onFailure = null)
        #nullable disable
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    // Get the speaker documents
                    DocumentReference speakerDocRef1 = GetSpeakerDocumentReference(speaker1TeamID, speaker1.speakerId);
                    DocumentReference speakerDocRef2 = GetSpeakerDocumentReference(speaker2TeamID, speaker2.speakerId);

                    await UpdateSpeakersAsync(speakerDocRef1, speakerDocRef2, speaker1, speaker2);
                    Debug.Log("<color=green>Speakers replaced successfully.</color>");
                    onSuccess?.Invoke(Task.CompletedTask);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"ReplaceSpeakersInFirestore encountered an error: {ex}");
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                onFailure?.Invoke();
            }
        }
        public async Task UpdateSpeakersAsync(DocumentReference speakerDocRef1, DocumentReference speakerDocRef2, Speaker speaker1, Speaker speaker2)
        {
            await FirebaseConnector.Instance.Db.RunTransactionAsync(async transaction =>
            {
                await Task.Yield(); // Ensure the method runs asynchronously
                {
                    transaction.Update(speakerDocRef1, new Dictionary<string, object>
                    {
                { "speakerName", speaker2.speakerName },
                { "speakerContact", speaker2.speakerContact },
                { "speakerEmail", speaker2.speakerEmail }
                    });

                    transaction.Update(speakerDocRef2, new Dictionary<string, object>
                {
            { "speakerName", speaker1.speakerName },
            { "speakerContact", speaker1.speakerContact },
            { "speakerEmail", speaker1.speakerEmail }
                });
                }
            });

            return;
        }

        public async void UpdateSpeakerInFireStore(Speaker speaker, string teamId, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    Speaker_DTO speaker_dto = DTOConverter.Instance.SpeakerToDTO(speaker);
                    if (string.IsNullOrEmpty(teamId))
                    {
                        Debug.LogError("Team ID is null or empty.");
                        onFailure?.Invoke();
                    }
                    DocumentReference speakerDocRef = GetSpeakerDocumentReference(teamId, speaker.speakerId);
                    await speakerDocRef.SetAsync(speaker_dto);
                    Debug.Log("<color=green>Speaker updated successfully.</color>");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"UpdateSpeakerInFireStore encountered an error: {ex}");
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                onFailure?.Invoke();
            }
        }
        #endregion

        #region Adjudicator Functions
        public async Task GetAllAdjudicatorsFromFirestore(UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    QuerySnapshot snapshot = await GetAdjudicatorCollectionReference().GetSnapshotAsync();
                    //remove all entries from AppCOns   
                    AppConstants.instance.selectedTouranment.adjudicatorsInTourney.Clear();
                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        Adjudicator_DTO adjudicator_dto = document.ConvertTo<Adjudicator_DTO>();
                        Adjudicator adjudicator = DTOConverter.Instance.DTOToAdjudicator(adjudicator_dto);
                        AppConstants.instance.selectedTouranment.adjudicatorsInTourney.Add(adjudicator);
                    }
                    Debug.Log("<color=green>Adjudicators updated successfully.</color>");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError("UpdateAdjudicatorsFromFirestore encountered an error: " + ex);
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                onFailure?.Invoke();
            }
        }
        public async void SaveAdjudicatorToFireStore(Adjudicator adjudicator, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    Adjudicator_DTO adjudicator_dto = DTOConverter.Instance.AdjudicatorToDTO(adjudicator);
                    DocumentReference adjudicatorDocRef = GetAdjudicatorDocumentReference(adjudicator_dto.adjudicatorId);
                    await adjudicatorDocRef.SetAsync(adjudicator_dto);
                    Debug.Log("<color=green>Adjudicator saved successfully.</color>");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"SaveAdjudicatorToFireStore encountered an error: {ex}");
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                onFailure?.Invoke();
            }
        }
        public async void UpdateAdjudicatorInFireStore(Adjudicator adjudicator, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    Adjudicator_DTO adjudicator_dto = DTOConverter.Instance.AdjudicatorToDTO(adjudicator);
                    DocumentReference adjudicatorDocRef = GetAdjudicatorDocumentReference(adjudicator_dto.adjudicatorId);
                    await adjudicatorDocRef.SetAsync(adjudicator_dto);
                    Debug.Log("<color=green>Adjudicator updated successfully.</color>");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"UpdateAdjudicatorInFireStore encountered an error: {ex}");
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                onFailure?.Invoke();
            }
        }
        public async void DeleteAdjudicatorFromFirestore(string adjudicatorId, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    DocumentReference adjudicatorDocRef = GetAdjudicatorDocumentReference(adjudicatorId);
                    await adjudicatorDocRef.DeleteAsync();
                    Debug.Log("<color=green>Adjudicator deleted successfully.</color>");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"DeleteAdjudicatorFromFirestore encountered an error: {ex}");
                    onFailure?.Invoke();
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
                onFailure?.Invoke();
            }
        }
        #endregion

        #region Round Functions

        public async Task SaveRoundAsync(string roundType, Rounds round, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            // try
            // {
                Debug.Log("<color=yellow>Starting SaveRoundAsync...</color>");

                // Convert the round to DTO
                Rounds_DTO roundDTO = DTOConverter.Instance.RoundsToDTO(round);
                Debug.Log($"<color=yellow>Round converted to DTO: {roundDTO.roundId}</color>");

                // Save the round DTO
                DocumentReference roundRef = GetRoundDocumentReference(roundType, roundDTO.roundId);
                await roundRef.SetAsync(roundDTO);
                Debug.Log($"<color=green>Round {roundDTO.roundId} saved successfully.</color>");

                // Extract matches from the round
                List<Match> matches = round.matches ?? new List<Match>();
                List<Match_DTO> matchDTOs = matches.ConvertAll(match => DTOConverter.Instance.MatchToDTO(match));
                Debug.Log($"<color=yellow>Extracted {matches.Count} matches from the round.</color>");

                // Save matches as subcollection
                foreach (var matchDTO in matchDTOs)
                {
                    DocumentReference matchRef = GetMatchDocumentReference(roundType, roundDTO.roundId, matchDTO.matchId);
                    await matchRef.SetAsync(matchDTO);
                    Debug.Log($"<color=green>Match {matchDTO.matchId} saved successfully.</color>");
                }

                Debug.Log("<color=green>All matches saved successfully.</color>");
                onSuccess?.Invoke();
            // }
            // catch (Exception ex)
            // {
            //     Debug.LogError($"Error saving round: {ex.Message}");
            //     onFailure?.Invoke();
            // }
        }
           public async Task<List<Rounds>> GetAllRoundsFromFirestore(string roundType)
        {
            List<Rounds> roundsList = new List<Rounds>();
        
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    QuerySnapshot snapshot = await GetRoundCollectionReference(roundType).GetSnapshotAsync();
                    Debug.Log($"<color=blue>Retrieved {snapshot.Count} round documents from Firestore.</color>");
        
                    foreach (var document in snapshot.Documents)
                    {
                        try
                        {
                            Rounds_DTO round_dto = document.ConvertTo<Rounds_DTO>();
                            Debug.Log($"<color=blue>Round ID {round_dto.roundId} retrieved from Firestore. TeamAvailable: {round_dto.availableTeamsIds}</color>");
                            Rounds round = DTOConverter.Instance.DTOToRounds(round_dto);
        
                            // Log the available teams in the round DTO
                            if (round_dto.availableTeamsIds != null)
                            {
                                Debug.Log($"<color=red>Round ID {round_dto.roundId} has {round_dto.availableTeamsIds.Count} available teams.</color>");
                            }
                            else
                            {
                                Debug.LogWarning($"<color=red>Round ID {round_dto.roundId} has no available teams.</color>");
                            }
        
                            // Retrieve matches for the round sequentially
                            QuerySnapshot matchesSnapshot = await GetMatchCollectionReference(roundType, round_dto.roundId).GetSnapshotAsync();
                            foreach (var matchDoc in matchesSnapshot.Documents)
                            {
                                try
                                {
                                    Match_DTO match_dto = matchDoc.ConvertTo<Match_DTO>();
                                    Match match = DTOConverter.Instance.DTOToMatch(match_dto);
                                    round.matches.Add(match);
                                }
                                catch (Exception ex)
                                {
                                    Debug.LogError($"Error processing match document {matchDoc.Id}: {ex}");
                                }
                            }
        
                            // Log the number of available teams in the round object
                            Debug.Log($"<color=blue>Round ID {round.roundId} has {round.availableTeams.Count} available teams after conversion.</color>");
                            roundsList.Add(round);
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError($"Error processing round document {document.Id}: {ex}");
                        }
                    }
        
                    // Sort the roundsList based on the last digit of roundId
                    roundsList = roundsList.OrderBy(r => int.Parse(r.roundId.Last().ToString())).ToList();
        
                    Debug.Log("<color=green>Rounds updated successfully.</color>");
                }
                catch (Exception ex)
                {
                    Debug.LogError("GetAllRoundsFromFirestore encountered an error: " + ex);
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
            }
        
            return roundsList;
        }
        public async Task UpdateRoundAsync(string roundType, Rounds round, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            try
            {
                Debug.Log("<color=yellow>Starting UpdateRoundAsync...</color>");
        
                // Convert the round to DTO
                Rounds_DTO roundDTO = DTOConverter.Instance.RoundsToDTO(round);
                Debug.Log($"<color=yellow>Round converted to DTO: {roundDTO.roundId}</color>");
        
                // Save the round DTO using UpdateAsync
                DocumentReference roundRef = GetRoundDocumentReference(roundType, roundDTO.roundId);
                await roundRef.UpdateAsync(roundDTO.ToDictionary());
                Debug.Log($"<color=green>Round {roundDTO.roundId} updated successfully.</color>");
        
                // Extract matches from the round
                List<Match> matches = round.matches ?? new List<Match>();
                List<Match_DTO> matchDTOs = matches.ConvertAll(match => DTOConverter.Instance.MatchToDTO(match));
                Debug.Log($"<color=yellow>Extracted {matches.Count} matches from the round.</color>");
        
                // Save matches as subcollection using UpdateAsync
                foreach (var matchDTO in matchDTOs)
                {
                    DocumentReference matchRef = GetMatchDocumentReference(roundType, roundDTO.roundId, matchDTO.matchId);
                    await matchRef.UpdateAsync(matchDTO.ToDictionary());
                    Debug.Log($"<color=green>Match {matchDTO.matchId} updated successfully.</color>");
                }
        
                Debug.Log("<color=green>All matches updated successfully.</color>");
                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating round: {ex.Message}");
                onFailure?.Invoke();
            }
        }
        #region Round SI Functions
        public async Task SaveRoundMotionToFirestore(string roundType, string roundId, Dictionary<string, string> motion, UnityAction< Dictionary<string, string>> onSuccess = null, UnityAction onFailure = null)
        {
            // try
            // {
                // Get the document reference for the specified round
                DocumentReference roundRef = GetRoundDocumentReference(roundType, roundId);
        
                // Create an update object with the motion data and motionAdded set to true
                var updateData = new Dictionary<string, object>
                {
                    { "motions", motion },
                    { "motionAdded", true }
                };
        
                // Save the motion data and update motionAdded to Firestore
                await roundRef.SetAsync(updateData, SetOptions.MergeAll);
        
                Debug.Log($"<color=green>Motion for round {roundId} saved successfully.</color>");
                onSuccess?.Invoke(motion);
            // }
            // catch (Exception ex)
            // {
            //     Debug.LogError($"<color=red>Error saving motion for round {roundId}: {ex.Message}</color>");
            //     onFailure?.Invoke();
            // }
        }
        public async Task SaveTeamAttendanceToFirestore(string roundType, string roundId, List<string> teamAttendance,List<Team> teamsList, UnityAction<List<Team>> onSuccess = null, UnityAction onFailure = null)
        {
            try
            {
                // Get the document reference for the specified round
                DocumentReference roundRef = GetRoundDocumentReference(roundType, roundId);
        
                // Create an update object with the team attendance data and teamAttendanceAdded set to true
                var updateData = new Dictionary<string, object>
                {
                    { "availableTeamsIds", teamAttendance },
                    { "teamAttendanceAdded", true }
                };
        
                // Save the team attendance data and update teamAttendanceAdded to Firestore
                await roundRef.SetAsync(updateData, SetOptions.MergeAll);
        
                Debug.Log($"<color=green>Team attendance for round {roundId} saved successfully.</color>");
                onSuccess?.Invoke(teamsList);
            }
            catch (Exception ex)
            {
                Debug.LogError($"<color=red>Error saving team attendance for round {roundId}: {ex.Message}</color>");
                onFailure?.Invoke();
            }
        } 
        public async Task SaveAdjudicatorAttendanceToFirestore(string roundType, string roundId, List<string> adjudicatorAttendance,List<Adjudicator> adjudicators, UnityAction<List<Adjudicator>> onSuccess = null, UnityAction onFailure = null)
        {
            try
            {
                // Get the document reference for the specified round
                DocumentReference roundRef = GetRoundDocumentReference(roundType, roundId);
        
                // Create an update object with the adjudicator attendance data and AdjudicatorAttendanceAdded set to true
                var updateData = new Dictionary<string, object>
                {
                    { "availableAdjudicatorsIds", adjudicatorAttendance },
                    { "AdjudicatorAttendanceAdded", true }
                };
        
                // Save the adjudicator attendance data and update AdjudicatorAttendanceAdded to Firestore
                await roundRef.SetAsync(updateData, SetOptions.MergeAll);
        
                Debug.Log($"<color=green>Adjudicator attendance for round {roundId} saved successfully.</color>");
                onSuccess?.Invoke(adjudicators);
            }
            catch (Exception ex)
            {
                Debug.LogError($"<color=red>Error saving adjudicator attendance for round {roundId}: {ex.Message}</color>");
                onFailure?.Invoke();
            }
        }
        public async Task SaveAllMatchesToFirestore(string roundType, string roundId, List<Match> matches, UnityAction<List<Match>> onSuccess = null, UnityAction onFailure = null)
        {
            try
            {
                // Get the document reference for the specified round
                DocumentReference roundRef = GetRoundDocumentReference(roundType, roundId);
                // Create a dictionary to hold the update
                Dictionary<string, object> updates = new Dictionary<string, object>
                {
                    { "drawGenerated", true }
                };

                // Update the drawEntered field to true
                await roundRef.UpdateAsync(updates);
                // Iterate through each match and save it to Firestore
                foreach (var match in matches)
                {
                    Match_DTO matchDTO = DTOConverter.Instance.MatchToDTO(match);
                    // Get the document reference for the match
                    DocumentReference matchRef = roundRef.Collection("matches").Document(matchDTO.matchId);

                    // Save the match data to Firestore
                    await matchRef.SetAsync(matchDTO);

                    foreach (var teamEntry in matchDTO.teamsinMatchIds)
                    {
                        var team = AppConstants.instance.selectedTouranment.teamsInTourney.Find(t => t.teamId == teamEntry.Key);
                        if (team != null)
                        {
                            TeamRoundData teamRoundData = team.teamRoundDatas.Find(trd => trd.teamRoundDataID == teamEntry.Value);
                            if (teamRoundData != null)
                            {
                            await AddTeamRoundData(team.teamId, teamRoundData);
                            }
                        }
                    }
                }

                Debug.Log($"<color=green>Draws for round {roundId} saved successfully.</color>");
                onSuccess?.Invoke(matches);
            }
            catch (Exception ex)
            {
                Debug.LogError($"<color=red>Error saving draws for round {roundId}: {ex.Message}</color>");
                onFailure?.Invoke();
            }
        }
        public async Task GetAllMatchesFromFirestore(string roundType, string roundId, UnityAction<List<Match>> onSuccess = null, UnityAction onFailure = null)
        {
            SemaphoreSlim semaphore = new SemaphoreSlim(4); // Limit to 4 concurrent tasks
        
            try
            {
                // Get the document reference for the specified round
                DocumentReference roundRef = GetRoundDocumentReference(roundType, roundId);
        
                // Get all matches for the round
                QuerySnapshot matchSnapshot = await roundRef.Collection("matches").GetSnapshotAsync();
                List<Match> matches = new List<Match>();
                List<TeamRoundData> teamRoundDataList = new List<TeamRoundData>();
        
                foreach (var matchDoc in matchSnapshot.Documents)
                {
                    Match_DTO matchDTO = matchDoc.ConvertTo<Match_DTO>();
                    Match match = DTOConverter.Instance.DTOToMatch(matchDTO);
                    matches.Add(match);
        
                    // Collect tasks for parallel execution
                    List<Task> tasks = new List<Task>();
        
                    // Get TeamRoundData for each team in the match
                    foreach (KeyValuePair<string, string> teaminmatch in match.teams)
                    {
                        await semaphore.WaitAsync(); // Wait for an available slot
        
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                await GetTeamRoundData(teaminmatch.Key, teaminmatch.Value);
                            }
                            finally
                            {
                                semaphore.Release(); // Release the slot
                            }
                        }));
                    }
        
                    // Execute all tasks in parallel
                    await Task.WhenAll(tasks);
                }
        
                // Invoke the success callback with the list of matches and team round data
                onSuccess?.Invoke(matches);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error getting matches from Firestore: {ex.Message}");
                // Invoke the failure callback
                onFailure?.Invoke();
            }
            finally
            {
                semaphore.Dispose(); // Dispose of the semaphore
            }
        }
        
        private async Task<List<SpeakerRoundData>> GetAllSpeakerRoundDataForTeamFromFirestore(string teamId, string teamRoundDataId)
{
    List<SpeakerRoundData> speakerRoundDataList = new List<SpeakerRoundData>();

    try
    {
        // Get the document reference for the specified team round data
        DocumentReference teamRoundDataRef = GetTeamRoundDataDocumentReference(teamId, teamRoundDataId);

        // Get all speaker round data for the team round data
        QuerySnapshot speakerRoundDataSnapshot = await teamRoundDataRef.Collection("speakersInRound").GetSnapshotAsync();

        foreach (DocumentSnapshot speakerRoundDataDoc in speakerRoundDataSnapshot.Documents)
        {
            SpeakerRoundData_DTO speakerRoundDataDTO = speakerRoundDataDoc.ConvertTo<SpeakerRoundData_DTO>();
            SpeakerRoundData speakerRoundData = DTOConverter.Instance.DTOToSpeakerRoundData(speakerRoundDataDTO);
            speakerRoundDataList.Add(speakerRoundData);
        }
    }
    catch (Exception ex)
    {
        Debug.LogError($"Error getting speaker round data from Firestore: {ex.Message}");
    }

    return speakerRoundDataList;
}
        public async Task UpdateMatchAtFirestore(string roundType, string roundId, Match match, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            try
            {
                Debug.Log("Starting UpdateMatchAtFirestore");
        
                // Get the document reference for the specified round
                DocumentReference roundRef = GetRoundDocumentReference(roundType, roundId);
                Debug.Log("Got round document reference");
        
                // Get the document reference for the match
                DocumentReference matchRef = roundRef.Collection("matches").Document(match.matchId);
                Debug.Log("Got match document reference");
        
                // Convert the match to DTO
                Match_DTO matchDTO = DTOConverter.Instance.MatchToDTO(match);
                Debug.Log("Converted match to DTO");
        
                // Create a Firestore batch
                WriteBatch batch = FirebaseFirestore.DefaultInstance.StartBatch();
                Debug.Log("Started Firestore batch");
        
                // Update the match in Firestore
                batch.Set(matchRef, matchDTO);
                Debug.Log("Added match to batch");
        
                // Commit the batch for the match update
                await batch.CommitAsync();
                Debug.Log("Committed batch for match update");
        
                // Create a new batch for team updates
                batch = FirebaseFirestore.DefaultInstance.StartBatch();
        
                // Semaphore to limit the number of concurrent tasks
                SemaphoreSlim semaphore = new SemaphoreSlim(4);
        
                List<Task> tasks = new List<Task>();
        
                foreach (var teamEntry in matchDTO.teamsinMatchIds)
                {
                    var team = AppConstants.instance.selectedTouranment.teamsInTourney.Find(t => t.teamId == teamEntry.Key);
                    if (team != null)
                    {
                        TeamRoundData teamRoundData = team.teamRoundDatas.Find(trd => trd.teamRoundDataID == teamEntry.Value);
                        if (teamRoundData != null)
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                await semaphore.WaitAsync();
                                try
                                {
                                    // Convert the team round data to DTO
                                    TeamRoundData_DTO teamRoundDataDTO = DTOConverter.Instance.TeamRoundDataToDTO(teamRoundData);
                                    Debug.Log($"Converted team round data to DTO for team {team.teamId}");
        
                                    // Get the document reference for the team round data
                                    DocumentReference teamRoundDataRef = GetTeamRoundDataDocumentReference(team.teamId, teamRoundData.teamRoundDataID);
                                    Debug.Log("Got team round data document reference");
        
                                    // Add the team round data update to the batch
                                    batch.Set(teamRoundDataRef, teamRoundDataDTO);
                                    Debug.Log("Added team round data to batch");
                                }
                                finally
                                {
                                    semaphore.Release();
                                }
                            }));
                        }
                    }
                }
        
                // Wait for all tasks to complete
                await Task.WhenAll(tasks);
                Debug.Log("All team updates added to batch");
        
                // Commit the batch for team updates
                await batch.CommitAsync();
                Debug.Log("Committed batch for team updates");
        
                // Invoke the success callback
                onSuccess?.Invoke();
                Debug.Log("Success callback invoked");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating match at Firestore: {ex.Message}");
                // Invoke the failure callback
                onFailure?.Invoke();
            }
        }
        public async Task UpdateRoundStateforAllPrelimsAtFirestore(UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
            try
            {
                Debug.Log("<color=orange>Updating round state for all prelims.</color>");

                // Get all preliminary rounds
                var prelimRounds = AppConstants.instance.selectedTouranment.preLimsInTourney;

                foreach (var round in prelimRounds)
                {
                // Update the state of each round
                DocumentReference roundRef = GetRoundDocumentReference(RoundCategory.PreLim.ToString(), round.roundId);
                await roundRef.UpdateAsync(new Dictionary<string, object>
                {
                    { "roundState", round.roundState }
                });

                Debug.Log($"<color=green>Round {round.roundId} state updated successfully.</color>");
                }

                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"UpdateRoundStateforAllPrelimsAtFirestore encountered an error: {ex}");
                onFailure?.Invoke();
            }
            }
            else
            {
            Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
            onFailure?.Invoke();
            }
        }
        public async Task UpdateRoundStateforAllNoviceBreaksAtFirestore(UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
            try
            {
                Debug.Log("<color=orange>Updating round state for all novice breaks.</color>");

                // Get all novice break rounds
                var noviceBreakRounds = AppConstants.instance.selectedTouranment.noviceBreaksInTourney;

                foreach (var round in noviceBreakRounds)
                {
                // Update the state of each round
                DocumentReference roundRef = GetRoundDocumentReference(RoundCategory.NoviceBreak.ToString(), round.roundId);
                await roundRef.UpdateAsync(new Dictionary<string, object>
                {
                    { "roundState", round.roundState }
                });

                Debug.Log($"<color=green>Round {round.roundId} state updated successfully.</color>");
                }

                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"UpdateRoundStateforAllNoviceBreaksAtFirestore encountered an error: {ex}");
                onFailure?.Invoke();
            }
            }
            else
            {
            Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
            onFailure?.Invoke();
            }
        }
        public async Task UpdateRoundStateforAllOpenBreaksAtFirestore(UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            if (FirebaseConnector.Instance.isFirebaseReady)
            {
            try
            {
                Debug.Log("<color=orange>Updating round state for all open breaks.</color>");

                // Get all open break rounds
                var openBreakRounds = AppConstants.instance.selectedTouranment.openBreaksInTourney;

                foreach (var round in openBreakRounds)
                {
                // Update the state of each round
                DocumentReference roundRef = GetRoundDocumentReference(RoundCategory.OpenBreak.ToString(), round.roundId);
                await roundRef.UpdateAsync(new Dictionary<string, object>
                {
                    { "roundState", round.roundState }
                });

                Debug.Log($"<color=green>Round {round.roundId} state updated successfully.</color>");
                }

                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"UpdateRoundStateforAllOpenBreaksAtFirestore encountered an error: {ex}");
                onFailure?.Invoke();
            }
            }
            else
            {
            Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
            onFailure?.Invoke();
            }
        }

        private async Task GetTeamRoundData(string teamId, string trdId)
            {
                try
                {
                    Debug.Log($"<color=lightblue>Starting GetTeamRoundData for team ID {teamId} and TRD ID {trdId}.</color>");
                    
                    // Get the query for the specified team round data
                    Query teamRoundDataQuery = GetTeamRoundDataQuery(teamId, trdId);

                    // Define the GetTeamRoundDataQuery method

                    Debug.Log($"<color=lightblue>Got Query for team ID {teamId} and TRD ID {trdId}.</color>");
                    
                    QuerySnapshot teamRoundDataSnapshot = await teamRoundDataQuery.GetSnapshotAsync();
                    Debug.Log($"<color=lightblue>Got QuerySnapshot for team ID {teamId} and TRD ID {trdId}. Documents count: {teamRoundDataSnapshot.Documents.Count()}</color>");
                    
                    if (teamRoundDataSnapshot.Documents.Count() > 0)
                    {
                        foreach (var document in teamRoundDataSnapshot.Documents)
                        {
                            TeamRoundData_DTO teamRoundDataDTO = document.ConvertTo<TeamRoundData_DTO>();
                            Debug.Log($"<color=lightblue>Converted to TeamRoundData_DTO. SRD in DTO: {teamRoundDataDTO.speakersInRound?.Count ?? 0}</color>");
                    
                            TeamRoundData teamRoundData = DTOConverter.Instance.DTOToTeamRoundData(teamRoundDataDTO);
                            Debug.Log($"<color=lightblue>Converted to TeamRoundData. SRD After Conversion: {teamRoundData.speakersInRound?.Count ?? 0}</color>");
                    
                            AppConstants.instance.AddTRD(teamRoundData, teamId);
                            Debug.Log($"<color=lightblue>Added TeamRoundData to AppConstants for team ID {teamId}.</color>");
                        }
                    }
                    else
                    {
                        Debug.Log($"<color=red>No documents found in QuerySnapshot for team ID {teamId} and TRD ID {trdId}.</color>");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error getting team round data from Firestore for team ID {teamId} and TRD ID {trdId}: {ex.Message}");
                }
            }
        public async Task AddTeamRoundData(string teamId, TeamRoundData teamRoundData, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            try
            {
                // Convert TeamRoundData to DTO
                TeamRoundData_DTO teamRoundDataDTO = DTOConverter.Instance.TeamRoundDataToDTO(teamRoundData);
                DocumentReference teamRoundDataDocRef = GetTeamRoundDataDocumentReference(teamId, teamRoundDataDTO.teamRoundDataID);
                await teamRoundDataDocRef.SetAsync(teamRoundDataDTO);
                Debug.Log("<color=green>Team round data saved successfully.</color>");
        
                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveTeamRoundData encountered an error: {ex}");
                onFailure?.Invoke();
            }
        }
        private Query GetTeamRoundDataQuery(string teamId, string trdId)
                    {
                        // Assuming you have a method to get the Firestore instance and collection reference
                        CollectionReference teamRoundDataCollection = FirebaseFirestore.DefaultInstance.Collection("teamRoundData");
                        return teamRoundDataCollection.WhereEqualTo("teamId", teamId).WhereEqualTo("trdId", trdId);
                    }
        public async Task RemoveTeamRoundData(string teamId, string teamRoundDataId, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            try
            {
                DocumentReference teamRoundDataDocRef = GetTeamRoundDataDocumentReference(teamId, teamRoundDataId);
                await teamRoundDataDocRef.DeleteAsync();
                Debug.Log("<color=green>Team round data deleted successfully.</color>");

                // Delete each speaker round data
                CollectionReference speakerRoundDataCollectionRef = teamRoundDataDocRef.Collection("speakers");
                QuerySnapshot speakerRoundDataSnapshot = await speakerRoundDataCollectionRef.GetSnapshotAsync();
                foreach (DocumentSnapshot speakerRoundDataDoc in speakerRoundDataSnapshot.Documents)
                {
                    await speakerRoundDataDoc.Reference.DeleteAsync();
                    Debug.Log("<color=green>Speaker round data deleted successfully.</color>");
                }

                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"RemoveTeamRoundData encountered an error: {ex}");
                onFailure?.Invoke();
            }
        }
        public async Task UpdateTeamRoundData(string teamId, TeamRoundData teamRoundData, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            try
            {
                // Convert TeamRoundData to DTO
                TeamRoundData_DTO teamRoundDataDTO = DTOConverter.Instance.TeamRoundDataToDTO(teamRoundData);
                DocumentReference teamRoundDataDocRef = GetTeamRoundDataDocumentReference(teamId, teamRoundDataDTO.teamRoundDataID);
                await teamRoundDataDocRef.SetAsync(teamRoundDataDTO);
                Debug.Log("<color=green>Team round data updated successfully.</color>");

                // Update each speaker round data
                if (teamRoundData.speakersInRound != null)
                {
                    foreach (var speakerRoundData in teamRoundData.speakersInRound)
                    {
                        SpeakerRoundData_DTO speakerRoundDataDTO = DTOConverter.Instance.SpeakerRoundDataToDTO(speakerRoundData);
                        DocumentReference speakerRoundDataDocRef = GetSpeakerRoundDataDocumentReference(teamId, teamRoundDataDTO.teamRoundDataID, speakerRoundDataDTO.speakerRoundDataID);
                        await speakerRoundDataDocRef.SetAsync(speakerRoundDataDTO);
                        Debug.Log("<color=green>Speaker round data updated successfully.</color>");
                    }
                }

                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"UpdateTeamRoundData encountered an error: {ex}");
                onFailure?.Invoke();
            }
        }
        public async Task<List<TeamRoundData>> GetAllTeamRoundDataForTeamFromFirestore(string teamId)
        {
            List<TeamRoundData> teamRoundDataList = new List<TeamRoundData>();

            if (FirebaseConnector.Instance.isFirebaseReady)
            {
                try
                {
                    QuerySnapshot snapshot = await GetTeamRoundDataCollectionReference(teamId).GetSnapshotAsync();
                    Debug.Log($"<color=blue>Retrieved {snapshot.Count} team round data documents from Firestore.</color>");

                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        TeamRoundData_DTO teamRoundDataDTO = document.ConvertTo<TeamRoundData_DTO>();
                        TeamRoundData teamRoundData = DTOConverter.Instance.DTOToTeamRoundData(teamRoundDataDTO);

                        // Retrieve speaker round data for the team round data
                        QuerySnapshot speakerRoundDataSnapshot = await GetSpeakerRoundDataCollectionReference(teamId, teamRoundDataDTO.teamRoundDataID).GetSnapshotAsync();
                        foreach (DocumentSnapshot speakerRoundDataDoc in speakerRoundDataSnapshot.Documents)
                        {
                            SpeakerRoundData_DTO speakerRoundDataDTO = speakerRoundDataDoc.ConvertTo<SpeakerRoundData_DTO>();
                            SpeakerRoundData speakerRoundData = DTOConverter.Instance.DTOToSpeakerRoundData(speakerRoundDataDTO);
                            teamRoundData.speakersInRound.Add(speakerRoundData);
                        }

                        teamRoundDataList.Add(teamRoundData);
                    }
                    Debug.Log("<color=green>Team round data updated successfully.</color>");
                }
                catch (Exception ex)
                {
                    Debug.LogError("GetAllTeamRoundDataForTeamFromFirestore encountered an error: " + ex);
                }
            }
            else
            {
                Debug.LogError("Firebase is not ready. Please wait for Firebase to initialize.");
            }

            return teamRoundDataList;
        }
        #endregion
        #endregion
    }
}