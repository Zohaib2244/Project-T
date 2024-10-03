using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.ListEntry
{
    public class LE_Adj_DrawOption : MonoBehaviour
    {
        private Adjudicator myAdjudicator;
        [SerializeField] private TMPro.TMP_Text adjudicatorNameTxt;

        public void SetAdjudicator(Adjudicator adjudicator)
        {
            myAdjudicator = adjudicator;
            adjudicatorNameTxt.text = myAdjudicator.adjudicatorName;
        }
    }
}