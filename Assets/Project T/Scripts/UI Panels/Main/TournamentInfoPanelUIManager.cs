using Scripts.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DG.Tweening;
using Scripts.UI;
using System.Collections.Generic;
using System.Numerics;

namespace Scripts.UIPanels
{
    public class TournamentInfoPanelUIManager : MonoBehaviour
    {
        #region Variables
        private TournamentInfo tournamentConfiguration = new TournamentInfo();
        [SerializeField] private Transform panel1;
        [SerializeField] private Transform panel2;
        [SerializeField] private TMP_InputField tournamentName;
        [SerializeField] private Toggle tournamentTypeToggle;

        [SerializeField] private TMP_Text preLimCount;
        [SerializeField] private Button NoviceBreakDropDownBtn;
        [SerializeField] private Button OpenBreakDropDownBtn;
        [SerializeField] private ButtonSelect teamCategoriersBtnSelect;
        [SerializeField] private AdvancedDropdown openBreakDropDown;
        [SerializeField] private AdvancedDropdown noviceBreakDropDown;
        [SerializeField] private Button saveTournamentBtn;
        [SerializeField] private Button increasePrelimCountBtn;
        [SerializeField] private Button decreasePrelimCountBtn;
        public bool generateID = false;
        #endregion

        void OnEnable()
        {
            ActivatePanel();
            openBreakDropDown.onChangedValue += OnOpenBreakSelected;
            noviceBreakDropDown.onChangedValue += OnNoviceBreakSelected;
        }
        void OnDisable()
        {
            DeActivatePanel();
            tournamentConfiguration = null;
            openBreakDropDown.onChangedValue -= OnOpenBreakSelected;
            noviceBreakDropDown.onChangedValue -= OnNoviceBreakSelected;
        }
        void ActivatePanel()
        {
            panel1.DOScale(UnityEngine.Vector3.one, 0.2f);
            panel2.DOScale(UnityEngine.Vector3.one, 0.2f);

            tournamentName.text = "";
            tournamentTypeToggle.Activate();

            if (AppConstants.instance.tournaments.Count == 1)
            {
                if (AppConstants.instance.tournaments[0].tournamentType == TournamentType.British)
                    tournamentTypeToggle.DisableOption(2);
                else
                    tournamentTypeToggle.DisableOption(1);
            }
            else if (AppConstants.instance.tournaments.Count == 2)
            {
                tournamentTypeToggle.DeActivate();
            }


            preLimCount.text = "4";
            // teamCategoriersBtnSelect.DeselectAll();
            openBreakDropDown.SetDefaultText();
            noviceBreakDropDown.SetDefaultText();
            NoviceBreakDropDownBtn.interactable = false;
            OpenBreakDropDownBtn.interactable = false;
            saveTournamentBtn.interactable = true;
            generateID = true;
        }
        void DeActivatePanel()
        {
            panel1.DOScale(UnityEngine.Vector3.one * 0.8f, 0.2f);
            panel2.DOScale(UnityEngine.Vector3.one * 0.8f, 0.2f);
            tournamentName.text = "";
            tournamentTypeToggle.DeActivate();
            preLimCount.text = "4";
            teamCategoriersBtnSelect.DeselectAll();
            openBreakDropDown.SetDefaultText();
            noviceBreakDropDown.SetDefaultText();
            NoviceBreakDropDownBtn.interactable = false;
            OpenBreakDropDownBtn.interactable = false;
            saveTournamentBtn.interactable = false;
            generateID = false;
        }

        public void ShowTournamentInfo(TournamentInfo tournamentInfo)
        {
            DeActivatePanel();
            // First delay of 0.02 seconds
            DOVirtual.DelayedCall(0.02f, () =>
            {
                ActivatePanel();

                // Second delay of 0.02 seconds
                DOVirtual.DelayedCall(0.02f, () =>
                {
                    generateID = false;
                    tournamentConfiguration = tournamentInfo;
                    tournamentName.text = tournamentConfiguration.tournamentName;
                    if (tournamentConfiguration.tournamentType == TournamentType.Asian)
                        tournamentTypeToggle.SelectOption1();
                    else
                        tournamentTypeToggle.SelectOption2();
                    preLimCount.text = tournamentConfiguration.noOfPrelims.ToString();
                    if (tournamentConfiguration.speakerCategories.Count > 1)
                    {
                        teamCategoriersBtnSelect.SelectOption(1);
                        int index = tournamentConfiguration.speakerCategories[0].breakType == BreakTypes.QF ? 0 :
                                    tournamentConfiguration.speakerCategories[0].breakType == BreakTypes.SF ? 1 :
                                    tournamentConfiguration.speakerCategories[0].breakType == BreakTypes.F ? 2 : 3;
                        openBreakDropDown.ratioShrinking = 1;
                        openBreakDropDown.SelectOption(index);
                        index = tournamentConfiguration.speakerCategories[1].breakType == BreakTypes.QF ? 0 :
                                    tournamentConfiguration.speakerCategories[1].breakType == BreakTypes.SF ? 1 :
                                    tournamentConfiguration.speakerCategories[1].breakType == BreakTypes.F ? 2 : 3;
                        noviceBreakDropDown.ratioShrinking = 1;
                        noviceBreakDropDown.SelectOption(index);
                    }
                    else
                    {
                        teamCategoriersBtnSelect.SelectOption(0);
                        int index = tournamentConfiguration.speakerCategories[0].breakType == BreakTypes.QF ? 0 :
                                    tournamentConfiguration.speakerCategories[0].breakType == BreakTypes.SF ? 1 :
                                    tournamentConfiguration.speakerCategories[0].breakType == BreakTypes.F ? 2 : 3;
                        openBreakDropDown.ratioShrinking = 1;
                        openBreakDropDown.SelectOption(index);
                    }
                });
            });
        }
        public void IncreasePrelimCount()
        {
            Debug.Log("Increase Prelim Count");
            int count = int.Parse(preLimCount.text);
            if (count < 5)
            {
                count++;
                preLimCount.text = count.ToString();
            }

            // Deactivate the increase button if the count reaches 5
            if (count >= 5)
            {
                increasePrelimCountBtn.interactable = false;
            }
            else
            {
                increasePrelimCountBtn.interactable = true;
            }

            // Reactivate the decrease button if the count is above 3
            if (count > 3)
            {
                decreasePrelimCountBtn.interactable = true;
            }
        }

