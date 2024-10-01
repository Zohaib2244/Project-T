using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scripts.Resources;
using TMPro;
using System.Linq;
using UnityEngine.UI;


namespace Scripts.UIPanels
{
    public class MainRoundsPanel : MonoBehaviour
    {
        #region Singleton
        private static MainRoundsPanel _instance;

        public static MainRoundsPanel Instance
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
        #region  Variables
        public Rounds selectedRound;
        public RoundPanelTypes SelectedRoundPanel;
        [SerializeField] private GameObject TopPanel;
        [SerializeField] private Sprite DefaultImage;
        [SerializeField] private Sprite MotionsPanelImage;
        [SerializeField] private Sprite AttendancePanelImage;
        [SerializeField] private Sprite DrawsPanelImage;
        [SerializeField] private Sprite BallotsPanelImage;

        [Header("Round Panels")]
        [SerializeField] private Transform MotionsPanel;
        // [SerializeField] private ButtonPanel MotionsButtonPanel;
        [SerializeField] private Transform AttendancePanel;
        [SerializeField] private Transform DrawsPanel;
        [SerializeField] private Transform BallotsPanel;

        [Header("Round Side Panel")]
        [SerializeField] private AdvancedDropdown preLimDropdown;
        [SerializeField] private AdvancedDropdown noviceBreakDropdown;
        [SerializeField] private AdvancedDropdown openBreakDropdown;
        [SerializeField] private Button preLimDropdownButton;
        [SerializeField] private Button noviceBreakDropdownButton;
        [SerializeField] private Button openBreakDropdownButton;
        [SerializeField] private ButtonSelect roundFunctionButton;

        [Header("Round Info Panel")]
        [SerializeField] private TMP_Text preLimRoundName;
        [SerializeField] private Toggle speakerCategoryToggle;
        [SerializeField] public Button goPublicButton;
        [SerializeField] private TMP_Text roundTypeText;
        #endregion

        #region Essentails
        void OnEnable()
        {
            selectedRound = AppConstants.instance.GetCurrentRound();
            // selectedRound.
            DisableAllPanels();
            ConfigureRoudnDropDown();
            ConfigureScreen();
            // speakerCategoryToggle.DeActivate();
            // DisableAllPanels();
            preLimDropdown.onChangedValue += ChangePrelimRound;
            noviceBreakDropdown.onChangedValue += ChangeNoviceBreakRound;
            openBreakDropdown.onChangedValue += ChangeOpenBreakRound;
        }

        private void OnDisable()
        {
            selectedRound = null;
        }
        #endregion

        #region Panel Switcher
        public void UpdatePanelSwitcherButtonsStates()
        {
            if (!selectedRound.motionAdded && !selectedRound.teamAttendanceAdded && !selectedRound.AdjudicatorAttendanceAdded && !selectedRound.ballotsAdded && !selectedRound.drawGenerated)
            {
                roundFunctionButton.DeselectAll();
                roundFunctionButton.DisableAllInteractability();
                roundFunctionButton.EnableInteractability(0);
                goPublicButton.interactable = false;
            }
            else if (selectedRound.motionAdded && !selectedRound.teamAttendanceAdded && !selectedRound.AdjudicatorAttendanceAdded && !selectedRound.ballotsAdded && !selectedRound.drawGenerated)
            {
                roundFunctionButton.DeselectAll();
                roundFunctionButton.DisableAllInteractability();
                roundFunctionButton.EnableInteractability(1);
                goPublicButton.interactable = false;
            }
            else if (selectedRound.motionAdded && selectedRound.teamAttendanceAdded && !selectedRound.AdjudicatorAttendanceAdded && !selectedRound.ballotsAdded && !selectedRound.drawGenerated)
            {
                roundFunctionButton.DeselectAll();
                roundFunctionButton.DisableAllInteractability();
                roundFunctionButton.EnableInteractability(1);
                goPublicButton.interactable = false;
            }
            else if (selectedRound.motionAdded && selectedRound.teamAttendanceAdded && selectedRound.AdjudicatorAttendanceAdded && !selectedRound.ballotsAdded && !selectedRound.drawGenerated)
            {
                roundFunctionButton.DeselectAll();
                roundFunctionButton.DisableAllInteractability();
                roundFunctionButton.EnableInteractability(2);
                goPublicButton.interactable = false;
            }
            else if (selectedRound.motionAdded && selectedRound.teamAttendanceAdded && selectedRound.AdjudicatorAttendanceAdded && selectedRound.ballotsAdded && !selectedRound.drawGenerated)
            {
                roundFunctionButton.DeselectAll();
                roundFunctionButton.DisableAllInteractability();
                roundFunctionButton.EnableInteractability(3);
                goPublicButton.interactable = false;
            }
            else if (selectedRound.motionAdded && selectedRound.teamAttendanceAdded && selectedRound.AdjudicatorAttendanceAdded && selectedRound.ballotsAdded && selectedRound.drawGenerated)
            {
                roundFunctionButton.DeselectAll();
                roundFunctionButton.EnableAllInteractability();
                goPublicButton.interactable = true;
            }
        }


