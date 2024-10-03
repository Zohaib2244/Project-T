using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Scripts.ListEntry
{
    public class LE_Team_DrawOption : MonoBehaviour
    {
        private Team myTeam;
        [SerializeField] private TMPro.TMP_Text teamNameTxt;


        public void SetTeam(Team team)
        {
            myTeam = team;
            teamNameTxt.text = myTeam.teamName;
        }
    }
}