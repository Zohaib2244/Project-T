using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeakersPanelEntry : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public TMP_Text nameText;
    public TMP_Text categoryText;
    public TMP_Text teamText;
    public TMP_Text instituteText;

    public void SetAdjudicatorData(string name, string type, string team, string institute)
    {
        if (nameText != null)
            nameText.text = name;

        if (categoryText != null)
            categoryText.text = type;

        if (teamText != null)
            teamText.text = team;

        if (instituteText != null)
            instituteText.text = institute;
    }
}