        public void DecreasePrelimCount()
        {
            Debug.Log("Decrease Prelim Count");
            int count = int.Parse(preLimCount.text);
            if (count > 3)
            {
                count--;
                preLimCount.text = count.ToString();
            }

            // Deactivate the decrease button if the count reaches 3
            if (count <= 3)
            {
                decreasePrelimCountBtn.interactable = false;
            }
            else
            {
                decreasePrelimCountBtn.interactable = true;
            }

            // Reactivate the increase button if the count is below 5
            if (count < 5)
            {
                increasePrelimCountBtn.interactable = true;
            }
        }

        public void SelectOpenOnly()
        {
            NoviceBreakDropDownBtn.interactable = false;
            OpenBreakDropDownBtn.interactable = true;
            if (tournamentConfiguration.speakerCategories == null || tournamentConfiguration.speakerCategories.Count == 0)
            {
                tournamentConfiguration.speakerCategories = new List<SpeakerCategories>
                {
                    new SpeakerCategories
                    {
                        speakerType = SpeakerTypes.Open,
                        breakType = BreakTypes.QF
                    }
                };
            }
            else
            {
                // Remove novice speaker category if it exists
                tournamentConfiguration.speakerCategories.RemoveAll(sc => sc.speakerType == SpeakerTypes.Novice);

                // Update the existing list
                tournamentConfiguration.speakerCategories.Clear();
                tournamentConfiguration.speakerCategories.Add(new SpeakerCategories
                {
                    speakerType = SpeakerTypes.Open,
                    breakType = BreakTypes.QF
                });
            }
        }

        public void SelectOpenAndNovice()
        {
            NoviceBreakDropDownBtn.interactable = true;
            OpenBreakDropDownBtn.interactable = true;

            if (tournamentConfiguration.speakerCategories == null || tournamentConfiguration.speakerCategories.Count == 0)
            {
                tournamentConfiguration.speakerCategories = new List<SpeakerCategories>
                {
                    new SpeakerCategories
                    {
                        speakerType = SpeakerTypes.Open,
                        breakType = BreakTypes.QF
                    },
                    new SpeakerCategories
                    {
                        speakerType = SpeakerTypes.Novice,
                        breakType = BreakTypes.QF
                    }
                };
            }
            else
            {
                // Update the existing list
                tournamentConfiguration.speakerCategories.Clear();
                tournamentConfiguration.speakerCategories.Add(new SpeakerCategories
                {
                    speakerType = SpeakerTypes.Open,
                    breakType = BreakTypes.QF
                });
                tournamentConfiguration.speakerCategories.Add(new SpeakerCategories
                {
                    speakerType = SpeakerTypes.Novice,
                    breakType = BreakTypes.QF
                });
            }
        }