        public void OpenMotionsPanel()
        {
            MotionsPanel.gameObject.SetActive(true);
            AttendancePanel.gameObject.SetActive(false);
            DrawsPanel.gameObject.SetActive(false);
            BallotsPanel.gameObject.SetActive(false);
            SelectedRoundPanel = RoundPanelTypes.MotionsPanel;
        }
        public void OpenAttendancePanel()
        {
            MotionsPanel.gameObject.SetActive(false);
            AttendancePanel.gameObject.SetActive(true);
            DrawsPanel.gameObject.SetActive(false);
            BallotsPanel.gameObject.SetActive(false);
            SelectedRoundPanel = RoundPanelTypes.AttendancePanel;
        }
        public void OpenDrawsPanel()
        {
            MotionsPanel.gameObject.SetActive(false);
            AttendancePanel.gameObject.SetActive(false);
            DrawsPanel.gameObject.SetActive(true);
            BallotsPanel.gameObject.SetActive(false);
            SelectedRoundPanel = RoundPanelTypes.DrawsPanel;
        }
        public void OpenBallotsPanel()
        {
            MotionsPanel.gameObject.SetActive(false);
            AttendancePanel.gameObject.SetActive(false);
            DrawsPanel.gameObject.SetActive(false);
            BallotsPanel.gameObject.SetActive(true);
            SelectedRoundPanel = RoundPanelTypes.BallotsPanel;
        }
        public void DisableAllPanels()
        {
            MotionsPanel.gameObject.SetActive(false);
            AttendancePanel.gameObject.SetActive(false);
            DrawsPanel.gameObject.SetActive(false);
            BallotsPanel.gameObject.SetActive(false);
            SelectedRoundPanel = RoundPanelTypes.None;
        }
        #endregion

        #region Round Configuration
        private void UpdateSelectedRound(Rounds rounds)
        {
            selectedRound = rounds;
            ConfigureScreen();
            DisableAllPanels();
        }
        private void ConfigureRoudnDropDown()
        {
            preLimDropdown.DeleteAllOptions();
            noviceBreakDropdown.DeleteAllOptions();
            openBreakDropdown.DeleteAllOptions();

            for (int i = 0; i < AppConstants.instance.selectedTouranment.preLimsInTourney.Count; i++)
            {
                preLimDropdown.AddOptions($"Round {i + 1}");
            }
            for (int i = 0; i < AppConstants.instance.selectedTouranment.noviceBreaksInTourney.Count; i++)
            {
                noviceBreakDropdown.AddOptions(AppConstants.instance.selectedTouranment.noviceBreaksInTourney[i].roundType.ToString());
            }
            for (int i = 0; i < AppConstants.instance.selectedTouranment.openBreaksInTourney.Count; i++)
            {
                openBreakDropdown.AddOptions(AppConstants.instance.selectedTouranment.openBreaksInTourney[i].roundType.ToString());
            }
        }
        private void ConfigureScreen()
        {
            roundFunctionButton.DeselectAll();
            goPublicButton.interactable = false;
            //Update Round Name Display Text
            // Debug.Log($"Selected round: {selectedRound.roundType}");
            switch (selectedRound.roundType)
            {
                case RoundTypes.PreLim:
                    int roundIndex = AppConstants.instance.selectedTouranment.preLimsInTourney.IndexOf(selectedRound) + 1;
                    preLimRoundName.text = $"Round {roundIndex}";
                    break;
                case RoundTypes.QF:
                    preLimRoundName.text = "Quarter Finals";
                    break;
                case RoundTypes.SF:
                    preLimRoundName.text = "Semi Finals";
                    break;
                case RoundTypes.F:
                    preLimRoundName.text = "Finals";
                    break;
                default:
                    break;
            }
            if (AppConstants.instance.selectedTouranment.speakerCategories.Count > 1)
            {
                noviceBreakDropdownButton.interactable = true;
            }
            else
            {
                noviceBreakDropdownButton.interactable = false;
            }
            UpdatePanelSwitcherButtonsStates();
        }

