using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Resources;
using Scripts.FirebaseConfig;


namespace Scripts.UIPanels.RoundPanels
{
    public class Round_MotionsPanel : MonoBehaviour
    {
        #region Singleton
        private static Round_MotionsPanel _instance;

        public static Round_MotionsPanel Instance
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

        #endregion
        [SerializeField] private GameObject motionPrefab;
        [SerializeField] private Transform motionContainer;
        [SerializeField] private GameObject[] motionPrefabs;

        void OnEnable()
        {
            ConfigureMotionsforRound();
            DebugMotions();
        }
        void DebugMotions()
        {
            var motions = MainRoundsPanel.Instance.selectedRound.motions;

            if (motions == null)
            {
                Debug.LogError("DebugMotions: motions is null.");
                return;
            }

            Debug.Log("DebugMotions: Logging motions...");

            // Assuming motions is a Dictionary<string, string>
            foreach (var kvp in motions)
            {
                Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
        }
        void OnDisable()
        {
            // Clear existing motion prefabs
            foreach (Transform child in motionContainer.transform)
            {
                Destroy(child.gameObject);
            }
        }
        public void ConfigureMotionsforRound()
        {
            // Clear existing motion prefabs
            foreach (Transform child in motionContainer.transform)
            {
                Destroy(child.gameObject);
            }

            // Determine the number of motion prefabs to generate based on tournament type
            int motionCount = 1; // Default to 1 for British
            if (AppConstants.instance.selectedTouranment.tournamentType == TournamentType.Asian)
            {
                motionCount = 3; // Set to 3 for Asian
            }

            // Generate motion prefabs
            for (int i = 0; i < motionCount; i++)
            {
                GameObject motionPrefabInstance = Instantiate(motionPrefab, motionContainer.transform);

                // Check if there already exists a motion for the round
                if (i < MainRoundsPanel.Instance.selectedRound.motions.Count)
                {
                    // Populate the motion prefab with the motion data
                    var motionKey = MainRoundsPanel.Instance.selectedRound.motions.Keys.ElementAt(i);
                    var motionValue = MainRoundsPanel.Instance.selectedRound.motions[motionKey];
                    Debug.Log($"Motion Key: {motionKey}, Motion Value: {motionValue}");
                    motionPrefabInstance.GetComponent<MotionsEntry>().SetMotion(new Dictionary<string, string> { { motionKey, motionValue } });
                }
                else
                {
                    // Leave the motion prefab empty
                    motionPrefabInstance.GetComponent<MotionsEntry>().SetMotion(new Dictionary<string, string>());
                }
            }
        }
        public async void SaveMotion(Dictionary<string, string> motion)
        {
            var motions = motion;

            if (motions == null)
            {
                Debug.LogError("DebugMotions: motions is null.");
                return;
            }

            Debug.Log("DebugMotions: Logging motions...");

            // Assuming motions is a Dictionary<string, string>
            foreach (var kvp in motions)
            {
                Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
            Loading.Instance.ShowLoadingScreen();
            Debug.Log("Saving motion For Round Type: " + MainRoundsPanel.Instance.selectedRound.roundCategory.ToString());
            await FirestoreManager.FireInstance.SaveRoundMotionToFirestore(MainRoundsPanel.Instance.selectedRound.roundCategory.ToString(), MainRoundsPanel.Instance.selectedRound.roundId, motion, OnMotionSavedSuccess);
        }

        private void OnMotionSavedSuccess(Dictionary<string, string> motion)
        {

            Loading.Instance.HideLoadingScreen();
            DialogueBox.Instance.ShowDialogueBox("Motion Saved Successfully.", Color.green);
            MainRoundsPanel.Instance.selectedRound.motionAdded = true;
            MainRoundsPanel.Instance.selectedRound.motions = motion;
            MainRoundsPanel.Instance.goPublicButton.interactable = true;
            MainRoundsPanel.Instance.UpdatePanelSwitcherButtonsStates();
            MainRoundsPanel.Instance.SaveRound();
            var motions = MainRoundsPanel.Instance.selectedRound.motions;

            if (motions == null)
            {
                Debug.LogError("DebugMotions: motions is null.");
                return;
            }

            Debug.Log("DebugMotions: Logging motions...");

            // Assuming motions is a Dictionary<string, string>
            foreach (var kvp in motions)
            {
                Debug.Log($"Key: {kvp.Key}, Value: {kvp.Value}");
            }
        }
        private void OnMotionSavedFailure()
        {
            Loading.Instance.HideLoadingScreen();
            DialogueBox.Instance.ShowDialogueBox("Failed to save motion.", Color.red);
        }
    }
}