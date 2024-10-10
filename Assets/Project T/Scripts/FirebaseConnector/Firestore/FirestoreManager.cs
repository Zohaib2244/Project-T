using UnityEngine;
using Firebase.Firestore;
using UnityEngine.Events;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            return GetTournamentDocumentReference(AppConstants.instance.selectedTouranment.tournamentId).Collection(roundType);
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

                            // Save each speaker round data for the team round data
                            if (teamRoundData.speakersInRound != null)
                            {
                                foreach (var speakerRoundData in teamRoundData.speakersInRound)
                                {
                                    SpeakerRoundData_DTO speakerRoundDataDTO = DTOConverter.Instance.SpeakerRoundDataToDTO(speakerRoundData);
                                    DocumentReference speakerRoundDataDocRef = GetSpeakerRoundDataDocumentReference(team_dto.teamId, teamRoundDataDTO.teamRoundDataID, speakerRoundDataDTO.speakerRoundDataID);
                                    await speakerRoundDataDocRef.SetAsync(speakerRoundDataDTO);
                                    Debug.Log($"<color=green>Speaker round data {speakerRoundDataDTO.speakerRoundDataID} saved successfully.</color>");
                                }
                            }
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
                    QuerySnapshot snapshot = await GetTeamCollectionReference().GetSnapshotAsync();
                    Debug.Log($"<color=blue>Retrieved {snapshot.Count} team documents from Firestore.</color>");
                    // Remove all entries from AppConstants
                    AppConstants.instance.selectedTouranment.teamsInTourney.Clear();
                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        Team_DTO team_dto = document.ConvertTo<Team_DTO>();
                        Team team = DTOConverter.Instance.DTOToTeam(team_dto);

                        // Retrieve speakers for the team
                        QuerySnapshot speakersSnapshot = await GetSpeakerCollectionReference(team_dto.teamId).GetSnapshotAsync();
                        foreach (DocumentSnapshot speakerDoc in speakersSnapshot.Documents)
                        {
                            Speaker_DTO speaker_dto = speakerDoc.ConvertTo<Speaker_DTO>();
                            Speaker speaker = DTOConverter.Instance.DTOToSpeaker(speaker_dto);
                            team.speakers.Add(speaker);
                        }

                        // Retrieve team round data for the team
                        QuerySnapshot teamRoundDataSnapshot = await GetTeamRoundDataCollectionReference(team_dto.teamId).GetSnapshotAsync();
                        Debug.Log($"<color=blue>Retrieved {teamRoundDataSnapshot.Count} team round data documents for team ID {team_dto.teamId} from Firestore.</color>");

                        foreach (DocumentSnapshot teamRoundDataDoc in teamRoundDataSnapshot.Documents)
                        {
                            TeamRoundData_DTO teamRoundDataDTO = teamRoundDataDoc.ConvertTo<TeamRoundData_DTO>();
                            TeamRoundData teamRoundData = DTOConverter.Instance.DTOToTeamRoundData(teamRoundDataDTO);
                            Debug.Log($"<color=blue>Converted team round data document to TeamRoundData object for team round data ID {teamRoundDataDTO.teamRoundDataID}.</color>");

                            // Retrieve speaker round data for the team round data
                            QuerySnapshot speakerRoundDataSnapshot = await GetSpeakerRoundDataCollectionReference(team_dto.teamId, teamRoundDataDTO.teamRoundDataID).GetSnapshotAsync();
                            Debug.Log($"<color=blue>Retrieved {speakerRoundDataSnapshot.Count} speaker round data documents for team round data ID {teamRoundDataDTO.teamRoundDataID} from Firestore.</color>");

                            foreach (DocumentSnapshot speakerRoundDataDoc in speakerRoundDataSnapshot.Documents)
                            {
                                SpeakerRoundData_DTO speakerRoundDataDTO = speakerRoundDataDoc.ConvertTo<SpeakerRoundData_DTO>();
                                SpeakerRoundData speakerRoundData = DTOConverter.Instance.DTOToSpeakerRoundData(speakerRoundDataDTO);
                                Debug.Log($"<color=blue>Converted speaker round data document to SpeakerRoundData object for speaker round data ID {speakerRoundDataDTO.speakerRoundDataID}.</color>");
                                teamRoundData.speakersInRound.Add(speakerRoundData);
                            }

                            team.teamRoundDatas.Add(teamRoundData);
                            Debug.Log($"<color=green>Added TeamRoundData object with ID {teamRoundDataDTO.teamRoundDataID} to team with ID {team_dto.teamId}.</color>");
                        }
                        // team.teamRoundDatas = await GetAllTeamRoundDataForTeamFromFirestore(team_dto.teamId);

                        AppConstants.instance.selectedTouranment.teamsInTourney.Add(team);
                    }
                    Debug.Log("<color=green>Teams updated successfully.</color>");
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
        }
        public async void UpdateTeamInFireStore(Team team, UnityAction onSuccess = null, UnityAction onFailure = null)
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
        #endregion

        #region Speaker Functions
        #nullable enable
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

                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        Rounds_DTO round_dto = document.ConvertTo<Rounds_DTO>();
                        Rounds round = DTOConverter.Instance.DTOToRounds(round_dto);

                        // Retrieve matches for the round
                        QuerySnapshot matchesSnapshot = await GetMatchCollectionReference(roundType, round_dto.roundId).GetSnapshotAsync();
                        foreach (DocumentSnapshot matchDoc in matchesSnapshot.Documents)
                        {
                            Match_DTO match_dto = matchDoc.ConvertTo<Match_DTO>();
                            Match match = DTOConverter.Instance.DTOToMatch(match_dto);
                            round.matches.Add(match);
                        }

                        roundsList.Add(round);
                    }
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
            try
            {
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
            }
            catch (Exception ex)
            {
                Debug.LogError($"<color=red>Error saving motion for round {roundId}: {ex.Message}</color>");
                onFailure?.Invoke();
            }
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
        
    
        
        #endregion

        #endregion

        #region TeamRoundData Functions
        public async Task AddTeamRoundData(string teamId, TeamRoundData teamRoundData, UnityAction onSuccess = null, UnityAction onFailure = null)
        {
            try
            {
                // Convert TeamRoundData to DTO
                TeamRoundData_DTO teamRoundDataDTO = DTOConverter.Instance.TeamRoundDataToDTO(teamRoundData);
                DocumentReference teamRoundDataDocRef = GetTeamRoundDataDocumentReference(teamId, teamRoundDataDTO.teamRoundDataID);
                await teamRoundDataDocRef.SetAsync(teamRoundDataDTO);
                Debug.Log("<color=green>Team round data saved successfully.</color>");

                // Save each speaker round data
                if (teamRoundData.speakersInRound != null)
                {
                    foreach (var speakerRoundData in teamRoundData.speakersInRound)
                    {
                        SpeakerRoundData_DTO speakerRoundDataDTO = DTOConverter.Instance.SpeakerRoundDataToDTO(speakerRoundData);
                        DocumentReference speakerRoundDataDocRef = GetSpeakerRoundDataDocumentReference(teamId, teamRoundDataDTO.teamRoundDataID, speakerRoundDataDTO.speakerRoundDataID);
                        await speakerRoundDataDocRef.SetAsync(speakerRoundDataDTO);
                        Debug.Log("<color=green>Speaker round data saved successfully.</color>");
                    }
                }

                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"SaveTeamRoundData encountered an error: {ex}");
                onFailure?.Invoke();
            }
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
    }
}