        private void ChangePrelimRound(int index)
        {
            // Get the value at the specified index from the prelim drop-down
            string selectedValue = preLimDropdown.optionsList[index].ToString();

            // Find the round it states
            var allRounds = AppConstants.instance.selectedTouranment.preLimsInTourney.ToList();

            if (index >= 0 && index < allRounds.Count)
            {
                var foundRound = allRounds[index];
                Debug.Log($"Selected round: {selectedRound.roundType}");
                // Add that round to the selected round
                UpdateSelectedRound(foundRound);
                noviceBreakDropdown.SetDefaultText();
                openBreakDropdown.SetDefaultText();
                roundTypeText.text = "- PreLim";
                DialogueBox.Instance.ShowDialogueBox("Round found!", Color.green);

            }
            else
            {
                DialogueBox.Instance.ShowDialogueBox("Round not found!", Color.red);
            }
        }
        private void ChangeNoviceBreakRound(int index)
        {
            // Get the value at the specified index from the prelim drop-down
            string selectedValue = noviceBreakDropdown.optionsList[index].ToString();

            // Find the round it states
            var allRounds = AppConstants.instance.selectedTouranment.noviceBreaksInTourney.ToList();

            if (index >= 0 && index < allRounds.Count)
            {
                var foundRound = allRounds[index];
                Debug.Log($"Selected round: {selectedRound.roundType}");
                // Add that round to the selected round
                UpdateSelectedRound(foundRound);
                preLimDropdown.SetDefaultText();
                openBreakDropdown.SetDefaultText();
                roundTypeText.text = "- NOVICE";
                Debug.Log($"Selected round: {selectedRound.roundType}");
            }
            else
            {
                DialogueBox.Instance.ShowDialogueBox("Round not found!", Color.red);
            }
        }
        private void ChangeOpenBreakRound(int index)
        {
            // Get the value at the specified index from the prelim drop-down
            string selectedValue = openBreakDropdown.optionsList[index].ToString();

            // Find the round it states
            var allRounds = AppConstants.instance.selectedTouranment.openBreaksInTourney.ToList();

            if (index >= 0 && index < allRounds.Count)
            {
                var foundRound = allRounds[index];
                Debug.Log($"Selected round: {selectedRound.roundType}");
                // Add that round to the selected round
                UpdateSelectedRound(foundRound);
                roundTypeText.text = "- OPEN";
                preLimDropdown.SetDefaultText();
                noviceBreakDropdown.SetDefaultText();
                Debug.Log($"Selected round: {selectedRound.roundType}");
            }
            else
            {
                DialogueBox.Instance.ShowDialogueBox("Round not found!", Color.red);
            }
        }



        #endregion
        #region Round Functions
        public void SaveRound()
        {
            switch (selectedRound.roundType)
            {
                case RoundTypes.PreLim:
                    AppConstants.instance.selectedTouranment.preLimsInTourney[AppConstants.instance.selectedTouranment.preLimsInTourney.IndexOf(selectedRound)] = selectedRound;
                    break;
                case RoundTypes.QF:
                    AppConstants.instance.selectedTouranment.noviceBreaksInTourney[AppConstants.instance.selectedTouranment.noviceBreaksInTourney.IndexOf(selectedRound)] = selectedRound;
                    break;
                case RoundTypes.SF:
                    AppConstants.instance.selectedTouranment.noviceBreaksInTourney[AppConstants.instance.selectedTouranment.noviceBreaksInTourney.IndexOf(selectedRound)] = selectedRound;
                    break;
                case RoundTypes.F:
                    AppConstants.instance.selectedTouranment.openBreaksInTourney[AppConstants.instance.selectedTouranment.openBreaksInTourney.IndexOf(selectedRound)] = selectedRound;
                    break;
                default:
                    break;
            }
            DialogueBox.Instance.ShowDialogueBox("Round Saved", Color.green);
        }
        public void GoPublic()
        {
            switch (SelectedRoundPanel)
            {
                case RoundPanelTypes.MotionsPanel:
                    foreach (var motion in selectedRound.motions)
                    {
                        Debug.Log("Motion: " + motion.Key);
                        if (!string.IsNullOrEmpty(motion.Value))
                        {
                            Debug.Log("InfoSlide: " + motion.Value);
                        }
                    }
                    DialogueBox.Instance.ShowDialogueBox("Motions Shown On Public Page", Color.green);
                    break;
                case RoundPanelTypes.AttendancePanel:
                    // OpenDrawsPanel();
                    break;
                case RoundPanelTypes.DrawsPanel:
                    // OpenBallotsPanel();
                    break;
                case RoundPanelTypes.BallotsPanel:
                    // OpenMotionsPanel();
                    break;
                default:
                    // OpenMotionsPanel();
                    break;
            }
        }
        public void SetRoundSilent()
        {
            selectedRound.isSilent = true;
        }
        #endregion
    }
}