using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Scripts.UIPanels;
using UnityEngine.UI;
namespace Scripts.ListEntry
{
    public class SpeakerPanelSpeakerListEntry : MonoBehaviour
    {
        [Header("Speaker List Entry")]
        [SerializeField] private TMPro.TMP_Text speakerName;
        [SerializeField] private TMPro.TMP_Text speakerTeamName;
        [SerializeField] private Button speakerSelectBtn;
        private Team myTeam;
        private Speaker mySpeaker;
        private int mySpeakerIndex;
        private bool isSelected;
        private bool isReplaceable;
        private void Start()
        {
            CRUDSpeakerPanel.Instance.OnSpeakerListEntryDeselect.AddListener(DeselectedAnim);
            CRUDSpeakerPanel.Instance.OnReplaceModeEnable.AddListener(ReplaceModeOn);
            CRUDSpeakerPanel.Instance.OnReplaceModeDisable.AddListener(ReplaceModeOff);
            CRUDSpeakerPanel.Instance.DisableAllReplaceable.AddListener(DeselectedAnim);
        }

        public void SetSpeakerListEntry(Team team, int speakerIndex)
        {
            myTeam = team;
            mySpeaker = team.speakers[speakerIndex];
            mySpeakerIndex = speakerIndex;

            speakerName.text = mySpeaker.speakerName;
            speakerTeamName.text = myTeam.teamName;
        }

        public (Team, int) GetAllSpeakerInfo(Team team, Speaker speaker)
        {
            myTeam = team;
            mySpeaker = speaker;
            return (myTeam, mySpeakerIndex);
        }
        public void SpeakerSelected()
        {
            CRUDSpeakerPanel.Instance.OnSpeakerListEntryDeselect?.Invoke();
            // Debug.Log("Speaker Selected");
            isSelected = true;
            // Debug.Log("ReplaceMode : " + CRUDSpeakerPanel.Instance.isReplaceModeOn);
            if (CRUDSpeakerPanel.Instance.isReplaceModeOn)
            {

                CRUDSpeakerPanel.Instance.ReplaceSpeakerWith(myTeam, mySpeakerIndex);
                CRUDSpeakerPanel.Instance.DisableAllReplaceable?.Invoke();
                isReplaceable = true;
                SelectedAnim();
                Debug.Log("Speaker Replaced");
            }
            else if (!CRUDSpeakerPanel.Instance.isReplaceModeOn)
            {
                CRUDSpeakerPanel.Instance.ShowSelectedSpeakerInfo(myTeam, mySpeakerIndex);
                SelectedAnim();
                Debug.Log("Speaker Info Shown");
            }
        }
        private void SelectedAnim()
        {
            if (CRUDSpeakerPanel.Instance.isReplaceModeOn)
            {
                speakerSelectBtn.interactable = false;
                transform.DOScale(Vector3.one * 0.9f, 0.1f);
            }
            else if (!CRUDSpeakerPanel.Instance.isReplaceModeOn)
            {
                speakerSelectBtn.interactable = false;
                transform.DOScale(Vector3.one * 0.9f, 0.1f);
            }
        }
        private void DeselectedAnim()
        {
            if (isSelected && !CRUDSpeakerPanel.Instance.isReplaceModeOn)
            {
                transform.DOScale(Vector3.one, 0.1f);
                speakerSelectBtn.interactable = true;
                isSelected = false;
            }
            else if (isSelected && CRUDSpeakerPanel.Instance.isReplaceModeOn && isReplaceable)
            {
                transform.DOScale(Vector3.one, 0.1f);
                speakerSelectBtn.interactable = true;
                isSelected = false;
                isReplaceable = false;
            }
        }
        private void ReplaceModeOn()
        {
            // Change button highlight color to FAC29C
            ColorBlock cb = speakerSelectBtn.colors;
            cb.highlightedColor = new Color32(0xFA, 0xC2, 0x9C, 0xFF); // Hex color FAC29C
            speakerSelectBtn.colors = cb;
        }
        private void ReplaceModeOff()
        {
            // Change button highlight color to 9CFAE7
            ColorBlock cb = speakerSelectBtn.colors;
            cb.highlightedColor = new Color32(0xDF, 0xFF, 0xD2, 0xFF); // Hex color DFFFD2
            speakerSelectBtn.colors = cb;
        }
    }
}
