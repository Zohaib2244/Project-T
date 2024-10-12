using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AdjudicatorPanelEntry : MonoBehaviour
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
    public TMP_Text instituteText;
    public TMP_Text typeText;

    public void SetAdjudicatorData(string name, string institute, string type)
    {
        if (nameText != null)
            nameText.text = name;

        if (instituteText != null)
            instituteText.text = institute;

        if (typeText != null)
            typeText.text = type;
    }
}

