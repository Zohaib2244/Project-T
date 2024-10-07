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
                    Destroy(_instance);
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
        public int swingsCount;
        [SerializeField] private GameObject TopPanel;
        [SerializeField] private Image RoundPanelImage;
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
        [SerializeField] private Button silentRoundButton;
        [SerializeField] private Color defaultColorSRBtn;
        [SerializeField] private TMP_Text roundTypeText;
        #endregion

        #region Essentails
        void OnEnable()
        {
            selectedRound = AppConstants.instance.GetCurrentRound();
            // selectedRound.
            DisableAllPanels();
            ConfigureRoundDropDown();
            UpdateBreaksDropDown();
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

        #region Panel Configuration
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
                roundFunctionButton.EnableInteractability(0);
                roundFunctionButton.EnableInteractability(1);
                goPublicButton.interactable = false;
            }
            else if (selectedRound.motionAdded && selectedRound.teamAttendanceAdded && !selectedRound.AdjudicatorAttendanceAdded && !selectedRound.ballotsAdded && !selectedRound.drawGenerated)
            {
                roundFunctionButton.DeselectAll();
                roundFunctionButton.DisableAllInteractability();
                roundFunctionButton.EnableInteractability(0);
                roundFunctionButton.EnableInteractability(1);
                goPublicButton.interactable = false;
            }
            else if (selectedRound.motionAdded && selectedRound.teamAttendanceAdded && selectedRound.AdjudicatorAttendanceAdded && !selectedRound.ballotsAdded && !selectedRound.drawGenerated)
            {
                roundFunctionButton.DeselectAll();
                roundFunctionButton.DisableAllInteractability();
                roundFunctionButton.EnableInteractability(0);
                roundFunctionButton.EnableInteractability(1);
                roundFunctionButton.EnableInteractability(2);
                goPublicButton.interactable = false;
            }
            else if (selectedRound.motionAdded && selectedRound.teamAttendanceAdded && selectedRound.AdjudicatorAttendanceAdded && !selectedRound.ballotsAdded && selectedRound.drawGenerated)
            {
                roundFunctionButton.DeselectAll();
                roundFunctionButton.DisableAllInteractability();
                roundFunctionButton.EnableInteractability(0);
                roundFunctionButton.EnableInteractability(1);
                roundFunctionButton.EnableInteractability(2);
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
            RoundPanelImage.sprite = MotionsPanelImage;
            MotionsPanel.gameObject.SetActive(true);
            AttendancePanel.gameObject.SetActive(false);
            DrawsPanel.gameObject.SetActive(false);
            BallotsPanel.gameObject.SetActive(false);
            SelectedRoundPanel = RoundPanelTypes.MotionsPanel;
            goPublicButton.interactable = false;
        }
        public void OpenAttendancePanel()
        {
            RoundPanelImage.sprite = AttendancePanelImage;
            MotionsPanel.gameObject.SetActive(false);
            AttendancePanel.gameObject.SetActive(true);
            DrawsPanel.gameObject.SetActive(false);
            BallotsPanel.gameObject.SetActive(false);
            SelectedRoundPanel = RoundPanelTypes.AttendancePanel;
            goPublicButton.interactable = false;
        }
        public void OpenDrawsPanel()
        {
            RoundPanelImage.sprite = DrawsPanelImage;
            MotionsPanel.gameObject.SetActive(false);
            AttendancePanel.gameObject.SetActive(false);
            DrawsPanel.gameObject.SetActive(true);
            BallotsPanel.gameObject.SetActive(false);
            SelectedRoundPanel = RoundPanelTypes.DrawsPanel;
            goPublicButton.interactable = false;
        }
        public void OpenBallotsPanel()
        {
            RoundPanelImage.sprite = BallotsPanelImage;
            MotionsPanel.gameObject.SetActive(false);
            AttendancePanel.gameObject.SetActive(false);
            DrawsPanel.gameObject.SetActive(false);
            BallotsPanel.gameObject.SetActive(true);
            SelectedRoundPanel = RoundPanelTypes.BallotsPanel;
            goPublicButton.interactable = false;
        }
        public void DisableAllPanels()
        {
            RoundPanelImage.sprite = DefaultImage;
            MotionsPanel.gameObject.SetActive(false);
            AttendancePanel.gameObject.SetActive(false);
            DrawsPanel.gameObject.SetActive(false);
            BallotsPanel.gameObject.SetActive(false);
            SelectedRoundPanel = RoundPanelTypes.None;
            goPublicButton.interactable = false;
        }

        #endregion

        #region Round Configuration
        private void UpdateSelectedRound(Rounds rounds)
        {
            selectedRound = rounds;
            ConfigureScreen();
            DisableAllPanels();
        }
        private void ConfigureRoundDropDown()
        {
            preLimDropdown.DeleteAllOptions();
            noviceBreakDropdown.DeleteAllOptions();
            openBreakDropdown.DeleteAllOptions();
        
            // Populate prelims dropdown
            for (int i = 0; i < AppConstants.instance.selectedTouranment.preLimsInTourney.Count; i++)
            {
                preLimDropdown.AddOptions($"Round {i + 1}");
            }
        
            // Deactivate novice and open dropdowns initially
            noviceBreakDropdown.gameObject.SetActive(false);
            openBreakDropdown.gameObject.SetActive(false);
        
            // Deactivate novice dropdown if the tournament is open
            if (AppConstants.instance.selectedTouranment.speakerCategories.Count == 1)
            {
                noviceBreakDropdown.gameObject.SetActive(false);
            }
        }
                private void UpdateBreaksDropDown()
        {
            var prelims = AppConstants.instance.selectedTouranment.preLimsInTourney;
            var noviceBreaks = AppConstants.instance.selectedTouranment.noviceBreaksInTourney;
            var openBreaks = AppConstants.instance.selectedTouranment.openBreaksInTourney;
        
            // Check if all prelims rounds are over
            bool allPrelimsOver = prelims.All(round => round.roundState == RoundStates.Completed);
        
            if (allPrelimsOver)
            {
                // Activate novice and open dropdowns
                if (AppConstants.instance.selectedTouranment.speakerCategories.Count <= 1)
                {
                    noviceBreakDropdown.gameObject.SetActive(false);
                }
                else
                {
                    noviceBreakDropdown.gameObject.SetActive(true);
                }
                openBreakDropdown.gameObject.SetActive(true);
        
                // Update novice breaks dropdown
                for (int i = 0; i < noviceBreaks.Count; i++)
                {
                    if (i == 0 || noviceBreaks[i - 1].roundState == RoundStates.Completed)
                    {
                        noviceBreakDropdown.AddOptions(noviceBreaks[i].roundType.ToString());
                    }
                }
        
                // Update open breaks dropdown
                for (int i = 0; i < openBreaks.Count; i++)
                {
                    if (i == 0 || openBreaks[i - 1].roundState == RoundStates.Completed)
                    {
                        openBreakDropdown.AddOptions(openBreaks[i].roundType.ToString());
                    }
                }
            }
        }
        private void ConfigureScreen()
        {
            roundFunctionButton.DeselectAll();
            goPublicButton.interactable = false;
            if(selectedRound.isSilent)
            {
                silentRoundButton.GetComponent<Image>().color = Color.red;
            }
            else
            {
                silentRoundButton.GetComponent<Image>().color = defaultColorSRBtn;
            }

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
        // Toggle the isSilent state
        selectedRound.isSilent = !selectedRound.isSilent;

        // Change the button color based on the isSilent state
        if (selectedRound.isSilent)
        {
            silentRoundButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            silentRoundButton.GetComponent<Image>().color = defaultColorSRBtn;
        }
    }
        #endregion

    }
}