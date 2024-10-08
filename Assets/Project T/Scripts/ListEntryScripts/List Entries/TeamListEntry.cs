using UnityEngine;
using TMPro;
using Scripts.UIPanels;
using UnityEngine.UI;
using DG.Tweening;


namespace Scripts.ListEntry
{
    public class TeamListEntry : MonoBehaviour
    {
        public Team myTeam;
        [SerializeField] private TMP_Text teamnametxt;
        [SerializeField] private TMP_Text teamInstitutetxt;
        [SerializeField] private Button myBtn;
        
        private Instituitions myInstitution;
        private ColorBlock myColorBlock;
        private bool isSelected = false;

        void Start()
        {
            myColorBlock = myBtn.colors;
            CRUDTeamPanel.Instance.DisableAllTeams.AddListener(DeSelect);
        }

        public void SetTeam(Team team)
        {
            myTeam = team;
            myInstitution = AppConstants.instance.GetInstituitionsFromID(myTeam.instituition);
            teamnametxt.text = myTeam.teamName.ToString();
            teamInstitutetxt.text = myInstitution.instituitionAbreviation;
        }
        public void SelectTeam()
        {
            if (isSelected)
            {
                DeSelect();
                CRUDTeamPanel.Instance.DeActivateAddTeamPanel();
                // CRUDTeamPanel.Instance.DisableAllTeams?.Invoke();
            }
            else
            {
                CRUDTeamPanel.Instance.DisableAllTeams?.Invoke();
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    isSelected = true;
                    myColorBlock.normalColor = new Color32(0xE6, 0xFF, 0xD1, 0xFF);
                    myBtn.colors = myColorBlock;
                    transform.DOScale(Vector3.one * 0.9f, 0.1f);
                    CRUDTeamPanel.Instance.ShowTeamInfo(myTeam);
                });
            }
        }
        private void DeSelect()
        {
            if (isSelected)
            {
                myColorBlock.normalColor = new Color32(0xFF, 0xFF, 0xFF, 0xFF); // White color in hex
                myBtn.colors = myColorBlock;
                isSelected = false;
                transform.DOScale(Vector3.one, 0.1f);
            }
        }
    }
}