        private void OnOpenBreakSelected(int index)
        {
            switch (index)
            {
                case 0:
                    tournamentConfiguration.speakerCategories[0].breakType = BreakTypes.QF;
                    break;
                case 1:
                    tournamentConfiguration.speakerCategories[0].breakType = BreakTypes.SF;
                    break;
                case 2:
                    tournamentConfiguration.speakerCategories[0].breakType = BreakTypes.F;
                    break;
                default:
                    break;
            }
        }
        private void OnNoviceBreakSelected(int index)
        {
            if (tournamentConfiguration.speakerCategories.Count > 1)
            {
                switch (index)
                {
                    case 0:
                        tournamentConfiguration.speakerCategories[1].breakType = BreakTypes.QF;
                        break;
                    case 1:
                        tournamentConfiguration.speakerCategories[1].breakType = BreakTypes.SF;
                        break;
                    case 2:
                        tournamentConfiguration.speakerCategories[1].breakType = BreakTypes.F;
                        break;
                    default:
                        break;
                }
            }
        }
        public async void SaveTournamentConfiguration()
        {
            saveTournamentBtn.interactable = false;
            tournamentConfiguration.tournamentName = tournamentName.text;
            tournamentConfiguration.noOfPrelims = int.Parse(preLimCount.text);
            InitializeRounds();
            Debug.Log("Tournament Name: " + tournamentConfiguration.tournamentName);
            if (generateID)
            {
                Debug.Log("Generating ID");
                //add id
                if (tournamentConfiguration.tournamentType == TournamentType.British)
                    tournamentConfiguration.tournamentId = "b01";
                else if (tournamentConfiguration.tournamentType == TournamentType.Asian)
                    tournamentConfiguration.tournamentId = "a01";
                Debug.Log("Generating ID");
                tournamentConfiguration.preLimsInTourney[0].roundState = RoundStates.InProgress;

                await AppConstants.instance.AddTournament(tournamentConfiguration);
            }

            DOVirtual.DelayedCall(1.5f, () => MainUIManager.Instance.SwitchPanel(Panels.TournamentSelectionPanel));
        }

        private void InitializeRounds()
        {
            Debug.Log("Initializing Rounds");
            int noviceBreaks = 0;
            switch (tournamentConfiguration.speakerCategories[1].breakType)
            {
                case BreakTypes.QF:
                    noviceBreaks = 3;
                    break;
                case BreakTypes.SF:
                    noviceBreaks = 2;
                    break;
                case BreakTypes.F:
                    noviceBreaks = 1;
                    break;
                default:
                    Debug.Log("Unknown break type selected");
                    break;
            }

            int openBreaks = 0;
            switch (tournamentConfiguration.speakerCategories[0].breakType)
            {
                case BreakTypes.QF:
                    openBreaks = 3;
                    break;
                case BreakTypes.SF:
                    openBreaks = 2;
                    break;
                case BreakTypes.F:
                    openBreaks = 1;
                    break;
                default:
                    Debug.Log("Unknown break type selected");
                    break;
            }

            // Initialize preliminary rounds
            AdjustRounds(tournamentConfiguration.preLimsInTourney, tournamentConfiguration.noOfPrelims, RoundTypes.PreLim, "Prelim");

            // Define the round types to be used, excluding PreLim
            RoundTypes[] breakTypes = { RoundTypes.QF, RoundTypes.SF, RoundTypes.F };

            // Initialize novice breaks
            AdjustRounds(tournamentConfiguration.noviceBreaksInTourney, noviceBreaks, breakTypes, "NoviceBreak");

            // Initialize open breaks
            AdjustRounds(tournamentConfiguration.openBreaksInTourney, openBreaks, breakTypes, "OpenBreak");
        }

        private void AdjustRounds(List<Rounds> roundsList, int requiredCount, RoundTypes roundType, string roundIdPrefix)
        {
            // Remove excess rounds if the list is longer than required
            while (roundsList.Count > requiredCount)
            {
                roundsList.RemoveAt(roundsList.Count - 1);
            }

            // Add missing rounds if the list is shorter than required
            while (roundsList.Count < requiredCount)
            {
                roundsList.Add(new Rounds
                {
                    roundId = $"R_{roundIdPrefix}{roundsList.Count + 1}",
                    roundType = roundType,
                    availableTeams = new List<Team>(),
                    availableAdjudicators = new List<Adjudicator>(),
                    roundState = RoundStates.NotStarted,
                    swings = new List<TeamRoundData>(),
                    matches = new List<Match>()
                });
            }
        }

        private void AdjustRounds(List<Rounds> roundsList, int requiredCount, RoundTypes[] breakTypes, string roundIdPrefix)
        {
            // Remove excess rounds if the list is longer than required
            while (roundsList.Count > requiredCount)
            {
                roundsList.RemoveAt(roundsList.Count - 1);
            }

            // Add missing rounds if the list is shorter than required
            while (roundsList.Count < requiredCount)
            {
                RoundTypes roundType;
                if (requiredCount == 1)
                {
                    roundType = RoundTypes.F;
                }
                else if (requiredCount == 2)
                {
                    roundType = (roundsList.Count == 0) ? RoundTypes.SF : RoundTypes.F;
                }
                else
                {
                    roundType = breakTypes[roundsList.Count % breakTypes.Length];
                }

                roundsList.Add(new Rounds
                {
                    roundId = $"R_{roundIdPrefix}{roundsList.Count + 1}",
                    roundType = roundType,
                    availableTeams = new List<Team>(),
                    availableAdjudicators = new List<Adjudicator>(),
                    roundState = RoundStates.NotStarted,
                    swings = new List<TeamRoundData>(),
                    matches = new List<Match>()
                });
            }
        }
    }
}