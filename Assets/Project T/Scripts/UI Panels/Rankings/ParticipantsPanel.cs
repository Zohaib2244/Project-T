using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticipantsPanel : MonoBehaviour
{
    public GameObject adjudicatorPanel;
    public GameObject speakersPanel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void toggleAdjudicatorPanel()
    {
        adjudicatorPanel.SetActive(true);
        speakersPanel.SetActive(false);
    }

    public void toggleSpeakersPanel()
    {
        adjudicatorPanel.SetActive(false);
        speakersPanel.SetActive(true);
    }
}
