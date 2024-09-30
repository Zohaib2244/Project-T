using UnityEngine;
using TMPro;
using Scripts.UIPanels;


namespace Scripts.ListEntry
{
    public class TeamListEntry : MonoBehaviour
    {
        public Team myTeam;
        [SerializeField] private TMP_Text teamnametxt;

        public void SetTeam(Team team)
        {
            myTeam = team;
            teamnametxt.text = myTeam.teamName.ToString();
        }
        public void SelectTeam()
        {
            CRUDTeamPanel.Instance.ShowTeamInfo(myTeam);
        }
    }
}